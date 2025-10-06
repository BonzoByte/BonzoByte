using BonzoByte.Core.Configs;
using BonzoByte.Core.Helpers;
using BonzoByte.Core.Models;
using BonzoByte.Core.Services.Interfaces; // IReferenceDataService
using HtmlAgilityPack;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Services
{
    public sealed class PlayerHtmlParser
    {
        private readonly string _archivePath;
        private readonly string _baseUrl;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IReferenceDataService _refData;

        // --- Regex obrasci (HR/EN varijante, dijakritika) ---
        private static readonly Regex RxBirth = new(
            @"ro[dđ]en:\s*(\d{1,2})\.(\d{1,2})\.(\d{2,4})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RxHeight = new(
            @"visina:\s*(\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RxWeight = new(
            @"te[zž]ina:\s*(\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RxPro = new(
            @"(pro\.?\s*karijera|turned\s*pro)\s*:\s*(\d{4})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RxPlays = new(
            @"igra:\s*([^\r\n<]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RxIso3AtEnd = new(
            @"\b([A-Z]{3})\b\s*$",
            RegexOptions.Compiled);

        // za čišćenje PlayerName iz <strong> ("Ime Prezime (674) / Spain" -> "Ime Prezime")
        private static readonly Regex RxTrimAfterParenOrSlash = new(
            @"\s*(?:\(|/).*$",
            RegexOptions.Compiled);

        public PlayerHtmlParser(
            ScrapingResultsSettings settings,
            IHttpClientFactory httpClientFactory,
            IReferenceDataService referenceDataService)
        {
            _baseUrl = settings.BaseUrl ?? throw new ArgumentNullException(nameof(settings.BaseUrl));
            _httpClientFactory = httpClientFactory;
            _refData = referenceDataService;
        }

        public async Task<Player> LoadOrDownloadAndParseAsync(int playerTPId, CancellationToken ct = default)
        {
            var filePath = Path.Combine("D:\\brArchives\\Players\\", $"{playerTPId}.br"); // ✅ koristi settings

            string html = string.Empty;
            if (File.Exists(filePath))
            {
                html = DecompressBrotliFile(filePath);
                html = HtmlCleaner.Clean(html);
            }
            if (!(html.Contains("player_info") && html.Contains("main_tour")))
            {
                var url = $"{_baseUrl}?a=player&p1_id={playerTPId}";
                var client = _httpClientFactory.CreateClient();
                html = await client.GetStringAsync(url, ct).ConfigureAwait(false);
                html = HtmlCleaner.Clean(html);
                SaveAsBrotli(filePath, html);
            }

            return ParsePlayerHtml(html, playerTPId)!;
        }

        // ---------------- Brotli utili ----------------

        private static string DecompressBrotliFile(string filePath)
        {
            using var input = File.OpenRead(filePath);
            using var decompressor = new BrotliStream(input, CompressionMode.Decompress);
            using var reader = new StreamReader(decompressor, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static void SaveAsBrotli(string path, string html)
        {
            using var output = File.Create(path);
            using var brotli = new BrotliStream(output, CompressionLevel.Optimal);
            using var writer = new StreamWriter(brotli, new UTF8Encoding(false));
            writer.Write(html);
        }

        // --------------- Parsiranje HTML-a ---------------

        private Player? ParsePlayerHtml(string html, int playerTPId)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var player = new Player { PlayerTPId = playerTPId };

            // 1) Header: uzmi prvi main_tour unutar player_info tablice
            var headerTd = doc.DocumentNode.SelectSingleNode("//table[contains(@class,'player_info')]//td[contains(@class,'main_tour')]");
            if (headerTd != null)
            {
                // Ime iz <strong>, ali očisti sve nakon '(' ili '/'
                var strongName = headerTd.SelectSingleNode(".//strong")?.InnerText?.Trim();
                if (!string.IsNullOrWhiteSpace(strongName))
                {
                    var cleanName = RxTrimAfterParenOrSlash.Replace(strongName, "").Trim();
                    player.PlayerName = cleanName;
                }

                // Resolve CountryTPId pametno (src, alt, trailing tekst)
                if (TryResolveCountry(headerTd, strongName, out var ctp))
                    player.CountryTPId = ctp; // ostaje 0 ako ne uspije
            }

            // 2) Detalji: td sa slikom i <br> linijama
            var detailsTd = doc.DocumentNode.SelectSingleNode("//table[contains(@class,'player_info')]//img[@id='images']/ancestor::td[1]")
                           ?? doc.DocumentNode.SelectSingleNode("//table[contains(@class,'player_info')]//tr[2]/td"); // fallback

            if (detailsTd != null)
            {
                // Normaliziraj <br> u '\n' prije uzimanja teksta (riješava tvoj "lines.Count == 1")
                var blockHtml = detailsTd.InnerHtml ?? string.Empty;
                var normHtml = Regex.Replace(blockHtml, @"<\s*br\s*/?>", "\n", RegexOptions.IgnoreCase);
                var tmp = new HtmlDocument();
                tmp.LoadHtml(normHtml);
                var detailsText = tmp.DocumentNode.InnerText; // čist plain text s novim redovima

                // Regexe vrtimo nad cijelim blokom (ne ovisimo o linijama)
                var mb = RxBirth.Match(detailsText);
                if (mb.Success && TryParseCroDate(mb.Groups[1].Value, mb.Groups[2].Value, mb.Groups[3].Value, out var birth))
                    player.PlayerBirthDate = birth;

                var mh = RxHeight.Match(detailsText);
                if (mh.Success && int.TryParse(mh.Groups[1].Value, out var h))
                    player.PlayerHeight = h;

                var mw = RxWeight.Match(detailsText);
                if (mw.Success && int.TryParse(mw.Groups[1].Value, out var w))
                    player.PlayerWeight = w;

                var mp = RxPro.Match(detailsText);
                if (mp.Success && int.TryParse(mp.Groups[2].Value, out var proYear))
                    player.PlayerTurnedPro = proYear;

                var mpl = RxPlays.Match(detailsText);
                if (mpl.Success)
                {
                    var raw = mpl.Groups[1].Value.Trim();
                    var norm = RemoveDiacritics(raw).ToLowerInvariant();
                    // mapping: 1=desna (right), 2=lijeva (left)
                    if (norm.Contains("ljev") || norm.Contains("lijev") || norm.Contains("left"))
                        player.PlaysId = 2;
                    else if (norm.Contains("desn") || norm.Contains("right"))
                        player.PlaysId = 1;
                }
            }

            return player;
        }

        // --------------- Country helper ---------------

        private bool TryResolveCountry(HtmlNode headerTd, string? strongName, out int countryTpId)
        {
            countryTpId = 0;

            // 0) Direktno iz ?cid=… (radi i s & i s &amp;)
            var mCid = Regex.Match(headerTd.InnerHtml, @"(?:\?|&|&amp;)cid=(\d+)", RegexOptions.IgnoreCase);
            if (mCid.Success && int.TryParse(mCid.Groups[1].Value, out var cidVal))
            {
                countryTpId = cidVal;
                return true;
            }

            // 1) Pokušaj iz src filename-a: /flags/circle/usa.png -> "usa"
            var img = headerTd.SelectSingleNode(".//img[contains(@class,'country_flag')]");
            var src = img?.GetAttributeValue("src", null);
            if (!string.IsNullOrWhiteSpace(src))
            {
                var token = Path.GetFileNameWithoutExtension(src)?.Trim();
                if (TryResolveCountryToken(token, out countryTpId))
                    return true;
            }

            // 2) Pokušaj iz alt-a: "flag USA" ili "flag Spain"
            var alt = img?.GetAttributeValue("alt", null);
            if (!string.IsNullOrWhiteSpace(alt))
            {
                var last = alt.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (!string.IsNullOrWhiteSpace(last) && TryResolveCountryToken(last, out countryTpId))
                    return true;
            }

            // 3) Pokušaj iz trailing teksta nakon imena (često " / Spain" ili "USA")
            var full = headerTd.InnerText ?? string.Empty;
            var tail = strongName is null ? full : full.Replace(strongName, "");
            tail = tail.Replace("(", " ").Replace(")", " ").Trim();

            var afterSlash = tail.Contains('/') ? tail[(tail.LastIndexOf('/') + 1)..] : tail;
            var cand = afterSlash.Trim();

            var mIso = RxIso3AtEnd.Match(cand);
            if (mIso.Success && TryResolveCountryToken(mIso.Groups[1].Value, out countryTpId))
                return true;

            if (!string.IsNullOrWhiteSpace(cand) && TryResolveCountryToken(cand, out countryTpId))
                return true;

            return false;
        }


        private bool TryResolveCountryToken(string? token, out int countryTpId)
        {
            countryTpId = 0;
            if (string.IsNullOrWhiteSpace(token)) return false;

            token = token.Trim();
            // Probaj ISO3
            if (_refData.CountriesByISO3.TryGetValue(token.ToUpperInvariant(), out var c3) && c3.CountryTPId.HasValue)
            { countryTpId = c3.CountryTPId.Value; return true; }

            // Probaj ISO2
            if (_refData.CountriesByISO2.TryGetValue(token.ToUpperInvariant(), out var c2) && c2.CountryTPId.HasValue)
            { countryTpId = c2.CountryTPId.Value; return true; }

            // Probaj kao puno ime (case-insensitive mape)
            if (_refData.CountriesByName.TryGetValue(token, out var cn) && cn.CountryTPId.HasValue)
            { countryTpId = cn.CountryTPId.Value; return true; }

            // Ako token izgleda kao filename “spain” → ime
            var asName = token.Replace('-', ' ').Trim();
            if (_refData.CountriesByName.TryGetValue(asName, out var cn2) && cn2.CountryTPId.HasValue)
            { countryTpId = cn2.CountryTPId.Value; return true; }

            return false;
        }

        // --------------- Helperi ---------------

        /// dd.MM.yy ili dd.MM.yyyy → DateTime (pivot: 00–29 ⇒ 2000+, inače 1900+)
        private static bool TryParseCroDate(string dd, string MM, string yyOrYyyy, out DateTime dt)
        {
            dt = default;
            if (!int.TryParse(dd, out var d) || !int.TryParse(MM, out var m)) return false;

            if (yyOrYyyy.Length == 2 && int.TryParse(yyOrYyyy, out var yy2))
            {
                var year = (yy2 <= 29 ? 2000 : 1900) + yy2;
                return DateTime.TryParseExact($"{d:00}.{m:00}.{year}", "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            }

            if (yyOrYyyy.Length == 4 && int.TryParse(yyOrYyyy, out var y4))
            {
                return DateTime.TryParseExact($"{d:00}.{m:00}.{y4}", "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            }

            return false;
        }

        /// Ukloni dijakritiku (npr. “ljevak” → “ljevak” bez kvačica)
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var norm = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(norm.Length);
            foreach (var ch in norm)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
