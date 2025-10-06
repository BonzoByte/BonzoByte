using BonzoByte.Core.Models;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class PerformanceStatsCalculator
    {
        public static void UpdatePlayerPerformanceStats(
            Models.Match currentMatch,
            List<Models.Match> allMatches,
            List<Models.Match> lastWeek,
            List<Models.Match> lastMonth,
            List<Models.Match> lastYear,
            Player p1,
            Player p2)
        {
            // 1) Global + po podlogama → puni Player snapshot do currentMatch (strictly < now)
            UpdateSinglePlayerStats(currentMatch, allMatches, lastWeek, lastMonth, lastYear, p1);
            UpdateSinglePlayerStats(currentMatch, allMatches, lastWeek, lastMonth, lastYear, p2);

            // 2) Favourite/Underdog → prema prosjeku M/SM/GSM za igrača
            UpdateFavouriteUnderdogStats(currentMatch, allMatches, p1, "");
            UpdateFavouriteUnderdogStats(currentMatch, lastYear, p1, "LastYear");
            UpdateFavouriteUnderdogStats(currentMatch, lastMonth, p1, "LastMonth");
            UpdateFavouriteUnderdogStats(currentMatch, lastWeek, p1, "LastWeek");

            UpdateFavouriteUnderdogStats(currentMatch, allMatches, p2, "");
            UpdateFavouriteUnderdogStats(currentMatch, lastYear, p2, "LastYear");
            UpdateFavouriteUnderdogStats(currentMatch, lastMonth, p2, "LastMonth");
            UpdateFavouriteUnderdogStats(currentMatch, lastWeek, p2, "LastWeek");
        }

        // ---------- Core ----------

        private static void UpdateSinglePlayerStats(
            Models.Match currentMatch,
            List<Models.Match> all,
            List<Models.Match> week,
            List<Models.Match> month,
            List<Models.Match> year,
            Player player)
        {
            var now = currentMatch.DateTime!.Value;
            int pid = player.PlayerTPId ?? 0;

            // pre-match (strictly before current)
            var totalSet = PreMatch(all, now);
            var weekSet = PreMatch(week, now);
            var monthSet = PreMatch(month, now);
            var yearSet = PreMatch(year, now);

            // TOTAL
            FillBlock(player, "Total", pid, totalSet);
            // LAST WEEK
            FillBlock(player, "LastWeek", pid, weekSet);
            // LAST MONTH
            FillBlock(player, "LastMonth", pid, monthSet);
            // LAST YEAR
            FillBlock(player, "LastYear", pid, yearSet);
        }

        private static void FillBlock(Player player, string prefix, int pid, IEnumerable<Models.Match> src)
        {
            var matches = src.Where(m => m.Player1TPId == pid || m.Player2TPId == pid).ToList();

            // GLOBAL wins/losses
            int wins = matches.Count(m => DidPlayerWin(m, pid));
            int losses = matches.Count - wins;
            SetWinsLosses(player, prefix, wins, losses);

            // GLOBAL sets
            int setsWon = matches.Sum(m => m.Player1TPId == pid ? (m.P1SetsWon ?? 0) : (m.P2SetsWon ?? 0));
            int setsLost = matches.Sum(m => m.Player1TPId == pid ? (m.P1SetsLoss ?? 0) : (m.P2SetsLoss ?? 0));
            SetSetsWinsLosses(player, prefix, setsWon, setsLost);

            // GLOBAL games
            int gamesWon = matches.Sum(m => m.Player1TPId == pid ? (m.P1GamesWon ?? 0) : (m.P2GamesWon ?? 0));
            int gamesLost = matches.Sum(m => m.Player1TPId == pid ? (m.P1GamesLoss ?? 0) : (m.P2GamesLoss ?? 0));
            SetGamesWinsLosses(player, prefix, gamesWon, gamesLost);

            // BY SURFACE S1..S4
            for (int s = 1; s <= 4; s++)
            {
                var ms = matches.Where(m => m.SurfaceId == s).ToList();

                int wS = ms.Count(m => DidPlayerWin(m, pid));
                int lS = ms.Count - wS;
                SetWinsLossesSurface(player, prefix, s, wS, lS);

                int setsWonS = ms.Sum(m => m.Player1TPId == pid ? (m.P1SetsWon ?? 0) : (m.P2SetsWon ?? 0));
                int setsLostS = ms.Sum(m => m.Player1TPId == pid ? (m.P1SetsLoss ?? 0) : (m.P2SetsLoss ?? 0));
                SetSetsWinsLossesSurface(player, prefix, s, setsWonS, setsLostS);

                int gamesWonS = ms.Sum(m => m.Player1TPId == pid ? (m.P1GamesWon ?? 0) : (m.P2GamesWon ?? 0));
                int gamesLostS = ms.Sum(m => m.Player1TPId == pid ? (m.P1GamesLoss ?? 0) : (m.P2GamesLoss ?? 0));
                SetGamesWinsLossesSurface(player, prefix, s, gamesWonS, gamesLostS);
            }
        }

        private static void UpdateFavouriteUnderdogStats(
            Models.Match currentMatch,
            List<Models.Match> matches,
            Player player,
            string prefix)
        {
            var now = currentMatch.DateTime!.Value;
            int pid = player.PlayerTPId ?? 0;

            var filtered = PreMatch(matches, now)
                .Where(m => m.Player1TPId == pid || m.Player2TPId == pid)
                .ToList();

            int winsFav = 0, winsDog = 0, lossesFav = 0, lossesDog = 0;
            double winProbAtWinFav = 0, winProbAtWinDog = 0;
            double winProbAtLossFav = 0, winProbAtLossDog = 0;
            int winFavCount = 0, winDogCount = 0, lossFavCount = 0, lossDogCount = 0;

            foreach (var m in filtered)
            {
                var pAvg = PlayerAvgWinProb(m, pid); // prosjek M/SM/GSM mapiran na igrača
                if (!pAvg.HasValue) continue;

                bool isFav = pAvg.Value >= 0.5;
                bool didWin = DidPlayerWin(m, pid);

                if (didWin)
                {
                    if (isFav) { winsFav++; winProbAtWinFav += pAvg.Value; winFavCount++; }
                    else { winsDog++; winProbAtWinDog += pAvg.Value; winDogCount++; }
                }
                else
                {
                    if (isFav) { lossesFav++; winProbAtLossFav += pAvg.Value; lossFavCount++; }
                    else { lossesDog++; winProbAtLossDog += pAvg.Value; lossDogCount++; }
                }
            }

            SetFavDogCounts(player, prefix, winsFav, winsDog, lossesFav, lossesDog);
            SetFavDogAverages(
                player, prefix,
                winFavCount > 0 ? winProbAtWinFav / winFavCount : 0,
                winDogCount > 0 ? winProbAtWinDog / winDogCount : 0,
                lossFavCount > 0 ? winProbAtLossFav / lossFavCount : 0,
                lossDogCount > 0 ? winProbAtLossDog / lossDogCount : 0
            );

            // Ako želiš i *ratio* polja, dodaj u setter i ovdje ih izračunaj.
        }

        // ---------- Win/Loss & WinProb helpers ----------

        private static IEnumerable<Models.Match> PreMatch(IEnumerable<Models.Match> src, DateTime nowExclusive)
            => src.Where(m => m.DateTime.HasValue && m.DateTime.Value < nowExclusive);

        private static bool DidPlayerWin(Models.Match m, int playerId)
        {
            bool p1Won = P1Won(m.Result);
            if (m.Player1TPId == playerId) return p1Won;
            if (m.Player2TPId == playerId) return !p1Won;
            return false;
        }

        private static bool P1Won(string? result)
        {
            if (string.IsNullOrWhiteSpace(result)) return true; // fallback
            // hvata "2-0", "2:1", " 2 : 0 (ret.)", "20"...
            var mm = Regex.Match(result, @"^\s*(\d)\D*(\d)");
            if (mm.Success)
            {
                int a = mm.Groups[1].Value[0] - '0';
                int b = mm.Groups[2].Value[0] - '0';
                return a >= b;
            }
            return result is "20" or "21" or "30" or "31" or "32";
        }

        private static double? PlayerAvgWinProb(Models.Match m, int playerId)
        {
            var vals = new List<double>(3);

            void add(double? p1)
            {
                if (!p1.HasValue) return;
                if (m.Player1TPId == playerId) vals.Add(p1.Value);
                else if (m.Player2TPId == playerId) vals.Add(1 - p1.Value);
            }

            add(m.WinProbabilityPlayer1M);
            add(m.WinProbabilityPlayer1SM);
            add(m.WinProbabilityPlayer1GSM);

            if (vals.Count == 0) return null;
            double sum = 0;
            for (int i = 0; i < vals.Count; i++) sum += vals[i];
            return sum / vals.Count;
        }

        // ---------- STRONG-TYPED SETTERS (bez refleksije) ----------

        // Wins/Losses GLOBAL by block
        private static void SetWinsLosses(Player p, string prefix, int wins, int losses)
        {
            switch (prefix)
            {
                case "Total": p.WinsTotal = wins; p.LossesTotal = losses; break;
                case "LastWeek": p.WinsLastWeek = wins; p.LossesLastWeek = losses; break;
                case "LastMonth": p.WinsLastMonth = wins; p.LossesLastMonth = losses; break;
                case "LastYear": p.WinsLastYear = wins; p.LossesLastYear = losses; break;
            }
        }

        private static void SetSetsWinsLosses(Player p, string prefix, int winsSets, int lossesSets)
        {
            switch (prefix)
            {
                case "Total": p.WinsSetsTotal = winsSets; p.LossesSetsTotal = lossesSets; break;
                case "LastWeek": p.WinsSetsLastWeek = winsSets; p.LossesSetsLastWeek = lossesSets; break;
                case "LastMonth": p.WinsSetsLastMonth = winsSets; p.LossesSetsLastMonth = lossesSets; break;
                case "LastYear": p.WinsSetsLastYear = winsSets; p.LossesSetsLastYear = lossesSets; break;
            }
        }

        private static void SetGamesWinsLosses(Player p, string prefix, int winsGames, int lossesGames)
        {
            switch (prefix)
            {
                case "Total": p.WinsGamesTotal = winsGames; p.LossesGamesTotal = lossesGames; break;
                case "LastWeek": p.WinsGamesLastWeek = winsGames; p.LossesGamesLastWeek = lossesGames; break;
                case "LastMonth": p.WinsGamesLastMonth = winsGames; p.LossesGamesLastMonth = lossesGames; break;
                case "LastYear": p.WinsGamesLastYear = winsGames; p.LossesGamesLastYear = lossesGames; break;
            }
        }

        // Wins/Losses BY SURFACE
        private static void SetWinsLossesSurface(Player p, string prefix, int s, int wins, int losses)
        {
            switch (prefix)
            {
                case "Total":
                    if (s == 1) { p.WinsTotalS1 = wins; p.LossesTotalS1 = losses; }
                    if (s == 2) { p.WinsTotalS2 = wins; p.LossesTotalS2 = losses; }
                    if (s == 3) { p.WinsTotalS3 = wins; p.LossesTotalS3 = losses; }
                    if (s == 4) { p.WinsTotalS4 = wins; p.LossesTotalS4 = losses; }
                    break;
                case "LastWeek":
                    if (s == 1) { p.WinsLastWeekS1 = wins; p.LossesLastWeekS1 = losses; }
                    if (s == 2) { p.WinsLastWeekS2 = wins; p.LossesLastWeekS2 = losses; }
                    if (s == 3) { p.WinsLastWeekS3 = wins; p.LossesLastWeekS3 = losses; }
                    if (s == 4) { p.WinsLastWeekS4 = wins; p.LossesLastWeekS4 = losses; }
                    break;
                case "LastMonth":
                    if (s == 1) { p.WinsLastMonthS1 = wins; p.LossesLastMonthS1 = losses; }
                    if (s == 2) { p.WinsLastMonthS2 = wins; p.LossesLastMonthS2 = losses; }
                    if (s == 3) { p.WinsLastMonthS3 = wins; p.LossesLastMonthS3 = losses; }
                    if (s == 4) { p.WinsLastMonthS4 = wins; p.LossesLastMonthS4 = losses; }
                    break;
                case "LastYear":
                    if (s == 1) { p.WinsLastYearS1 = wins; p.LossesLastYearS1 = losses; }
                    if (s == 2) { p.WinsLastYearS2 = wins; p.LossesLastYearS2 = losses; }
                    if (s == 3) { p.WinsLastYearS3 = wins; p.LossesLastYearS3 = losses; }
                    if (s == 4) { p.WinsLastYearS4 = wins; p.LossesLastYearS4 = losses; }
                    break;
            }
        }

        private static void SetSetsWinsLossesSurface(Player p, string prefix, int s, int winsSets, int lossesSets)
        {
            switch (prefix)
            {
                case "Total":
                    if (s == 1) { p.WinsSetsTotalS1 = winsSets; p.LossesSetsTotalS1 = lossesSets; }
                    if (s == 2) { p.WinsSetsTotalS2 = winsSets; p.LossesSetsTotalS2 = lossesSets; }
                    if (s == 3) { p.WinsSetsTotalS3 = winsSets; p.LossesSetsTotalS3 = lossesSets; }
                    if (s == 4) { p.WinsSetsTotalS4 = winsSets; p.LossesSetsTotalS4 = lossesSets; }
                    break;
                case "LastWeek":
                    if (s == 1) { p.WinsSetsLastWeekS1 = winsSets; p.LossesSetsLastWeekS1 = lossesSets; }
                    if (s == 2) { p.WinsSetsLastWeekS2 = winsSets; p.LossesSetsLastWeekS2 = lossesSets; }
                    if (s == 3) { p.WinsSetsLastWeekS3 = winsSets; p.LossesSetsLastWeekS3 = lossesSets; }
                    if (s == 4) { p.WinsSetsLastWeekS4 = winsSets; p.LossesSetsLastWeekS4 = lossesSets; }
                    break;
                case "LastMonth":
                    if (s == 1) { p.WinsSetsLastMonthS1 = winsSets; p.LossesSetsLastMonthS1 = lossesSets; }
                    if (s == 2) { p.WinsSetsLastMonthS2 = winsSets; p.LossesSetsLastMonthS2 = lossesSets; }
                    if (s == 3) { p.WinsSetsLastMonthS3 = winsSets; p.LossesSetsLastMonthS3 = lossesSets; }
                    if (s == 4) { p.WinsSetsLastMonthS4 = winsSets; p.LossesSetsLastMonthS4 = lossesSets; }
                    break;
                case "LastYear":
                    if (s == 1) { p.WinsSetsLastYearS1 = winsSets; p.LossesSetsLastYearS1 = lossesSets; }
                    if (s == 2) { p.WinsSetsLastYearS2 = winsSets; p.LossesSetsLastYearS2 = lossesSets; }
                    if (s == 3) { p.WinsSetsLastYearS3 = winsSets; p.LossesSetsLastYearS3 = lossesSets; }
                    if (s == 4) { p.WinsSetsLastYearS4 = winsSets; p.LossesSetsLastYearS4 = lossesSets; }
                    break;
            }
        }

        private static void SetGamesWinsLossesSurface(Player p, string prefix, int s, int winsGames, int lossesGames)
        {
            switch (prefix)
            {
                case "Total":
                    if (s == 1) { p.WinsGamesTotalS1 = winsGames; p.LossesGamesTotalS1 = lossesGames; }
                    if (s == 2) { p.WinsGamesTotalS2 = winsGames; p.LossesGamesTotalS2 = lossesGames; }
                    if (s == 3) { p.WinsGamesTotalS3 = winsGames; p.LossesGamesTotalS3 = lossesGames; }
                    if (s == 4) { p.WinsGamesTotalS4 = winsGames; p.LossesGamesTotalS4 = lossesGames; }
                    break;
                case "LastWeek":
                    if (s == 1) { p.WinsGamesLastWeekS1 = winsGames; p.LossesGamesLastWeekS1 = lossesGames; }
                    if (s == 2) { p.WinsGamesLastWeekS2 = winsGames; p.LossesGamesLastWeekS2 = lossesGames; }
                    if (s == 3) { p.WinsGamesLastWeekS3 = winsGames; p.LossesGamesLastWeekS3 = lossesGames; }
                    if (s == 4) { p.WinsGamesLastWeekS4 = winsGames; p.LossesGamesLastWeekS4 = lossesGames; }
                    break;
                case "LastMonth":
                    if (s == 1) { p.WinsGamesLastMonthS1 = winsGames; p.LossesGamesLastMonthS1 = lossesGames; }
                    if (s == 2) { p.WinsGamesLastMonthS2 = winsGames; p.LossesGamesLastMonthS2 = lossesGames; }
                    if (s == 3) { p.WinsGamesLastMonthS3 = winsGames; p.LossesGamesLastMonthS3 = lossesGames; }
                    if (s == 4) { p.WinsGamesLastMonthS4 = winsGames; p.LossesGamesLastMonthS4 = lossesGames; }
                    break;
                case "LastYear":
                    if (s == 1) { p.WinsGamesLastYearS1 = winsGames; p.LossesGamesLastYearS1 = lossesGames; }
                    if (s == 2) { p.WinsGamesLastYearS2 = winsGames; p.LossesGamesLastYearS2 = lossesGames; }
                    if (s == 3) { p.WinsGamesLastYearS3 = winsGames; p.LossesGamesLastYearS3 = lossesGames; }
                    if (s == 4) { p.WinsGamesLastYearS4 = winsGames; p.LossesGamesLastYearS4 = lossesGames; }
                    break;
            }
        }

        // Favourite/Underdog setteri
        private static void SetFavDogCounts(Player p, string prefix, int winsFav, int winsDog, int lossesFav, int lossesDog)
        {
            switch (prefix)
            {
                case "":
                    p.TotalWinsAsFavourite = winsFav;
                    p.TotalWinsAsUnderdog = winsDog;
                    p.TotalLossesAsFavourite = lossesFav;
                    p.TotalLossesAsUnderdog = lossesDog;
                    break;
                case "LastWeek":
                    p.TotalWinsAsFavouriteLastWeek = winsFav;
                    p.TotalWinsAsUnderdogLastWeek = winsDog;
                    p.TotalLossesAsFavouriteLastWeek = lossesFav;
                    p.TotalLossesAsUnderdogLastWeek = lossesDog;
                    break;
                case "LastMonth":
                    p.TotalWinsAsFavouriteLastMonth = winsFav;
                    p.TotalWinsAsUnderdogLastMonth = winsDog;
                    p.TotalLossesAsFavouriteLastMonth = lossesFav;
                    p.TotalLossesAsUnderdogLastMonth = lossesDog;
                    break;
                case "LastYear":
                    p.TotalWinsAsFavouriteLastYear = winsFav;
                    p.TotalWinsAsUnderdogLastYear = winsDog;
                    p.TotalLossesAsFavouriteLastYear = lossesFav;
                    p.TotalLossesAsUnderdogLastYear = lossesDog;
                    break;
            }
        }

        private static void SetFavDogAverages(Player p, string prefix, double avgWinFav, double avgWinDog, double avgLossFav, double avgLossDog)
        {
            switch (prefix)
            {
                case "":
                    p.AverageWinningProbabilityAtWonAsFavourite = avgWinFav;
                    p.AverageWinningProbabilityAtWonAsUnderdog = avgWinDog;
                    p.AverageWinningProbabilityAtLossAsFavourite = avgLossFav;
                    p.AverageWinningProbabilityAtLossAsUnderdog = avgLossDog;
                    break;
                case "LastWeek":
                    p.AverageWinningProbabilityAtWonAsFavouriteLastWeek = avgWinFav;
                    p.AverageWinningProbabilityAtWonAsUnderdogLastWeek = avgWinDog;
                    p.AverageWinningProbabilityAtLossAsFavouriteLastWeek = avgLossFav;
                    p.AverageWinningProbabilityAtLossAsUnderdogLastWeek = avgLossDog;
                    break;
                case "LastMonth":
                    p.AverageWinningProbabilityAtWonAsFavouriteLastMonth = avgWinFav;
                    p.AverageWinningProbabilityAtWonAsUnderdogLastMonth = avgWinDog;
                    p.AverageWinningProbabilityAtLossAsFavouriteLastMonth = avgLossFav;
                    p.AverageWinningProbabilityAtLossAsUnderdogLastMonth = avgLossDog;
                    break;
                case "LastYear":
                    p.AverageWinningProbabilityAtWonAsFavouriteLastYear = avgWinFav;
                    p.AverageWinningProbabilityAtWonAsUnderdogLastYear = avgWinDog;
                    p.AverageWinningProbabilityAtLossAsFavouriteLastYear = avgLossFav;
                    p.AverageWinningProbabilityAtLossAsUnderdogLastYear = avgLossDog;
                    break;
            }
        }
    }
}