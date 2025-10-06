using BonzoByte.Core.Models;

namespace BonzoByte.Core.Helpers;

public static class DateSinceLastWinLossHelper
{
    // Ako želiš “calendar day” semantiku (preporučeno) i clamp >= 0
    private static int? DaysSince(DateTime? matchDt, DateTime? lastDt, bool useCalendarDays = true, bool clampNonNegative = true)
    {
        if (matchDt is null || lastDt is null) return (int?)null;

        // opcionalno: osiguraj isti Kind (ako ti ikad uleti mix)
        // matchDt = DateTime.SpecifyKind(matchDt.Value, DateTimeKind.Local);
        // lastDt  = DateTime.SpecifyKind(lastDt.Value, DateTimeKind.Local);

        DateTime a = useCalendarDays ? matchDt.Value.Date : matchDt.Value;
        DateTime b = useCalendarDays ? lastDt.Value.Date : lastDt.Value;

        var ts = a - b;

        // Za integer dane koristi floor TotalDays (ne TimeSpan.Days komponentu).
        int days = (int)Math.Floor(ts.TotalDays);

        if (clampNonNegative && days < 0) days = 0;
        return days;
    }

    public static void UpdateDateWinLoss(Models.Match match, Models.Player p1, Models.Player p2)
    {
        // Guard – ako bi ikad bio null
        if (match.DateTime is null)
            throw new InvalidOperationException("Match.DateTime is null; očekuje se lokalno vrijeme.");

        // “Days since” se računaju iz STARIH vrijednosti na igračima (prije updatea)
        match.Player1DaysSinceLastWin = DaysSince(match.DateTime, p1.DateSinceLastWin);
        match.Player2DaysSinceLastWin = DaysSince(match.DateTime, p2.DateSinceLastWin);
        match.Player1DaysSinceLastWinS1 = DaysSince(match.DateTime, p1.DateSinceLastWinS1);
        match.Player2DaysSinceLastWinS1 = DaysSince(match.DateTime, p2.DateSinceLastWinS1);
        match.Player1DaysSinceLastWinS2 = DaysSince(match.DateTime, p1.DateSinceLastWinS2);
        match.Player2DaysSinceLastWinS2 = DaysSince(match.DateTime, p2.DateSinceLastWinS2);
        match.Player1DaysSinceLastWinS3 = DaysSince(match.DateTime, p1.DateSinceLastWinS3);
        match.Player2DaysSinceLastWinS3 = DaysSince(match.DateTime, p2.DateSinceLastWinS3);
        match.Player1DaysSinceLastWinS4 = DaysSince(match.DateTime, p1.DateSinceLastWinS4);
        match.Player2DaysSinceLastWinS4 = DaysSince(match.DateTime, p2.DateSinceLastWinS4);

        match.Player1DaysSinceLastLoss = DaysSince(match.DateTime, p1.DateSinceLastLoss);
        match.Player2DaysSinceLastLoss = DaysSince(match.DateTime, p2.DateSinceLastLoss);
        match.Player1DaysSinceLastLossS1 = DaysSince(match.DateTime, p1.DateSinceLastLossS1);
        match.Player2DaysSinceLastLossS1 = DaysSince(match.DateTime, p2.DateSinceLastLossS1);
        match.Player1DaysSinceLastLossS2 = DaysSince(match.DateTime, p1.DateSinceLastLossS2);
        match.Player2DaysSinceLastLossS2 = DaysSince(match.DateTime, p2.DateSinceLastLossS2);
        match.Player1DaysSinceLastLossS3 = DaysSince(match.DateTime, p1.DateSinceLastLossS3);
        match.Player2DaysSinceLastLossS3 = DaysSince(match.DateTime, p2.DateSinceLastLossS3);
        match.Player1DaysSinceLastLossS4 = DaysSince(match.DateTime, p1.DateSinceLastLossS4);
        match.Player2DaysSinceLastLossS4 = DaysSince(match.DateTime, p2.DateSinceLastLossS4);

        // Sad ažuriraj “zadnji win/loss” datume na igračima (p1 je pobjednik)
        p1.DateSinceLastWin = match.DateTime;
        p2.DateSinceLastLoss = match.DateTime;

        switch (match.SurfaceId)
        {
            case 1:
                p1.DateSinceLastWinS1 = match.DateTime;
                p2.DateSinceLastLossS1 = match.DateTime;
                break;
            case 2:
                p1.DateSinceLastWinS2 = match.DateTime;
                p2.DateSinceLastLossS2 = match.DateTime;
                break;
            case 3:
                p1.DateSinceLastWinS3 = match.DateTime;
                p2.DateSinceLastLossS3 = match.DateTime;
                break;
            case 4:
                p1.DateSinceLastWinS4 = match.DateTime;
                p2.DateSinceLastLossS4 = match.DateTime;
                break;
            default:
                // ako ti ikad dođe SurfaceId izvan 1..4, možda logirati
                break;
        }
    }
}