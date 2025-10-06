namespace BonzoByte.Core.Models
{
    public sealed class FeatureVector
    {
        public required int MatchTPId { get; init; }
        public required IReadOnlyList<string> Names { get; init; }    // kanonski redoslijed
        public required double[] Values { get; init; }                // istim redoslijedom kao Names

        // Za routing po iskustvu / H2H / podlozi
        public int P1Matches { get; init; }
        public int P2Matches { get; init; }

        // NEW:
        public int SurfaceId { get; init; }       // 1=unknown, 2=clay, 3=grass, 4=hard (po tvojoj mapi)
        public int H2HMatches { get; init; }      // broj prethodnih međusobnih mečeva (>=1 znači imamo H2H signal)
    }
}