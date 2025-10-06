using HtmlAgilityPack;

namespace BonzoByte.Core.Helpers
{
    public static class HtmlParsingExtensions
    {
        public static string? InnerTextTrimmed(this HtmlNode? node) =>
            node?.InnerText?.Trim();

        public static bool HasClass(this HtmlNode node, string cls) =>
            node.GetAttributeValue("class", "").Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Any(c => string.Equals(c, cls, StringComparison.OrdinalIgnoreCase));
    }
}