using BonzoByte.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace BonzoByte.Core.Services
{
    public class MissingArchiveDetectorService
    {
        private readonly BrotliArchiveScanner _archiveScanner;
        private readonly ILogger<MissingArchiveDetectorService> _logger;

        public MissingArchiveDetectorService(
            BrotliArchiveScanner archiveScanner,
            ILogger<MissingArchiveDetectorService> logger)
        {
            _archiveScanner = archiveScanner;
            _logger = logger;
        }

        public List<string> GetMissingArchiveKeys()
        {
            var existingKeys = _archiveScanner.GetAllArchiveKeys();

            // 🧠 Ekstrakcija najkasnijeg datuma iz ključeva
            var latestDate = existingKeys.Keys
                .Select(key =>
                {
                    var parts = key.Split('_');
                    return parts.Length >= 3 && DateTime.TryParseExact(
                        $"{parts[0]}_{parts[1]}_{parts[2]}", "yyyy_MM_dd", null,
                        System.Globalization.DateTimeStyles.None, out var date)
                        ? date : (DateTime?)null;
                })
                .Where(dt => dt.HasValue)
                .Max() ?? new DateTime(1990, 1, 1);

            // 📅 Raspon od -3 dana unatrag od najkasnijeg do +3 dana u budućnost od danas
            var fromDate = latestDate.AddDays(-12);
            var toDate = DateTime.Now.Date.AddDays(+3);

            _logger.LogInformation("📅 Calculated date range: {From} – {To}", fromDate.ToShortDateString(), toDate.ToShortDateString());

            // 🧾 Generiranje svih mogućih ključeva (yyyy_MM_dd_t1 do t4)
            var expectedKeys = new List<string>();
            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                for (int tp = 1; tp <= 4; tp++)
                {
                    expectedKeys.Add($"{date:yyyy_MM_dd}_t{tp}");
                }
            }

            return expectedKeys;
        }
    }
}