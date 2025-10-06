using BonzoByte.Core.Helpers;

namespace BonzoByte.Core.Services
{
    public class MatchDetailsDownloaderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _matchInfoPath;
        private readonly string _matchOddsPath;

        public MatchDetailsDownloaderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _matchInfoPath = @"d:\brArchives\MatchInfos\";
            _matchOddsPath = @"d:\brArchives\MatchOdds\Working";
        }

        //public bool IsMatchInfoArchived(long matchTPId)
        //{
        //    string path = Path.Combine(_matchInfoPath, $"{matchTPId}.br");
        //    return File.Exists(path);
        //}

        public bool IsMatchOddsArchived(long matchTPId)
        {
            return File.Exists("d:\\brArchives\\MatchOdds\\Working\\" + matchTPId.ToString() + ".br");
        }

        //public async Task<bool> DownloadMatchInfoAsync(long matchTPId)
        //{
        //    string url = $"https://www.tennisprediction.com/?a=match_info&ma_id={matchTPId}";
        //    string outputFile = Path.Combine(_matchInfoPath, $"{matchTPId}.br");

        //    try
        //    {
        //        var response = await _httpClient.GetAsync(url);
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine($"[!] Failed match_info ({(int)response.StatusCode}) {url}");
        //            return false;
        //        }

        //        var html = await response.Content.ReadAsStringAsync();
        //        string cleaned = HtmlCleaner.Clean(html);

        //        BrotliCompressor.CompressStringToFile(cleaned, outputFile);
        //        Console.WriteLine($"[✓] MatchInfo saved: {outputFile}");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[X] Error fetching match_info: {ex.Message}");
        //        return false;
        //    }
        //}

        public async Task<bool> DownloadMatchOddsAsync(long matchTPId)
        {
            //string url = $"https://www.tennisprediction.com/?a=odds&ma_id={matchTPId}";
            //string outputFile = Path.Combine("d:\\brArchives\\MatchOdds\\Working\\", $"{matchTPId}.br");

            //try
            //{
            //    var response = await _httpClient.GetAsync(url);
            //    if (!response.IsSuccessStatusCode)
            //    {
            //        Console.WriteLine($"[!] Failed odds ({(int)response.StatusCode}) {url}");
            //        return false;
            //    }

            //    var html = await response.Content.ReadAsStringAsync();
            //    string cleaned = HtmlCleaner.Clean(html);

            //    BrotliCompressor.CompressStringToFile(cleaned, outputFile);
            //    Console.WriteLine($"[✓] MatchOdds saved: {outputFile}");
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"[X] Error fetching match_odds: {ex.Message}");
            //    return false;
            //}
            return false;
        }

        public async Task DownloadMatchAssetsIfMissingAsync(long matchTPId)
        {
            //if (!IsMatchInfoArchived(matchTPId))
            //{
            //    await DownloadMatchInfoAsync(matchTPId);
            //    //await Task.Delay(Random.Shared.Next(800, 1500));
            //}

            if (!IsMatchOddsArchived(matchTPId))
            {
                await DownloadMatchOddsAsync(matchTPId);
                //await Task.Delay(Random.Shared.Next(800, 1500));
            }
        }
    }
}