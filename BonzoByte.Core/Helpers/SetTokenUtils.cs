using System.Globalization;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class SetTokenUtils
    {
        // Compact build: "AB" ili "AB(tb)"
        public static string BuildCompact(int a, int b, int? tb = null) =>
            tb.HasValue ? $"{a}{b}({tb.Value})" : $"{a}{b}";

        // Fleksibilni parser: podržava "a-b", "a:b", "ab", opcionalno "(tb)"
        private static readonly Regex Flex =
            new(@"^\s*(?<a>\d+)\s*(?:[:\-]?\s*)(?<b>\d+)\s*(?:\(\s*(?<tb>\d+)\s*\))?\s*$",
                RegexOptions.Compiled);

        public static bool TryParseFlexible(string token, out int a, out int b, out int? tb)
        {
            a = b = 0; tb = null;
            if (string.IsNullOrWhiteSpace(token)) return false;

            var m = Flex.Match(token);
            if (!m.Success) return false;

            if (!int.TryParse(m.Groups["a"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out a)) return false;
            if (!int.TryParse(m.Groups["b"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out b)) return false;

            if (m.Groups["tb"].Success &&
                int.TryParse(m.Groups["tb"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var t))
                tb = t;

            return true;
        }
    }
}