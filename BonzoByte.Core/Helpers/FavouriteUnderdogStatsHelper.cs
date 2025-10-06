namespace BonzoByte.Core.Helpers;

public static class FavouriteUnderdogStatsHelper
{
    public static void UpdateFavouriteUnderdogStatsByTimeFrame(List<Models.Match> lastWeekMatchesToRemove, List<Models.Match> lastMonthMatchesToRemove, List<Models.Match> lastYearMatchesToRemove, List<Models.Player>? players)
    {
        foreach (var m in lastYearMatchesToRemove)
        {
            var p1 = players!.Find(p => p.PlayerTPId == m.Player1TPId);
            var p2 = players!.Find(p => p.PlayerTPId == m.Player2TPId);

            var winProbP1 = (m.WinProbabilityPlayer1M + m.WinProbabilityPlayer1SM + m.WinProbabilityPlayer1GSM) / 3;

            if (winProbP1 > 0.5)
            {
                var p1AverageWinsAsFavouriteLastYear = p1!.AverageWinningProbabilityAtWonAsFavouriteLastYear ?? 0.5;
                var p2AverageLossAsUnderdogLastYear = p2!.AverageWinningProbabilityAtLossAsUnderdogLastYear ?? 0.5;
                var p1AverageWinsAsFavouriteLastYearNew = ((p1.TotalWinsAsFavouriteLastYear * p1AverageWinsAsFavouriteLastYear) - winProbP1) / (p1.TotalWinsAsFavouriteLastYear - 1 == 0 ? 0.5 : p1.TotalWinsAsFavouriteLastYear - 1);
                var p2AverageLossAsUnderdogLastYearNew = ((p2.TotalLossesAsUnderdogLastYear * p2AverageLossAsUnderdogLastYear) - (1 - winProbP1)) / (p2.TotalLossesAsUnderdogLastYear - 1 == 0 ? 0.5 : p2.TotalLossesAsUnderdogLastYear - 1);

                p1.TotalWinsAsFavouriteLastYear--;
                p2.TotalLossesAsUnderdogLastYear--;
                p1.AverageWinningProbabilityAtWonAsFavouriteLastYear = p1AverageWinsAsFavouriteLastYearNew;
                p2.AverageWinningProbabilityAtLossAsUnderdogLastYear = p2AverageLossAsUnderdogLastYearNew;
            }
            else if (winProbP1 < 0.5)
            {
                var p1AverageWinsAsUnderdogLastYear = p1.AverageWinningProbabilityAtWonAsUnderdogLastYear ?? 0.5;
                var p2AverageLossAsFavouriteLastYear = p2.AverageWinningProbabilityAtLossAsFavouriteLastYear ?? 0.5;
                var p1AverageWinsAsUnderdogLastYearNew = ((p1.TotalWinsAsUnderdogLastYear * p1AverageWinsAsUnderdogLastYear) - winProbP1) / (p1.TotalWinsAsUnderdogLastYear - 1 == 0 ? 0.5 : p1.TotalWinsAsUnderdogLastYear - 1);
                var p2AverageLossAsFavouriteLastYearNew = ((p2.TotalLossesAsFavouriteLastYear * p2AverageLossAsFavouriteLastYear) - (1 - winProbP1)) / (p2.TotalLossesAsFavouriteLastYear - 1 == 0 ? 0.5 : p2.TotalLossesAsFavouriteLastYear - 1);

                p1.TotalWinsAsUnderdogLastYear--;
                p2.TotalLossesAsFavouriteLastYear--;
                p1.AverageWinningProbabilityAtWonAsUnderdogLastYear = p1AverageWinsAsUnderdogLastYearNew;
                p2.AverageWinningProbabilityAtLossAsFavouriteLastYear = p2AverageLossAsFavouriteLastYearNew;
            }
        }

        foreach (var m in lastMonthMatchesToRemove)
        {
            var p1 = players!.Find(p => p.PlayerTPId == m.Player1TPId);
            var p2 = players!.Find(p => p.PlayerTPId == m.Player2TPId);

            var winProbP1 = (m.WinProbabilityPlayer1M + m.WinProbabilityPlayer1SM + m.WinProbabilityPlayer1GSM) / 3;

            if (winProbP1 > 0.5)
            {
                var p1AverageWinsAsFavouriteLastMonth = p1!.AverageWinningProbabilityAtWonAsFavouriteLastMonth ?? 0.5;
                var p2AverageLossAsUnderdogLastMonth = p2!.AverageWinningProbabilityAtLossAsUnderdogLastMonth ?? 0.5;
                var p1AverageWinsAsFavouriteLastMonthNew = ((p1.TotalWinsAsFavouriteLastMonth * p1AverageWinsAsFavouriteLastMonth) - winProbP1) / (p1.TotalWinsAsFavouriteLastMonth - 1 == 0 ? 0.5 : p1.TotalWinsAsFavouriteLastMonth - 1);
                var p2AverageLossAsUnderdogLastMonthNew = ((p2.TotalLossesAsUnderdogLastMonth * p2AverageLossAsUnderdogLastMonth) - (1 - winProbP1)) / (p2.TotalLossesAsUnderdogLastMonth - 1 == 0 ? 0.5 : p2.TotalLossesAsUnderdogLastMonth - 1);

                p1.TotalWinsAsFavouriteLastMonth--;
                p2.TotalLossesAsUnderdogLastMonth--;
                p1.AverageWinningProbabilityAtWonAsFavouriteLastMonth = p1AverageWinsAsFavouriteLastMonthNew;
                p2.AverageWinningProbabilityAtLossAsUnderdogLastMonth = p2AverageLossAsUnderdogLastMonthNew;
            }
            else if (winProbP1 < 0.5)
            {
                var p1AverageWinsAsUnderdogLastMonth = p1.AverageWinningProbabilityAtWonAsUnderdogLastMonth ?? 0.5;
                var p2AverageLossAsFavouriteLastMonth = p2.AverageWinningProbabilityAtLossAsFavouriteLastMonth ?? 0.5;
                var p1AverageWinsAsUnderdogLastMonthNew = ((p1.TotalWinsAsUnderdogLastMonth * p1AverageWinsAsUnderdogLastMonth) - winProbP1) / (p1.TotalWinsAsUnderdogLastMonth - 1 == 0 ? 0.5 : p1.TotalWinsAsUnderdogLastMonth - 1);
                var p2AverageLossAsFavouriteLastMonthNew = ((p2.TotalLossesAsFavouriteLastMonth * p2AverageLossAsFavouriteLastMonth) - (1 - winProbP1)) / (p2.TotalLossesAsFavouriteLastMonth - 1 == 0 ? 0.5 : p2.TotalLossesAsFavouriteLastMonth - 1);

                p1.TotalWinsAsUnderdogLastMonth--;
                p2.TotalLossesAsFavouriteLastMonth--;
                p1.AverageWinningProbabilityAtWonAsUnderdogLastMonth = p1AverageWinsAsUnderdogLastMonthNew;
                p2.AverageWinningProbabilityAtLossAsFavouriteLastMonth = p2AverageLossAsFavouriteLastMonthNew;
            }
        }
        foreach (var m in lastWeekMatchesToRemove)
        {
            var p1 = players!.Find(p => p.PlayerTPId == m.Player1TPId);
            var p2 = players!.Find(p => p.PlayerTPId == m.Player2TPId);

            var winProbP1 = (m.WinProbabilityPlayer1M + m.WinProbabilityPlayer1SM + m.WinProbabilityPlayer1GSM) / 3;

            if (winProbP1 > 0.5)
            {
                var p1AverageWinsAsFavouriteLastWeek = p1!.AverageWinningProbabilityAtWonAsFavouriteLastWeek ?? 0.5;
                var p2AverageLossAsUnderdogLastWeek = p2!.AverageWinningProbabilityAtLossAsUnderdogLastWeek ?? 0.5;
                var p1AverageWinsAsFavouriteLastWeekNew = ((p1.TotalWinsAsFavouriteLastWeek * p1AverageWinsAsFavouriteLastWeek) - winProbP1) / (p1.TotalWinsAsFavouriteLastWeek - 1 == 0 ? 0.5 : p1.TotalWinsAsFavouriteLastWeek - 1);
                var p2AverageLossAsUnderdogLastWeekNew = ((p2.TotalLossesAsUnderdogLastWeek * p2AverageLossAsUnderdogLastWeek) - (1 - winProbP1)) / (p2.TotalLossesAsUnderdogLastWeek - 1 == 0 ? 0.5 : p2.TotalLossesAsUnderdogLastWeek - 1);

                p1.TotalWinsAsFavouriteLastWeek--;
                p2.TotalLossesAsUnderdogLastWeek--;
                p1.AverageWinningProbabilityAtWonAsFavouriteLastWeek = p1AverageWinsAsFavouriteLastWeekNew;
                p2.AverageWinningProbabilityAtLossAsUnderdogLastWeek = p2AverageLossAsUnderdogLastWeekNew;
            }
            else if (winProbP1 < 0.5)
            {
                var p1AverageWinsAsUnderdogLastWeek = p1.AverageWinningProbabilityAtWonAsUnderdogLastWeek ?? 0.5;
                var p2AverageLossAsFavouriteLastWeek = p2.AverageWinningProbabilityAtLossAsFavouriteLastWeek ?? 0.5;
                var p1AverageWinsAsUnderdogLastWeekNew = ((p1.TotalWinsAsUnderdogLastWeek * p1AverageWinsAsUnderdogLastWeek) - winProbP1) / (p1.TotalWinsAsUnderdogLastWeek - 1 == 0 ? 0.5 : p1.TotalWinsAsUnderdogLastWeek - 1);
                var p2AverageLossAsFavouriteLastWeekNew = ((p2.TotalLossesAsFavouriteLastWeek * p2AverageLossAsFavouriteLastWeek) - (1 - winProbP1)) / (p2.TotalLossesAsFavouriteLastWeek - 1 == 0 ? 0.5 : p2.TotalLossesAsFavouriteLastWeek - 1);
                
                p1.TotalWinsAsUnderdogLastWeek--;
                p2.TotalLossesAsFavouriteLastWeek--;
                p1.AverageWinningProbabilityAtWonAsUnderdogLastWeek = p1AverageWinsAsUnderdogLastWeekNew;
                p2.AverageWinningProbabilityAtLossAsFavouriteLastWeek = p2AverageLossAsFavouriteLastWeekNew;
            }
        }
    }

