using HtmlAgilityPack;

namespace BonzoByte.Core.Helpers
{
    public static class MatchHtmlExtractor
    {
        /// <summary>
        /// Extracts only the main HTML block with match data ("table_outer_main") and removes any irrelevant nodes.
        /// </summary>
        /// <param name="fullHtml">Raw HTML string extracted from .br archive.</param>
        /// <returns>Cleaned HTML string containing only match-relevant content.</returns>
        public static string ExtractRelevantHtml(string fullHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(fullHtml);

            // 1. Ukloni nepotrebni dio ako postoji
            var nodeToRemove = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='left_main_form']");
            if (nodeToRemove != null)
            {
                nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
            }

            // 2. Dohvati glavni node s podacima o mečevima
            var matchDataNode = htmlDoc.DocumentNode.SelectSingleNode(".//div[@class='table_outer_main']");

            return matchDataNode?.OuterHtml ?? "";
        }
    }
}