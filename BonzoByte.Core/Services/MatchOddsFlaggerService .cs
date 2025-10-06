using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;

namespace BonzoByte.Core.Services
{
    public class MatchOddsFlaggerService
    {
        private const double NEUTRAL_EPS = 0.05;
        private const double STRONG_EPS = 0.12;
        private const double LJUMP = 0.35;

        private readonly IMatchOddsRepository _repo;

        public MatchOddsFlaggerService(IMatchOddsRepository repo)
        {
            _repo = repo;
        }

        public async Task FlagAsync(int matchTPId, bool isFinished, DateTime? matchStartLocal = null)
        {
            var rows = (await _repo.GetOddsByMatchAsync(matchTPId)).ToList();
            if (rows.Count == 0) return;

            // Per-row z = ln(p1/p2)
            var zmap = rows.ToDictionary(
                r => r.OddsId!.Value,
                r => (r.Player1Odds.HasValue && r.Player2Odds.HasValue && r.Player1Odds.Value > 0 && r.Player2Odds.Value > 0)
                        ? Math.Log((double)(r.Player1Odds.Value / r.Player2Odds.Value))
                        : (double?)null
            );

            // --- Bookie consensus ---
            var bookieSign = new Dictionary<int, int?>(); // +1 / -1 / null
            foreach (var g in rows.Where(r => r.BookieId.HasValue).GroupBy(r => r.BookieId!.Value))
            {
                var zs = g.Select(r => zmap[r.OddsId!.Value])
                          .Where(z => z.HasValue && Math.Abs(z.Value) >= NEUTRAL_EPS)
                          .Select(z => z!.Value)
                          .ToList();
                if (zs.Count >= 3)
                {
                    int pos = zs.Count(z => z > 0);
                    int neg = zs.Count - pos;
                    double frac = (double)Math.Max(pos, neg) / zs.Count;
                    bookieSign[g.Key] = frac >= 2.0 / 3.0 ? (pos > neg ? +1 : -1) : (int?)null;
                }
                else bookieSign[g.Key] = null;
            }

            // --- Global consensus ---
            var votes = bookieSign.Values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
            int? global = null;
            if (votes.Count > 0)
            {
                int pos = votes.Count(v => v > 0);
                int neg = votes.Count - pos;
                double frac = (double)Math.Max(pos, neg) / votes.Count;
                if (frac >= 2.0 / 3.0) global = pos > neg ? +1 : -1;
            }

            // --- Large jump per bookie series ---
            var largeJumpSet = new HashSet<int>(); // OddsId to flag later
            foreach (var g in rows.Where(r => r.BookieId.HasValue).GroupBy(r => r.BookieId!.Value))
            {
                var seq = g.OrderBy(r => r.CoalescedTime ?? r.SourceFileTime ?? DateTime.MinValue)
                           .ThenBy(r => r.SeriesOrdinal ?? int.MaxValue)
                           .ToList();
                for (int i = 1; i < seq.Count; i++)
                {
                    var prev = seq[i - 1];
                    var cur = seq[i];
                    var zp = zmap[prev.OddsId!.Value];
                    var zc = zmap[cur.OddsId!.Value];
                    if (zp.HasValue && zc.HasValue)
                    {
                        if (Math.Abs(zc.Value - zp.Value) > LJUMP)
                            largeJumpSet.Add(cur.OddsId!.Value); // flagiramo "noviji"
                    }
                }
            }

            // --- Iterate all rows and compose mask/flags ---
            foreach (var r in rows)
            {
                short mask = r.SuspiciousMask ?? 0;
                bool changed = false;

                // Opposite to bookie majority
                if (r.BookieId.HasValue && bookieSign.TryGetValue(r.BookieId.Value, out var bSign) && bSign.HasValue)
                {
                    var z = zmap[r.OddsId!.Value];
                    if (z.HasValue && Math.Abs(z.Value) >= STRONG_EPS)
                    {
                        int sign = z.Value > 0 ? +1 : -1;
                        if (sign != bSign.Value)
                        {
                            mask |= (short)SuspiciousBits.OppositeToBookieMajority;
                        }
                    }
                }

                // Opposite to global
                if (global.HasValue)
                {
                    var z = zmap[r.OddsId!.Value];
                    if (z.HasValue && Math.Abs(z.Value) >= STRONG_EPS)
                    {
                        int sign = z.Value > 0 ? +1 : -1;
                        if (sign != global.Value)
                        {
                            mask |= (short)SuspiciousBits.OppositeToGlobalConsensus;
                        }
                    }
                }

                // Large jump
                if (largeJumpSet.Contains(r.OddsId!.Value))
                    mask |= (short)SuspiciousBits.LargeJumpWithinBookieSeries;

                // Post-result heuristika
                if (isFinished
                    && (r.SeriesOrdinal ?? -1) == 0
                    && r.DateTime == null) // header bez vremena
                {
                    var z = zmap[r.OddsId!.Value];
                    bSign = (r.BookieId.HasValue && bookieSign.ContainsKey(r.BookieId.Value)) ? bookieSign[r.BookieId.Value] : null;
                    bool contraBookie = z.HasValue && bSign.HasValue && Math.Abs(z.Value) >= STRONG_EPS && (z.Value > 0 ? +1 : -1) != bSign.Value;
                    bool contraGlobal = z.HasValue && global.HasValue && Math.Abs(z.Value) >= STRONG_EPS && (z.Value > 0 ? +1 : -1) != global.Value;

                    if (contraBookie || contraGlobal)
                        mask |= (short)SuspiciousBits.PostResultNoTimeHeader;
                }

                // Likely switched
                bool likely =
                    ((mask & (short)SuspiciousBits.PostResultNoTimeHeader) != 0
                        && ((mask & (short)SuspiciousBits.OppositeToBookieMajority) != 0
                          || (mask & (short)SuspiciousBits.OppositeToGlobalConsensus) != 0))
                    ||
                    ((mask & (short)SuspiciousBits.OppositeToBookieMajority) != 0
                      && (mask & (short)SuspiciousBits.OppositeToGlobalConsensus) != 0
                      && zmap[r.OddsId!.Value].HasValue
                      && Math.Abs(zmap[r.OddsId!.Value]!.Value) >= STRONG_EPS);

                bool isSuspiciousFinal =
                    (mask & (short)SuspiciousBits.OverroundOutOfRange) != 0
                 || (mask & (short)SuspiciousBits.OppositeToBookieMajority) != 0
                 || (mask & (short)SuspiciousBits.OppositeToGlobalConsensus) != 0
                 || (mask & (short)SuspiciousBits.LargeJumpWithinBookieSeries) != 0
                 || (mask & (short)SuspiciousBits.PostResultNoTimeHeader) != 0;
                // NearCoinflipNeutral (32) i AfterMatchStart (128) ne čine red "sumnjivim" sami po sebi.

                changed = (mask != (r.SuspiciousMask ?? 0))
                       || (likely != (r.IsLikelySwitched ?? false))
                       || (isSuspiciousFinal != (r.IsSuspicious ?? false));

                if (changed)
                {
                    await _repo.UpdateFlagsAsync(r.OddsId!.Value, isSuspiciousFinal, likely, mask);
                }
            }
        }
    }
}