using BonzoByte.Core.DTOs;
using HtmlAgilityPack;

namespace BonzoByte.Core.Helpers
{
    public static class MatchRoundParser
    {
        public static List<MatchRoundInfo> ParseMatchRoundsFromTournamentHtml(string html)
        {
            var result = new List<MatchRoundInfo>();
            var seenMatchIds = new HashSet<long>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var trNodes = doc.DocumentNode.SelectNodes("//tr[starts-with(@class, 'match')]");
            if (trNodes == null) return result;

            foreach (var tr in trNodes)
            {
                var roundTd = tr.SelectSingleNode(".//td[starts-with(@class, 'main_round')]");
                var porTd = tr.SelectSingleNode(".//td[@class='main_por']");

                if (roundTd == null || porTd == null)
                    continue;

                var roundText = roundTd.InnerText.Trim();

                var links = porTd.SelectNodes(".//a[contains(@href, 'ma_id=')]");
                if (links == null) continue;

                foreach (var link in links)
                {
                    var href = link.GetAttributeValue("href", "");
                    var maIdStr = System.Web.HttpUtility.ParseQueryString(href).Get("ma_id");

                    if (long.TryParse(maIdStr, out long matchTPId))
                    {
                        if (!seenMatchIds.Contains(matchTPId))
                        {
                            result.Add(new MatchRoundInfo
                            {
                                MatchTPId = matchTPId,
                                RoundName = roundText
                            });

                            seenMatchIds.Add(matchTPId);
                        }
                    }
                }
            }

            return result;
        }
    }
}