using BonzoByte.Core.Models;
using BonzoByte.Core.Services.Interfaces;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class PlayerParser
    {
        public static (Player player, string? seed) ParsePlayerTd(HtmlNode playerTd, IReferenceDataService reference)
        {
            // 1. Dohvat <a href="?a=player&p1_id=...">
            var linkNode = playerTd.SelectSingleNode(".//a[contains(@href, 'p1_id')]")
                           ?? playerTd.ParentNode?.SelectSingleNode(".//a[contains(@href, 'p1_id')]");

            var href = linkNode?.GetAttributeValue("href", "") ?? "";
            var name = HtmlHelper.Decode(linkNode?.InnerText?.Trim()) ?? "";
            var playerIdMatch = Regex.Match(href, @"p1_id=(\d+)");
            var playerTPId = playerIdMatch.Success ? int.Parse(playerIdMatch.Groups[1].Value) : 0;

            // 2. Dohvat ISO3 country koda i eventualnog seeda iz okoline
            var spanText = string.IsNullOrWhiteSpace(playerTd.InnerText)
                ? playerTd.ParentNode?.InnerText ?? ""
                : playerTd.InnerText;

            var countryMatch = Regex.Match(spanText, @"\(([A-Z]{3})\)");
            var seedMatch = Regex.Match(spanText, @"\[(.*?)\]");

            string? iso3 = countryMatch.Success ? countryMatch.Groups[1].Value : null;
            reference.CountriesByISO3.TryGetValue(iso3 ?? "", out var country);

            string? seed = seedMatch.Success ? seedMatch.Groups[1].Value : null;

            // 3. Provjera postoji li već igrač
            if (!reference.Players.ContainsKey(playerTPId))
            {
                var newPlayer = new Player
                {
                    PlayerTPId = playerTPId,
                    PlayerName = name,
                    CountryTPId = country?.CountryTPId ?? 118 // default = Unknown
                };

                reference.AddPlayer(newPlayer);

                //Console.WriteLine($"✅ Dodan novi igrač: {newPlayer.PlayerName} (TPId: {newPlayer.PlayerTPId})");
            }

            // 4. Vraćamo igrača iz reference kolekcije
            reference.TryGetPlayer(playerTPId, out var parsedPlayer);

            return (parsedPlayer!, seed);
        }
    }
}