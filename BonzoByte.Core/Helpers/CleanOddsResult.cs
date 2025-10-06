using BonzoByte.Core.Models;

namespace BonzoByte.Core.Helpers
{
    public sealed class CleanOddsResult
    {
        public List<MatchOdds> Kept { get; set; } = new();
        public List<MatchOdds> Dropped { get; set; } = new();
        public double? BestP1 { get; set; }
        public double? BestP2 { get; set; }
        public double? medP1 { get; set; }
        public double? medP2 { get; set; }

        private static double Median(IEnumerable<double> xs)
        {
            var arr = xs.OrderBy(x => x).ToArray();
            if (arr.Length == 0) return double.NaN;
            int m = arr.Length / 2;
            return (arr.Length % 2 == 0) ? (arr[m - 1] + arr[m]) / 2.0 : arr[m];
        }

        private static bool IsOverroundOk(double o1, double o2)
        {
            double orr = 1.0 / o1 + 1.0 / o2;
            return orr >= 1.00 && orr <= 1.15;
        }

        // ⬇️ učini public static
        public static CleanOddsResult CleanOddsForMatch(IEnumerable<MatchOdds> rows, DateTime? matchStartLocal, bool isFinished)
        {
            var all = rows
                .Where(r => r.Player1Odds.HasValue && r.Player2Odds.HasValue
                            && (double)r.Player1Odds.Value >= 1.01 && r.Player1Odds.Value <= 100
                            && (double)r.Player2Odds.Value >= 1.01 && r.Player2Odds.Value <= 100)
                .ToList();

            if (all.Count == 0) return new CleanOddsResult();

            // === 1) VREMENSKI FILTER sa FAIL-SAFE-om ===
            // Pokušaj odrezati sve nakon starta; ali ako ubije previše uzoraka, vrati se na 'all'.
            List<MatchOdds> timeFiltered = all;

            if (matchStartLocal.HasValue)
            {
                var cutoff = matchStartLocal.Value.AddMinutes(-1);
                var pre = all.Where(r => !r.DateTime.HasValue || r.DateTime.Value <= cutoff).ToList();

                if (pre.Count >= 3)
                    timeFiltered = pre;           // prihvati filter
                                                  // inače: fail-safe → zadrži 'all' bez vremenskog reza
            }
            else if (isFinished)
            {
                // Ako je završeni meč, ali nemamo start, probaj konzervativno ograničiti po ingestu/originalnom vremenu
                DateTime? latestAllowed =
                    all.Min(r => r.IngestedAt) ??
                    all.Min(r => r.DateTime);

                if (latestAllowed.HasValue)
                {
                    // koristi latestAllowed umjesto matchStartLocal!
                    var cutoff = latestAllowed.Value; // ili latestAllowed.Value.AddMinutes(-1) ako želiš buffer
                    var pre = all.Where(r =>
                    {
                        var t = r.DateTime ?? r.IngestedAt; // preferiraj izvorni timestamp; fallback ingest
                        return !t.HasValue || t.Value <= cutoff;
                    }).ToList();

                    if (pre.Count >= 3)
                        timeFiltered = pre;
                }
            }

            if (timeFiltered.Count == 0) return new CleanOddsResult(); // ekstremni slučaj

            // === 2) OVERROUND sanity (sa fallbackom) ===
            var sane = timeFiltered.Where(r => IsOverroundOk((double)r.Player1Odds!.Value, (double)r.Player2Odds!.Value)).ToList();
            if (sane.Count < 3) sane = timeFiltered; // fail-safe: treba uzoraka za median

            // === 3) KONSENZUS ===
            double medP1 = Median(sane.Select(r => (double)r.Player1Odds!.Value));
            double medP2 = Median(sane.Select(r => (double)r.Player2Odds!.Value));
            bool consensusP1Higher = medP1 > medP2;

            // === 4) DETEKCIJA FLIP-a (outlier) ===
            const double flipBand = 0.20; // 20%
            bool LooksFlipped(MatchOdds r)
            {
                double p1 = (double)r.Player1Odds!.Value, p2 = (double)r.Player2Odds!.Value;
                if (consensusP1Higher)
                    return (p1 < medP1 * (1 - flipBand)) && (p2 > medP2 * (1 + flipBand));
                else
                    return (p2 < medP2 * (1 - flipBand)) && (p1 > medP1 * (1 + flipBand));
            }

            var kept = new List<MatchOdds>();
            var dropped = new List<MatchOdds>();
            foreach (var r in sane)
                if (LooksFlipped(r)) dropped.Add(r); else kept.Add(r);

            // ako si pretvrd → fail-safe
            if (kept.Count < 3 && sane.Count >= 3)
            {
                kept = sane; dropped.Clear();
            }

            var res = new CleanOddsResult { Kept = kept, Dropped = dropped };
            if (kept.Count > 0)
            {
                res.BestP1 = kept.Max(r => (double)r.Player1Odds!.Value);
                res.BestP2 = kept.Max(r => (double)r.Player2Odds!.Value);
                res.medP1 = medP1;
                res.medP2 = medP2;
            }
            return res;
        }
    }
}