    public static void UpdateFavouriteUnderdogStats(Models.Match match, Models.Player p1, Models.Player p2)
    {
        match.Player1TotalWinsAsFavourite                                = p1.TotalWinsAsFavourite;
        match.Player2TotalWinsAsFavourite                                = p2.TotalWinsAsFavourite;
        match.Player1TotalWinsAsUnderdog                                 = p1.TotalWinsAsUnderdog;
        match.Player2TotalWinsAsUnderdog                                 = p2.TotalWinsAsUnderdog;
        match.Player1TotalLossesAsFavourite                              = p1.TotalLossesAsFavourite;
        match.Player2TotalLossesAsFavourite                              = p2.TotalLossesAsFavourite;
        match.Player1TotalLossesAsUnderdog                               = p1.TotalLossesAsUnderdog;
        match.Player2TotalLossesAsUnderdog                               = p2.TotalLossesAsUnderdog;
        match.Player1AverageWinningProbabilityAtWonAsFavourite           = p1.AverageWinningProbabilityAtWonAsFavourite;
        match.Player2AverageWinningProbabilityAtWonAsFavourite           = p2.AverageWinningProbabilityAtWonAsFavourite;
        match.Player1AverageWinningProbabilityAtWonAsUnderdog            = p1.AverageWinningProbabilityAtWonAsUnderdog;
        match.Player2AverageWinningProbabilityAtWonAsUnderdog            = p2.AverageWinningProbabilityAtWonAsUnderdog;
        match.Player1AverageWinningProbabilityAtLossAsFavourite          = p1.AverageWinningProbabilityAtLossAsFavourite;
        match.Player2AverageWinningProbabilityAtLossAsFavourite          = p2.AverageWinningProbabilityAtLossAsFavourite;
        match.Player1AverageWinningProbabilityAtLossAsUnderdog           = p1.AverageWinningProbabilityAtLossAsUnderdog;
        match.Player2AverageWinningProbabilityAtLossAsUnderdog           = p2.AverageWinningProbabilityAtLossAsUnderdog;
        match.Player1TotalWinsAsFavouriteLastYear                        = p1.TotalWinsAsFavouriteLastYear;
        match.Player2TotalWinsAsFavouriteLastYear                        = p2.TotalWinsAsFavouriteLastYear;
        match.Player1TotalWinsAsUnderdogLastYear                         = p1.TotalWinsAsUnderdogLastYear;
        match.Player2TotalWinsAsUnderdogLastYear                         = p2.TotalWinsAsUnderdogLastYear;
        match.Player1TotalLossesAsFavouriteLastYear                      = p1.TotalLossesAsFavouriteLastYear;
        match.Player2TotalLossesAsFavouriteLastYear                      = p2.TotalLossesAsFavouriteLastYear;
        match.Player1TotalLossesAsUnderdogLastYear                       = p1.TotalLossesAsUnderdogLastYear;
        match.Player2TotalLossesAsUnderdogLastYear                       = p2.TotalLossesAsUnderdogLastYear;
        match.Player1AverageWinningProbabilityAtWonAsFavouriteLastYear   = p1.AverageWinningProbabilityAtWonAsFavouriteLastYear;
        match.Player2AverageWinningProbabilityAtWonAsFavouriteLastYear   = p2.AverageWinningProbabilityAtWonAsFavouriteLastYear;
        match.Player1AverageWinningProbabilityAtWonAsUnderdogLastYear    = p1.AverageWinningProbabilityAtWonAsUnderdogLastYear;
        match.Player2AverageWinningProbabilityAtWonAsUnderdogLastYear    = p2.AverageWinningProbabilityAtWonAsUnderdogLastYear;
        match.Player1AverageWinningProbabilityAtLossAsFavouriteLastYear  = p1.AverageWinningProbabilityAtLossAsFavouriteLastYear;
        match.Player2AverageWinningProbabilityAtLossAsFavouriteLastYear  = p2.AverageWinningProbabilityAtLossAsFavouriteLastYear;
        match.Player1AverageWinningProbabilityAtLossAsUnderdogLastYear   = p1.AverageWinningProbabilityAtLossAsUnderdogLastYear;
        match.Player2AverageWinningProbabilityAtLossAsUnderdogLastYear   = p2.AverageWinningProbabilityAtLossAsUnderdogLastYear;
        match.Player1TotalWinsAsFavouriteLastMonth                       = p1.TotalWinsAsFavouriteLastMonth;
        match.Player2TotalWinsAsFavouriteLastMonth                       = p2.TotalWinsAsFavouriteLastMonth;
        match.Player1TotalWinsAsUnderdogLastMonth                        = p1.TotalWinsAsUnderdogLastMonth;
        match.Player2TotalWinsAsUnderdogLastMonth                        = p2.TotalWinsAsUnderdogLastMonth;
        match.Player1TotalLossesAsFavouriteLastMonth                     = p1.TotalLossesAsFavouriteLastMonth;
        match.Player2TotalLossesAsFavouriteLastMonth                     = p2.TotalLossesAsFavouriteLastMonth;
        match.Player1TotalLossesAsUnderdogLastMonth                      = p1.TotalLossesAsUnderdogLastMonth;
        match.Player2TotalLossesAsUnderdogLastMonth                      = p2.TotalLossesAsUnderdogLastMonth;
        match.Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth  = p1.AverageWinningProbabilityAtWonAsFavouriteLastMonth;
        match.Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth  = p2.AverageWinningProbabilityAtWonAsFavouriteLastMonth;
        match.Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth   = p1.AverageWinningProbabilityAtWonAsUnderdogLastMonth;
        match.Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth   = p2.AverageWinningProbabilityAtWonAsUnderdogLastMonth;
        match.Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth = p1.AverageWinningProbabilityAtLossAsFavouriteLastMonth;
        match.Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth = p2.AverageWinningProbabilityAtLossAsFavouriteLastMonth;
        match.Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth  = p1.AverageWinningProbabilityAtLossAsUnderdogLastMonth;
        match.Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth  = p2.AverageWinningProbabilityAtLossAsUnderdogLastMonth;
        match.Player1TotalWinsAsFavouriteLastWeek                        = p1.TotalWinsAsFavouriteLastWeek;
        match.Player2TotalWinsAsFavouriteLastWeek                        = p2.TotalWinsAsFavouriteLastWeek;
        match.Player1TotalWinsAsUnderdogLastWeek                         = p1.TotalWinsAsUnderdogLastWeek;
        match.Player2TotalWinsAsUnderdogLastWeek                         = p2.TotalWinsAsUnderdogLastWeek;
        match.Player1TotalLossesAsFavouriteLastWeek                      = p1.TotalLossesAsFavouriteLastWeek;
        match.Player2TotalLossesAsFavouriteLastWeek                      = p2.TotalLossesAsFavouriteLastWeek;
        match.Player1TotalLossesAsUnderdogLastWeek                       = p1.TotalLossesAsUnderdogLastWeek;
        match.Player2TotalLossesAsUnderdogLastWeek                       = p2.TotalLossesAsUnderdogLastWeek;
        match.Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek   = p1.AverageWinningProbabilityAtWonAsFavouriteLastWeek;
        match.Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek   = p2.AverageWinningProbabilityAtWonAsFavouriteLastWeek;
        match.Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek    = p1.AverageWinningProbabilityAtWonAsUnderdogLastWeek;
        match.Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek    = p2.AverageWinningProbabilityAtWonAsUnderdogLastWeek;
        match.Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek  = p1.AverageWinningProbabilityAtLossAsFavouriteLastWeek;
        match.Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek  = p2.AverageWinningProbabilityAtLossAsFavouriteLastWeek;
        match.Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek   = p1.AverageWinningProbabilityAtLossAsUnderdogLastWeek;
        match.Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek   = p2.AverageWinningProbabilityAtLossAsUnderdogLastWeek;
    }

