using BonzoByte.Core.Models;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    /// <summary>
    /// Pomaže postaviti determinističan i unikatan <see cref="DateTime"/> unutar dana
    /// za mečeve koji nemaju stvarno vrijeme ili sudaraju s postojećima.
    /// Algoritam:
    /// 1) Nađe sve mečeve na isti datum u kojima sudjeluje P1 ili P2.
    /// 2) Doda i trenutni meč, ukloni duplikate po MatchTPId.
    /// 3) Sortira po MatchTPId i uzme indeks kao broj sekundi od ponoći.
    /// 4) Ako postoji kolizija, pomiče se sekundu po sekundu do prve slobodne.
    /// 
    /// Invarianti:
    /// - Ako meč već ima vrijeme i nema kolizije, ne dira se.
    /// - Rezultat je stabilan za isti ulaz (deterministički).
    /// </summary>
    public static class MatchDateTimeHelper
    {
        /// <summary>
        /// Normalizira <see cref="Match.DateTime"/> na stabilnu sekundu unutar njegovog datuma,
        /// u odnosu na ostale mečeve istog dana u kojima sudjeluju Player1 ili Player2.
        /// </summary>
        /// <param name="match">Meč koji se normalizira. Očekuje se da su Player1TPId/Player2TPId postavljeni.</param>
        /// <param name="existing">Kolekcija već postojećih mečeva (isti model kao u bazi/cacheu).</param>
        public static void NormalizeMatchDateTime(Models.Match match, IReadOnlyCollection<Models.Match> existing)
        {
            if (match == null) return;
            if (!match.DateTime.HasValue) return;
            if (match.Player1TPId == null || match.Player2TPId == null) return;

            var dt = match.DateTime.Value;
            var date = dt.Date;

            int p1 = match.Player1TPId.Value;
            int p2 = match.Player2TPId.Value;

            // Kandidati: mečevi tog dana gdje igra P1 ili P2
            var sameDayForEitherPlayer = existing
                .Where(m => m.DateTime.HasValue && m.DateTime.Value.Date == date &&
                            (m.Player1TPId == p1 || m.Player2TPId == p1 ||
                             m.Player1TPId == p2 || m.Player2TPId == p2))
                .ToList();

            bool timeMissing = dt.TimeOfDay == TimeSpan.Zero;

            bool exactCollision = sameDayForEitherPlayer.Any(m =>
                m.DateTime!.Value == dt &&
                // različit meč (ili drugi rezultat) – tj. realna kolizija
                (m.MatchTPId != match.MatchTPId ||
                 !string.Equals(m.ResultDetails, match.ResultDetails, StringComparison.Ordinal)));

            // Ako već ima smisleno vrijeme i nema sudara, ne diramo ga.
            if (!timeMissing && !exactCollision)
                return;

            // Stabilan poredak unutar dana za oba igrača: po MatchTPId
            var daySet = sameDayForEitherPlayer
                .Concat(new[] { match })
                .DistinctBy(m => m.MatchTPId) // ako je već bio u existing
                .OrderBy(m => m.MatchTPId)    // deterministički ključ
                .ToList();

            int index = daySet.FindIndex(m => m.MatchTPId == match.MatchTPId);
            if (index < 0) index = daySet.Count - 1;

            // Dodijeli sekundu unutar dana prema poziciji
            match.DateTime = date.AddSeconds(index);

            // Ako i dalje postoji kolizija (npr. drugi turnir s istim indexom), gurni naprijed dok ne bude slobodno
            // (ovo je jeftino jer je skup mali).
            // Napomena: ne dodajemo match u existing; provjeravamo samo protiv existing-a.
            while (existing.Any(m => m.DateTime.HasValue && m.DateTime!.Value == match.DateTime))
            {
                match.DateTime = match.DateTime!.Value.AddSeconds(1);
            }
        }

        public static string? ExtractFirstDateAsFormattedString(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tdNode = htmlDoc.DocumentNode.SelectSingleNode("//td[@id='main_tit']");

            if (tdNode == null)
                return null;

            var text = tdNode.InnerText;

            // Traži prvi datum u formatu d.M.yyyy
            var dateMatch = Regex.Match(text, @"\b(\d{1,2})\.(\d{1,2})\.(\d{4})\b");

            if (!dateMatch.Success)
                return null;

            if (DateTime.TryParseExact(
                dateMatch.Value,
                "d.M.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate))
            {
                return parsedDate.ToString("yyyy_MM_dd");
            }

            return null;
        }
    }

    // .NET 6+ ima LINQ DistinctBy; ako si na starijem, dodaj ovu ekstenziju:
    internal static class LinqDistinctByShim
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var seen = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seen.Add(keySelector(element)))
                    yield return element;
            }
        }
    }
}