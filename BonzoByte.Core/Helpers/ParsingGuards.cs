using HtmlAgilityPack;
using System.Globalization;

namespace BonzoByte.Core.Helpers
{
    public static class ParsingGuards
    {
        public static bool TryParseInt(string? s, out int v) =>
            int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

        public static bool TryParseLong(string? s, out long v) =>
            long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

        public static bool TryParseDateDMY(string? s, out DateTime dt)
        {
            dt = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var formats = new[] {"d.M.yyyy","dd.MM.yyyy","d.MM.yyyy","dd.M.yyyy","d. M. yyyy","dd. M. yyyy","d. MM. yyyy","dd. MM. yyyy"
};
            return DateTime.TryParseExact(s.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        }

        public static string? TryDecompressBrotliToString(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                return BrotliCompressor.DecompressFileToString(path);
            }
            catch (FormatException) { return null; }
            catch (IOException) { return null; }
        }

        /// <summary>
        /// Minimalni “shape” koji očekujemo na stranici
        /// </summary>
        public static bool IsLikelyValidPage(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//div[@class='table_outer_main']") != null
                && doc.DocumentNode.SelectSingleNode("//table") != null;
        }

        public static bool TrySelectSingle(HtmlNode root, string xPath, out HtmlNode node)
        {
            node = root.SelectSingleNode(xPath);
            return node != null;
        }

        public static string CleanTournamentBlock(string html)
        {
            return html.Replace("</table>", "")
                       .Trim()
                       .Replace("> <", "><")
                       .Replace(" title=\"prikaži informacije o igraču i njegove mečeve (slični mečevi u proteklih 360 dana)\"", "")
                       .Replace(" title=\"informacije o meču / usporedi igrače (slični mečevi u proteklih 360 dana)\"", "")
                       .Replace(" title=\"prikaži međusobne mečeve\"", "")
                       .Replace(" title=\"AI analiza\"", "")
                       .Replace("<strong>", "")
                       .Replace("</strong>", "")
                       .Replace(" >", "")
                       .Replace(" <", "")
                       .Replace(" align=\"center\"", "");
        }
    }
}