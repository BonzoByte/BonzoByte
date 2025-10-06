using BonzoByte.Core.Models;

namespace BonzoByte.Core.Helpers;

public static class StreakHelper
{

    public static void UpdateStreaks(
        Models.Match match,
        Player p1,
        Player p2)
    {
        match.Player1Streak   = p1.Streak;
        match.Player1StreakS1 = p1.StreakS1;
        match.Player1StreakS2 = p1.StreakS2;
        match.Player1StreakS3 = p1.StreakS3;
        match.Player1StreakS4 = p1.StreakS4;
        match.Player2Streak   = p2.Streak;
        match.Player2StreakS1 = p2.StreakS1;
        match.Player2StreakS2 = p2.StreakS2;
        match.Player2StreakS3 = p2.StreakS3;
        match.Player2StreakS4 = p2.StreakS4;

        if (p1.Streak > 0)  p1.Streak++; else p1.Streak =  1;
        if (p2.Streak < 0)  p2.Streak--; else p2.Streak = -1;
        if (p1.Streak == 0) p1.Streak =  1;
        if (p2.Streak == 0) p2.Streak = -1;

        switch (match.SurfaceId)
        {
            case 1:
                if (p1.StreakS1 > 0)  p1.StreakS1++; else p1.StreakS1 =  1;
                if (p2.StreakS1 < 0)  p2.StreakS1--; else p2.StreakS1 = -1;
                if (p1.StreakS1 == 0) p1.StreakS1 = 1;
                if (p2.StreakS1 == 0) p2.StreakS1 = -1;
                break;
            case 2:
                if (p1.StreakS2 > 0)  p1.StreakS2++; else p1.StreakS2 =  1;
                if (p2.StreakS2 < 0)  p2.StreakS2--; else p2.StreakS2 = -1;
                if (p1.StreakS2 == 0) p1.StreakS2 =  1;
                if (p2.StreakS2 == 0) p2.StreakS2 = -1;
                break;
            case 3:
                if (p1.StreakS3 > 0) p1.StreakS3++; else p1.StreakS3 =  1;
                if (p2.StreakS3 < 0) p2.StreakS3--; else p2.StreakS3 = -1;
                if (p1.StreakS3 == 0) p1.StreakS3 =  1;
                if (p2.StreakS3 == 0) p2.StreakS3 = -1;
                break;
            case 4:
                if (p1.StreakS4 > 0) p1.StreakS4++; else p1.StreakS4 =  1;
                if (p2.StreakS4 < 0) p2.StreakS4--; else p2.StreakS4 = -1;
                if (p1.StreakS4 == 0) p1.StreakS4 =  1;
                if (p2.StreakS4 == 0) p2.StreakS4 = -1;
                break;
            default:
                break;
        }
    }
}