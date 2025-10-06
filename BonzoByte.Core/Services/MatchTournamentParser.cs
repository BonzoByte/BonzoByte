using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.DAL.Repositories;
using BonzoByte.Core.DTOs;
using BonzoByte.Core.Helpers;
using BonzoByte.Core.Models;
using BonzoByte.Core.Services;
using BonzoByte.Core.Services.Interfaces;
using BonzoByte.ML;
using HtmlAgilityPack;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using Player = BonzoByte.Core.Models.Player;

namespace BonzoByte.Core.Services
{
    public class MatchTournamentParser
    {
        private readonly IReferenceDataService _reference;
        private readonly ITournamentEventRepository _tournamentEventRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchOddsRepository _matchOddsRepository;
        private readonly MatchOddsParserService _matchOddsParser;
        private readonly ScrapingPathResolver _scrapingPathResolver;
        private readonly TournamentParserHelper _tournamentHelper;
        private static TournamentEventDownloaderService? _tournamentEventDownloaderService;
        private static MatchDetailsDownloaderService? _matchDetailsDownloaderService;
        private static PlayerHtmlParser _playerDetailService;
        private readonly List<Models.Match> _activeMatches = new();
        private List<Models.Match>? _dayliMatches = new List<Models.Match>();
        private List<Models.Match>? _ordered;
        private bool _orderedDirty = true;
        private readonly List<Models.Match> _matchesLastWeek = new();
        private readonly List<Models.Match> _matchesLastMonth = new();
        private readonly List<Models.Match> _matchesLastYear = new();
        private readonly NNRouterScorer _nnRouterScorer;
        private readonly Dictionary<DateOnly, List<MatchDTO>> _dailyByDate = new();
        private readonly MatchOddsFlaggerService _matchOddsFlagger;
        private readonly string _dailyOutDir = @"D:\BrotliArchives\DayliMatches";
        private readonly string _matchDetailsOutDir = @"D:\BrotliArchives\MatchDetails";
        private readonly string _playerDetailsOutDir = @"D:\BrotliArchives\PlayerDetails";

        public MatchTournamentParser(IReferenceDataService reference, ITournamentEventRepository tournamentEventRepository, IPlayerRepository playerRepository, IMatchRepository matchRepository, IMatchOddsRepository matchOddsRepository, TournamentParserHelper tournamentHelper, MatchDetailsDownloaderService matchDetailsDownloaderService, ScrapingPathResolver scrapingPathResolver, NNRouterScorer nnRouterScorer, MatchOddsParserService matchOddsParser, PlayerHtmlParser playerDetailService, MatchOddsFlaggerService matchOddsFlagger)
        {
            _reference = reference;
            _tournamentEventRepository = tournamentEventRepository;
            _playerRepository = playerRepository;
            _matchRepository = matchRepository;
            _matchOddsRepository = matchOddsRepository;
            _tournamentHelper = tournamentHelper;
            _matchDetailsDownloaderService = matchDetailsDownloaderService;
            _scrapingPathResolver = scrapingPathResolver;
            _nnRouterScorer = nnRouterScorer;
            _matchOddsParser = matchOddsParser;
            _playerDetailService = playerDetailService;
            _matchOddsFlagger = matchOddsFlagger;
        }

        public static int? MapRoundNameToId(string roundName)
        {
            if (roundName != null)
            {
                return roundName.Trim().ToLower() switch
                {
                    "kvalifikacije" => 1,
                    "1. kolo" => 2,
                    "2. kolo" => 3,
                    "3. kolo" => 4,
                    "4. kolo" => 5,
                    "5. kolo" => 6,
                    "osmina-finala" => 7,
                    "cetvrt-finala" => 8,
                    "polu-finale" => 9,
                    "finale" => 10,
                    _ => null
                };
            }
            else return null;
        }

        public async Task<MatchParseResultDTO?> ParseAsync(string html, TournamentEventDownloaderService tournamentEventDownloaderService)
        {
            string? dateString = MatchDateTimeHelper.ExtractFirstDateAsFormattedString(html);

            _tournamentEventDownloaderService = tournamentEventDownloaderService;

            bool isQualificationMatch = false;

            // Rani shape guard
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            if (!ParsingGuards.IsLikelyValidPage(htmlDoc))
            {
                Console.WriteLine("[Guard] HTML not in expected shape — skipping page.");
                return null;
            }

            // Sigurno čitanje t_p (bez NullRef/FormatException)
            var navListActive = htmlDoc.DocumentNode.SelectSingleNode("//ul[@id='navlist']//li[@id='active']/a");
            var tournamentLevelIdText = navListActive != null
                ? Regex.Match(navListActive.GetAttributeValue("href", ""), @"t_p=(\d+)").Groups[1].Value
                : "0";
            ParsingGuards.TryParseInt(tournamentLevelIdText, out var tournamentLevelId);
            isQualificationMatch = tournamentLevelIdText == "3";

            // “Ošišaj” okvir i clean
            html = Regex.Replace(html, "^.*?<div class=\"table_outer_main\">", "");
            html = Regex.Replace(html, "<div id=\"left_main_form\">.*$", "");
            int pos = html.IndexOf("<tr> <td");
            if (pos == -1) return null; ;

            var tournamentTypeHTML = html[..pos];
            var mainTitText = tournamentTypeHTML.Trim().ToUpperInvariant();

            int tournamentTypeId = 0;
            if (mainTitText!.Contains("ATP") && mainTitText.Contains("SINGL")) tournamentTypeId = 2;
            else if (mainTitText.Contains("WTA") && mainTitText.Contains("SINGL")) tournamentTypeId = 4;
            else if (mainTitText.Contains("ATP") && mainTitText.Contains("PAROVI") || mainTitText.Contains("DOUBLES")) tournamentTypeId = 1;
            else if (mainTitText.Contains("WTA") && mainTitText.Contains("PAROVI") || mainTitText.Contains("DOUBLES")) tournamentTypeId = 3;

            // Datum bez bacanja
            DateTime? tournamentDate = null;
            var matchDate = Regex.Match(mainTitText, @"\b\d{1,2}\.\s*\d{1,2}\.\s*\d{4}\b");
            if (matchDate.Success && ParsingGuards.TryParseDateDMY(matchDate.Value, out var dt))
                tournamentDate = dt;

            //Console.WriteLine($"🎯 Nađen turnir s datumom: {tournamentDate}");

            // Clean remainder
            html = ParsingGuards.CleanTournamentBlock(html[pos..]);

            List<string> tournamentEvents = new List<string>();
            List<string> tournamentEventsData = new List<string>();

            while (html.Contains("td class=\"main_tour\""))
            {
                pos = html.IndexOf("</tr>");
                var tournamentEventHTML = html.Substring(0, pos + 5);
                tournamentEvents.Add(tournamentEventHTML);
                html = html.Substring(pos + 5).Trim();
                pos = html.IndexOf("td class=\"main_tour\"");
                if (pos < 0)
                {
                    tournamentEventsData.Add(html);
                }
                else
                {
                    var _tournamentEventsData = html.Substring(0, pos - 5);
                    tournamentEventsData.Add(_tournamentEventsData);
                    html = html.Substring(pos - 5).Trim();
                }
            }

            for (int i = 0; i < tournamentEventsData.Count; i++)
            {
                int prize = 0;
                int countryId = 0;
                int surfaceId = 0;
                string? tournamentName = null;
                int tournamentEventTpId = 0;

                var node = new HtmlDocument();
                node.LoadHtml(tournamentEvents[i]);

                // 🟡 1. HREF → TID
                var aNode = node.DocumentNode.SelectSingleNode("//a[contains(@href, 'tid=')]");
                if (aNode != null)
                {
                    var href = aNode.GetAttributeValue("href", "");
                    var tid = HttpUtility.ParseQueryString(href).Get("tid");
                    if (int.TryParse(tid, out var parsedTid))
                        tournamentEventTpId = parsedTid;
                }

                // 🟡 2. tournamentInfo td
                var tournamentInfo = node.DocumentNode.SelectSingleNode("//td[contains(@class, 'main_tour')]");
                if (tournamentInfo != null)
                {
                    // 🟢 Surface
                    surfaceId = HtmlHelper.ExtractSurfaceId(
                        tournamentInfo.SelectSingleNode(".//img[contains(@class, 'surface_icon')]"),
                        _reference
                    );

                    // 🟢 Name
                    tournamentName = HtmlHelper.Decode(
                        tournamentInfo.SelectSingleNode(".//a")?.InnerText
                    );

                    // 🟢 Country & Prize (iz <span>)
                    var spanNode = tournamentInfo.SelectSingleNode(".//span");
                    if (spanNode != null)
                    {
                        countryId = HtmlHelper.ExtractCountryId(spanNode, _reference);
                        try { prize = HtmlHelper.ExtractPrize(spanNode); }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"[PrizeParse] Format: '{spanNode?.InnerTextTrimmed()}' | {ex.Message}");
                            prize = 0;
                        }
                    }

                    //Console.WriteLine($"📍 Turnir info: {tournamentName} (TID={tournamentEventTpId}), CountryId={countryId}, SurfaceId={surfaceId}, Prize={prize}");
                }

                var newTournamentEvent = new TournamentEvent
                {
                    TournamentEventTPId = tournamentEventTpId,
                    TournamentEventName = tournamentName,
                    CountryTPId = countryId,
                    TournamentEventDate = tournamentDate,
                    TournamentLevelId = tournamentLevelId,
                    TournamentTypeId = tournamentTypeId,
                    Prize = prize,
                    SurfaceId = surfaceId
                };

                if (!_reference.TournamentEvents.ContainsKey(tournamentEventTpId))
                {
                    _reference.AddTournamentEvent(newTournamentEvent);

                    await _tournamentEventRepository.InsertTournamentEvent(newTournamentEvent, new Models.Match());

                    if (!_tournamentEventDownloaderService.IsTournamentEventArchived(tournamentEventTpId))
                    {
                        bool downloaded = await _tournamentEventDownloaderService.DownloadAndArchiveTournamentEventAsync(tournamentEventTpId);
                        if (!downloaded) Console.WriteLine($"[!] Upozorenje: turnir {tournamentEventTpId} nije uspješno preuzet.");
                    }
                }

                // Rounds mapa preko TryDecompress
                var roundsFromTournamentHtml = new Dictionary<long, string>();
                string archivePath = _scrapingPathResolver.GetTournamentEventArchivePath(tournamentEventTpId);
                var tournamentHtml = ParsingGuards.TryDecompressBrotliToString(archivePath);
                if (!string.IsNullOrEmpty(tournamentHtml))
                {
                    var roundInfos = MatchRoundParser.ParseMatchRoundsFromTournamentHtml(tournamentHtml);
                    foreach (var info in roundInfos)
                    {
                        var roundName = (info.RoundName ?? "").TrimStart('-');
                        if (isQualificationMatch) roundName = "kvalifikacije";
                        roundsFromTournamentHtml[info.MatchTPId] = roundName;
                    }
                }

                var _matchNodes = new HtmlDocument();
                _matchNodes.LoadHtml(tournamentEventsData[i]);
                var matchNodes = _matchNodes.DocumentNode.SelectNodes(".//tr");

                if (matchNodes == null) continue;

                int k = 0;

                for (int j = 0; j < matchNodes.Count; j++)
                {
                    while (k + 1 < matchNodes.Count && matchNodes[k].GetAttributeValue("class", "").StartsWith("match") && matchNodes[k + 1].GetAttributeValue("class", "").StartsWith("match"))
                    {
                        var row1 = matchNodes[k];
                        var row2 = matchNodes[k + 1];

                        // Sigurni td selektori u petlji
                        var player1Td = row1.SelectSingleNode(".//td[contains(@class, 'main_player')]");
                        var player2Td = row2.SelectSingleNode(".//td[contains(@class, 'main_player')]");
                        //var infoTd = row1.SelectSingleNode(".//td[contains(@class, 'main_por')]");
                        if (player1Td is null || player2Td is null || row1.OuterHtml.Contains("/<br>") || row2.OuterHtml.Contains("/<br>"))// || infoTd is null)
                        {
                            Console.WriteLine($"[Parser] Skip: missing td (TE={tournamentEventTpId})");
                            k += 2; continue;
                        }

                        // ➕ Parsiraj igrače
                        var (player1, seed1) = PlayerParser.ParsePlayerTd(player1Td, _reference);
                        var (player2, seed2) = PlayerParser.ParsePlayerTd(player2Td, _reference);

                        await EnsurePlayerPersistedAsync(player1, tournamentTypeId);
                        await EnsurePlayerPersistedAsync(player2, tournamentTypeId);

                        int p1Id = (int)player1.PlayerTPId!;
                        int p2Id = (int)player2.PlayerTPId!;

                        if (!_reference.Players.ContainsKey(p1Id))
                        {
                            _tournamentHelper.ApplyDefaultPlayerValues(player1);

                            var p1Details = await _playerDetailService.LoadOrDownloadAndParseAsync(p1Id);

                            player1.PlayerName = p1Details.PlayerName;
                            player1.CountryTPId ??= p1Details.CountryTPId;   // nemoj ostaviti null ako je NOT NULL u bazi
                            player1.PlayerBirthDate = p1Details.PlayerBirthDate;
                            player1.PlayerHeight = p1Details.PlayerHeight;
                            player1.PlayerWeight = p1Details.PlayerWeight;
                            player1.PlayerTurnedPro = p1Details.PlayerTurnedPro;
                            player1.PlaysId = p1Details.PlaysId;
                            player1.TournamentTypeId = tournamentTypeId;

                            await _playerRepository.InsertPlayerAsync(player1);   // prvo DB
                            _reference.Players[p1Id] = player1;                   // onda cache
                        }

                        if (!_reference.Players.ContainsKey(p2Id))
                        {
                            _tournamentHelper.ApplyDefaultPlayerValues(player2);

                            var p2Details = await _playerDetailService.LoadOrDownloadAndParseAsync(p2Id);

                            player2.PlayerName = p2Details.PlayerName;
                            player2.CountryTPId ??= p2Details.CountryTPId;
                            player2.PlayerBirthDate = p2Details.PlayerBirthDate;
                            player2.PlayerHeight = p2Details.PlayerHeight;
                            player2.PlayerWeight = p2Details.PlayerWeight;
                            player2.PlayerTurnedPro = p2Details.PlayerTurnedPro;
                            player2.PlaysId = p2Details.PlaysId;
                            player2.TournamentTypeId = tournamentTypeId;

                            await _playerRepository.InsertPlayerAsync(player2);
                            _reference.Players[p2Id] = player2;
                        }

                        // MatchTPId + round bez KeyNotFound-a
                        var (matchTPId, player1TPId, player2TPId) = MatchParser.ParseMatchInfoTd(row1);
                        if (matchTPId == 0) { Console.WriteLine("[!] Meč ID nije pronađen – preskačem."); k += 2; continue; }

                        string round = "1. kolo";
                        if (!roundsFromTournamentHtml.TryGetValue(matchTPId, out round))
                        {
                            // fallback ostaje default ili naknadni pokušaj iz HTML-a
                        }

                        var match = new Models.Match
                        {
                            MatchTPId = matchTPId,
                            TournamentEventTPId = tournamentEventTpId,
                            DateTime = tournamentDate,
                            Player1TPId = player1TPId,
                            Player2TPId = player2TPId,
                            Player1Seed = seed1,
                            Player2Seed = seed2,
                            SurfaceId = newTournamentEvent.SurfaceId
                        };

                        bool IsFinished(Models.Match m) =>
                            (m.IsFinished ?? false) || (m.Result is "20" or "21" or "30" or "31" or "32");

                        bool SamePairEitherOrder(Models.Match a, Models.Match b) =>
                           (a.Player1TPId == b.Player1TPId && a.Player2TPId == b.Player2TPId) ||
                           (a.Player1TPId == b.Player2TPId && a.Player2TPId == b.Player1TPId);

                        // Tolerancija za “isti slot” (sitne promjene termina u istom danu)
                        bool AreSameSlot(DateTime? a, DateTime? b, TimeSpan tol) =>
                            !a.HasValue || !b.HasValue ? true : (a.Value - b.Value).Duration() <= tol;

                        // --- U tvom mjestu gdje provjeravaš duplikat:
                        var candidates = _reference.Matches.Values.ToList()
                            .Where(m => m.TournamentEventTPId == match.TournamentEventTPId && SamePairEitherOrder(m, match))
                            .ToList();

                        if (candidates.Count > 0)
                        {
                            var finishedExisting = candidates.FirstOrDefault(IsFinished);
                            var activeExisting = candidates.FirstOrDefault(m => !IsFinished(m));

                            if (finishedExisting != null)
                            {
                                // Već imamo završenu verziju → sve novo (active/finished) tretiraj kao duplikat
                                k += 2;
                                continue;
                            }

                            if (activeExisting != null)
                            {
                                if (IsFinished(match))
                                {
                                    // Ovo je FINALIZACIJA logičkog meča → nemoj preskočiti!
                                    // (po želji: obriši aktivnu verziju iz kolekcija/DB pa nastavi u finished granu)
                                    await _matchRepository.DeleteActiveByIdAsync(activeExisting.MatchTPId!.Value);
                                    _reference.Matches.Remove(activeExisting.MatchTPId!.Value);
                                    _matchesLastWeek.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                    _matchesLastMonth.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                    _matchesLastYear.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                    _dayliMatches.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                    // → dalje bez continue; pusti normalnu finished obradu
                                }
                                else
                                {
                                    // Oba su aktivna → razlikuj “isti slot” vs. “reschedule”
                                    var sameSlot = AreSameSlot(activeExisting.DateTime, match.DateTime, TimeSpan.FromHours(3));

                                    if (sameSlot)
                                    {
                                        // Vjerojatno isti dnevni feed/duplikat → preskoči
                                        k += 2;
                                        continue;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            // Reschedule (često novi ID, drugi dan/termin) → zamijeni stariji aktivni
                                            await _matchRepository.DeleteActiveByIdAsync(activeExisting.MatchTPId!.Value);
                                            _reference.Matches.Remove(activeExisting.MatchTPId!.Value);
                                            _matchesLastWeek.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                            _matchesLastMonth.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                            _matchesLastYear.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                            _dayliMatches!.RemoveAll(m => m.MatchTPId == activeExisting.MatchTPId);
                                            // → nastavi i ubaci ovaj novi aktivni
                                        }
                                        catch (Exception ex)
                                        {   
                                            string aaa;
                                            aaa = "";
                                        }

                                    }
                                }
                            }
                        }
                        // ako nema kandidata → normalni tok (insert active/finished)

                        //TODO privremeno ne parsam oddse dok se baza ne napuni
                        //if (match.MatchTPId is int mtp)
                        //{
                        //    try
                        //    {
                        //        _reference.TournamentEvents.TryGetValue(match.TournamentEventTPId ?? -1, out var te);
                        //        _reference.TournamentTypes.TryGetValue(te.TournamentTypeId ?? -1, out var tt);
                        //        if (tt!.TournamentTypeId == 2 || tt!.TournamentTypeId == 4)
                        //        {
                        //            await _matchOddsParser.ParseOddsFromArchiveAsync(
                        //                mtp,
                        //                match.DateTime,                 // lokalno (kako ga već držiš)
                        //                isFinished: match.IsFinished ?? false);
                        //        }
                        //        else continue;
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Console.WriteLine($"[Odds] Failed for MatchTPId={mtp}: {ex.Message}");
                        //    }
                        //}

                        try { match.RoundId = RoundMapper.Map(round!) ?? MapRoundNameToId(round!); }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"[PrizeParse] Format: '{round}' | {ex.Message}");
                            round = "";
                        }

