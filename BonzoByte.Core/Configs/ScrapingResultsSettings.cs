namespace BonzoByte.Core.Configs
{
    public class ScrapingResultsSettings
    {
        // Base URL of the scraping source (e.g. tennisprediction.com)
        public string  BaseUrl              { get; set; } = "https://www.tennisprediction.com/";

        // Root paths for HTML and JSON (used when UseLocalArchive == false)
        public string  HtmlRootPath         { get; set; } = string.Empty;
        public string  JsonRootPath         { get; set; } = string.Empty;

        // Subfolders for each data type (used for organizing scraped data)
        public string  MatchInfosPath       { get; set; } = "MatchInfos";
        public string  MatchOddsPath        { get; set; } = "MatchOdds";
        public string  PlayersPath          { get; set; } = "d:\\brArchives\\Players";
        public string  ResultsPath          { get; set; } = "Results\\Working";
        public string  TournamentEventsPath { get; set; } = "TournamentEvents";
        public string  MatchPagesPath       { get; set; } = "MatchPages\\Working";

        // Local fallback for archive access (used in development or offline mode)
        public string  LocalArchiveRootPath { get; set; } = string.Empty;
        public bool    UseLocalArchive      { get; set; }

        // Error log output (optional)
        public string? ErrorLogPath         { get; set; }

        /// <summary>
        /// Returns the full path to a specific HTML subfolder, depending on archive mode.
        /// </summary>
        public string GetHtmlSubfolderPath(string subfolderName)
        {
            return UseLocalArchive ? Path.Combine(LocalArchiveRootPath, subfolderName) : Path.Combine(HtmlRootPath, subfolderName);
        }

        /// <summary>
        /// Returns the full path to a specific JSON subfolder, depending on archive mode.
        /// </summary>
        public string GetJsonSubfolderPath(string subfolderName)
        {
            return UseLocalArchive ? Path.Combine(LocalArchiveRootPath, subfolderName) : Path.Combine(JsonRootPath, subfolderName);
        }

        /// <summary>
        /// Returns the full HTML path for the given folder name (MatchInfos, Results, etc.).
        /// </summary>
        public string GetHtmlPathForType(DataType type)
        {
            return type switch
            {
                DataType.MatchInfos       => GetHtmlSubfolderPath(MatchInfosPath),
                DataType.MatchOdds        => GetHtmlSubfolderPath(MatchOddsPath),
                DataType.Players          => GetHtmlSubfolderPath(PlayersPath),
                DataType.Results          => GetHtmlSubfolderPath(ResultsPath),
                DataType.TournamentEvents => GetHtmlSubfolderPath(TournamentEventsPath),
                _                         => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        /// <summary>
        /// Returns the full JSON path for the given folder name (MatchInfos, Results, etc.).
        /// </summary>
        public string GetJsonPathForType(DataType type)
        {
            return type switch
            {
                DataType.MatchInfos       => GetJsonSubfolderPath(MatchInfosPath),
                DataType.MatchOdds        => GetJsonSubfolderPath(MatchOddsPath),
                DataType.Players          => GetJsonSubfolderPath(PlayersPath),
                DataType.Results          => GetJsonSubfolderPath(ResultsPath),
                DataType.TournamentEvents => GetJsonSubfolderPath(TournamentEventsPath),
                _                         => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }

    public enum DataType
    {
        MatchInfos,
        MatchOdds,
        Players,
        Results,
        TournamentEvents
    }
}