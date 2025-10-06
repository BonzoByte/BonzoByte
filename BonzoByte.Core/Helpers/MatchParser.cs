using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class MatchParser
    {
        public static (int matchId, int player1Id, int player2Id) ParseMatchInfoTd(HtmlNode row)
        {
            int matchId = 0, player1Id = 0, player2Id = 0;

            var aTags = row.SelectNodes(".//a");
            if (aTags == null) return (0, 0, 0);

            foreach (var a in aTags)
            {
                var href = a.GetAttributeValue("href", "");

                if (href.Contains("ma_id=") && matchId == 0)
                {
                    var m = Regex.Match(href, @"ma_id=(\d+)");
                    if (m.Success) matchId = int.Parse(m.Groups[1].Value);
                }

                if (href.Contains("p1_id=") && player1Id == 0)
                {
                    var m = Regex.Match(href, @"p1_id=(\d+)");
                    if (m.Success) player1Id = int.Parse(m.Groups[1].Value);
                }

                if (href.Contains("p2_id=") && player2Id == 0)
                {
                    var m = Regex.Match(href, @"p2_id=(\d+)");
                    if (m.Success) player2Id = int.Parse(m.Groups[1].Value);
                }
            }

            return (matchId, player1Id, player2Id);
        }

        public static void ParseMatchRows(HtmlNode matchRowP1, HtmlNode matchRowP2, Models.Match match)
        {
            // VRIJEME MEČA (iz prvog reda ima rowspan=2; fallback i na drugi)
            var timeNode1 = matchRowP1.SelectSingleNode(".//td[starts-with(@class,'main_time')]");
            var timeNode2 = matchRowP2.SelectSingleNode(".//td[starts-with(@class,'main_time')]");
            var timeText = Clean(timeNode1?.InnerText) ?? Clean(timeNode2?.InnerText);

            var timeOfDay = TryParseTimeOfDay(timeText);
            if (timeOfDay.HasValue && match.DateTime.HasValue)
            {
                // spoji lokalni datum (tournamentDate koji si već postavio) + HH:mm
                var localDateTime = CombineLocalDateAndTime(match.DateTime.Value, timeOfDay.Value);

                // 1) ako *u modelu* čuvaš lokalno vrijeme:
                match.DateTime = localDateTime;

                // 2) ako imaš i UTC property (preporuka dodati u model/bazu kao DateTime2):
                // match.DateTimeUtc = ToUtcFromZagreb(localDateTime);
            }

            // Match ID (iz "main_por" td)
            var matchIdNode = matchRowP1.SelectSingleNode(".//td[@class='main_por']//a[contains(@href, 'ma_id=')]");
            var href = matchIdNode?.GetAttributeValue("href", "");
            var matchIdMatch = Regex.Match(href ?? "", @"ma_id=(\d+)");
            if (matchIdMatch.Success && int.TryParse(matchIdMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var mid))
                match.MatchTPId = mid;

            // Rezultat (finalni setovi npr. 2:0)
            var res1 = matchRowP1.SelectSingleNode(".//td[@class='main_res_f']")?.InnerText.Trim();
            var res2 = matchRowP2.SelectSingleNode(".//td[@class='main_res_f']")?.InnerText.Trim();
            match.Result = $"{res1}{res2}";

            // Detalji setova
            var setScoresP1 = matchRowP1.SelectNodes(".//td[@class='main_res']")
                ?.Select(n => Clean(n.InnerText))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList() ?? new();

            var setScoresP2 = matchRowP2.SelectNodes(".//td[@class='main_res']")
                ?.Select(n => Clean(n.InnerText))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList() ?? new();

            // Guard: def./ret./w/o. i slični uzroci – ako ima slova ili točku u bilo kojem tokenu, skip meč
            static bool ContainsAlphaOrDot(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return false;
                foreach (var ch in s)
                    if (char.IsLetter(ch) || ch == '.') return true;
                return false;
            }

            if (setScoresP1.Any(ContainsAlphaOrDot) || setScoresP2.Any(ContainsAlphaOrDot))
            {
                match.IsFinished = false;
                //InvalidResultLogger.Log(
                //    $"[SkipCalc] Alpha/dot token in sets | MatchTPId={match.MatchTPId ?? 0}",
                //    $"P1=[{string.Join(' ', setScoresP1)}] | P2=[{string.Join(' ', setScoresP2)}]"
                //);
                return;
            }

            var combined = new List<string>();
            var len = Math.Min(setScoresP1.Count, setScoresP2.Count);

            for (int i = 0; i < len; i++)
            {
                var p1 = setScoresP1[i]!;
                var p2 = setScoresP2[i]!;

                if (!ResultDetailsValidator.TryExtractInt(p1, out var a)) continue;
                if (!ResultDetailsValidator.TryExtractInt(p2, out var b)) continue;

                var tb = ResultDetailsValidator.ExtractTieBreak(p1) ?? ResultDetailsValidator.ExtractTieBreak(p2);
                combined.Add(SetTokenUtils.BuildCompact(a, b, tb)); // npr. 76(5) ili 108
            }

            match.ResultDetails = string.Join(" ", combined);

            // Konzistencija: broj set tokena mora biti dovoljan za najavljeni rezultat
            var resultCompact = Regex.Replace(match.Result ?? "", @"\D", ""); // npr. "2-0" -> "20"
            int neededSets = resultCompact switch
            {
                "20" or "21" => 2,
                "30" or "31" or "32" => 3,
                _ => 0
            };
            if (neededSets > 0 && combined.Count < neededSets)
            {
                match.IsFinished = false;
                //InvalidResultLogger.Log(
                //    $"[SkipCalc] Too few set tokens for result | MatchTPId={match.MatchTPId ?? 0}",
                //    $"result='{resultCompact}' needs={neededSets}, details='{match.ResultDetails}'"
                //);
                return;
            }

            // Validacija detalja + završna konzistencija rezultat ↔ detalji
            if (ResultDetailsValidator.IsValid(match.ResultDetails, match.Result))
            {
                var (w1, w2) = CountSetsWonFromCompact(match.ResultDetails);
                var fromDetails = $"{w1}{w2}";
                var compactRes = Regex.Replace(match.Result ?? "", @"\D", ""); // npr. "2-1" -> "21"

                if (compactRes != fromDetails)
                {
                    // 🩹 Flaster: izvor tvrdi "21", detalji jasno kažu "30" → prepiši na "30"
                    if (compactRes == "21" && fromDetails == "30")
                    {
                        match.IsFinished = true;
                        match.Result = fromDetails;
                        //InvalidResultLogger.Log(
                        //    $"[Patched] Overridden source result 21→30 based on details | MatchTPId={match.MatchTPId ?? 0}",
                        //    $"details='{match.ResultDetails}'"
                        //);
                    }
                    else
                    {
                        match.IsFinished = false;
                        //InvalidResultLogger.Log(
                        //    $"[SkipCalc] Result/details mismatch | MatchTPId={match.MatchTPId ?? 0}",
                        //    $"result='{match.Result}' compact='{compactRes}' vs details='{match.ResultDetails}' => '{fromDetails}'"
                        //);
                        return;
                    }
                }
                else
                {
                    // sve konzistentno
                    match.IsFinished = true;
                    match.Result = fromDetails; // normalizirani zbirni ishod
                }
            }
            else
            {
                match.IsFinished = false;
                //InvalidResultLogger.Log(
                //    $"MatchTPId={match.MatchTPId ?? 0}, TE={match.TournamentEventTPId ?? 0}",
                //    match.ResultDetails ?? "(null)"
                //);
                return;
            }


            // Koeficijenti i postoci
            var oddsNodeP1 = matchRowP1.SelectSingleNode(".//td[contains(@class, 'main_odds_m')]");
            var percNodeP1 = matchRowP1.SelectSingleNode(".//td[contains(@class, 'main_perc')]");

            var oddsNodeP2 = matchRowP2.SelectSingleNode(".//td[contains(@class, 'main_odds_m')]");
            var percNodeP2 = matchRowP2.SelectSingleNode(".//td[contains(@class, 'main_perc')]");

            match.Player1Odds = TryParseDouble(oddsNodeP1?.InnerText);
            match.Player2Odds = TryParseDouble(oddsNodeP2?.InnerText);

            match.Player1Percentage = TryParseDouble(percNodeP1?.InnerText.Replace("%", ""));
            match.Player2Percentage = TryParseDouble(percNodeP2?.InnerText.Replace("%", ""));
        }


        private static (int p1, int p2) CountSetsWonFromCompact(string resultDetails)
        {
            int p1 = 0, p2 = 0;
            foreach (var tok in resultDetails.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (SetTokenUtils.TryParseFlexible(tok, out var a, out var b, out _))
                {
                    if (a > b) p1++; else if (b > a) p2++;
                }
            }
            return (p1, p2);
        }

        private static string? Clean(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return s.Replace('\u00A0', ' ').Trim();
        }

        private static string? RemovePercent(string? s) => Clean(s)?.Replace("%", "").Trim();

        private static double? TryParseDouble(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            input = input.Replace(",", ".").Trim();
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                ? result : (double?)null;
        }

        private static TimeSpan? TryParseTimeOfDay(string? s)
        {
            s = Clean(s);
            if (string.IsNullOrWhiteSpace(s)) return null;

            // podrži "10:00", "10.00", eventualno razmake
            var m = Regex.Match(s, @"\b(?<h>[01]?\d|2[0-3])[:\.](?<m>[0-5]\d)\b");
            if (!m.Success) return null;

            var h = int.Parse(m.Groups["h"].Value, CultureInfo.InvariantCulture);
            var mm = int.Parse(m.Groups["m"].Value, CultureInfo.InvariantCulture);
            return new TimeSpan(h, mm, 0);
        }

        private static DateTime CombineLocalDateAndTime(DateTime localDateOnly, TimeSpan timeOfDay)
        {
            // localDateOnly ima 00:00; tretiramo ga kao "Europe/Zagreb" lokalni
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb");
            var local = DateTime.SpecifyKind(localDateOnly.Date + timeOfDay, DateTimeKind.Unspecified);
            // Ako želiš ostaviti lokalno u modelu, vrati 'local'.
            // Ako želiš odmah UTC, vrati ConvertTimeToUtc(local, tz)
            return local;
        }

        private static DateTime ToUtcFromZagreb(DateTime localDateTime)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb");
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified), tz);
        }
    }
}