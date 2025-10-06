using BonzoByte.Core.Configs;
using BonzoByte.Core.Helpers;

namespace BonzoByte.Core.Services
{
    public class TournamentEventDownloaderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _tournamentEventsPath;

        public TournamentEventDownloaderService(HttpClient httpClient, ScrapingResultsSettings settings)
        {
            _httpClient = httpClient;
            _tournamentEventsPath = Path.Combine(settings.LocalArchiveRootPath, settings.TournamentEventsPath);
        }

        public async Task<bool> DownloadAndArchiveTournamentEventAsync(int tournamentEventTpId)
        {
            string url = $"https://www.tennisprediction.com/tournament/?a=tournament&tid={tournamentEventTpId}";
            string outputFile = Path.Combine(_tournamentEventsPath, tournamentEventTpId.ToString() + ".br");

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[!] Failed ({(int)response.StatusCode}) {url}");
                    return false;
                }

                var html = await response.Content.ReadAsStringAsync();
                string cleanedHtml = HtmlCleaner.Clean(html);
                BrotliCompressor.CompressStringToFile(cleanedHtml, outputFile);

                Console.WriteLine($"[✓] TournamentEvent downloaded & saved to archive: {outputFile}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[X] Error fetching tournament {tournamentEventTpId}: {ex.Message}");
                return false;
            }
        }

        public bool IsTournamentEventArchived(int tournamentEventTpId)
        {
            string archivePath = Path.Combine(_tournamentEventsPath, tournamentEventTpId.ToString() + ".br");
            return File.Exists(archivePath);
        }
    }
}