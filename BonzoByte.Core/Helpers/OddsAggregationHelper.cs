using System.Globalization;

namespace BonzoByte.Core.Helpers
{
    public sealed class AggregatedOdds
    {
        public double? Player1Odds { get; init; }
        public double? Player2Odds { get; init; }
        public int SnapshotsUsed { get; init; }
        public int BookiesUsed { get; init; }
        public double? AverageOverround { get; init; }

        public static readonly AggregatedOdds Empty = new AggregatedOdds
        {
            Player1Odds = null,
            Player2Odds = null,
            SnapshotsUsed = 0,
            BookiesUsed = 0,
            AverageOverround = null
        };
    }

    public static class OddsAggregationHelper
    {
        /// <summary>
        /// Agregira kvote po implied vjerojatnostima (p = 1/odds), koristi median (robustno).
        /// Radi QC filter: raspon kvota, raspon overround-a, deduplikacija po bookie-ju (uzima najnoviji).
        /// </summary>
        public static AggregatedOdds AggregateByImpliedMedian(
            IEnumerable<Models.MatchOdds> odds,
            double minOdds = 1.01,
            double maxOdds = 21.0,
            double minOverround = 1.00,
            double maxOverround = 1.20)
        {
            if (odds is null) return AggregatedOdds.Empty;

            var cleaned = odds
                .Where(o => o.Player1Odds.HasValue && o.Player2Odds.HasValue)
                .Select(o => new
                {
                    o.BookieId,
                    o.DateTime,
                    o1 = o.Player1Odds!.Value,
                    o2 = o.Player2Odds!.Value
                })
                .Where(x => (double)x.o1 >= minOdds && (double)x.o1 <= maxOdds && (double)x.o2 >= minOdds && (double)x.o2 <= maxOdds)
                .Select(x => new
                {
                    x.BookieId,
                    x.DateTime,
                    x.o1,
                    x.o2,
                    p1 = (double)1.0 / (double)x.o1,
                    p2 = (double)1.0 / (double)x.o2,
                    over = ((double)1.0 / (double)x.o1) + ((double)1.0 / (double)x.o2)
                })
                .Where(x => x.over >= minOverround && x.over <= maxOverround)
                .ToList();

            if (cleaned.Count == 0) return AggregatedOdds.Empty;

            // Dedup: po bookie-ju uzmi najnoviji zapis (za aktivne je to najkorisnije).
            var perBookie = cleaned
                .GroupBy(x => x.BookieId ?? -1) // bookieId može biti null; grupiraj pod -1
                .Select(g => g.OrderByDescending(r => r.DateTime ?? DateTime.MinValue).First())
                .ToList();

            if (perBookie.Count == 0) return AggregatedOdds.Empty;

            var p1List = perBookie.Select(x => x.p1).OrderBy(v => v).ToList();
            var p2List = perBookie.Select(x => x.p2).OrderBy(v => v).ToList();

            double p1Agg = Median(p1List);
            double p2Agg = Median(p2List);

            // Pretvori natrag u sintetske kvote
            double o1Syn = 1.0 / p1Agg;
            double o2Syn = 1.0 / p2Agg;

            return new AggregatedOdds
            {
                Player1Odds = o1Syn,
                Player2Odds = o2Syn,
                SnapshotsUsed = cleaned.Count,
                BookiesUsed = perBookie.Count,
                AverageOverround = perBookie.Average(x => x.over)
            };
        }

        private static double Median(IReadOnlyList<double> sortedAsc)
        {
            int n = sortedAsc.Count;
            if (n == 0) return double.NaN;
            if ((n & 1) == 1) return sortedAsc[n / 2];
            // even
            return 0.5 * (sortedAsc[(n / 2) - 1] + sortedAsc[n / 2]);
        }
    }
}