                        // Downloader bez rušenja
                        try
                        {
                            await _matchDetailsDownloaderService!.DownloadMatchAssetsIfMissingAsync(matchTPId);
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine($"[Assets] IO fail for MatchTPId={matchTPId}: {ex.Message} — continuing.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Assets] Unexpected for MatchTPId={matchTPId}: {ex.Message}");
                        }

                        int p1 = match.Player1TPId ?? 0;
                        int p2 = match.Player2TPId ?? 0;

                        player1 = _reference.Players.Values.ToList().First(p => p.PlayerTPId == player1TPId);
                        player2 = _reference.Players.Values.ToList().First(p => p.PlayerTPId == player2TPId);

                        var surface = match.SurfaceId ?? 0;

                        MatchParser.ParseMatchRows(row1, row2, match);
                        
                        // Dupe check (uključuje i već aktivne u bazi)
                        bool exists = await _matchRepository.ExistsAsync(
                            match.TournamentEventTPId ?? 0,
                            match.Player1TPId ?? 0,
                            match.Player2TPId ?? 0
                        );
                        if (exists) { k += 2; continue; }

                        MatchDateTimeHelper.NormalizeMatchDateTime(match, _reference.Matches.Values.ToList()); // DT normalizacija

                        var tags = new Dictionary<string, object>();

