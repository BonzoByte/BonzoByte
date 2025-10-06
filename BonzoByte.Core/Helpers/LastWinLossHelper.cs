using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class LastWinLossHelper
    {
        public static void InjectLastWinLossInfo(Models.Match match, List<Models.Match> previousMatches)
        {
            if (match.Player1TPId == null || match.Player2TPId == null || match.DateTime == null)
                return;

            var now = match.DateTime.Value;
            var weekAgo = now.AddDays(-7);
            var monthAgo = now.AddMonths(-1);
            var yearAgo = now.AddYears(-1);

            // Player 1
            InjectForPlayer(match, previousMatches, match.Player1TPId.Value, isPlayer1: true, now, weekAgo, monthAgo, yearAgo);

            // Player 2
            InjectForPlayer(match, previousMatches, match.Player2TPId.Value, isPlayer1: false, now, weekAgo, monthAgo, yearAgo);
        }

        private static void InjectForPlayer(
            Models.Match match,
            List<Models.Match> prev,
            int playerId,
            bool isPlayer1,
            DateTime now,
            DateTime weekAgo,
            DateTime monthAgo,
            DateTime yearAgo)
        {
            // Pre-filter: samo završeni mečevi prije "now" u kojima je igrač sudjelovao
            var playerPrev = prev.Where(m =>
                    m.DateTime != null &&
                    m.DateTime.Value < now &&
                    (m.Player1TPId == playerId || m.Player2TPId == playerId) &&
                    IsFinished(m.Result))
                .ToList();

            // ===== 1) DaysSince (global) =====
            var lastWin = LastDate(playerPrev, playerId, surfaceId: null, predicateWin: true);
            var lastLoss = LastDate(playerPrev, playerId, surfaceId: null, predicateWin: false);

            if (isPlayer1)
            {
                match.Player1DaysSinceLastWin = DaysSince(lastWin, now);
                match.Player1DaysSinceLastLoss = DaysSince(lastLoss, now);
            }
            else
            {
                match.Player2DaysSinceLastWin = DaysSince(lastWin, now);
                match.Player2DaysSinceLastLoss = DaysSince(lastLoss, now);
            }

            // ===== 2) Wins/Losses — GLOBAL last week/month/year =====
            int wW = Count(playerPrev, playerId, null, weekAgo, now, wantWin: true);
            int lW = Count(playerPrev, playerId, null, weekAgo, now, wantWin: false);
            int wM = Count(playerPrev, playerId, null, monthAgo, now, wantWin: true);
            int lM = Count(playerPrev, playerId, null, monthAgo, now, wantWin: false);
            int wY = Count(playerPrev, playerId, null, yearAgo, now, wantWin: true);
            int lY = Count(playerPrev, playerId, null, yearAgo, now, wantWin: false);

            if (isPlayer1)
            {
                match.Player1WinsLastWeek = wW; match.Player1LossesLastWeek = lW;
                match.Player1WinsLastMonth = wM; match.Player1LossesLastMonth = lM;
                match.Player1WinsLastYear = wY; match.Player1LossesLastYear = lY;
            }
            else
            {
                match.Player2WinsLastWeek = wW; match.Player2LossesLastWeek = lW;
                match.Player2WinsLastMonth = wM; match.Player2LossesLastMonth = lM;
                match.Player2WinsLastYear = wY; match.Player2LossesLastYear = lY;
            }

            // ===== 3) DaysSince + Wins/Losses — PO PODLOGAMA S1..S4 =====
            for (int s = 1; s <= 4; s++)
            {
                var lastWinS = LastDate(playerPrev, playerId, s, predicateWin: true);
                var lastLossS = LastDate(playerPrev, playerId, s, predicateWin: false);
                var dWinS = DaysSince(lastWinS, now);
                var dLossS = DaysSince(lastLossS, now);

                if (isPlayer1)
                {
                    if (s == 1) { match.Player1DaysSinceLastWinS1 = dWinS; match.Player1DaysSinceLastLossS1 = dLossS; }
                    if (s == 2) { match.Player1DaysSinceLastWinS2 = dWinS; match.Player1DaysSinceLastLossS2 = dLossS; }
                    if (s == 3) { match.Player1DaysSinceLastWinS3 = dWinS; match.Player1DaysSinceLastLossS3 = dLossS; }
                    if (s == 4) { match.Player1DaysSinceLastWinS4 = dWinS; match.Player1DaysSinceLastLossS4 = dLossS; }
                }
                else
                {
                    if (s == 1) { match.Player2DaysSinceLastWinS1 = dWinS; match.Player2DaysSinceLastLossS1 = dLossS; }
                    if (s == 2) { match.Player2DaysSinceLastWinS2 = dWinS; match.Player2DaysSinceLastLossS2 = dLossS; }
                    if (s == 3) { match.Player2DaysSinceLastWinS3 = dWinS; match.Player2DaysSinceLastLossS3 = dLossS; }
                    if (s == 4) { match.Player2DaysSinceLastWinS4 = dWinS; match.Player2DaysSinceLastLossS4 = dLossS; }
                }

                int wsW = Count(playerPrev, playerId, s, weekAgo, now, wantWin: true);
                int lsW = Count(playerPrev, playerId, s, weekAgo, now, wantWin: false);
                int wsM = Count(playerPrev, playerId, s, monthAgo, now, wantWin: true);
                int lsM = Count(playerPrev, playerId, s, monthAgo, now, wantWin: false);
                int wsY = Count(playerPrev, playerId, s, yearAgo, now, wantWin: true);
                int lsY = Count(playerPrev, playerId, s, yearAgo, now, wantWin: false);

                if (isPlayer1)
                {
                    if (s == 1)
                    {
                        match.Player1WinsLastWeekS1 = wsW; match.Player1LossesLastWeekS1 = lsW;
                        match.Player1WinsLastMonthS1 = wsM; match.Player1LossesLastMonthS1 = lsM;
                        match.Player1WinsLastYearS1 = wsY; match.Player1LossesLastYearS1 = lsY;
                    }
                    if (s == 2)
                    {
                        match.Player1WinsLastWeekS2 = wsW; match.Player1LossesLastWeekS2 = lsW;
                        match.Player1WinsLastMonthS2 = wsM; match.Player1LossesLastMonthS2 = lsM;
                        match.Player1WinsLastYearS2 = wsY; match.Player1LossesLastYearS2 = lsY;
                    }
                    if (s == 3)
                    {
                        match.Player1WinsLastWeekS3 = wsW; match.Player1LossesLastWeekS3 = lsW;
                        match.Player1WinsLastMonthS3 = wsM; match.Player1LossesLastMonthS3 = lsM;
                        match.Player1WinsLastYearS3 = wsY; match.Player1LossesLastYearS3 = lsY;
                    }
                    if (s == 4)
                    {
                        match.Player1WinsLastWeekS4 = wsW; match.Player1LossesLastWeekS4 = lsW;
                        match.Player1WinsLastMonthS4 = wsM; match.Player1LossesLastMonthS4 = lsM;
                        match.Player1WinsLastYearS4 = wsY; match.Player1LossesLastYearS4 = lsY;
                    }
                }
                else
                {
                    if (s == 1)
                    {
                        match.Player2WinsLastWeekS1 = wsW; match.Player2LossesLastWeekS1 = lsW;
                        match.Player2WinsLastMonthS1 = wsM; match.Player2LossesLastMonthS1 = lsM;
                        match.Player2WinsLastYearS1 = wsY; match.Player2LossesLastYearS1 = lsY;
                    }
                    if (s == 2)
                    {
                        match.Player2WinsLastWeekS2 = wsW; match.Player2LossesLastWeekS2 = lsW;
                        match.Player2WinsLastMonthS2 = wsM; match.Player2LossesLastMonthS2 = lsM;
                        match.Player2WinsLastYearS2 = wsY; match.Player2LossesLastYearS2 = lsY;
                    }
                    if (s == 3)
                    {
                        match.Player2WinsLastWeekS3 = wsW; match.Player2LossesLastWeekS3 = lsW;
                        match.Player2WinsLastMonthS3 = wsM; match.Player2LossesLastMonthS3 = lsM;
                        match.Player2WinsLastYearS3 = wsY; match.Player2LossesLastYearS3 = lsY;
                    }
                    if (s == 4)
                    {
                        match.Player2WinsLastWeekS4 = wsW; match.Player2LossesLastWeekS4 = lsW;
                        match.Player2WinsLastMonthS4 = wsM; match.Player2LossesLastMonthS4 = lsM;
                        match.Player2WinsLastYearS4 = wsY; match.Player2LossesLastYearS4 = lsY;
                    }
                }
            }
        }

        // ===== Centralna win/loss logika (ne ovisi o P1/P2 ulozi) =====

        private static int Count(List<Models.Match> pool, int playerId, int? surfaceId, DateTime since, DateTime until, bool wantWin)
        {
            var q = pool.Where(m =>
                m.DateTime!.Value >= since &&
                m.DateTime!.Value < until &&
                (surfaceId == null || m.SurfaceId == surfaceId));

            int count = 0;
            foreach (var m in q)
            {
                bool p1Won = P1Won(m.Result);
                bool playerIsP1 = m.Player1TPId == playerId;
                bool playerIsP2 = m.Player2TPId == playerId;
                if (!playerIsP1 && !playerIsP2) continue;

                bool isWin = (playerIsP1 && p1Won) || (playerIsP2 && !p1Won);
                if (isWin == wantWin) count++;
            }
            return count;
        }

        private static DateTime? LastDate(List<Models.Match> pool, int playerId, int? surfaceId, bool predicateWin)
        {
            DateTime? last = null;
            foreach (var m in pool)
            {
                if (surfaceId != null && m.SurfaceId != surfaceId) continue;

                bool p1Won = P1Won(m.Result);
                bool playerIsP1 = m.Player1TPId == playerId;
                bool playerIsP2 = m.Player2TPId == playerId;
                if (!playerIsP1 && !playerIsP2) continue;

                bool isWin = (playerIsP1 && p1Won) || (playerIsP2 && !p1Won);
                if (isWin != predicateWin) continue;

                var dt = m.DateTime!.Value;
                if (last == null || dt > last.Value) last = dt;
            }
            return last;
        }

        // Brzi parser rezultata: “2-0”, “2:1”, “2 : 0 (ret.)”, …
        // true  => P1 je pobijedio
        // false => P2 je pobijedio
        private static bool P1Won(string? result)
        {
            var m = Regex.Match(result ?? "", @"^\s*(\d)\D*(\d)");
            if (!m.Success) return true; // fallback (ali u poolu već filtriramo IsFinished)
            int a = m.Groups[1].Value[0] - '0';
            int b = m.Groups[2].Value[0] - '0';
            return a >= b;
        }

        private static bool IsFinished(string? result)
        {
            var m = Regex.Match(result ?? "", @"^\s*(\d)\D*(\d)");
            return m.Success;
        }

        private static int? DaysSince(DateTime? last, DateTime now)
            => last == null ? (int?)null : (int)(now.Date - last.Value.Date).TotalDays;
    }
}