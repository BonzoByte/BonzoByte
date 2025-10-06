namespace BonzoByte.Core.Helpers
{
    public static class DateRangeHelper
    {
        public static List<string> GenerateArchiveKeys(DateTime fromDate, DateTime toDate)
        {
            var keys = new List<string>();

            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                for (int tp = 1; tp <= 4; tp++)
                {
                    keys.Add($"{date:yyyy_MM_dd}_{tp}");
                }
            }

            return keys;
        }
    }
}