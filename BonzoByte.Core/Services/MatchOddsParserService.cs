using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Helpers;
using BonzoByte.Core.Models;
using BonzoByte.Core.Services.Interfaces;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Services
{
    [Flags]
    public enum SuspiciousBits : short
    {
        OverroundOutOfRange = 1,
        OppositeToBookieMajority = 2,
        OppositeToGlobalConsensus = 4,
        LargeJumpWithinBookieSeries = 8,
        PostResultNoTimeHeader = 16,
        NearCoinflipNeutral = 32,
        DuplicateRow = 64,
        AfterMatchStart = 128
    }

    public class MatchOddsParserService
    {
        private const double ORR_MIN = 1.00;
        private const double ORR_MAX = 1.20;
        private const double NEUTRAL_EPS = 0.05; // |ln(p1/p2)|
        private readonly IReferenceDataService _reference;
        private readonly IBookieRepository _bookieRepository;
        private readonly IMatchOddsRepository _matchOddsRepository;
        private readonly string _matchOddsFolder;
        private readonly TimeZoneInfo _tz;

        public MatchOddsParserService(
            IReferenceDataService reference,
            IBookieRepository bookieRepository,
            IMatchOddsRepository matchOddsRepository)
        {
            _reference = reference;
            _bookieRepository = bookieRepository;
            _matchOddsRepository = matchOddsRepository;

            _matchOddsFolder = @"d:\brArchives\MatchOdds"; // premjesti u settings kasnije
            _tz = ResolveTimeZone();
        }

        public async Task<IReadOnlyList<MatchOdds>> ParseOddsFromArchiveAsync(
            int matchTPId,
            DateTime? matchStartLocal = null,
            bool isFinished = false,
            CancellationToken ct = default)
        {
            var result = new List<MatchOdds>();
            string brPath = Path.Combine(_matchOddsFolder, "Working", $"{matchTPId}.br");
            if (!File.Exists(brPath)) return result;

            string html;
            try
            {
                html = BrotliCompressor.DecompressFileToString(brPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Odds] Decompress fail for {matchTPId}: {ex.Message}");
                return result;
            }

            if (html.IndexOf("Nema tečaja", StringComparison.OrdinalIgnoreCase) >= 0)
                return result;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Uzmi sve redove koje trebamo: header (odd1) i povijesne (odd)
            var rows = doc.DocumentNode.SelectNodes("//tr[@class='odd1' or @class='odd']");
            if (rows == null || rows.Count == 0) return result;

            DateTime sourceFileUtc = File.GetLastWriteTimeUtc(brPath);
            DateTime? cutoffUtc = null;
            if (matchStartLocal.HasValue)
            {
                cutoffUtc = ToUtcAssumeLocal(matchStartLocal.Value);
            }

            string? currentBookieName = null;
            int seriesOrdinal = -1;

            foreach (var row in rows)
            {
                if (ct.IsCancellationRequested) break;

                var cls = row.GetAttributeValue("class", "");
                var cells = row.SelectNodes("td");
                if (cells == null || cells.Count < 3) continue;

                DateTime? oddsLocal = null;
                string? bookieNameThisRow = null;

                if (cls == "odd1")
                {
                    // Header: postavlja bookie i ordinal reset
                    var (bn, t) = ParseBookieCell(cells[0]);
                    currentBookieName = string.IsNullOrWhiteSpace(bn) ? currentBookieName : bn;
                    bookieNameThisRow = currentBookieName;
                    seriesOrdinal = 0;
                    oddsLocal = t;
                }
                else
                {
                    // Povijesni red: nema imena, ima vrijeme u <span class="odds_time">
                    bookieNameThisRow = currentBookieName;
                    if (string.IsNullOrWhiteSpace(bookieNameThisRow))
                        continue; // nemamo head
                    seriesOrdinal++;
                    oddsLocal = ParseTimeFromCell(cells[0]);
                }

                if (!TryParseDecimalFlexible(cells[1].InnerText, out decimal p1)) continue;
                if (!TryParseDecimalFlexible(cells[2].InnerText, out decimal p2)) continue;

                if (p1 == 0 || p2 == 0) continue;

                // LOOKUP/INSERT BOOKIE — ATOMSKI I THREAD-SAFE
                var (bookie, isNew) = _reference.GetOrAddBookieByName(bookieNameThisRow!);
                if (isNew)
                {
                    // Ako repo ima overload s CancellationToken-om, dodaj ct.
                    await _bookieRepository.InsertBookieAsync(bookie /*, ct*/);
                    Console.WriteLine($"➕ Dodan novi bookie: {bookie.BookieName} (ID={bookie.BookieId})");
                }

                // UTC vremena
                DateTime? oddsUtc = oddsLocal.HasValue ? ToUtcAssumeLocal(oddsLocal.Value) : (DateTime?)null;

                // Bazni flagovi
                short mask = 0;
                var orr = (double)(1m / p1 + 1m / p2);
                if (orr < ORR_MIN || orr > ORR_MAX)
                    mask |= (short)SuspiciousBits.OverroundOutOfRange;

                double z = Math.Log((double)(p1 / p2));
                if (Math.Abs(z) < NEUTRAL_EPS)
                    mask |= (short)SuspiciousBits.NearCoinflipNeutral;

                // After-match (ako znamo cutoff)
                if (cutoffUtc.HasValue)
                {
                    var coalesced = oddsUtc ?? sourceFileUtc;
                    if (coalesced > cutoffUtc.Value)
                        mask |= (short)SuspiciousBits.AfterMatchStart;
                }

                var mo = new MatchOdds
                {
                    MatchTPId = matchTPId,
                    BookieId = bookie.BookieId,
                    DateTime = oddsUtc,
                    SourceFileTime = sourceFileUtc,
                    SeriesOrdinal = seriesOrdinal,
                    Player1Odds = p1,
                    Player2Odds = p2,
                    IngestedAt = DateTime.UtcNow,
                    SuspiciousMask = mask,
                    IsSuspicious = (mask & (short)SuspiciousBits.OverroundOutOfRange) != 0, // bazno; ostalo radi flagger
                    IsLikelySwitched = false
                };

                var newId = await _matchOddsRepository.InsertMatchOddsAsync(mo /*, ct*/);
                mo.OddsId = newId;              // ✅ sad više nije null
                _reference.AddOrUpdateMatchOdds(mo);

                result.Add(mo);
            }

            Console.WriteLine($"💾 MatchOdds: {result.Count} zapisa za MatchTPId={matchTPId}");
            return result;
        }

        // ---------- helpers ----------

        private static bool TryParseDecimalFlexible(string input, out decimal value)
        {
            var s = HtmlEntity.DeEntitize(input ?? "").Trim();
            s = Regex.Replace(s, @"[^\d\.,\-+eE]", "");
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value)) return true;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out value)) return true;
            s = s.Replace(',', '.');
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static (string BookieName, DateTime? OddsTime) ParseBookieCell(HtmlNode td)
        {
            // vrijeme
            var timeNode = td.SelectSingleNode(".//span[contains(@class,'odds_time')]");
            DateTime? oddsTime = null;
            if (timeNode != null)
            {
                var s = HtmlEntity.DeEntitize(timeNode.InnerText).Trim();
                oddsTime = ParseDateBestEffortLocal(s);
            }

            // ime (ukloni time/img/i)
            var clone = td.Clone();
            var spans = clone.SelectNodes(".//span[contains(@class,'odds_time')]");
            if (spans != null) foreach (var s in spans) s.Remove();
            var imgs = clone.SelectNodes(".//img|.//i");
            if (imgs != null) foreach (var n in imgs) n.Remove();

            var a = clone.SelectSingleNode(".//a");
            string rawName;
            if (a != null) rawName = HtmlEntity.DeEntitize(a.InnerText).Trim();
            else
            {
                var brs = clone.SelectNodes(".//br");
                if (brs != null) foreach (var br in brs) br.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(" "), br);
                rawName = HtmlEntity.DeEntitize(clone.InnerText).Trim();
            }

            var bookieName = Regex.Replace(rawName, @"\s+", " ");
            return (bookieName, oddsTime);
        }

        private static DateTime? ParseTimeFromCell(HtmlNode td)
        {
            var span = td.SelectSingleNode(".//span[contains(@class,'odds_time')]");
            if (span == null) return null;
            var s = HtmlEntity.DeEntitize(span.InnerText).Trim();
            return ParseDateBestEffortLocal(s);
        }

        private static DateTime? ParseDateBestEffortLocal(string s)
        {
            // tretiramo kao lokalno (Europe/Zagreb)
            string[] patterns = {
                "dd.MM.yy, HH:mm:ss", "dd.MM.yy, HH:mm",
                "dd.MM.yyyy HH:mm", "dd.MM.yyyy. HH:mm",
                "yyyy-MM-dd HH:mm", "dd.MM.yyyy", "yyyy-MM-dd"
            };
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1)) return dt1;
            foreach (var p in patterns)
                if (DateTime.TryParseExact(s, p, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt2)) return dt2;
            return null;
        }

        private DateTime ToUtcAssumeLocal(DateTime local)
        {
            if (local.Kind == DateTimeKind.Utc) return local;
            if (local.Kind == DateTimeKind.Local) return local.ToUniversalTime();
            // Unspecified -> pretpostavi naš time zone
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), _tz);
        }

        private static TimeZoneInfo ResolveTimeZone()
        {
            // Windows: "Central European Standard Time", Linux: "Europe/Zagreb"
            try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb"); }
            catch
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            }
        }
    }
}