                        if (match.IsFinished == true)
                        {
                            var current = match.DateTime!.Value;

                            // 1) Pre-match polja koja ne mijenjaju TS (ako ti tu treba što prije rezultata)
                            //ComputePrematchFields(match, player1, player2);

                            // 2) H2H: prvo pre-match snapshot (OLD), pa odmah "finish" verzija
                            var latestH2H = _matchRepository.GetLatestH2HMatchAsync(p1, p2);
                            H2HHelper.InjectPreMatchSnapshot(match, _ => latestH2H);   // samo OLD i copy post iz prethodnog
                            H2HHelper.ComputeAndInjectH2H(match, _ => latestH2H);      // postavi POST stanje za ovaj završeni meč

                            // 3) Granular/TS, streakovi i svi agregati koji se rade SAMO na završenim mečevima
                            TrueSkillGranularUpdater.InjectSetAndGameStats(match);
                            MatchHelper.InjectCurrentTrueSkillSnapshot(match, player1, player2);
                            TrueSkillUpdateHelper.ComputeAndApplyGlobalFromMatchSnapshots(match);
                            TrueSkillUpdateHelper.ComputeAndApplySurfaceFromPlayers(match, player1, player2, useMsdForSmGsm: true, useGlobalMsdForSurfaceM: true);
                            TrueSkillUpdateHelper.BackfillMissingSurfaceWinProbabilities(match, player1, player2, useMsdForSmGsm: true, useGlobalMsdForSurfaceM: true);
                            StreakHelper.UpdateStreaks(match, player1, player2);

                            var matchesToRemoveLastWeek = _matchesLastWeek.Where(m => m.DateTime!.Value < current.AddDays(-7)).ToList();
                            var matchesToRemoveLastMonth = _matchesLastMonth.Where(m => m.DateTime!.Value < current.AddMonths(-1)).ToList();
                            var matchesToRemoveLastYear = _matchesLastYear.Where(m => m.DateTime!.Value < current.AddYears(-1)).ToList();

                            WinLossHelper.UpdateWinLossByTimeFrame(matchesToRemoveLastWeek, matchesToRemoveLastMonth, matchesToRemoveLastYear, _reference.Players.Values.ToList());
                            WinLossHelper.UpdateMatchWinLoss(match, player1, player2);
                            WinLossHelper.UpdatePlayerWinLoss(match, player1, player2);
                            FavouriteUnderdogStatsHelper.UpdateFavouriteUnderdogStatsByTimeFrame(matchesToRemoveLastWeek, matchesToRemoveLastMonth, matchesToRemoveLastYear, _reference.Players.Values.ToList());
                            FavouriteUnderdogStatsHelper.UpdateFavouriteUnderdogStats(match, player1, player2);
                            FavouriteUnderdogStatsHelper.UpdatePlayerFavouriteUnderdogStats(match, player1, player2);

                            TrueSkillUpdateHelper.PushPosteriorToPlayers(match, player1, player2);
                            DateSinceLastWinLossHelper.UpdateDateWinLoss(match, player1, player2);

                            // 4) Memorija / cache liste
                            _reference.Matches[match.MatchTPId!.Value] = match;
                            _matchesLastYear!.Add(match);
                            _matchesLastMonth!.Add(match);
                            _matchesLastWeek!.Add(match);
                            _dayliMatches!.Add(match);   // (typo, ali pretpostavljam namjerno)
                            _orderedDirty = true;

                            // 5) DB insert
                            await _matchRepository.InsertMatchAsync(match);


                            // 6) NN scoring -> pobrini se da match.WinProbabilityNN bude popunjen PRIJE arhive
                            try
                            {
                                if (match.MatchTPId.HasValue)
                                {
                                    double nn = await _nnRouterScorer.ScoreAndUpdateAsync(match.MatchTPId.Value);
                                    match.WinProbabilityNN = nn;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[NNRouter] Scoring failed for MatchTPId={match.MatchTPId}: {ex.Message}");
                            }

                            //TODO privremeno ne parsam oddse dok se baza ne napuni
                            //if (match.MatchTPId is int mid)
                            //{
                            //    // Aktivni meč = nema valjan rezultat → parsiraj kvote
                            //    if (!(match.IsFinished ?? false))
                            //    {
                            //        await _matchOddsParser.ParseOddsFromArchiveAsync(
                            //            mid,
                            //            match.DateTime,                 // lokalno (kako ga već držiš)
                            //            isFinished: match.IsFinished ?? false);
                            //    }
                            //    else
                            //    {
                            //        // Povijesni meč → opcionalno JEDNOM parsiraj (ako želiš snapshot), pa premjesti u Finished
                            //        await _matchOddsParser.ParseOddsFromArchiveAsync(
                            //            mid,
                            //            match.DateTime,                 // lokalno (kako ga već držiš)
                            //            isFinished: match.IsFinished ?? false);
                            //        MoveOddsFileToFinished(mid);
                            //    }

                            //    // ⬇️ Konsenzus i “odrezivanje” čudnih bookie-ja
                            //    var allForMatch = _reference.GetLatestMatchOddsByBookie(mid);
                            //    var clean = CleanOddsResult.CleanOddsForMatch(
                            //        allForMatch,
                            //        match.DateTime,                              // start lokalno (po našem TZ)
                            //        isFinished: match.IsFinished == true
                            //    );

                            //    double? bestP1 = clean.BestP1;
                            //    double? bestP2 = clean.BestP2;
                            //    double? medP1 = clean.medP1;
                            //    double? medP2 = clean.medP2;

                            //    // NN → value margine
                            //    double? wp1 = match.WinProbabilityNN;
                            //    double? wp2 = wp1.HasValue ? 1 - wp1.Value : (double?)null;

                            //    double? vm1 = (bestP1.HasValue && wp1.HasValue) ? bestP1.Value * wp1.Value - 1 : (double?)null;
                            //    double? vm2 = (bestP2.HasValue && wp2.HasValue) ? bestP2.Value * wp2.Value - 1 : (double?)null;
                            //    double? medVm1 = (medP1.HasValue && wp1.HasValue) ? medP1.Value * wp1.Value - 1 : (double?)null;
                            //    double? medVm2 = (medP2.HasValue && wp2.HasValue) ? medP2.Value * wp2.Value - 1 : (double?)null;

                            //    int? who2Bet = 0; // 0 default
                            //    int? medWho2Bet = 0; // 0 default
                            //    const double edge = 0.0;
                            //    const double tieBand = 1e-7;

                            //    if (vm1.HasValue || vm2.HasValue)
                            //    {
                            //        var e1 = vm1 ?? double.NegativeInfinity;
                            //        var e2 = vm2 ?? double.NegativeInfinity;

                            //        bool p1ok = e1 > edge;
                            //        bool p2ok = e2 > edge;

                            //        if (!p1ok && !p2ok) who2Bet = 0;
                            //        else if (p1ok && !p2ok) who2Bet = 1;
                            //        else if (!p1ok && p2ok) who2Bet = 2;
                            //        else
                            //        {
                            //            var diff = e1 - e2;
                            //            who2Bet = Math.Abs(diff) <= tieBand ? 0 : (diff > 0 ? 1 : 2);
                            //        }
                            //    }

                            //    if (medVm1.HasValue || medVm2.HasValue)
                            //    {
                            //        var e1 = medVm1 ?? double.NegativeInfinity;
                            //        var e2 = medVm2 ?? double.NegativeInfinity;

                            //        bool p1ok = e1 > edge;
                            //        bool p2ok = e2 > edge;

                            //        if (!p1ok && !p2ok) medWho2Bet = 0;
                            //        else if (p1ok && !p2ok) medWho2Bet = 1;
                            //        else if (!p1ok && p2ok) medWho2Bet = 2;
                            //        else
                            //        {
                            //            var diff = e1 - e2;
                            //            medWho2Bet = Math.Abs(diff) <= tieBand ? 0 : (diff > 0 ? 1 : 2);
                            //        }
                            //    }

                            //    // Ako već tu puniš DTO – super. Ako DTO radiš kasnije, ove vrijednosti proslijediš.
                            //    // Primjer kad radiš dto kasnije:

                            //    tags["BestP1"] = bestP1;
                            //    tags["BestP2"] = bestP2;
                            //    tags["VM1"] = vm1;
                            //    tags["VM2"] = vm2;
                            //    tags["Who2Bet"] = who2Bet;
                            //    tags["MedP1"] = medP1;
                            //    tags["MedP2"] = medP2;
                            //    tags["MedVM1"] = medVm1;
                            //    tags["MedVM2"] = medVm2;
                            //    tags["MedWho2Bet"] = medWho2Bet;
                            //    tags["OddsQuality"] = clean.Kept.Count < 3 ? "insufficient_consensus" : "ok";
                            //}

                            // 7) Persist stanja igrača
                            await _playerRepository.UpdatePlayerAsync(player1, match);
                            await _playerRepository.UpdatePlayerAsync(player2, match);

                            //TODO : privremeno ne parsam oddse dok se baza ne napuni
                            // 8) Dnevna arhiva (sad kad je NN sigurno u match-u)
                            // sve kvote za meč
                            //var _all = _reference.GetOddsForMatch((int)match.MatchTPId);
                            //// najbolje po strani
                            //var (_bestP1, _bestP2) = _reference.GetBestOddsBySide((int)match.MatchTPId);
                            //// prosjek po strani
                            //var (_avgP1, _avgP2) = _reference.GetAverageOdds((int)match.MatchTPId);
                            //await AccumulateForDailyArchiveAsync(match, tags);

                            // 9) Očisti prozore
                            _matchesLastWeek.RemoveAll(m => m.DateTime!.Value < current.AddDays(-7));
                            _matchesLastMonth.RemoveAll(m => m.DateTime!.Value < current.AddMonths(-1));
                            _matchesLastYear.RemoveAll(m => m.DateTime!.Value < current.AddYears(-1));
                            _dayliMatches.RemoveAll(m => m.DateTime!.Value < current.AddDays(-1));

                            // 10) Glavnu kolekciju držimo “mršavom”
                            _reference.Matches.Remove(match.MatchTPId!.Value);
                        }

                        else
                        {
                            // Ako želiš, i za active poravnaj vrijeme u odnosu na dnevne mečeve
                            MatchDateTimeHelper.NormalizeMatchDateTime(match, _reference.Matches.Values.ToList());

                            // Prethodni H2H zapis iz baze (strictly prije ovog meča)
                            var latestH2H = _matchRepository.GetLatestH2HMatchAsync(p1, p2);

                            // 1) Injektaj PRE-MATCH H2H snapshot + pWin (bez posteriora)
                            H2HHelper.ComputeAndInjectH2H(match, _ => latestH2H /*, useGlobalMSDForAllModels: true*/);

                            // 2) Injektaj *trenutni* TS snapshot iz igrača u meč (Old vrijednosti)
                            MatchHelper.InjectCurrentTrueSkillSnapshot(match, player1, player2);

                            // 3) Pobrinemo se da TS pWin-ovi po surfaceu nisu null (bez recalca)
                            TrueSkillUpdateHelper.BackfillMissingSurfaceWinProbabilities(
                                match, player1, player2,
                                useMsdForSmGsm: true, useGlobalMsdForSurfaceM: true);

                            // 4) “Derived” metrike koje ne diraju igrače
                            DateSinceLastWinLossHelper.UpdateDateWinLoss(match, player1, player2);
                            StreakHelper.UpdateStreaks(match, player1, player2);

                            // 5) Spremi ACTIVE meč u bazu
                            await _matchRepository.InsertMatchAsync(match);

                            // 6) NN scoring (imamo kompletne pre-match feature-e)
                            try
                            {
                                if (match.MatchTPId.HasValue)
                                    await _nnRouterScorer.ScoreAndUpdateAsync(match.MatchTPId.Value);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[NNRouter] Scoring failed for MatchTPId={match.MatchTPId}: {ex.Message}");
                            }

                            //await _matchOddsParser.ParseOddsFromArchiveAsync((int)match.MatchTPId, match.DateTime, match.IsFinished ?? false);
                            //await _matchOddsFlagger.FlagAsync((int)match.MatchTPId, match.IsFinished ?? false, match.DateTime);
                            //await AccumulateForDailyArchiveAsync(match, tags);
                        }

                        k += 2;
                    }
                }
            }

            return new MatchParseResultDTO
            {
                Matches = _dayliMatches!,
                MatchesLastWeek = _matchesLastWeek!,
                MatchesLastMonth = _matchesLastMonth!,
                MatchesLastYear = _matchesLastYear!,
                Players = _reference.Players.Values.ToList(),
                TournamentEvents = _reference.TournamentEvents.Values.ToList()
            };
        }

        private static readonly Dictionary<DateOnly, SemaphoreSlim> _dailyLocks = new();

        private SemaphoreSlim GetDailyGate(DateOnly key)
        {
            lock (_dailyLocks)
                return _dailyLocks.TryGetValue(key, out var s) ? s : (_dailyLocks[key] = new SemaphoreSlim(1, 1));
        }

        private void ComputePrematchFields(Models.Match match, Player player1, Player player2)
        {
            var p1 = match.Player1TPId ?? 0;
            var p2 = match.Player2TPId ?? 0;

            // H2H snapshot (pre-match)
            Models.Match latestH2H = _matchRepository.GetLatestH2HMatchAsync(p1, p2);
            H2HHelper.ComputeAndInjectH2H(match, _ => latestH2H);

            // TS snapshot + pre-match p-ovi (M/SM/GSM i surface-pick)
            MatchHelper.InjectCurrentTrueSkillSnapshot(match, player1, player2);
            TrueSkillUpdateHelper.ComputeAndApplyGlobalFromMatchSnapshots(match);
            TrueSkillUpdateHelper.ComputeAndApplySurfaceFromPlayers(match, player1, player2,
                useMsdForSmGsm: true, useGlobalMsdForSurfaceM: true);
            TrueSkillUpdateHelper.BackfillMissingSurfaceWinProbabilities(match, player1, player2,
                useMsdForSmGsm: true, useGlobalMsdForSurfaceM: true);
        }

        private static void NullOutPosteriorTrueSkillFields(Models.Match m)
        {
            var props = typeof(Models.Match)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                    (p.Name.StartsWith("Player1TrueSkillMean", StringComparison.Ordinal) ||
                     p.Name.StartsWith("Player2TrueSkillMean", StringComparison.Ordinal) ||
                     p.Name.StartsWith("Player1TrueSkillStandardDeviation", StringComparison.Ordinal) ||
                     p.Name.StartsWith("Player2TrueSkillStandardDeviation", StringComparison.Ordinal))
                    && !p.Name.Contains("Old", StringComparison.OrdinalIgnoreCase)
                    && p.PropertyType == typeof(double?));

            foreach (var p in props)
                p.SetValue(m, null);
        }

        // helper možeš staviti u istu klasu
        private async Task EnsurePlayerPersistedAsync(Player p, int tournamentTypeId)
        {
            if (p?.PlayerTPId is not int id) return;

            // 1) Ako je već u bazi -> pobrini se da je i u cacheu
            var existsInDb = await _playerRepository.ExistsAsync(id);
            if (existsInDb)
            {
                if (!_reference.Players.ContainsKey(id))
                {
                    // Ako imaš repo get, koristi njega; ako ne, zadrži "p" kakav je
                    // var dbPlayer = await _playerRepository.GetByTPIdAsync(id);
                    // if (dbPlayer != null) _reference.Players[id] = dbPlayer;
                    _reference.Players[id] = p;
                }
                return;
            }

            // 2) Nije u bazi -> popuni detalje i INSERT
            _tournamentHelper.ApplyDefaultPlayerValues(p);

            var details = await _playerDetailService.LoadOrDownloadAndParseAsync(id);

            p.PlayerName = details.PlayerName;
            p.CountryTPId ??= details.CountryTPId;  // važno zbog FK/NOT NULL
            p.PlayerBirthDate = details.PlayerBirthDate;
            p.PlayerHeight = details.PlayerHeight;
            p.PlayerWeight = details.PlayerWeight;
            p.PlayerTurnedPro = details.PlayerTurnedPro;
            p.PlaysId = details.PlaysId;
            p.TournamentTypeId = tournamentTypeId;

            // 3) DB pa cache (redoslijed bitan)
            await _playerRepository.InsertPlayerAsync(p);
            _reference.Players[id] = p;
        }

        private async Task AccumulateForDailyArchiveAsync(Models.Match match, Dictionary<string, object> tags)
        {
            if (match?.MatchTPId is null || match.DateTime is null) return;

            var key = DateOnly.FromDateTime(match.DateTime.Value.Date);

            _reference.Players.TryGetValue(match.Player1TPId ?? -1, out var p1);
            _reference.Players.TryGetValue(match.Player2TPId ?? -1, out var p2);

            var dtoDetails = await ToMongoDtoAsync(match, p1!, p2!); // detaljni per-match DTO

            if (dtoDetails == null) return;

            var dto = new MatchDTO
            {
                matchTPId = dtoDetails.matchTPId,
                dateTime = dtoDetails.dateTime,
                tournamentEventTPId = dtoDetails.tournamentEventTPId,
                tournamentEventCountryTPId = dtoDetails.tournamentEventCountryTPId,
                tournamentEventCountryISO2 = dtoDetails.tournamentEventCountryISO2,
                tournamentEventCountryISO3 = dtoDetails.tournamentEventCountryISO3,
                tournamentEventCountryFull = dtoDetails.tournamentEventCountryFull,
                prize = dtoDetails.prize,
                tournamentEventSurfaceId = dtoDetails.tournamentEventSurfaceId,
                tournamentEventSurfaceName = dtoDetails.tournamentEventSurfaceName,
                tournamentEventDate = dtoDetails.tournamentEventDate,
                tournamentEventName = dtoDetails.tournamentEventName,
                tournamentLevelId = dtoDetails.tournamentLevelId,
                tournamentLevelName = dtoDetails.tournamentLevelName,
                tournamentTypeId = dtoDetails.tournamentTypeId,
                tournamentTypeName = dtoDetails.tournamentTypeName,
                player1TPId = dtoDetails.player1TPId,
                player1Seed = dtoDetails.player1Seed,
                player1Name = dtoDetails.player1Name,
                player1CountryTPId = dtoDetails.player1CountryTPId,
                player1CountryISO2 = dtoDetails.player1CountryISO2,
                player1CountryISO3 = dtoDetails.player1CountryISO3,
                player1CountryFull = dtoDetails.player1CountryFull,
                player1PlaysId = dtoDetails.player1PlaysId,
                player1PlaysName = dtoDetails.player1PlaysName,
                player2TPId = dtoDetails.player2TPId,
                player2Seed = dtoDetails.player2Seed,
                player2Name = dtoDetails.player2Name,
                player2CountryTPId = dtoDetails.player2CountryTPId,
                player2CountryISO2 = dtoDetails.player2CountryISO2,
                player2CountryISO3 = dtoDetails.player2CountryISO3,
                player2CountryFull = dtoDetails.player2CountryFull,
                player2PlaysId = dtoDetails.player2PlaysId,
                player2PlaysName = dtoDetails.player2PlaysName,
                result = dtoDetails.result,
                resultDetails = dtoDetails.resultDetails,
                player1Odds = dtoDetails.player1Odds,
                player2Odds = dtoDetails.player2Odds,
                player1Percentage = dtoDetails.player1Percentage,
                player2Percentage = dtoDetails.player2Percentage,
                matchSurfaceId = dtoDetails.matchSurfaceId,
                matchSurfaceName = dtoDetails.matchSurfaceName,
                roundId = dtoDetails.roundId,
                roundName = dtoDetails.roundName,
                winProbabilityPlayer1NN = dtoDetails.winProbabilityPlayer1NN,
                winProbabilityPlayer2NN = dtoDetails.winProbabilityPlayer2NN,
                bestP1 = dtoDetails.bestP1,
                bestP2 = dtoDetails.bestP2,
                valueMarginPlayer1 = dtoDetails.valueMarginPlayer1,
                valueMarginPlayer2 = dtoDetails.valueMarginPlayer2,
                who2Bet = dtoDetails.who2Bet,
                isFinished = dtoDetails.isFinished
            };

            // --- QUOTES za detaljni DTO (opcionalno; dnevni dto obično ne treba punu povijest)
            var allRows = _reference.GetOddsForMatch(match.MatchTPId.Value).ToList();
            var quotes = allRows.Select(mo => new OddsQuoteDTO
            {
                OddsId = mo.OddsId ?? 0,
                MatchTPId = mo.MatchTPId ?? match.MatchTPId.Value,
                BookieId = mo.BookieId ?? 0,
                BookieName = (mo.BookieId.HasValue && _reference.BookiesById.TryGetValue(mo.BookieId.Value, out var bk))
                                    ? bk.BookieName : null,
                SourceFileTime = mo.SourceFileTime,
                CoalescedTime = mo.CoalescedTime ?? mo.DateTime ?? mo.SourceFileTime ?? DateTime.UtcNow,
                Player1Odds = mo.Player1Odds,
                Player2Odds = mo.Player2Odds
            }).Where(q => q.Player1Odds.HasValue && q.Player2Odds.HasValue)
              .ToList();

            if (quotes.Count > 0)
                dtoDetails.Odds = OddsProjectionService.Build(match.MatchTPId.Value, quotes, includeMergedSeries: true);

            // Zaokruži brojeve za oba DTO-a
            FrontendNumberShaper.Shape(dtoDetails, FeCfg);
            FrontendNumberShaper.Shape(dto, FeCfg);

            // --- Kumulacija u memoriji + zapis dnevnog .br ---
            var gate = GetDailyGate(key);
            await gate.WaitAsync();
            List<MatchDTO> snapshot;
            try
            {
                if (!_dailyByDate.TryGetValue(key, out var list))
                {
                    list = new List<MatchDTO>();
                    _dailyByDate[key] = list;
                }

                list.RemoveAll(x => x.matchTPId == dto.matchTPId);
                list.Add(dto);

                // snapshot radi thread-safe zapis bez držanja locka tijekom IO
                snapshot = list.OrderBy(x => x.dateTime).ToList();
            }
            finally
            {
                gate.Release();
            }

            await WriteDailyArchiveAsync(key, snapshot);

            // Per-match detaljni .br
            await matchDetailsBrotliArchivesAsync(match.MatchTPId.Value, dtoDetails);
        }

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        private async Task WriteDailyArchiveAsync(DateOnly key, List<MatchDTO> items, CancellationToken ct = default)
        {
            Directory.CreateDirectory(_dailyOutDir);
            var path = Path.Combine(_dailyOutDir, $"{key:yyyyMMdd}.br");

            // serialize once
            var json = JsonSerializer.Serialize(items, _jsonOpts);

            // brotli write (overwrite cijeli dan svaki put – najjednostavnije i robusno)
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var brotli = new System.IO.Compression.BrotliStream(fs, System.IO.Compression.CompressionLevel.Optimal, leaveOpen: false);
            using var sw = new StreamWriter(brotli, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            await sw.WriteAsync(json.AsMemory(), ct);
            await sw.FlushAsync();
        }

        private static readonly FrontendShapeConfig FeCfg = new()
        {
            PercentDecimals = 2, // 0.4999999 -> 50
            StatDecimals = 2, // mean/sd -> 2 dec
            OddsDecimals = 2, // kvote -> 2 dec
            DefaultDecimals = 2
        };

        private void InjectH2HPreMatchOnly(Models.Match match)
        {
            if (match.Player1TPId is null || match.Player2TPId is null)
                return;

            var prev = _matchRepository.GetLatestH2HMatchAsync(match.Player1TPId.Value, match.Player2TPId.Value);

            // Pre-match snapshot = post stanje iz prethodnog meča među ovim igračima
            int preP1 = prev?.Player1H2H ?? 0;
            int preP2 = prev?.Player2H2H ?? 0;

            match.Player1H2HOld = preP1;
            match.Player2H2HOld = preP2;

            // Za aktivni meč (bez rezultata) H2H == H2HOld
            match.Player1H2H = preP1;
            match.Player2H2H = preP2;
        }

        private static void MoveOddsFileToFinished(int matchTPId)
        {
            var srcDir = @"d:\brArchives\MatchOdds\Working";
            var src = Path.Combine(srcDir, $"{matchTPId}.br");
            if (!File.Exists(src)) return;

            var dstDir = @"d:\brArchives\MatchOdds\Finished";
            Directory.CreateDirectory(dstDir);
            var dst = Path.Combine(dstDir, Path.GetFileName(src));

            try { File.Move(src, dst, overwrite: true); }
            catch (Exception ex) { Console.WriteLine($"[Odds] Move fail for {src}: {ex.Message}"); }
        }

        private async Task<MatchDetailsDTO> ToMongoDtoAsync(Models.Match m, Player p1, Player p2)
        {
            // ---- dohvat primarnih entiteta (safe-null) ----
            var te = _reference.TournamentEvents.Values.ToList().FirstOrDefault(t => t.TournamentEventTPId == m.TournamentEventTPId);

            // --- NOVO: povuci sve kvote i agregiraj ---
            double? aggP1Odds = null, aggP2Odds = null;
            try
            {
                if (m.MatchTPId.HasValue)
                {
                    var allOdds = _reference.GetOddsForMatch(m.MatchTPId.Value); 
                    var agg = OddsAggregationHelper.AggregateByImpliedMedian(allOdds);

                    if (agg.BookiesUsed >= 2) // uvjet možeš pooštriti na >=3
                    {
                        aggP1Odds = agg.Player1Odds;
                        aggP2Odds = agg.Player2Odds;
                    }
                }
            }
            catch { /* best-effort, ne ruši parser */ }

            // Odaberi koje kvote idu u DTO: preferiraj agregirane, fallback na raw iz Match-a
            double? dtoP1Odds = aggP1Odds ?? m.Player1Odds;
            double? dtoP2Odds = aggP2Odds ?? m.Player2Odds;

            // NN proba
            double? p1NN = m.WinProbabilityNN;
            double? p2NN = p1NN.HasValue ? (1.0 - p1NN.Value) : (double?)null;

            // Implicitne vjerojatnosti iz DTO kvota
            double? impliedP1 = (dtoP1Odds.HasValue && dtoP1Odds > 0) ? 1.0 / dtoP1Odds : null;
            double? impliedP2 = (dtoP2Odds.HasValue && dtoP2Odds > 0) ? 1.0 / dtoP2Odds : null;

            double? vm1 = (p1NN.HasValue && impliedP1.HasValue) ? p1NN.Value - impliedP1.Value : null;
            double? vm2 = (p2NN.HasValue && impliedP2.HasValue) ? p2NN.Value - impliedP2.Value : null;

            int? who2Bet = null;

            if (vm1.HasValue || vm2.HasValue)
            {
                var best = new[]
                {
                    (Player: 1, Value: vm1 ?? double.NegativeInfinity),
                    (Player: 2, Value: vm2 ?? double.NegativeInfinity)
                }
                .OrderByDescending(x => x.Value)
                .First();

                if (best.Value > 0)
                    who2Bet = best.Player; // 1 ili 2
                else
                    who2Bet = 0; // nema edge-a
            }
            else
            {
                who2Bet = 0;
            }

            // Tournament Event country
            Country? teCountry = null;
            if (te?.CountryTPId is int teCId && _reference.Countries.TryGetValue(teCId, out var teC))
                teCountry = teC;
            else if (te?.CountryTPId is int teC2Id && _reference.Countries.TryGetValue(teC2Id, out var teC2))
                teCountry = teC2; // fallback ako je naziv polja drugačiji

            // Surface (uzmi s meča; ako nema, probaj s TE)
            int? teSurfaceId = te!.SurfaceId;
            int? mSurfaceId = m.SurfaceId;

            _reference.Surfaces.TryGetValue(teSurfaceId ?? -1, out var tournamentEventsurface);
            _reference.Surfaces.TryGetValue(mSurfaceId ?? -1, out var matchSurface);

            // Round
            _reference.Rounds.TryGetValue(m.RoundId ?? -1, out var round);

            // Level / Type (s TE)
            _reference.TournamentLevels.TryGetValue(te?.TournamentLevelId ?? -1, out var level);
            _reference.TournamentTypes.TryGetValue(te?.TournamentTypeId ?? -1, out var type);

            // Player countries
            Country? p1Country = null, p2Country = null;
            if (p1?.CountryTPId is int p1CId) _reference.Countries.TryGetValue(p1CId, out p1Country);
            if (p2?.CountryTPId is int p2CId) _reference.Countries.TryGetValue(p2CId, out p2Country);

            Plays? p1Plays = null, p2Plays = null;
            if (p1?.PlaysId is int p1PId) _reference.Plays.TryGetValue(p1PId, out p1Plays);
            if (p2?.PlaysId is int p2PId) _reference.Plays.TryGetValue(p2PId, out p2Plays);

            // ---- Prize (ako ga imaš na TE ili Match) ----
            string? prize = te?.Prize?.ToString() ?? "";

            if (m.IsFinished == false) return null;
            // ---- sastavi DTO ----
            var dto = new MatchDetailsDTO
            {
                matchTPId                                                                         = (int)m.MatchTPId!,
                dateTime                                                                          = m.DateTime,
                tournamentEventTPId                                                               = te!.TournamentEventTPId,
                tournamentEventCountryTPId                                                        = te.CountryTPId,
                tournamentEventCountryISO2                                                        = teCountry!.CountryISO2,
                tournamentEventCountryISO3                                                        = teCountry!.CountryISO3,
                tournamentEventCountryFull                                                        = teCountry!.CountryFull,
                prize                                                                             = te.Prize,
                tournamentEventSurfaceId                                                          = te.SurfaceId,
                tournamentEventSurfaceName                                                        = tournamentEventsurface!.SurfaceName,
                tournamentEventDate                                                               = te.TournamentEventDate,
                tournamentEventName                                                               = te.TournamentEventName,
                tournamentLevelId                                                                 = te.TournamentLevelId,
                tournamentLevelName                                                               = level!.TournamentLevelName,
                tournamentTypeId                                                                  = te.TournamentTypeId,
                tournamentTypeName                                                                = type!.TournamentTypeName,
                player1TPId                                                                       = m.Player1TPId,
                player1Seed                                                                       = m.Player1Seed,
                player1Name                                                                       = p1!.PlayerName,
                player1CountryTPId                                                                = p1.CountryTPId,
                player1CountryISO2                                                                = p1Country!.CountryISO2,
                player1CountryISO3                                                                = p1Country!.CountryISO3,
                player1CountryFull                                                                = p1Country!.CountryFull,
                player1PlaysId                                                                    = p1.PlaysId,
                player1PlaysName                                                                  = p1Plays != null ? p1Plays!.PlaysName : "",
                player2TPId                                                                       = m.Player2TPId,
                player2Seed                                                                       = m.Player1Seed,
                player2Name                                                                       = p2!.PlayerName,
                player2CountryTPId                                                                = p2.CountryTPId,
                player2CountryISO2                                                                = p2Country!.CountryISO2,
                player2CountryISO3                                                                = p2Country!.CountryISO3,
                player2CountryFull                                                                = p2Country!.CountryFull,
                player2PlaysId                                                                    = p2.PlaysId,
                player2PlaysName                                                                  = p2Plays != null ? p2Plays!.PlaysName : "",
                result                                                                            = m.Result,
                resultDetails                                                                     = m.ResultDetails!.Trim(),
                player1Odds                                                                       = m.Player1Odds,
                player2Odds                                                                       = m.Player2Odds,
                player1Percentage                                                                 = m.Player1Percentage,
                player2Percentage                                                                 = m.Player2Percentage,
                matchSurfaceId                                                                    = m.SurfaceId,
                matchSurfaceName                                                                  = matchSurface!.SurfaceName,
                roundId                                                                           = m.RoundId,
                roundName                                                                         = round != null ? round.RoundName : "",
                player1TrueSkillMeanM                                                             = m.Player1TrueSkillMeanM,
                player1TrueSkillStandardDeviationM                                                = m.Player1TrueSkillStandardDeviationM,
                player2TrueSkillMeanM                                                             = m.Player2TrueSkillMeanM                                                             ,
                player2TrueSkillStandardDeviationM                                                = m.Player2TrueSkillStandardDeviationM                                                ,
                player1TrueSkillMeanOldM                                                          = m.Player1TrueSkillMeanOldM                                                          ,
                player1TrueSkillStandardDeviationOldM                                             = m.Player1TrueSkillStandardDeviationOldM                                             ,
                player2TrueSkillMeanOldM                                                          = m.Player2TrueSkillMeanOldM                                                          ,
                player2TrueSkillStandardDeviationOldM                                             = m.Player2TrueSkillStandardDeviationOldM                                             ,
                winProbabilityPlayer1M                                                            = m.WinProbabilityPlayer1M                                                            ,
                winProbabilityPlayer2M                                                            = 1 - m.WinProbabilityPlayer1M                                                            ,
                player1TrueSkillMeanSM                                                            = m.Player1TrueSkillMeanSM                                                            ,
                player1TrueSkillStandardDeviationSM                                               = m.Player1TrueSkillStandardDeviationSM                                               ,
                player2TrueSkillMeanSM                                                            = m.Player2TrueSkillMeanSM                                                            ,
                player2TrueSkillStandardDeviationSM                                               = m.Player2TrueSkillStandardDeviationSM                                               ,
                player1TrueSkillMeanOldSM                                                         = m.Player1TrueSkillMeanOldSM                                                         ,
                player1TrueSkillStandardDeviationOldSM                                            = m.Player1TrueSkillStandardDeviationOldSM                                            ,
                player2TrueSkillMeanOldSM                                                         = m.Player2TrueSkillMeanOldSM                                                         ,
                player2TrueSkillStandardDeviationOldSM                                            = m.Player2TrueSkillStandardDeviationOldSM                                            ,
                winProbabilityPlayer1SM                                                           = m.WinProbabilityPlayer1SM                                                           ,
                winProbabilityPlayer2SM                                                           = 1 - m.WinProbabilityPlayer1SM                                                           ,
                player1TrueSkillMeanGSM                                                           = m.Player1TrueSkillMeanGSM                                                           ,
                player1TrueSkillStandardDeviationGSM                                              = m.Player1TrueSkillStandardDeviationGSM                                              ,
                player2TrueSkillMeanGSM                                                           = m.Player2TrueSkillMeanGSM                                                           ,
                player2TrueSkillStandardDeviationGSM                                              = m.Player2TrueSkillStandardDeviationGSM                                              ,
                player1TrueSkillMeanOldGSM                                                        = m.Player1TrueSkillMeanOldGSM                                                        ,
                player1TrueSkillStandardDeviationOldGSM                                           = m.Player1TrueSkillStandardDeviationOldGSM                                           ,
                player2TrueSkillMeanOldGSM                                                        = m.Player2TrueSkillMeanOldGSM                                                        ,
                player2TrueSkillStandardDeviationOldGSM                                           = m.Player2TrueSkillStandardDeviationOldGSM                                           ,
                winProbabilityPlayer1GSM                                                          = m.WinProbabilityPlayer1GSM                                                          ,
                winProbabilityPlayer2GSM                                                          = 1 - m.WinProbabilityPlayer1GSM                                                          ,
                player1TrueSkillMeanMS1                                                           = m.Player1TrueSkillMeanMS1                                                           ,
                player1TrueSkillStandardDeviationMS1                                              = m.Player1TrueSkillStandardDeviationMS1                                              ,
                player2TrueSkillMeanMS1                                                           = m.Player2TrueSkillMeanMS1                                                           ,
                player2TrueSkillStandardDeviationMS1                                              = m.Player2TrueSkillStandardDeviationMS1                                              ,
                player1TrueSkillMeanOldMS1                                                        = m.Player1TrueSkillMeanOldMS1                                                        ,
                player1TrueSkillStandardDeviationOldMS1                                           = m.Player1TrueSkillStandardDeviationOldMS1                                           ,
                player2TrueSkillMeanOldMS1                                                        = m.Player2TrueSkillMeanOldMS1                                                        ,
                player2TrueSkillStandardDeviationOldMS1                                           = m.Player2TrueSkillStandardDeviationOldMS1                                           ,
                winProbabilityPlayer1MS1                                                          = m.WinProbabilityPlayer1MS1                                                          ,
                winProbabilityPlayer2MS1                                                          = 1 - m.WinProbabilityPlayer1MS1                                                          ,
                player1TrueSkillMeanSMS1                                                          = m.Player1TrueSkillMeanSMS1                                                          ,
                player1TrueSkillStandardDeviationSMS1                                             = m.Player1TrueSkillStandardDeviationSMS1                                             ,
                player2TrueSkillMeanSMS1                                                          = m.Player2TrueSkillMeanSMS1                                                          ,
                player2TrueSkillStandardDeviationSMS1                                             = m.Player2TrueSkillStandardDeviationSMS1                                             ,
                player1TrueSkillMeanOldSMS1                                                       = m.Player1TrueSkillMeanOldSMS1                                                       ,
                player1TrueSkillStandardDeviationOldSMS1                                          = m.Player1TrueSkillStandardDeviationOldSMS1                                          ,
                player2TrueSkillMeanOldSMS1                                                       = m.Player2TrueSkillMeanOldSMS1                                                       ,
                player2TrueSkillStandardDeviationOldSMS1                                          = m.Player2TrueSkillStandardDeviationOldSMS1                                          ,
                winProbabilityPlayer1SMS1                                                         = m.WinProbabilityPlayer1SMS1                                                         ,
                winProbabilityPlayer2SMS1                                                         = 1 - m.WinProbabilityPlayer1SMS1                                                         ,
                player1TrueSkillMeanGSMS1                                                         = m.Player1TrueSkillMeanGSMS1                                                         ,
                player1TrueSkillStandardDeviationGSMS1                                            = m.Player1TrueSkillStandardDeviationGSMS1                                            ,
                player2TrueSkillMeanGSMS1                                                         = m.Player2TrueSkillMeanGSMS1                                                         ,
                player2TrueSkillStandardDeviationGSMS1                                            = m.Player2TrueSkillStandardDeviationGSMS1                                            ,
                player1TrueSkillMeanOldGSMS1                                                      = m.Player1TrueSkillMeanOldGSMS1                                                      ,
                player1TrueSkillStandardDeviationOldGSMS1                                         = m.Player1TrueSkillStandardDeviationOldGSMS1                                         ,
                player2TrueSkillMeanOldGSMS1                                                      = m.Player2TrueSkillMeanOldGSMS1                                                      ,
                player2TrueSkillStandardDeviationOldGSMS1                                         = m.Player2TrueSkillStandardDeviationOldGSMS1                                         ,
                winProbabilityPlayer1GSMS1                                                        = m.WinProbabilityPlayer1GSMS1                                                        ,
                winProbabilityPlayer2GSMS1                                                        = 1 - m.WinProbabilityPlayer1GSMS1                                                        ,
                player1TrueSkillMeanMS2                                                           = m.Player1TrueSkillMeanMS2                                                           ,
                player1TrueSkillStandardDeviationMS2                                              = m.Player1TrueSkillStandardDeviationMS2                                              ,
                player2TrueSkillMeanMS2                                                           = m.Player2TrueSkillMeanMS2                                                           ,
                player2TrueSkillStandardDeviationMS2                                              = m.Player2TrueSkillStandardDeviationMS2                                              ,
                player1TrueSkillMeanOldMS2                                                        = m.Player1TrueSkillMeanOldMS2                                                        ,
                player1TrueSkillStandardDeviationOldMS2                                           = m.Player1TrueSkillStandardDeviationOldMS2                                           ,
                player2TrueSkillMeanOldMS2                                                        = m.Player2TrueSkillMeanOldMS2                                                        ,
                player2TrueSkillStandardDeviationOldMS2                                           = m.Player2TrueSkillStandardDeviationOldMS2                                           ,
                winProbabilityPlayer1MS2                                                          = m.WinProbabilityPlayer1MS2                                                          ,
                winProbabilityPlayer2MS2                                                          = 1 - m.WinProbabilityPlayer1MS2                                                          ,
                player1TrueSkillMeanSMS2                                                          = m.Player1TrueSkillMeanSMS2                                                          ,
                player1TrueSkillStandardDeviationSMS2                                             = m.Player1TrueSkillStandardDeviationSMS2                                             ,
                player2TrueSkillMeanSMS2                                                          = m.Player2TrueSkillMeanSMS2                                                          ,
                player2TrueSkillStandardDeviationSMS2                                             = m.Player2TrueSkillStandardDeviationSMS2                                             ,
                player1TrueSkillMeanOldSMS2                                                       = m.Player1TrueSkillMeanOldSMS2                                                       ,
                player1TrueSkillStandardDeviationOldSMS2                                          = m.Player1TrueSkillStandardDeviationOldSMS2                                          ,
                player2TrueSkillMeanOldSMS2                                                       = m.Player2TrueSkillMeanOldSMS2                                                       ,
                player2TrueSkillStandardDeviationOldSMS2                                          = m.Player2TrueSkillStandardDeviationOldSMS2                                          ,
                winProbabilityPlayer1SMS2                                                         = m.WinProbabilityPlayer1SMS2                                                         ,
                winProbabilityPlayer2SMS2                                                         = 1 - m.WinProbabilityPlayer1SMS2                                                         ,
                player1TrueSkillMeanGSMS2                                                         = m.Player1TrueSkillMeanGSMS2                                                         ,
                player1TrueSkillStandardDeviationGSMS2                                            = m.Player1TrueSkillStandardDeviationGSMS2                                            ,
                player2TrueSkillMeanGSMS2                                                         = m.Player2TrueSkillMeanGSMS2                                                         ,
                player2TrueSkillStandardDeviationGSMS2                                            = m.Player2TrueSkillStandardDeviationGSMS2                                            ,
                player1TrueSkillMeanOldGSMS2                                                      = m.Player1TrueSkillMeanOldGSMS2                                                      ,
                player1TrueSkillStandardDeviationOldGSMS2                                         = m.Player1TrueSkillStandardDeviationOldGSMS2                                         ,
                player2TrueSkillMeanOldGSMS2                                                      = m.Player2TrueSkillMeanOldGSMS2                                                      ,
                player2TrueSkillStandardDeviationOldGSMS2                                         = m.Player2TrueSkillStandardDeviationOldGSMS2                                         ,
                winProbabilityPlayer1GSMS2                                                        = m.WinProbabilityPlayer1GSMS2                                                        ,
                winProbabilityPlayer2GSMS2                                                        = 1 - m.WinProbabilityPlayer1GSMS2                                                        ,
                player1TrueSkillMeanMS3                                                           = m.Player1TrueSkillMeanMS3                                                           ,
                player1TrueSkillStandardDeviationMS3                                              = m.Player1TrueSkillStandardDeviationMS3                                              ,
                player2TrueSkillMeanMS3                                                           = m.Player2TrueSkillMeanMS3                                                           ,
                player2TrueSkillStandardDeviationMS3                                              = m.Player2TrueSkillStandardDeviationMS3                                              ,
                player1TrueSkillMeanOldMS3                                                        = m.Player1TrueSkillMeanOldMS3                                                        ,
                player1TrueSkillStandardDeviationOldMS3                                           = m.Player1TrueSkillStandardDeviationOldMS3                                           ,
                player2TrueSkillMeanOldMS3                                                        = m.Player2TrueSkillMeanOldMS3                                                        ,
                player2TrueSkillStandardDeviationOldMS3                                           = m.Player2TrueSkillStandardDeviationOldMS3                                           ,
                winProbabilityPlayer1MS3                                                          = m.WinProbabilityPlayer1MS3                                                          ,
                winProbabilityPlayer2MS3                                                          = 1 - m.WinProbabilityPlayer1MS3                                                          ,
                player1TrueSkillMeanSMS3                                                          = m.Player1TrueSkillMeanSMS3                                                          ,
                player1TrueSkillStandardDeviationSMS3                                             = m.Player1TrueSkillStandardDeviationSMS3                                             ,
                player2TrueSkillMeanSMS3                                                          = m.Player2TrueSkillMeanSMS3                                                          ,
                player2TrueSkillStandardDeviationSMS3                                             = m.Player2TrueSkillStandardDeviationSMS3                                             ,
                player1TrueSkillMeanOldSMS3                                                       = m.Player1TrueSkillMeanOldSMS3                                                       ,
                player1TrueSkillStandardDeviationOldSMS3                                          = m.Player1TrueSkillStandardDeviationOldSMS3                                          ,
                player2TrueSkillMeanOldSMS3                                                       = m.Player2TrueSkillMeanOldSMS3                                                       ,
                player2TrueSkillStandardDeviationOldSMS3                                          = m.Player2TrueSkillStandardDeviationOldSMS3                                          ,
                winProbabilityPlayer1SMS3                                                         = m.WinProbabilityPlayer1SMS3                                                         ,
                winProbabilityPlayer2SMS3                                                         = 1 - m.WinProbabilityPlayer1SMS3                                                         ,
                player1TrueSkillMeanGSMS3                                                         = m.Player1TrueSkillMeanGSMS3                                                         ,
                player1TrueSkillStandardDeviationGSMS3                                            = m.Player1TrueSkillStandardDeviationGSMS3                                            ,
                player2TrueSkillMeanGSMS3                                                         = m.Player2TrueSkillMeanGSMS3                                                         ,
                player2TrueSkillStandardDeviationGSMS3                                            = m.Player2TrueSkillStandardDeviationGSMS3                                            ,
                player1TrueSkillMeanOldGSMS3                                                      = m.Player1TrueSkillMeanOldGSMS3                                                      ,
                player1TrueSkillStandardDeviationOldGSMS3                                         = m.Player1TrueSkillStandardDeviationOldGSMS3                                         ,
                player2TrueSkillMeanOldGSMS3                                                      = m.Player2TrueSkillMeanOldGSMS3                                                      ,
                player2TrueSkillStandardDeviationOldGSMS3                                         = m.Player2TrueSkillStandardDeviationOldGSMS3                                         ,
                winProbabilityPlayer1GSMS3                                                        = m.WinProbabilityPlayer1GSMS3                                                        ,
                winProbabilityPlayer2GSMS3                                                        = 1 - m.WinProbabilityPlayer1GSMS3                                                        ,
                player1TrueSkillMeanMS4                                                           = m.Player1TrueSkillMeanMS4                                                           ,
                player1TrueSkillStandardDeviationMS4                                              = m.Player1TrueSkillStandardDeviationMS4                                              ,
                player2TrueSkillMeanMS4                                                           = m.Player2TrueSkillMeanMS4                                                           ,
                player2TrueSkillStandardDeviationMS4                                              = m.Player2TrueSkillStandardDeviationMS4                                              ,
                player1TrueSkillMeanOldMS4                                                        = m.Player1TrueSkillMeanOldMS4                                                        ,
                player1TrueSkillStandardDeviationOldMS4                                           = m.Player1TrueSkillStandardDeviationOldMS4                                           ,
                player2TrueSkillMeanOldMS4                                                        = m.Player2TrueSkillMeanOldMS4                                                        ,
                player2TrueSkillStandardDeviationOldMS4                                           = m.Player2TrueSkillStandardDeviationOldMS4                                           ,
                winProbabilityPlayer1MS4                                                          = m.WinProbabilityPlayer1MS4                                                          ,
                winProbabilityPlayer2MS4                                                          = 1 - m.WinProbabilityPlayer1MS4                                                          ,
                player1TrueSkillMeanSMS4                                                          = m.Player1TrueSkillMeanSMS4                                                          ,
                player1TrueSkillStandardDeviationSMS4                                             = m.Player1TrueSkillStandardDeviationSMS4                                             ,
                player2TrueSkillMeanSMS4                                                          = m.Player2TrueSkillMeanSMS4                                                          ,
                player2TrueSkillStandardDeviationSMS4                                             = m.Player2TrueSkillStandardDeviationSMS4                                             ,
                player1TrueSkillMeanOldSMS4                                                       = m.Player1TrueSkillMeanOldSMS4                                                       ,
                player1TrueSkillStandardDeviationOldSMS4                                          = m.Player1TrueSkillStandardDeviationOldSMS4                                          ,
                player2TrueSkillMeanOldSMS4                                                       = m.Player2TrueSkillMeanOldSMS4                                                       ,
                player2TrueSkillStandardDeviationOldSMS4                                          = m.Player2TrueSkillStandardDeviationOldSMS4                                          ,
                winProbabilityPlayer1SMS4                                                         = m.WinProbabilityPlayer1SMS4                                                         ,
                winProbabilityPlayer2SMS4                                                         = 1 - m.WinProbabilityPlayer1SMS4                                                         ,
                player1TrueSkillMeanGSMS4                                                         = m.Player1TrueSkillMeanGSMS4                                                         ,
                player1TrueSkillStandardDeviationGSMS4                                            = m.Player1TrueSkillStandardDeviationGSMS4                                            ,
                player2TrueSkillMeanGSMS4                                                         = m.Player2TrueSkillMeanGSMS4                                                         ,
                player2TrueSkillStandardDeviationGSMS4                                            = m.Player2TrueSkillStandardDeviationGSMS4                                            ,
                player1TrueSkillMeanOldGSMS4                                                      = m.Player1TrueSkillMeanOldGSMS4                                                      ,
                player1TrueSkillStandardDeviationOldGSMS4                                         = m.Player1TrueSkillStandardDeviationOldGSMS4                                         ,
                player2TrueSkillMeanOldGSMS4                                                      = m.Player2TrueSkillMeanOldGSMS4                                                      ,
                player2TrueSkillStandardDeviationOldGSMS4                                         = m.Player2TrueSkillStandardDeviationOldGSMS4                                         ,
                winProbabilityPlayer1GSMS4                                                        = m.WinProbabilityPlayer1GSMS4                                                        ,
                winProbabilityPlayer2GSMS4                                                        = 1 - m.WinProbabilityPlayer1GSMS4                                                        ,
                player1WinsTotal                                                                  = m.Player1WinsTotal                                                                  ,
                player1LossesTotal                                                                = m.Player1LossesTotal                                                                ,
                player1WinsLastYear                                                               = m.Player1WinsLastYear                                                               ,
                player1LossesLastYear                                                             = m.Player1LossesLastYear                                                             ,
                player1WinsLastMonth                                                              = m.Player1WinsLastMonth                                                              ,
                player1LossesLastMonth                                                            = m.Player1LossesLastMonth                                                            ,
                player1WinsLastWeek                                                               = m.Player1WinsLastWeek                                                               ,
                player1LossesLastWeek                                                             = m.Player1LossesLastWeek                                                             ,
                player2WinsTotal                                                                  = m.Player2WinsTotal                                                                  ,
                player2LossesTotal                                                                = m.Player2LossesTotal                                                                ,
                player2WinsLastYear                                                               = m.Player2WinsLastYear                                                               ,
                player2LossesLastYear                                                             = m.Player2LossesLastYear                                                             ,
                player2WinsLastMonth                                                              = m.Player2WinsLastMonth                                                              ,
                player2LossesLastMonth                                                            = m.Player2LossesLastMonth                                                            ,
                player2WinsLastWeek                                                               = m.Player2WinsLastWeek                                                               ,
                player2LossesLastWeek                                                             = m.Player2LossesLastWeek                                                             ,
                player1WinsTotalS1                                                                = m.Player1WinsTotalS1                                                                ,
                player1LossesTotalS1                                                              = m.Player1LossesTotalS1                                                              ,
                player1WinsLastYearS1                                                             = m.Player1WinsLastYearS1                                                             ,
                player1LossesLastYearS1                                                           = m.Player1LossesLastYearS1                                                           ,
                player1WinsLastMonthS1                                                            = m.Player1WinsLastMonthS1                                                            ,
                player1LossesLastMonthS1                                                          = m.Player1LossesLastMonthS1                                                          ,
                player1WinsLastWeekS1                                                             = m.Player1WinsLastWeekS1                                                             ,
                player1LossesLastWeekS1                                                           = m.Player1LossesLastWeekS1                                                           ,
                player2WinsTotalS1                                                                = m.Player2WinsTotalS1                                                                ,
                player2LossesTotalS1                                                              = m.Player2LossesTotalS1                                                              ,
                player2WinsLastYearS1                                                             = m.Player2WinsLastYearS1                                                             ,
                player2LossesLastYearS1                                                           = m.Player2LossesLastYearS1                                                           ,
                player2WinsLastMonthS1                                                            = m.Player2WinsLastMonthS1                                                            ,
                player2LossesLastMonthS1                                                          = m.Player2LossesLastMonthS1                                                          ,
                player2WinsLastWeekS1                                                             = m.Player2WinsLastWeekS1                                                             ,
                player2LossesLastWeekS1                                                           = m.Player2LossesLastWeekS1                                                           ,
                player1WinsTotalS2                                                                = m.Player1WinsTotalS2                                                                ,
                player1LossesTotalS2                                                              = m.Player1LossesTotalS2                                                              ,
                player1WinsLastYearS2                                                             = m.Player1WinsLastYearS2                                                             ,
                player1LossesLastYearS2                                                           = m.Player1LossesLastYearS2                                                           ,
                player1WinsLastMonthS2                                                            = m.Player1WinsLastMonthS2                                                            ,
                player1LossesLastMonthS2                                                          = m.Player1LossesLastMonthS2                                                          ,
                player1WinsLastWeekS2                                                             = m.Player1WinsLastWeekS2                                                             ,
                player1LossesLastWeekS2                                                           = m.Player1LossesLastWeekS2                                                           ,
                player2WinsTotalS2                                                                = m.Player2WinsTotalS2                                                                ,
                player2LossesTotalS2                                                              = m.Player2LossesTotalS2                                                              ,
                player2WinsLastYearS2                                                             = m.Player2WinsLastYearS2                                                             ,
                player2LossesLastYearS2                                                           = m.Player2LossesLastYearS2                                                           ,
                player2WinsLastMonthS2                                                            = m.Player2WinsLastMonthS2                                                            ,
                player2LossesLastMonthS2                                                          = m.Player2LossesLastMonthS2                                                          ,
                player2WinsLastWeekS2                                                             = m.Player2WinsLastWeekS2                                                             ,
                player2LossesLastWeekS2                                                           = m.Player2LossesLastWeekS2                                                           ,
                player1WinsTotalS3                                                                = m.Player1WinsTotalS3                                                                ,
                player1LossesTotalS3                                                              = m.Player1LossesTotalS3                                                              ,
                player1WinsLastYearS3                                                             = m.Player1WinsLastYearS3                                                             ,
                player1LossesLastYearS3                                                           = m.Player1LossesLastYearS3                                                           ,
                player1WinsLastMonthS3                                                            = m.Player1WinsLastMonthS3                                                            ,
                player1LossesLastMonthS3                                                          = m.Player1LossesLastMonthS3                                                          ,
                player1WinsLastWeekS3                                                             = m.Player1WinsLastWeekS3                                                             ,
                player1LossesLastWeekS3                                                           = m.Player1LossesLastWeekS3                                                           ,
                player2WinsTotalS3                                                                = m.Player2WinsTotalS3                                                                ,
                player2LossesTotalS3                                                              = m.Player2LossesTotalS3                                                              ,
                player2WinsLastYearS3                                                             = m.Player2WinsLastYearS3                                                             ,
                player2LossesLastYearS3                                                           = m.Player2LossesLastYearS3                                                           ,
                player2WinsLastMonthS3                                                            = m.Player2WinsLastMonthS3                                                            ,
                player2LossesLastMonthS3                                                          = m.Player2LossesLastMonthS3                                                          ,
                player2WinsLastWeekS3                                                             = m.Player2WinsLastWeekS3                                                             ,
                player2LossesLastWeekS3                                                           = m.Player2LossesLastWeekS3                                                           ,
                player1WinsTotalS4                                                                = m.Player1WinsTotalS4                                                                ,
                player1LossesTotalS4                                                              = m.Player1LossesTotalS4                                                              ,
                player1WinsLastYearS4                                                             = m.Player1WinsLastYearS4                                                             ,
                player1LossesLastYearS4                                                           = m.Player1LossesLastYearS4                                                           ,
                player1WinsLastMonthS4                                                            = m.Player1WinsLastMonthS4                                                            ,
                player1LossesLastMonthS4                                                          = m.Player1LossesLastMonthS4                                                          ,
                player1WinsLastWeekS4                                                             = m.Player1WinsLastWeekS4                                                             ,
                player1LossesLastWeekS4                                                           = m.Player1LossesLastWeekS4                                                           ,
                player2WinsTotalS4                                                                = m.Player2WinsTotalS4                                                                ,
                player2LossesTotalS4                                                              = m.Player2LossesTotalS4                                                              ,
                player2WinsLastYearS4                                                             = m.Player2WinsLastYearS4                                                             ,
                player2LossesLastYearS4                                                           = m.Player2LossesLastYearS4                                                           ,
                player2WinsLastMonthS4                                                            = m.Player2WinsLastMonthS4                                                            ,
                player2LossesLastMonthS4                                                          = m.Player2LossesLastMonthS4                                                          ,
                player2WinsLastWeekS4                                                             = m.Player2WinsLastWeekS4                                                             ,
                player2LossesLastWeekS4                                                           = m.Player2LossesLastWeekS4                                                           ,
                player1WinsSetsTotal                                                              = m.Player1WinsSetsTotal                                                              ,
                player1LossesSetsTotal                                                            = m.Player1LossesSetsTotal                                                            ,
                player1WinsSetsLastYear                                                           = m.Player1WinsSetsLastYear                                                           ,
                player1LossesSetsLastYear                                                         = m.Player1LossesSetsLastYear                                                         ,
                player1WinsSetsLastMonth                                                          = m.Player1WinsSetsLastMonth                                                          ,
                player1LossesSetsLastMonth                                                        = m.Player1LossesSetsLastMonth                                                        ,
                player1WinsSetsLastWeek                                                           = m.Player1WinsSetsLastWeek                                                           ,
                player1LossesSetsLastWeek                                                         = m.Player1LossesSetsLastWeek                                                         ,
                player2WinsSetsTotal                                                              = m.Player2WinsSetsTotal                                                              ,
                player2LossesSetsTotal                                                            = m.Player2LossesSetsTotal                                                            ,
                player2WinsSetsLastYear                                                           = m.Player2WinsSetsLastYear                                                           ,
                player2LossesSetsLastYear                                                         = m.Player2LossesSetsLastYear                                                         ,
                player2WinsSetsLastMonth                                                          = m.Player2WinsSetsLastMonth                                                          ,
                player2LossesSetsLastMonth                                                        = m.Player2LossesSetsLastMonth                                                        ,
                player2WinsSetsLastWeek                                                           = m.Player2WinsSetsLastWeek                                                           ,
                player2LossesSetsLastWeek                                                         = m.Player2LossesSetsLastWeek                                                         ,
                player1WinsSetsTotalS1                                                            = m.Player1WinsSetsTotalS1                                                            ,
                player1LossesSetsTotalS1                                                          = m.Player1LossesSetsTotalS1                                                          ,
                player1WinsSetsLastYearS1                                                         = m.Player1WinsSetsLastYearS1                                                         ,
                player1LossesSetsLastYearS1                                                       = m.Player1LossesSetsLastYearS1                                                       ,
                player1WinsSetsLastMonthS1                                                        = m.Player1WinsSetsLastMonthS1                                                        ,
                player1LossesSetsLastMonthS1                                                      = m.Player1LossesSetsLastMonthS1                                                      ,
                player1WinsSetsLastWeekS1                                                         = m.Player1WinsSetsLastWeekS1                                                         ,
                player1LossesSetsLastWeekS1                                                       = m.Player1LossesSetsLastWeekS1                                                       ,
                player2WinsSetsTotalS1                                                            = m.Player2WinsSetsTotalS1                                                            ,
                player2LossesSetsTotalS1                                                          = m.Player2LossesSetsTotalS1                                                          ,
                player2WinsSetsLastYearS1                                                         = m.Player2WinsSetsLastYearS1                                                         ,
                player2LossesSetsLastYearS1                                                       = m.Player2LossesSetsLastYearS1                                                       ,
                player2WinsSetsLastMonthS1                                                        = m.Player2WinsSetsLastMonthS1                                                        ,
                player2LossesSetsLastMonthS1                                                      = m.Player2LossesSetsLastMonthS1                                                      ,
                player2WinsSetsLastWeekS1                                                         = m.Player2WinsSetsLastWeekS1                                                         ,
                player2LossesSetsLastWeekS1                                                       = m.Player2LossesSetsLastWeekS1                                                       ,
                player1WinsSetsTotalS2                                                            = m.Player1WinsSetsTotalS2                                                            ,
                player1LossesSetsTotalS2                                                          = m.Player1LossesSetsTotalS2                                                          ,
                player1WinsSetsLastYearS2                                                         = m.Player1WinsSetsLastYearS2                                                         ,
                player1LossesSetsLastYearS2                                                       = m.Player1LossesSetsLastYearS2                                                       ,
                player1WinsSetsLastMonthS2                                                        = m.Player1WinsSetsLastMonthS2                                                        ,
                player1LossesSetsLastMonthS2                                                      = m.Player1LossesSetsLastMonthS2                                                      ,
                player1WinsSetsLastWeekS2                                                         = m.Player1WinsSetsLastWeekS2                                                         ,
                player1LossesSetsLastWeekS2                                                       = m.Player1LossesSetsLastWeekS2                                                       ,
                player2WinsSetsTotalS2                                                            = m.Player2WinsSetsTotalS2                                                            ,
                player2LossesSetsTotalS2                                                          = m.Player2LossesSetsTotalS2                                                          ,
                player2WinsSetsLastYearS2                                                         = m.Player2WinsSetsLastYearS2                                                         ,
                player2LossesSetsLastYearS2                                                       = m.Player2LossesSetsLastYearS2                                                       ,
                player2WinsSetsLastMonthS2                                                        = m.Player2WinsSetsLastMonthS2                                                        ,
                player2LossesSetsLastMonthS2                                                      = m.Player2LossesSetsLastMonthS2                                                      ,
                player2WinsSetsLastWeekS2                                                         = m.Player2WinsSetsLastWeekS2                                                         ,
                player2LossesSetsLastWeekS2                                                       = m.Player2LossesSetsLastWeekS2                                                       ,
                player1WinsSetsTotalS3                                                            = m.Player1WinsSetsTotalS3                                                            ,
                player1LossesSetsTotalS3                                                          = m.Player1LossesSetsTotalS3                                                          ,
                player1WinsSetsLastYearS3                                                         = m.Player1WinsSetsLastYearS3                                                         ,
                player1LossesSetsLastYearS3                                                       = m.Player1LossesSetsLastYearS3                                                       ,
                player1WinsSetsLastMonthS3                                                        = m.Player1WinsSetsLastMonthS3                                                        ,
                player1LossesSetsLastMonthS3                                                      = m.Player1LossesSetsLastMonthS3                                                      ,
                player1WinsSetsLastWeekS3                                                         = m.Player1WinsSetsLastWeekS3                                                         ,
                player1LossesSetsLastWeekS3                                                       = m.Player1LossesSetsLastWeekS3                                                       ,
                player2WinsSetsTotalS3                                                            = m.Player2WinsSetsTotalS3                                                            ,
                player2LossesSetsTotalS3                                                          = m.Player2LossesSetsTotalS3                                                          ,
                player2WinsSetsLastYearS3                                                         = m.Player2WinsSetsLastYearS3                                                         ,
                player2LossesSetsLastYearS3                                                       = m.Player2LossesSetsLastYearS3                                                       ,
                player2WinsSetsLastMonthS3                                                        = m.Player2WinsSetsLastMonthS3                                                        ,
                player2LossesSetsLastMonthS3                                                      = m.Player2LossesSetsLastMonthS3                                                      ,
                player2WinsSetsLastWeekS3                                                         = m.Player2WinsSetsLastWeekS3                                                         ,
                player2LossesSetsLastWeekS3                                                       = m.Player2LossesSetsLastWeekS3                                                       ,
                player1WinsSetsTotalS4                                                            = m.Player1WinsSetsTotalS4                                                            ,
                player1LossesSetsTotalS4                                                          = m.Player1LossesSetsTotalS4                                                          ,
                player1WinsSetsLastYearS4                                                         = m.Player1WinsSetsLastYearS4                                                         ,
                player1LossesSetsLastYearS4                                                       = m.Player1LossesSetsLastYearS4                                                       ,
                player1WinsSetsLastMonthS4                                                        = m.Player1WinsSetsLastMonthS4                                                        ,
                player1LossesSetsLastMonthS4                                                      = m.Player1LossesSetsLastMonthS4                                                      ,
                player1WinsSetsLastWeekS4                                                         = m.Player1WinsSetsLastWeekS4                                                         ,
                player1LossesSetsLastWeekS4                                                       = m.Player1LossesSetsLastWeekS4                                                       ,
                player2WinsSetsTotalS4                                                            = m.Player2WinsSetsTotalS4                                                            ,
                player2LossesSetsTotalS4                                                          = m.Player2LossesSetsTotalS4                                                          ,
                player2WinsSetsLastYearS4                                                         = m.Player2WinsSetsLastYearS4                                                         ,
                player2LossesSetsLastYearS4                                                       = m.Player2LossesSetsLastYearS4                                                       ,
                player2WinsSetsLastMonthS4                                                        = m.Player2WinsSetsLastMonthS4                                                        ,
                player2LossesSetsLastMonthS4                                                      = m.Player2LossesSetsLastMonthS4                                                      ,
                player2WinsSetsLastWeekS4                                                         = m.Player2WinsSetsLastWeekS4                                                         ,
                player2LossesSetsLastWeekS4                                                       = m.Player2LossesSetsLastWeekS4                                                       ,
                player1WinsGamesTotal                                                             = m.Player1WinsGamesTotal                                                             ,
                player1LossesGamesTotal                                                           = m.Player1LossesGamesTotal                                                           ,
                player1WinsGamesLastYear                                                          = m.Player1WinsGamesLastYear                                                          ,
                player1LossesGamesLastYear                                                        = m.Player1LossesGamesLastYear                                                        ,
                player1WinsGamesLastMonth                                                         = m.Player1WinsGamesLastMonth                                                         ,
                player1LossesGamesLastMonth                                                       = m.Player1LossesGamesLastMonth                                                       ,
                player1WinsGamesLastWeek                                                          = m.Player1WinsGamesLastWeek                                                          ,
                player1LossesGamesLastWeek                                                        = m.Player1LossesGamesLastWeek                                                        ,
                player2WinsGamesTotal                                                             = m.Player2WinsGamesTotal                                                             ,
                player2LossesGamesTotal                                                           = m.Player2LossesGamesTotal                                                           ,
                player2WinsGamesLastYear                                                          = m.Player2WinsGamesLastYear                                                          ,
                player2LossesGamesLastYear                                                        = m.Player2LossesGamesLastYear                                                        ,
                player2WinsGamesLastMonth                                                         = m.Player2WinsGamesLastMonth                                                         ,
                player2LossesGamesLastMonth                                                       = m.Player2LossesGamesLastMonth                                                       ,
                player2WinsGamesLastWeek                                                          = m.Player2WinsGamesLastWeek                                                          ,
                player2LossesGamesLastWeek                                                        = m.Player2LossesGamesLastWeek                                                        ,
                player1WinsGamesTotalS1                                                           = m.Player1WinsGamesTotalS1                                                           ,
                player1LossesGamesTotalS1                                                         = m.Player1LossesGamesTotalS1                                                         ,
                player1WinsGamesLastYearS1                                                        = m.Player1WinsGamesLastYearS1                                                        ,
                player1LossesGamesLastYearS1                                                      = m.Player1LossesGamesLastYearS1                                                      ,
                player1WinsGamesLastMonthS1                                                       = m.Player1WinsGamesLastMonthS1                                                       ,
                player1LossesGamesLastMonthS1                                                     = m.Player1LossesGamesLastMonthS1                                                     ,
                player1WinsGamesLastWeekS1                                                        = m.Player1WinsGamesLastWeekS1                                                        ,
                player1LossesGamesLastWeekS1                                                      = m.Player1LossesGamesLastWeekS1                                                      ,
                player2WinsGamesTotalS1                                                           = m.Player2WinsGamesTotalS1                                                           ,
                player2LossesGamesTotalS1                                                         = m.Player2LossesGamesTotalS1                                                         ,
                player2WinsGamesLastYearS1                                                        = m.Player2WinsGamesLastYearS1                                                        ,
                player2LossesGamesLastYearS1                                                      = m.Player2LossesGamesLastYearS1                                                      ,
                player2WinsGamesLastMonthS1                                                       = m.Player2WinsGamesLastMonthS1                                                       ,
                player2LossesGamesLastMonthS1                                                     = m.Player2LossesGamesLastMonthS1                                                     ,
                player2WinsGamesLastWeekS1                                                        = m.Player2WinsGamesLastWeekS1                                                        ,
                player2LossesGamesLastWeekS1                                                      = m.Player2LossesGamesLastWeekS1                                                      ,
                player1WinsGamesTotalS2                                                           = m.Player1WinsGamesTotalS2                                                           ,
                player1LossesGamesTotalS2                                                         = m.Player1LossesGamesTotalS2                                                         ,
                player1WinsGamesLastYearS2                                                        = m.Player1WinsGamesLastYearS2                                                        ,
                player1LossesGamesLastYearS2                                                      = m.Player1LossesGamesLastYearS2                                                      ,
                player1WinsGamesLastMonthS2                                                       = m.Player1WinsGamesLastMonthS2                                                       ,
                player1LossesGamesLastMonthS2                                                     = m.Player1LossesGamesLastMonthS2                                                     ,
                player1WinsGamesLastWeekS2                                                        = m.Player1WinsGamesLastWeekS2                                                        ,
                player1LossesGamesLastWeekS2                                                      = m.Player1LossesGamesLastWeekS2                                                      ,
                player2WinsGamesTotalS2                                                           = m.Player2WinsGamesTotalS2                                                           ,
                player2LossesGamesTotalS2                                                         = m.Player2LossesGamesTotalS2                                                         ,
                player2WinsGamesLastYearS2                                                        = m.Player2WinsGamesLastYearS2                                                        ,
                player2LossesGamesLastYearS2                                                      = m.Player2LossesGamesLastYearS2                                                      ,
                player2WinsGamesLastMonthS2                                                       = m.Player2WinsGamesLastMonthS2                                                       ,
                player2LossesGamesLastMonthS2                                                     = m.Player2LossesGamesLastMonthS2                                                     ,
                player2WinsGamesLastWeekS2                                                        = m.Player2WinsGamesLastWeekS2                                                        ,
                player2LossesGamesLastWeekS2                                                      = m.Player2LossesGamesLastWeekS2                                                      ,
                player1WinsGamesTotalS3                                                           = m.Player1WinsGamesTotalS3                                                           ,
                player1LossesGamesTotalS3                                                         = m.Player1LossesGamesTotalS3                                                         ,
                player1WinsGamesLastYearS3                                                        = m.Player1WinsGamesLastYearS3                                                        ,
                player1LossesGamesLastYearS3                                                      = m.Player1LossesGamesLastYearS3                                                      ,
                player1WinsGamesLastMonthS3                                                       = m.Player1WinsGamesLastMonthS3                                                       ,
                player1LossesGamesLastMonthS3                                                     = m.Player1LossesGamesLastMonthS3                                                     ,
                player1WinsGamesLastWeekS3                                                        = m.Player1WinsGamesLastWeekS3                                                        ,
                player1LossesGamesLastWeekS3                                                      = m.Player1LossesGamesLastWeekS3                                                      ,
                player2WinsGamesTotalS3                                                           = m.Player2WinsGamesTotalS3                                                           ,
                player2LossesGamesTotalS3                                                         = m.Player2LossesGamesTotalS3                                                         ,
                player2WinsGamesLastYearS3                                                        = m.Player2WinsGamesLastYearS3                                                        ,
                player2LossesGamesLastYearS3                                                      = m.Player2LossesGamesLastYearS3                                                      ,
                player2WinsGamesLastMonthS3                                                       = m.Player2WinsGamesLastMonthS3                                                       ,
                player2LossesGamesLastMonthS3                                                     = m.Player2LossesGamesLastMonthS3                                                     ,
                player2WinsGamesLastWeekS3                                                        = m.Player2WinsGamesLastWeekS3                                                        ,
                player2LossesGamesLastWeekS3                                                      = m.Player2LossesGamesLastWeekS3                                                      ,
                player1WinsGamesTotalS4                                                           = m.Player1WinsGamesTotalS4                                                           ,
                player1LossesGamesTotalS4                                                         = m.Player1LossesGamesTotalS4                                                         ,
                player1WinsGamesLastYearS4                                                        = m.Player1WinsGamesLastYearS4                                                        ,
                player1LossesGamesLastYearS4                                                      = m.Player1LossesGamesLastYearS4                                                      ,
                player1WinsGamesLastMonthS4                                                       = m.Player1WinsGamesLastMonthS4                                                       ,
                player1LossesGamesLastMonthS4                                                     = m.Player1LossesGamesLastMonthS4                                                     ,
                player1WinsGamesLastWeekS4                                                        = m.Player1WinsGamesLastWeekS4                                                        ,
                player1LossesGamesLastWeekS4                                                      = m.Player1LossesGamesLastWeekS4                                                      ,
                player2WinsGamesTotalS4                                                           = m.Player2WinsGamesTotalS4                                                           ,
                player2LossesGamesTotalS4                                                         = m.Player2LossesGamesTotalS4                                                         ,
                player2WinsGamesLastYearS4                                                        = m.Player2WinsGamesLastYearS4                                                        ,
                player2LossesGamesLastYearS4                                                      = m.Player2LossesGamesLastYearS4                                                      ,
                player2WinsGamesLastMonthS4                                                       = m.Player2WinsGamesLastMonthS4                                                       ,
                player2LossesGamesLastMonthS4                                                     = m.Player2LossesGamesLastMonthS4                                                     ,
                player2WinsGamesLastWeekS4                                                        = m.Player2WinsGamesLastWeekS4                                                        ,
                player2LossesGamesLastWeekS4                                                      = m.Player2LossesGamesLastWeekS4                                                      ,
                player1DaysSinceLastWin                                                           = m.Player1DaysSinceLastWin                                                           ,
                player2DaysSinceLastWin                                                           = m.Player2DaysSinceLastWin                                                           ,
                player1DaysSinceLastWinS1                                                         = m.Player1DaysSinceLastWinS1                                                         ,
                player2DaysSinceLastWinS1                                                         = m.Player2DaysSinceLastWinS1                                                         ,
                player1DaysSinceLastWinS2                                                         = m.Player1DaysSinceLastWinS2                                                         ,
                player2DaysSinceLastWinS2                                                         = m.Player2DaysSinceLastWinS2                                                         ,
                player1DaysSinceLastWinS3                                                         = m.Player1DaysSinceLastWinS3                                                         ,
                player2DaysSinceLastWinS3                                                         = m.Player2DaysSinceLastWinS3                                                         ,
                player1DaysSinceLastWinS4                                                         = m.Player1DaysSinceLastWinS4                                                         ,
                player2DaysSinceLastWinS4                                                         = m.Player2DaysSinceLastWinS4                                                         ,
                player1DaysSinceLastLoss                                                          = m.Player1DaysSinceLastLoss                                                          ,
                player2DaysSinceLastLoss                                                          = m.Player2DaysSinceLastLoss                                                          ,
                player1DaysSinceLastLossS1                                                        = m.Player1DaysSinceLastLossS1                                                        ,
                player2DaysSinceLastLossS1                                                        = m.Player2DaysSinceLastLossS1                                                        ,
                player1DaysSinceLastLossS2                                                        = m.Player1DaysSinceLastLossS2                                                        ,
                player2DaysSinceLastLossS2                                                        = m.Player2DaysSinceLastLossS2                                                        ,
                player1DaysSinceLastLossS3                                                        = m.Player1DaysSinceLastLossS3                                                        ,
                player2DaysSinceLastLossS3                                                        = m.Player2DaysSinceLastLossS3                                                        ,
                player1DaysSinceLastLossS4                                                        = m.Player1DaysSinceLastLossS4                                                        ,
                player2DaysSinceLastLossS4                                                        = m.Player2DaysSinceLastLossS4                                                        ,
                player1TotalWinsAsFavourite                                                       = m.Player1TotalWinsAsFavourite                                                       ,
                player2TotalWinsAsFavourite                                                       = m.Player2TotalWinsAsFavourite                                                       ,
                player1TotalWinsAsUnderdog                                                        = m.Player1TotalWinsAsUnderdog                                                        ,
                player2TotalWinsAsUnderdog                                                        = m.Player2TotalWinsAsUnderdog                                                        ,
                player1TotalLossesAsFavourite                                                     = m.Player1TotalLossesAsFavourite                                                     ,
                player2TotalLossesAsFavourite                                                     = m.Player2TotalLossesAsFavourite                                                     ,
                player1TotalLossesAsUnderdog                                                      = m.Player1TotalLossesAsUnderdog                                                      ,
                player2TotalLossesAsUnderdog                                                      = m.Player2TotalLossesAsUnderdog                                                      ,
                player1AverageWinningProbabilityAtWonAsFavourite                                  = m.Player1AverageWinningProbabilityAtWonAsFavourite                                  ,
                player2AverageWinningProbabilityAtWonAsFavourite                                  = m.Player2AverageWinningProbabilityAtWonAsFavourite                                  ,
                player1AverageWinningProbabilityAtWonAsUnderdog                                   = m.Player1AverageWinningProbabilityAtWonAsUnderdog                                   ,
                player2AverageWinningProbabilityAtWonAsUnderdog                                   = m.Player2AverageWinningProbabilityAtWonAsUnderdog                                   ,
                player1AverageWinningProbabilityAtLossAsFavourite                                 = m.Player1AverageWinningProbabilityAtLossAsFavourite                                 ,
                player2AverageWinningProbabilityAtLossAsFavourite                                 = m.Player2AverageWinningProbabilityAtLossAsFavourite                                 ,
                player1AverageWinningProbabilityAtLossAsUnderdog                                  = m.Player1AverageWinningProbabilityAtLossAsUnderdog                                  ,
                player2AverageWinningProbabilityAtLossAsUnderdog                                  = m.Player2AverageWinningProbabilityAtLossAsUnderdog                                  ,
                player1TotalWinsAsFavouriteLastYear                                               = m.Player1TotalWinsAsFavouriteLastYear                                               ,
                player2TotalWinsAsFavouriteLastYear                                               = m.Player2TotalWinsAsFavouriteLastYear                                               ,
                player1TotalWinsAsUnderdogLastYear                                                = m.Player1TotalWinsAsUnderdogLastYear                                                ,
                player2TotalWinsAsUnderdogLastYear                                                = m.Player2TotalWinsAsUnderdogLastYear                                                ,
                player1TotalLossesAsFavouriteLastYear                                             = m.Player1TotalLossesAsFavouriteLastYear                                             ,
                player2TotalLossesAsFavouriteLastYear                                             = m.Player2TotalLossesAsFavouriteLastYear                                             ,
                player1TotalLossesAsUnderdogLastYear                                              = m.Player1TotalLossesAsUnderdogLastYear                                              ,
                player2TotalLossesAsUnderdogLastYear                                              = m.Player2TotalLossesAsUnderdogLastYear                                              ,
                player1AverageWinningProbabilityAtWonAsFavouriteLastYear                          = m.Player1AverageWinningProbabilityAtWonAsFavouriteLastYear                          ,
                player2AverageWinningProbabilityAtWonAsFavouriteLastYear                          = m.Player2AverageWinningProbabilityAtWonAsFavouriteLastYear                          ,
                player1AverageWinningProbabilityAtWonAsUnderdogLastYear                           = m.Player1AverageWinningProbabilityAtWonAsUnderdogLastYear                           ,
                player2AverageWinningProbabilityAtWonAsUnderdogLastYear                           = m.Player2AverageWinningProbabilityAtWonAsUnderdogLastYear                           ,
                player1AverageWinningProbabilityAtLossAsFavouriteLastYear                         = m.Player1AverageWinningProbabilityAtLossAsFavouriteLastYear                         ,
                player2AverageWinningProbabilityAtLossAsFavouriteLastYear                         = m.Player2AverageWinningProbabilityAtLossAsFavouriteLastYear                         ,
                player1AverageWinningProbabilityAtLossAsUnderdogLastYear                          = m.Player1AverageWinningProbabilityAtLossAsUnderdogLastYear                          ,
                player2AverageWinningProbabilityAtLossAsUnderdogLastYear                          = m.Player2AverageWinningProbabilityAtLossAsUnderdogLastYear                          ,
                player1TotalWinsAsFavouriteLastMonth                                              = m.Player1TotalWinsAsFavouriteLastMonth                                              ,
                player2TotalWinsAsFavouriteLastMonth                                              = m.Player2TotalWinsAsFavouriteLastMonth                                              ,
                player1TotalWinsAsUnderdogLastMonth                                               = m.Player1TotalWinsAsUnderdogLastMonth                                               ,
                player2TotalWinsAsUnderdogLastMonth                                               = m.Player2TotalWinsAsUnderdogLastMonth                                               ,
                player1TotalLossesAsFavouriteLastMonth                                            = m.Player1TotalLossesAsFavouriteLastMonth                                            ,
                player2TotalLossesAsFavouriteLastMonth                                            = m.Player2TotalLossesAsFavouriteLastMonth                                            ,
                player1TotalLossesAsUnderdogLastMonth                                             = m.Player1TotalLossesAsUnderdogLastMonth                                             ,
                player2TotalLossesAsUnderdogLastMonth                                             = m.Player2TotalLossesAsUnderdogLastMonth                                             ,
                player1AverageWinningProbabilityAtWonAsFavouriteLastMonth                         = m.Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth                         ,
                player2AverageWinningProbabilityAtWonAsFavouriteLastMonth                         = m.Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth                         ,
                player1AverageWinningProbabilityAtWonAsUnderdogLastMonth                          = m.Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth                          ,
                player2AverageWinningProbabilityAtWonAsUnderdogLastMonth                          = m.Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth                          ,
                player1AverageWinningProbabilityAtLossAsFavouriteLastMonth                        = m.Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth                        ,
                player2AverageWinningProbabilityAtLossAsFavouriteLastMonth                        = m.Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth                        ,
                player1AverageWinningProbabilityAtLossAsUnderdogLastMonth                         = m.Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth                         ,
                player2AverageWinningProbabilityAtLossAsUnderdogLastMonth                         = m.Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth                         ,
                player1TotalWinsAsFavouriteLastWeek                                               = m.Player1TotalWinsAsFavouriteLastWeek                                               ,
                player2TotalWinsAsFavouriteLastWeek                                               = m.Player2TotalWinsAsFavouriteLastWeek                                               ,
                player1TotalWinsAsUnderdogLastWeek                                                = m.Player1TotalWinsAsUnderdogLastWeek                                                ,
                player2TotalWinsAsUnderdogLastWeek                                                = m.Player2TotalWinsAsUnderdogLastWeek                                                ,
                player1TotalLossesAsFavouriteLastWeek                                             = m.Player1TotalLossesAsFavouriteLastWeek                                             ,
                player2TotalLossesAsFavouriteLastWeek                                             = m.Player2TotalLossesAsFavouriteLastWeek                                             ,
                player1TotalLossesAsUnderdogLastWeek                                              = m.Player1TotalLossesAsUnderdogLastWeek                                              ,
                player2TotalLossesAsUnderdogLastWeek                                              = m.Player2TotalLossesAsUnderdogLastWeek                                              ,
                player1AverageWinningProbabilityAtWonAsFavouriteLastWeek                          = m.Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek                          ,
                player2AverageWinningProbabilityAtWonAsFavouriteLastWeek                          = m.Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek                          ,
                player1AverageWinningProbabilityAtWonAsUnderdogLastWeek                           = m.Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek                           ,
                player2AverageWinningProbabilityAtWonAsUnderdogLastWeek                           = m.Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek                           ,
                player1AverageWinningProbabilityAtLossAsFavouriteLastWeek                         = m.Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek                         ,
                player2AverageWinningProbabilityAtLossAsFavouriteLastWeek                         = m.Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek                         ,
                player1AverageWinningProbabilityAtLossAsUnderdogLastWeek                          = m.Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek                          ,
                player2AverageWinningProbabilityAtLossAsUnderdogLastWeek                          = m.Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek                          ,
                player1H2H                                                                        = m.Player1H2H                                                                        ,
                player2H2H                                                                        = m.Player2H2H                                                                        ,
                player1H2HOld                                                                     = m.Player1H2HOld                                                                     ,
                player2H2HOld                                                                     = m.Player2H2HOld                                                                     ,
                player1H2HTrueSkillMeanM                                                          = m.Player1H2HTrueSkillMeanM                                                          ,
                player1H2HTrueSkillStandardDeviationM                                             = m.Player1H2HTrueSkillStandardDeviationM                                             ,
                player2H2HTrueSkillMeanM                                                          = m.Player2H2HTrueSkillMeanM                                                          ,
                player2H2HTrueSkillStandardDeviationM                                             = m.Player2H2HTrueSkillStandardDeviationM                                             ,
                player1H2HTrueSkillMeanOldM                                                       = m.Player1H2HTrueSkillMeanOldM                                                       ,
                player1H2HTrueSkillStandardDeviationOldM                                          = m.Player1H2HTrueSkillStandardDeviationOldM                                          ,
                player2H2HTrueSkillMeanOldM                                                       = m.Player2H2HTrueSkillMeanOldM                                                       ,
                player2H2HTrueSkillStandardDeviationOldM                                          = m.Player2H2HTrueSkillStandardDeviationOldM                                          ,
                winProbabilityPlayer1H2HM                                                         = m.WinProbabilityPlayer1H2HM                                                         ,
                winProbabilityPlayer2H2HM                                                         = 1 - m.WinProbabilityPlayer1H2HM                                                         ,
                player1H2HTrueSkillMeanSM                                                         = m.Player1H2HTrueSkillMeanSM                                                         ,
                player1H2HTrueSkillStandardDeviationSM                                            = m.Player1H2HTrueSkillStandardDeviationSM                                            ,
                player2H2HTrueSkillMeanSM                                                         = m.Player2H2HTrueSkillMeanSM                                                         ,
                player2H2HTrueSkillStandardDeviationSM                                            = m.Player2H2HTrueSkillStandardDeviationSM                                            ,
                player1H2HTrueSkillMeanOldSM                                                      = m.Player1H2HTrueSkillMeanOldSM                                                      ,
                player1H2HTrueSkillStandardDeviationOldSM                                         = m.Player1H2HTrueSkillStandardDeviationOldSM                                         ,
                player2H2HTrueSkillMeanOldSM                                                      = m.Player2H2HTrueSkillMeanOldSM                                                      ,
                player2H2HTrueSkillStandardDeviationOldSM                                         = m.Player2H2HTrueSkillStandardDeviationOldSM                                         ,
                winProbabilityPlayer1H2HSM                                                        = m.WinProbabilityPlayer1H2HSM                                                        ,
                winProbabilityPlayer2H2HSM                                                        = 1 - m.WinProbabilityPlayer1H2HSM                                                        ,
                player1H2HTrueSkillMeanGSM                                                        = m.Player1H2HTrueSkillMeanGSM                                                        ,
                player1H2HTrueSkillStandardDeviationGSM                                           = m.Player1H2HTrueSkillStandardDeviationGSM                                           ,
                player2H2HTrueSkillMeanGSM                                                        = m.Player2H2HTrueSkillMeanGSM                                                        ,
                player2H2HTrueSkillStandardDeviationGSM                                           = m.Player2H2HTrueSkillStandardDeviationGSM                                           ,
                player1H2HTrueSkillMeanOldGSM                                                     = m.Player1H2HTrueSkillMeanOldGSM                                                     ,
                player1H2HTrueSkillStandardDeviationOldGSM                                        = m.Player1H2HTrueSkillStandardDeviationOldGSM                                        ,
                player2H2HTrueSkillMeanOldGSM                                                     = m.Player2H2HTrueSkillMeanOldGSM                                                     ,
                player2H2HTrueSkillStandardDeviationOldGSM                                        = m.Player2H2HTrueSkillStandardDeviationOldGSM                                        ,
                winProbabilityPlayer1H2HGSM                                                       = m.WinProbabilityPlayer1H2HGSM                                                       ,
                winProbabilityPlayer2H2HGSM                                                       = 1 - m.WinProbabilityPlayer1H2HGSM                                                       ,
                player1H2HS1                                                                      = m.Player1H2HS1                                                                      ,
                player2H2HS1                                                                      = m.Player2H2HS1                                                                      ,
                player1H2HOldS1                                                                   = m.Player1H2HOldS1                                                                   ,
                player2H2HOldS1                                                                   = m.Player2H2HOldS1                                                                   ,
                player1H2HTrueSkillMeanMS1                                                        = m.Player1H2HTrueSkillMeanMS1                                                        ,
                player1H2HTrueSkillStandardDeviationMS1                                           = m.Player1H2HTrueSkillStandardDeviationMS1                                           ,
                player2H2HTrueSkillMeanMS1                                                        = m.Player2H2HTrueSkillMeanMS1                                                        ,
                player2H2HTrueSkillStandardDeviationMS1                                           = m.Player2H2HTrueSkillStandardDeviationMS1                                           ,
                player1H2HTrueSkillMeanOldMS1                                                     = m.Player1H2HTrueSkillMeanOldMS1                                                     ,
                player1H2HTrueSkillStandardDeviationOldMS1                                        = m.Player1H2HTrueSkillStandardDeviationOldMS1                                        ,
                player2H2HTrueSkillMeanOldMS1                                                     = m.Player2H2HTrueSkillMeanOldMS1                                                     ,
                player2H2HTrueSkillStandardDeviationOldMS1                                        = m.Player2H2HTrueSkillStandardDeviationOldMS1                                        ,
                winProbabilityPlayer1H2HMS1                                                       = m.WinProbabilityPlayer1H2HMS1                                                       ,
                winProbabilityPlayer2H2HMS1                                                       = 1 - m.WinProbabilityPlayer1H2HMS1                                                       ,
                player1H2HTrueSkillMeanSMS1                                                       = m.Player1H2HTrueSkillMeanSMS1                                                       ,
                player1H2HTrueSkillStandardDeviationSMS1                                          = m.Player1H2HTrueSkillStandardDeviationSMS1                                          ,
                player2H2HTrueSkillMeanSMS1                                                       = m.Player2H2HTrueSkillMeanSMS1                                                       ,
                player2H2HTrueSkillStandardDeviationSMS1                                          = m.Player2H2HTrueSkillStandardDeviationSMS1                                          ,
                player1H2HTrueSkillMeanOldSMS1                                                    = m.Player1H2HTrueSkillMeanOldSMS1                                                    ,
                player1H2HTrueSkillStandardDeviationOldSMS1                                       = m.Player1H2HTrueSkillStandardDeviationOldSMS1                                       ,
                player2H2HTrueSkillMeanOldSMS1                                                    = m.Player2H2HTrueSkillMeanOldSMS1                                                    ,
                player2H2HTrueSkillStandardDeviationOldSMS1                                       = m.Player2H2HTrueSkillStandardDeviationOldSMS1                                       ,
                winProbabilityPlayer1H2HSMS1                                                      = m.WinProbabilityPlayer1H2HSMS1                                                      ,
                winProbabilityPlayer2H2HSMS1                                                      = 1 - m.WinProbabilityPlayer1H2HSMS1                                                      ,
                player1H2HTrueSkillMeanGSMS1                                                      = m.Player1H2HTrueSkillMeanGSMS1                                                      ,
                player1H2HTrueSkillStandardDeviationGSMS1                                         = m.Player1H2HTrueSkillStandardDeviationGSMS1                                         ,
                player2H2HTrueSkillMeanGSMS1                                                      = m.Player2H2HTrueSkillMeanGSMS1                                                      ,
                player2H2HTrueSkillStandardDeviationGSMS1                                         = m.Player2H2HTrueSkillStandardDeviationGSMS1                                         ,
                player1H2HTrueSkillMeanOldGSMS1                                                   = m.Player1H2HTrueSkillMeanOldGSMS1                                                   ,
                player1H2HTrueSkillStandardDeviationOldGSMS1                                      = m.Player1H2HTrueSkillStandardDeviationOldGSMS1                                      ,
                player2H2HTrueSkillMeanOldGSMS1                                                   = m.Player2H2HTrueSkillMeanOldGSMS1                                                   ,
                player2H2HTrueSkillStandardDeviationOldGSMS1                                      = m.Player2H2HTrueSkillStandardDeviationOldGSMS1                                      ,
                winProbabilityPlayer1H2HGSMS1                                                     = m.WinProbabilityPlayer1H2HGSMS1                                                     ,
                winProbabilityPlayer2H2HGSMS1                                                     = 1 - m.WinProbabilityPlayer1H2HGSMS1                                                     ,
                player1H2HS2                                                                      = m.Player1H2HS2                                                                      ,
                player2H2HS2                                                                      = m.Player2H2HS2                                                                      ,
                player1H2HOldS2                                                                   = m.Player1H2HOldS2                                                                   ,
                player2H2HOldS2                                                                   = m.Player2H2HOldS2                                                                   ,
                player1H2HTrueSkillMeanMS2                                                        = m.Player1H2HTrueSkillMeanMS2                                                        ,
                player1H2HTrueSkillStandardDeviationMS2                                           = m.Player1H2HTrueSkillStandardDeviationMS2                                           ,
                player2H2HTrueSkillMeanMS2                                                        = m.Player2H2HTrueSkillMeanMS2                                                        ,
                player2H2HTrueSkillStandardDeviationMS2                                           = m.Player2H2HTrueSkillStandardDeviationMS2                                           ,
                player1H2HTrueSkillMeanOldMS2                                                     = m.Player1H2HTrueSkillMeanOldMS2                                                     ,
                player1H2HTrueSkillStandardDeviationOldMS2                                        = m.Player1H2HTrueSkillStandardDeviationOldMS2                                        ,
                player2H2HTrueSkillMeanOldMS2                                                     = m.Player2H2HTrueSkillMeanOldMS2                                                     ,
                player2H2HTrueSkillStandardDeviationOldMS2                                        = m.Player2H2HTrueSkillStandardDeviationOldMS2                                        ,
                winProbabilityPlayer1H2HMS2                                                       = m.WinProbabilityPlayer1H2HMS2                                                       ,
                winProbabilityPlayer2H2HMS2                                                       = 1 - m.WinProbabilityPlayer1H2HMS2                                                       ,
                player1H2HTrueSkillMeanSMS2                                                       = m.Player1H2HTrueSkillMeanSMS2                                                       ,
                player1H2HTrueSkillStandardDeviationSMS2                                          = m.Player1H2HTrueSkillStandardDeviationSMS2                                          ,
                player2H2HTrueSkillMeanSMS2                                                       = m.Player2H2HTrueSkillMeanSMS2                                                       ,
                player2H2HTrueSkillStandardDeviationSMS2                                          = m.Player2H2HTrueSkillStandardDeviationSMS2                                          ,
                player1H2HTrueSkillMeanOldSMS2                                                    = m.Player1H2HTrueSkillMeanOldSMS2                                                    ,
                player1H2HTrueSkillStandardDeviationOldSMS2                                       = m.Player1H2HTrueSkillStandardDeviationOldSMS2                                       ,
                player2H2HTrueSkillMeanOldSMS2                                                    = m.Player2H2HTrueSkillMeanOldSMS2                                                    ,
                player2H2HTrueSkillStandardDeviationOldSMS2                                       = m.Player2H2HTrueSkillStandardDeviationOldSMS2                                       ,
                winProbabilityPlayer1H2HSMS2                                                      = m.WinProbabilityPlayer1H2HSMS2                                                      ,
                winProbabilityPlayer2H2HSMS2                                                      = 1 - m.WinProbabilityPlayer1H2HSMS2                                                      ,
                player1H2HTrueSkillMeanGSMS2                                                      = m.Player1H2HTrueSkillMeanGSMS2                                                      ,
                player1H2HTrueSkillStandardDeviationGSMS2                                         = m.Player1H2HTrueSkillStandardDeviationGSMS2                                         ,
                player2H2HTrueSkillMeanGSMS2                                                      = m.Player2H2HTrueSkillMeanGSMS2                                                      ,
                player2H2HTrueSkillStandardDeviationGSMS2                                         = m.Player2H2HTrueSkillStandardDeviationGSMS2                                         ,
                player1H2HTrueSkillMeanOldGSMS2                                                   = m.Player1H2HTrueSkillMeanOldGSMS2                                                   ,
                player1H2HTrueSkillStandardDeviationOldGSMS2                                      = m.Player1H2HTrueSkillStandardDeviationOldGSMS2                                      ,
                player2H2HTrueSkillMeanOldGSMS2                                                   = m.Player2H2HTrueSkillMeanOldGSMS2                                                   ,
                player2H2HTrueSkillStandardDeviationOldGSMS2                                      = m.Player2H2HTrueSkillStandardDeviationOldGSMS2                                      ,
                winProbabilityPlayer1H2HGSMS2                                                     = m.WinProbabilityPlayer1H2HGSMS2                                                     ,
                winProbabilityPlayer2H2HGSMS2                                                     = 1 - m.WinProbabilityPlayer1H2HGSMS2                                                     ,
                player1H2HS3                                                                      = m.Player1H2HS3                                                                      ,
                player2H2HS3                                                                      = m.Player2H2HS3                                                                      ,
                player1H2HOldS3                                                                   = m.Player1H2HOldS3                                                                   ,
                player2H2HOldS3                                                                   = m.Player2H2HOldS3                                                                   ,
                player1H2HTrueSkillMeanMS3                                                        = m.Player1H2HTrueSkillMeanMS3                                                        ,
                player1H2HTrueSkillStandardDeviationMS3                                           = m.Player1H2HTrueSkillStandardDeviationMS3                                           ,
                player2H2HTrueSkillMeanMS3                                                        = m.Player2H2HTrueSkillMeanMS3                                                        ,
                player2H2HTrueSkillStandardDeviationMS3                                           = m.Player2H2HTrueSkillStandardDeviationMS3                                           ,
                player1H2HTrueSkillMeanOldMS3                                                     = m.Player1H2HTrueSkillMeanOldMS3                                                     ,
                player1H2HTrueSkillStandardDeviationOldMS3                                        = m.Player1H2HTrueSkillStandardDeviationOldMS3                                        ,
                player2H2HTrueSkillMeanOldMS3                                                     = m.Player2H2HTrueSkillMeanOldMS3                                                     ,
                player2H2HTrueSkillStandardDeviationOldMS3                                        = m.Player2H2HTrueSkillStandardDeviationOldMS3                                        ,
                winProbabilityPlayer1H2HMS3                                                       = m.WinProbabilityPlayer1H2HMS3                                                       ,
                winProbabilityPlayer2H2HMS3                                                       = 1 - m.WinProbabilityPlayer1H2HMS3                                                       ,
                player1H2HTrueSkillMeanSMS3                                                       = m.Player1H2HTrueSkillMeanSMS3                                                       ,
                player1H2HTrueSkillStandardDeviationSMS3                                          = m.Player1H2HTrueSkillStandardDeviationSMS3                                          ,
                player2H2HTrueSkillMeanSMS3                                                       = m.Player2H2HTrueSkillMeanSMS3                                                       ,
                player2H2HTrueSkillStandardDeviationSMS3                                          = m.Player2H2HTrueSkillStandardDeviationSMS3                                          ,
                player1H2HTrueSkillMeanOldSMS3                                                    = m.Player1H2HTrueSkillMeanOldSMS3                                                    ,
                player1H2HTrueSkillStandardDeviationOldSMS3                                       = m.Player1H2HTrueSkillStandardDeviationOldSMS3                                       ,
                player2H2HTrueSkillMeanOldSMS3                                                    = m.Player2H2HTrueSkillMeanOldSMS3                                                    ,
                player2H2HTrueSkillStandardDeviationOldSMS3                                       = m.Player2H2HTrueSkillStandardDeviationOldSMS3                                       ,
                winProbabilityPlayer1H2HSMS3                                                      = m.WinProbabilityPlayer1H2HSMS3                                                      ,
                winProbabilityPlayer2H2HSMS3                                                      = 1 - m.WinProbabilityPlayer1H2HSMS3                                                      ,
                player1H2HTrueSkillMeanGSMS3                                                      = m.Player1H2HTrueSkillMeanGSMS3                                                      ,
                player1H2HTrueSkillStandardDeviationGSMS3                                         = m.Player1H2HTrueSkillStandardDeviationGSMS3                                         ,
                player2H2HTrueSkillMeanGSMS3                                                      = m.Player2H2HTrueSkillMeanGSMS3                                                      ,
                player2H2HTrueSkillStandardDeviationGSMS3                                         = m.Player2H2HTrueSkillStandardDeviationGSMS3                                         ,
                player1H2HTrueSkillMeanOldGSMS3                                                   = m.Player1H2HTrueSkillMeanOldGSMS3                                                   ,
                player1H2HTrueSkillStandardDeviationOldGSMS3                                      = m.Player1H2HTrueSkillStandardDeviationOldGSMS3                                      ,
                player2H2HTrueSkillMeanOldGSMS3                                                   = m.Player2H2HTrueSkillMeanOldGSMS3                                                   ,
                player2H2HTrueSkillStandardDeviationOldGSMS3                                      = m.Player2H2HTrueSkillStandardDeviationOldGSMS3                                      ,
                winProbabilityPlayer1H2HGSMS3                                                     = m.WinProbabilityPlayer1H2HGSMS3                                                     ,
                winProbabilityPlayer2H2HGSMS3                                                     = 1 - m.WinProbabilityPlayer1H2HGSMS3                                                     ,
                player1H2HS4                                                                      = m.Player1H2HS4                                                                      ,
                player2H2HS4                                                                      = m.Player2H2HS4                                                                      ,
                player1H2HOldS4                                                                   = m.Player1H2HOldS4                                                                   ,
                player2H2HOldS4                                                                   = m.Player2H2HOldS4                                                                   ,
                player1H2HTrueSkillMeanMS4                                                        = m.Player1H2HTrueSkillMeanMS4                                                        ,
                player1H2HTrueSkillStandardDeviationMS4                                           = m.Player1H2HTrueSkillStandardDeviationMS4                                           ,
                player2H2HTrueSkillMeanMS4                                                        = m.Player2H2HTrueSkillMeanMS4                                                        ,
                player2H2HTrueSkillStandardDeviationMS4                                           = m.Player2H2HTrueSkillStandardDeviationMS4                                           ,
                player1H2HTrueSkillMeanOldMS4                                                     = m.Player1H2HTrueSkillMeanOldMS4                                                     ,
                player1H2HTrueSkillStandardDeviationOldMS4                                        = m.Player1H2HTrueSkillStandardDeviationOldMS4                                        ,
                player2H2HTrueSkillMeanOldMS4                                                     = m.Player2H2HTrueSkillMeanOldMS4                                                     ,
                player2H2HTrueSkillStandardDeviationOldMS4                                        = m.Player2H2HTrueSkillStandardDeviationOldMS4                                        ,
                winProbabilityPlayer1H2HMS4                                                       = m.WinProbabilityPlayer1H2HMS4                                                       ,
                winProbabilityPlayer2H2HMS4                                                       = 1 - m.WinProbabilityPlayer1H2HMS4                                                       ,
                player1H2HTrueSkillMeanSMS4                                                       = m.Player1H2HTrueSkillMeanSMS4                                                       ,
                player1H2HTrueSkillStandardDeviationSMS4                                          = m.Player1H2HTrueSkillStandardDeviationSMS4                                          ,
                player2H2HTrueSkillMeanSMS4                                                       = m.Player2H2HTrueSkillMeanSMS4                                                       ,
                player2H2HTrueSkillStandardDeviationSMS4                                          = m.Player2H2HTrueSkillStandardDeviationSMS4                                          ,
                player1H2HTrueSkillMeanOldSMS4                                                    = m.Player1H2HTrueSkillMeanOldSMS4                                                    ,
                player1H2HTrueSkillStandardDeviationOldSMS4                                       = m.Player1H2HTrueSkillStandardDeviationOldSMS4                                       ,
                player2H2HTrueSkillMeanOldSMS4                                                    = m.Player2H2HTrueSkillMeanOldSMS4                                                    ,
                player2H2HTrueSkillStandardDeviationOldSMS4                                       = m.Player2H2HTrueSkillStandardDeviationOldSMS4                                       ,
                winProbabilityPlayer1H2HSMS4                                                      = m.WinProbabilityPlayer1H2HSMS4                                                      ,
                winProbabilityPlayer2H2HSMS4                                                      = 1 - m.WinProbabilityPlayer1H2HSMS4                                                      ,
                player1H2HTrueSkillMeanGSMS4                                                      = m.Player1H2HTrueSkillMeanGSMS4                                                      ,
                player1H2HTrueSkillStandardDeviationGSMS4                                         = m.Player1H2HTrueSkillStandardDeviationGSMS4                                         ,
                player2H2HTrueSkillMeanGSMS4                                                      = m.Player2H2HTrueSkillMeanGSMS4                                                      ,
                player2H2HTrueSkillStandardDeviationGSMS4                                         = m.Player2H2HTrueSkillStandardDeviationGSMS4                                         ,
                player1H2HTrueSkillMeanOldGSMS4                                                   = m.Player1H2HTrueSkillMeanOldGSMS4                                                   ,
                player1H2HTrueSkillStandardDeviationOldGSMS4                                      = m.Player1H2HTrueSkillStandardDeviationOldGSMS4                                      ,
                player2H2HTrueSkillMeanOldGSMS4                                                   = m.Player2H2HTrueSkillMeanOldGSMS4                                                   ,
                player2H2HTrueSkillStandardDeviationOldGSMS4                                      = m.Player2H2HTrueSkillStandardDeviationOldGSMS4                                      ,
                winProbabilityPlayer1H2HGSMS4                                                     = m.WinProbabilityPlayer1H2HGSMS4                                                     ,
                winProbabilityPlayer2H2HGSMS4                                                     = 1 - m.WinProbabilityPlayer1H2HGSMS4                                                     ,
                player1Streak                                                                     = m.Player1Streak                                                                     ,
                player2Streak                                                                     = m.Player2Streak                                                                     ,
                player1StreakS1                                                                   = m.Player1StreakS1                                                                   ,
                player2StreakS1                                                                   = m.Player2StreakS1                                                                   ,
                player1StreakS2                                                                   = m.Player1StreakS2                                                                   ,
                player2StreakS2                                                                   = m.Player2StreakS2                                                                   ,
                player1StreakS3                                                                   = m.Player1StreakS3                                                                   ,
                player2StreakS3                                                                   = m.Player2StreakS3                                                                   ,
                player1StreakS4                                                                   = m.Player1StreakS4                                                                   ,
                player2StreakS4                                                                   = m.Player2StreakS4                                                                   ,
                p1SetsWon                                                                         = m.P1SetsWon                                                                         ,
                p2SetsWon                                                                         = m.P2SetsWon                                                                         ,
                p1GamesWon                                                                        = m.P1GamesWon                                                                        ,
                p2GamesWon                                                                        = m.P2GamesWon                                                                        ,
                p1SetsLoss                                                                        = m.P1SetsLoss                                                                        ,
                p2SetsLoss                                                                        = m.P2SetsLoss                                                                        ,
                p1GamesLoss                                                                       = m.P1GamesLoss                                                                       ,
                p2GamesLoss                                                                       = m.P2GamesLoss                                                                       ,
                winProbabilityPlayer1NN                                                           = m.WinProbabilityNN,
                winProbabilityPlayer2NN                                                           = 1 - m.WinProbabilityNN,
                valueMarginPlayer1                                                                = vm1,
                valueMarginPlayer2                                                                = vm2,
                who2Bet                                                                           = who2Bet,
                isFinished                                                                        = m.IsFinished,
            };

            if (m.MatchTPId.HasValue)
            {
                var quotes = await GetQuotesForMatchAsync(m.MatchTPId.Value, m.DateTime, m.IsFinished ?? false);
                if (quotes.Count > 0)
                {
                    dto.Odds = OddsProjectionService.Build(m.MatchTPId.Value, quotes, includeMergedSeries: true);
                }
            }

            return dto;
        }

        // Pozovi ovo JEDNOM nakon što si završio PARSIRANJE (npr. nakon cijelog “tjedna”, ili nakon svakog dana – kako ti paše).
        public async Task FlushDailyBrotliArchivesAsync(CancellationToken ct = default)
        {
            Directory.CreateDirectory(_dailyOutDir);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // imena u DTO su već camelCase kako želiš
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };

            foreach (var kvp in _dailyByDate)
            {
                ct.ThrowIfCancellationRequested();

                var date = kvp.Key;                 // DateOnly
                var list = kvp.Value;               // List<MatchMongoDTO>

                // sort opcionalno – npr. po vremenu ili _id
                list.Sort((a, b) =>
                {
                    var t1 = a.dateTime ?? DateTime.MinValue;
                    var t2 = b.dateTime ?? DateTime.MinValue;
                    return t1.CompareTo(t2);
                });

                var json = JsonSerializer.Serialize(list, jsonOptions);

                var outPath = Path.Combine(_dailyOutDir, $"{date:yyyyMMdd}.br");
                BrotliCompressor.CompressStringToFile(json, outPath);

                Console.WriteLine($"[DailyArchive] Wrote {list.Count} matches -> {outPath}");
            }

            // Ako želiš, isprazni buffer nakon flush-a
            _dailyByDate.Clear();
        }

        public async Task matchDetailsBrotliArchivesAsync(int matchTPId, MatchDetailsDTO dto, CancellationToken ct = default)
        {
            Directory.CreateDirectory(_matchDetailsOutDir);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };

            ct.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(dto, jsonOptions);

            var outPath = Path.Combine(_matchDetailsOutDir, $"{matchTPId}.br");
            BrotliCompressor.CompressStringToFile(json, outPath);

            //Console.WriteLine($"[DailyArchive] Wrote {list.Count} matches -> {outPath}");
        }

        //public async Task playerDetailsBrotliArchivesAsync(int playerTPId, PlayerDetailsDTO dto, CancellationToken ct = default)
        //{
        //    Directory.CreateDirectory(_matchDetailsOutDir);

        //    var jsonOptions = new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = null,
        //        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //        WriteIndented = false
        //    };

        //    ct.ThrowIfCancellationRequested();

        //    var json = JsonSerializer.Serialize(dto, jsonOptions);

        //    var outPath = Path.Combine(_matchDetailsOutDir, $"{playerTPId}.br");
        //    BrotliCompressor.CompressStringToFile(json, outPath);

        //    //Console.WriteLine($"[DailyArchive] Wrote {list.Count} matches -> {outPath}");
        //}

        public sealed record ColumnSpec(
            string Name,
            SqlDbType Type,
            int? Length = null,      // za (n)varchar/char
            byte? Scale = null,      // za decimal
            bool IsUnicode = false,  // nvarchar
            bool Nullable = true
        );

        private async Task<List<OddsQuoteDTO>> GetQuotesForMatchAsync(int matchTPId, DateTime? matchStartLocal, bool isFinished)
        {
            // 1) probaj iz baze (ako je parser već trčao ranije)
            var rows = (await _matchOddsRepository.GetOddsByMatchAsync(matchTPId)).ToList();

            // 2) ako nema u bazi, parsiraj iz .br HTML-a (d:\brArchives\MatchOdds\Working\{matchTPId}.br)
            if (rows.Count == 0)
            {
                var parsed = await _matchOddsParser.ParseOddsFromArchiveAsync(matchTPId, matchStartLocal, isFinished);
                rows = parsed.ToList(); // pažnja: ova metoda i SPREM A u DB; to želiš ili napravi “read-only” varijantu ako ne
            }

            // 3) mapiranje u DTO-e za projekciju
            var quotes = rows
                .Where(mo => mo.Player1Odds.HasValue && mo.Player2Odds.HasValue) // filter prije mapiranja
                .Select(mo => new OddsQuoteDTO
                {
                    OddsId = mo.OddsId ?? 0,
                    MatchTPId = mo.MatchTPId ?? matchTPId,            // ✅ popuni
                    BookieId = mo.BookieId ?? 0,
                    BookieName = (mo.BookieId.HasValue && _reference.BookiesById.TryGetValue(mo.BookieId.Value, out var bk))
                                        ? bk.BookieName : null,
                    SourceFileTime = (DateTime)mo.SourceFileTime!,                     // ✅ postavi umjesto defaulta
                    CoalescedTime = mo.CoalescedTime ?? mo.DateTime ?? mo.SourceFileTime ?? DateTime.UtcNow,
                    Player1Odds = mo.Player1Odds,
                    Player2Odds = mo.Player2Odds
                })
                .ToList();

            return quotes;
        }
    }
}