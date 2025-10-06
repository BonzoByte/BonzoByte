using BonzoByte.Core.Configs;
using BonzoByte.Core.Helpers;

namespace BonzoByte.Core.Services
{
    public class MatchPageDownloaderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _outputRoot;

        public MatchPageDownloaderService(HttpClient httpClient, ScrapingResultsSettings settings)
        {
            _httpClient = httpClient;
            _outputRoot = settings.UseLocalArchive ? Path.Combine(settings.LocalArchiveRootPath, settings.ResultsPath, "Working") : settings.GetHtmlSubfolderPath("Working");
        }

        public async Task DownloadMatchPagesAsync(List<DateTime> datesToScrape)
        {
            foreach (var date in datesToScrape)
            {
                for (int tp = 1; tp <= 4; tp++)
                {
                    string fileName = $"{date.Year}_{date.Month.ToString().PadLeft(2, '0')}_{date.Day.ToString().PadLeft(2, '0')}_{tp}.br";
                    string filePath = Path.Combine(_outputRoot, fileName);

                    string url = $"https://www.tennisprediction.com/match/?t_p={tp}&year={date.Year}&month={date.Month}&day={date.Day}";
                    try
                    {
                        var response = await _httpClient.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();

                            // ✅ Komprimiraj i spremi kao .br
                            BrotliCompressor.CompressStringToFile(content, filePath);

                            Console.WriteLine($"[✓] Downloaded & compressed {url}");
                        }
                        else
                        {
                            Console.WriteLine($"[!] Failed ({(int)response.StatusCode}) {url}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[X] Error fetching {url}: {ex.Message}");
                    }

                    await Task.Delay(Random.Shared.Next(1000, 2000));
                }
            }
        }
    }
}