using BonzoByte.Core.Configs;

namespace BonzoByte.Core.Services
{
    public class ResultArchiveManager
    {
        private readonly string _archivePath;

        public ResultArchiveManager(ScrapingResultsSettings settings)
        {
            _archivePath = settings.UseLocalArchive ? Path.Combine(settings.LocalArchiveRootPath, settings.ResultsPath) : settings.GetHtmlSubfolderPath(settings.ResultsPath);
        }

        public HashSet<string> LoadExistingArchiveKeys()
        {
            if (!Directory.Exists(_archivePath)) return new HashSet<string>();

            var files = Directory.GetFiles(_archivePath, "*.br");
            var keys  = files.Select(file => Path.GetFileNameWithoutExtension(file)).ToHashSet();

            return keys;
        }

        public static string GenerateArchiveKey(DateTime date, int index)
        {
            return $"{date:yyyy_MM_dd}_{index}";
        }
    }
}