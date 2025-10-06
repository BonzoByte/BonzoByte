using BonzoByte.Core.Services.Interfaces;
using HtmlAgilityPack;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class HtmlHelper
    {
        public static string? Decode(string? input)
        {
            return string.IsNullOrWhiteSpace(input) ? null : WebUtility.HtmlDecode(input.Trim());
        }

        public static int ExtractSurfaceId(HtmlNode? surfaceImgNode, IReferenceDataService reference)
        {
            if (surfaceImgNode == null)
                return 1; // fallback na 'unknown'

            var src = surfaceImgNode.GetAttributeValue("src", "");
            var match = Regex.Match(src, @"surface_(\d+)\.png");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var parsedSurfaceId))
            {
                if (reference.Surfaces.TryGetValue(parsedSurfaceId, out var surface))
                    return surface.SurfaceId ?? 1;

                //Console.WriteLine($"⚠️ Nepoznata podloga: surface_{parsedSurfaceId}");
            }

            return 1;
        }

        public static int ExtractCountryId(HtmlNode? spanNode, IReferenceDataService reference)
        {
            if (spanNode == null)
                return 118; // fallback na 'World'

            var text = spanNode.InnerText?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return 118;

            var match = Regex.Match(text, @"\((.*?)\)");
            if (!match.Success)
                return 118;

            var countryName = match.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(countryName))
                return 118;

            if (reference.CountriesByName.TryGetValue(countryName, out var country))
                return country.CountryTPId ?? 118;

            Console.WriteLine($"⚠️ Nepoznata država: '{countryName}' — dodaj u šifrarnik ako treba.");
            return 118;
        }

        public static int ExtractPrize(HtmlNode? spanNode)
        {
            if (spanNode == null)
                return 0;

            var text = spanNode.InnerText?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            var match = Regex.Match(text, @"[:：] ?(.+?USD)", RegexOptions.IgnoreCase);
            if (!match.Success)
                return 0;

            var prizeText = match.Groups[1].Value;
            var index = prizeText.IndexOf("USD", StringComparison.OrdinalIgnoreCase);
            if (index > 0)
                prizeText = prizeText.Substring(0, index);

            prizeText = prizeText.Replace(" ", "");

            return int.TryParse(prizeText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var prize) ? prize : 0;
        }
    }
}