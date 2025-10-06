namespace BonzoByte.Core.Services
{
    public class ResultScraperScheduler
    {
        private readonly MatchDateService     _matchDateService;
        private readonly ResultArchiveManager _archiveManager;

        public ResultScraperScheduler(
            MatchDateService     matchDateService,
            ResultArchiveManager archiveManager)
        {
            _matchDateService = matchDateService;
            _archiveManager   = archiveManager;
        }

        public async Task<List<DateTime>> GetDatesToScrapeAsync()
        {
            var (fromDate, toDate) = await _matchDateService.GetMatchDateRangeAsync();
            var existingKeys       = _archiveManager.LoadExistingArchiveKeys();

            var datesToScrape = new List<DateTime>();

            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                var expectedKey = ResultArchiveManager.GenerateArchiveKey(date, 1);

                if (!existingKeys.Contains(expectedKey)) datesToScrape.Add(date);
            }

            return datesToScrape;
        }
    }
}