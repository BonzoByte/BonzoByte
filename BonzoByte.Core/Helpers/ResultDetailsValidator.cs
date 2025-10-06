// ResultDetailsValidator.cs
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class ResultDetailsValidator
    {
        // Dozvoljeni znakovi: znamenke, zagrade, razmaci
        private static readonly Regex AllowedCharsRegex =
            new(@"^[0-9\(\)\s]+$", RegexOptions.Compiled);

        // Jedan set token: najmanje 2 znamenke (AB), opcionalno "(tb)"
        private static readonly Regex SetTokenRegex =
            new(@"^\d{2,}(?:\(\d+\))?$", RegexOptions.Compiled);

        public static bool IsValid(string? resultDetails, string? result)
        {
            if (!(result == "20" || result == "21" || result == "30" || result == "31" || result == "32"))
                return false;

            if (string.IsNullOrWhiteSpace(resultDetails)) return false;
            if (!AllowedCharsRegex.IsMatch(resultDetails)) return false;

            if (string.IsNullOrWhiteSpace(result)) return false;

            // --- validiraj detalje setova ---
            var tokensResultDetails = resultDetails.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokensResultDetails.Length == 0) return false;
            foreach (var t in tokensResultDetails)
                if (!SetTokenRegex.IsMatch(t)) return false;

            // ✱ Nemoj primjenjivati SetTokenRegex na 'tokensResult' – to nije set-detalj nego zbirni ishod
            return true;
        }


        // Ostavi pomoćne ako ih koristiš drugdje
        public static bool TryExtractInt(string? s, out int v)
        {
            v = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var m = Regex.Match(s, @"\d+");
            return m.Success && int.TryParse(m.Value, out v);
        }

        public static int? ExtractTieBreak(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var m = Regex.Match(s, @"\((\d+)\)");
            if (m.Success && int.TryParse(m.Groups[1].Value, out var tb)) return tb;
            return null;
        }
    }
}