    public static void UpdatePlayerFavouriteUnderdogStats(Models.Match match, Models.Player p1, Models.Player p2)
    {
        var winProbP1 = (match.WinProbabilityPlayer1M + match.WinProbabilityPlayer1SM + match.WinProbabilityPlayer1GSM) / 3;

        if (winProbP1 > 0.5)
        {
            var p1AverageWinsAsFavourite             = p1.AverageWinningProbabilityAtWonAsFavourite ?? 0.5;
            var p2AverageLossAsUnderdog              = p2.AverageWinningProbabilityAtLossAsUnderdog ?? 0.5;
            var p1AverageWinsAsFavouriteNew          = ((p1.TotalWinsAsFavourite  * p1AverageWinsAsFavourite) + winProbP1)       / (p1.TotalWinsAsFavourite  + 1);
            var p2AverageLossAsUnderdogNew           = ((p2.TotalLossesAsUnderdog * p2AverageLossAsUnderdog)  + (1 - winProbP1)) / (p2.TotalLossesAsUnderdog + 1);
            
            var p1AverageWinsAsFavouriteLastYear     = p1.AverageWinningProbabilityAtWonAsFavouriteLastYear ?? 0.5;
            var p2AverageLossAsUnderdogLastYear      = p2.AverageWinningProbabilityAtLossAsUnderdogLastYear ?? 0.5;
            var p1AverageWinsAsFavouriteLastYearNew  = ((p1.TotalWinsAsFavouriteLastYear  * p1AverageWinsAsFavouriteLastYear) + winProbP1)       / (p1.TotalWinsAsFavouriteLastYear + 1);
            var p2AverageLossAsUnderdogLastYearNew   = ((p2.TotalLossesAsUnderdogLastYear * p2AverageLossAsUnderdogLastYear)  + (1 - winProbP1)) / (p2.TotalLossesAsUnderdogLastYear + 1);

            var p1AverageWinsAsFavouriteLastMonth    = p1.AverageWinningProbabilityAtWonAsFavouriteLastMonth ?? 0.5;
            var p2AverageLossAsUnderdogLastMonth     = p2.AverageWinningProbabilityAtLossAsUnderdogLastMonth ?? 0.5;
            var p1AverageWinsAsFavouriteLastMonthNew = ((p1.TotalWinsAsFavouriteLastMonth  * p1AverageWinsAsFavouriteLastMonth) + winProbP1)       / (p1.TotalWinsAsFavouriteLastMonth + 1);
            var p2AverageLossAsUnderdogLastMonthNew  = ((p2.TotalLossesAsUnderdogLastMonth * p2AverageLossAsUnderdogLastMonth)  + (1 - winProbP1)) / (p2.TotalLossesAsUnderdogLastMonth + 1);

            var p1AverageWinsAsFavouriteLastWeek     = p1.AverageWinningProbabilityAtWonAsFavouriteLastWeek ?? 0.5;
            var p2AverageLossAsUnderdogLastWeek      = p2.AverageWinningProbabilityAtLossAsUnderdogLastWeek ?? 0.5;
            var p1AverageWinsAsFavouriteLastWeekNew  = ((p1.TotalWinsAsFavouriteLastWeek  * p1AverageWinsAsFavouriteLastWeek) + winProbP1)       / (p1.TotalWinsAsFavouriteLastWeek + 1);
            var p2AverageLossAsUnderdogLastWeekNew   = ((p2.TotalLossesAsUnderdogLastWeek * p2AverageLossAsUnderdogLastWeek)  + (1 - winProbP1)) / (p2.TotalLossesAsUnderdogLastWeek + 1);

            p1.TotalWinsAsFavourite ++;
            p2.TotalLossesAsUnderdog++;
            p1.AverageWinningProbabilityAtWonAsFavourite          = p1AverageWinsAsFavouriteNew;
            p2.AverageWinningProbabilityAtLossAsUnderdog          = p2AverageLossAsUnderdogNew;
            
            p1.TotalWinsAsFavouriteLastYear++;
            p2.TotalLossesAsUnderdogLastYear++;
            p1.AverageWinningProbabilityAtWonAsFavouriteLastYear  = p1AverageWinsAsFavouriteLastYearNew;
            p2.AverageWinningProbabilityAtLossAsUnderdogLastYear  = p2AverageLossAsUnderdogLastYearNew;
            
            p1.TotalWinsAsFavouriteLastMonth++;
            p2.TotalLossesAsUnderdogLastMonth++;
            p1.AverageWinningProbabilityAtWonAsFavouriteLastMonth = p1AverageWinsAsFavouriteLastMonthNew;
            p2.AverageWinningProbabilityAtLossAsUnderdogLastMonth = p2AverageLossAsUnderdogLastMonthNew;
            
            p1.TotalWinsAsFavouriteLastWeek++;
            p2.TotalLossesAsUnderdogLastWeek++;
            p1.AverageWinningProbabilityAtWonAsFavouriteLastWeek  = p1AverageWinsAsFavouriteLastWeekNew;
            p2.AverageWinningProbabilityAtLossAsUnderdogLastWeek  = p2AverageLossAsUnderdogLastWeekNew;
        }
        else if (winProbP1 < 0.5)
        {
            var p1AverageWinsAsUnderdog              = p1.AverageWinningProbabilityAtWonAsUnderdog ?? 0.5;
            var p2AverageLossAsFavourite             = p2.AverageWinningProbabilityAtLossAsFavourite ?? 0.5;
            var p1AverageWinsAsUnderdogNew           = ((p1.TotalWinsAsUnderdog             * p1AverageWinsAsUnderdog)           + winProbP1)       / (p1.TotalWinsAsUnderdog    + 1);
            var p2AverageLossAsFavouriteNew          = ((p2.TotalLossesAsFavourite          * p2AverageLossAsFavourite)          + (1 - winProbP1)) / (p2.TotalLossesAsFavourite + 1);

            var p1AverageWinsAsUnderdogLastYear      = p1.AverageWinningProbabilityAtWonAsUnderdogLastYear ?? 0.5;
            var p2AverageLossAsFavouriteLastYear     = p2.AverageWinningProbabilityAtLossAsFavouriteLastYear ?? 0.5;
            var p1AverageWinsAsUnderdogLastYearNew   = ((p1.TotalWinsAsUnderdogLastYear     * p1AverageWinsAsUnderdogLastYear)   + winProbP1)       / (p1.TotalWinsAsUnderdogLastYear + 1);
            var p2AverageLossAsFavouriteLastYearNew  = ((p2.TotalLossesAsFavouriteLastYear  * p2AverageLossAsFavouriteLastYear)  + (1 - winProbP1)) / (p2.TotalLossesAsFavouriteLastYear + 1);

            var p1AverageWinsAsUnderdogLastMonth     = p1.AverageWinningProbabilityAtWonAsUnderdogLastMonth ?? 0.5;
            var p2AverageLossAsFavouriteLastMonth    = p2.AverageWinningProbabilityAtLossAsFavouriteLastMonth ?? 0.5;
            var p1AverageWinsAsUnderdogLastMonthNew  = ((p1.TotalWinsAsUnderdogLastMonth    * p1AverageWinsAsUnderdogLastMonth)  + winProbP1)       / (p1.TotalWinsAsUnderdogLastMonth + 1);
            var p2AverageLossAsFavouriteLastMonthNew = ((p2.TotalLossesAsFavouriteLastMonth * p2AverageLossAsFavouriteLastMonth) + (1 - winProbP1)) / (p2.TotalLossesAsFavouriteLastMonth + 1);

            var p1AverageWinsAsUnderdogLastWeek      = p1.AverageWinningProbabilityAtWonAsUnderdogLastWeek ?? 0.5;
            var p2AverageLossAsFavouriteLastWeek     = p2.AverageWinningProbabilityAtLossAsFavouriteLastWeek ?? 0.5;
            var p1AverageWinsAsUnderdogLastWeekNew   = ((p1.TotalWinsAsUnderdogLastWeek     * p1AverageWinsAsUnderdogLastWeek)   + winProbP1)       / (p1.TotalWinsAsUnderdogLastWeek + 1);
            var p2AverageLossAsFavouriteLastWeekNew  = ((p2.TotalLossesAsFavouriteLastWeek  * p2AverageLossAsFavouriteLastWeek)  + (1 - winProbP1)) / (p2.TotalLossesAsFavouriteLastWeek + 1);

            p1.TotalWinsAsUnderdog++;
            p2.TotalLossesAsFavourite++;
            p1.AverageWinningProbabilityAtWonAsUnderdog = p1AverageWinsAsUnderdogNew;
            p2.AverageWinningProbabilityAtLossAsFavourite = p2AverageLossAsFavouriteNew;

            p1.TotalWinsAsUnderdogLastYear++;
            p2.TotalLossesAsFavouriteLastYear++;
            p1.AverageWinningProbabilityAtWonAsUnderdogLastYear    = p1AverageWinsAsUnderdogLastYearNew;
            p2.AverageWinningProbabilityAtLossAsFavouriteLastYear  = p2AverageLossAsFavouriteLastYearNew;

            p1.TotalWinsAsUnderdogLastMonth++;
            p2.TotalLossesAsFavouriteLastMonth++;
            p1.AverageWinningProbabilityAtWonAsUnderdogLastMonth   = p1AverageWinsAsUnderdogLastMonthNew;
            p2.AverageWinningProbabilityAtLossAsFavouriteLastMonth = p2AverageLossAsFavouriteLastMonthNew;

            p1.TotalWinsAsUnderdogLastWeek++;
            p2.TotalLossesAsFavouriteLastWeek++;
            p1.AverageWinningProbabilityAtWonAsUnderdogLastWeek    = p1AverageWinsAsUnderdogLastWeekNew;
            p2.AverageWinningProbabilityAtLossAsFavouriteLastWeek  = p2AverageLossAsFavouriteLastWeekNew;
        }
    }
}