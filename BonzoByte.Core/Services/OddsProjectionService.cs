using BonzoByte.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonzoByte.Core.Services
{
    public static class OddsProjectionService
    {
        public static MatchOddsBundleDTO Build(int matchTPId, IReadOnlyList<OddsQuoteDTO> rows, bool includeMergedSeries = true)
        {
            var result = new MatchOddsBundleDTO { MatchTPId = matchTPId };

            foreach (var grp in rows.GroupBy(r => new { r.BookieId, r.BookieName }).OrderBy(g => g.Key.BookieId))
            {
                var quotes = grp.OrderBy(r => r.CoalescedTime).ThenBy(r => r.OddsId).ToList();

                var bo = new BookieOddsDTO
                {
                    Bookie = new BookieDTO { BookieId = grp.Key.BookieId, BookieName = grp.Key.BookieName },
                    Quotes = quotes,
                    Opening = quotes.FirstOrDefault(),
                    Closing = quotes.LastOrDefault(),
                };

                var p1 = quotes.Select(q => q.Player1Odds).Where(v => v.HasValue).Select(v => v!.Value).ToList();
                var p2 = quotes.Select(q => q.Player2Odds).Where(v => v.HasValue).Select(v => v!.Value).ToList();

                if (p1.Count > 0)
                {
                    bo.P1Min = p1.Min(); bo.P1Max = p1.Max();
                    bo.P1Avg = Math.Round(p1.Average(), 3); bo.P1Median = Median(p1);
                }
                if (p2.Count > 0)
                {
                    bo.P2Min = p2.Min(); bo.P2Max = p2.Max();
                    bo.P2Avg = Math.Round(p2.Average(), 3); bo.P2Median = Median(p2);
                }

                result.Bookies.Add(bo);
            }

            var all = rows.OrderBy(r => r.CoalescedTime).ThenBy(r => r.OddsId).ToList();
            var allP1 = all.Select(x => x.Player1Odds).Where(v => v.HasValue).Select(v => v!.Value).ToList();
            var allP2 = all.Select(x => x.Player2Odds).Where(v => v.HasValue).Select(v => v!.Value).ToList();

            result.Overall = new MarketAggregateDTO
            {
                Opening = all.FirstOrDefault(),
                Closing = all.LastOrDefault(),
                P1Min = allP1.Count > 0 ? allP1.Min() : null,
                P1Max = allP1.Count > 0 ? allP1.Max() : null,
                P1Avg = allP1.Count > 0 ? Math.Round(allP1.Average(), 3) : null,
                P1Median = allP1.Count > 0 ? Median(allP1) : null,
                P2Min = allP2.Count > 0 ? allP2.Min() : null,
                P2Max = allP2.Count > 0 ? allP2.Max() : null,
                P2Avg = allP2.Count > 0 ? Math.Round(allP2.Average(), 3) : null,
                P2Median = allP2.Count > 0 ? Median(allP2) : null,
                Samples = Math.Max(allP1.Count, allP2.Count)
            };

            if (includeMergedSeries)
            {
                result.Merged = rows
                    .GroupBy(r => r.CoalescedTime)
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        var gp1 = g.Select(x => x.Player1Odds).Where(v => v.HasValue).Select(v => v!.Value).ToList();
                        var gp2 = g.Select(x => x.Player2Odds).Where(v => v.HasValue).Select(v => v!.Value).ToList();
                        return new MergedOddsPointDTO
                        {
                            CoalescedTime = g.Key,
                            AvgP1 = gp1.Count > 0 ? Math.Round(gp1.Average(), 3) : null,
                            AvgP2 = gp2.Count > 0 ? Math.Round(gp2.Average(), 3) : null,
                            MedianP1 = gp1.Count > 0 ? Median(gp1) : null,
                            MedianP2 = gp2.Count > 0 ? Median(gp2) : null,
                            Samples = Math.Max(gp1.Count, gp2.Count)
                        };
                    })
                    .ToList();
            }

            return result;
        }

        private static decimal Median(List<decimal> values)
        {
            values.Sort();
            int n = values.Count;
            if (n == 0) return 0m;
            return (n % 2 == 1) ? values[n / 2] : Math.Round((values[n / 2 - 1] + values[n / 2]) / 2m, 3);
        }
    }
}