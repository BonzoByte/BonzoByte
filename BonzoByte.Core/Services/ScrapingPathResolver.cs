using BonzoByte.Core.Configs;

namespace BonzoByte.Core.Services
{
    public class ScrapingPathResolver
    {
        private readonly ScrapingResultsSettings _settings;

        public ScrapingPathResolver(ScrapingResultsSettings settings)
        {
            _settings = settings;
        }

        public string GetHtmlPath(DataType type)
        {
            return _settings.GetHtmlPathForType(type);
        }

        public string GetJsonPath(DataType type)
        {
            return _settings.GetJsonPathForType(type);
        }

        public string GetErrorLogPath()
        {
            return _settings.ErrorLogPath ?? Path.Combine(_settings.LocalArchiveRootPath, "error.log");
        }

        public string GetTournamentEventArchivePath(int tournamentEventTpId)
        {
            return Path.Combine(_settings.LocalArchiveRootPath, _settings.TournamentEventsPath, $"{tournamentEventTpId}.br");
        }

        public string GetMatchInfoArchivePath(long matchTPId)
        {
            return Path.Combine(_settings.LocalArchiveRootPath, _settings.MatchInfosPath, $"{matchTPId}.br");
        }

        public string GetMatchOddsArchivePath(long matchTPId)
        {
            return Path.Combine(_settings.LocalArchiveRootPath, _settings.MatchOddsPath, $"{matchTPId}.br");
        }

        public bool IsUsingLocalArchive => _settings.UseLocalArchive;
    }
}