using System.Net;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class HtmlCleaner
    {
        public static string Clean(string rawHtml)
        {
            if (string.IsNullOrWhiteSpace(rawHtml))
                return rawHtml;

            var cleaned = rawHtml;

            // Sanacija zatvaranja tagova
            cleaned = Regex.Replace(cleaned, @"</td>\s*/?a>", "</a></td>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            cleaned = Regex.Replace(cleaned, @"</td>a>", "</a></td>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            cleaned = Regex.Replace(cleaned, @"</td>/a>", "</a></td>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            cleaned = Regex.Replace(cleaned, @"</td></strong>", "</strong></td>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            cleaned = Regex.Replace(cleaned, @"(<strong>[^<]*)</td>", "$1</strong></td>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Prazni td-ovi
            cleaned = Regex.Replace(cleaned, @"<td[^>]*>\s*</td>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Uklanjanje komentara
            cleaned = Regex.Replace(cleaned, @"<!--.*?-->", "", RegexOptions.Singleline | RegexOptions.Compiled);

            // Zamjene posebnih znakova
            cleaned = cleaned.Replace("\r", "").Replace("\n", "").Replace("&nbsp;", " ");

            // Višestruki razmaci
            cleaned = Regex.Replace(cleaned, @"\s{2,}", " ", RegexOptions.Compiled);

            // Zatvaranje font tagova
            cleaned = Regex.Replace(cleaned, @"(<font[^>]*>[^<]*)</td>", "$1</font></td>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Redundandni <br><br>
            cleaned = Regex.Replace(cleaned, @"<br\s*/?>\s*<br\s*/?>", "<br>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Decode HTML entiteta (&eacute; -> é)
            cleaned = WebUtility.HtmlDecode(cleaned);

            return cleaned;
        }
    }
}