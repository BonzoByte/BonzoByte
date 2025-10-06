using BonzoByte.Core.DTOs;
using BonzoByte.Core.Helpers;
using BonzoByte.Core.Models;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace BonzoByte.Core.Services
{
    public class MatchPageParserService
    {
        private readonly MatchTournamentParser _tournamentParser;
        private readonly TournamentEventDownloaderService _tournamentEventDownloaderService;

        public MatchPageParserService(MatchTournamentParser tournamentParser, TournamentEventDownloaderService tournamentEventDownloaderService)
        {
            _tournamentParser = tournamentParser;
            _tournamentEventDownloaderService = tournamentEventDownloaderService;
        }

        public async Task<List<Match>> ParseTournamentEventsFromArchive(string archivePath, List<Models.Match> matches)
        {
            if (matches == null)
                matches = new List<Models.Match>();
            try
            {
                string decompressedHtml = BrotliCompressor.DecompressFileToString(archivePath);
                string cleanedHtml = HtmlCleaner.Clean(decompressedHtml);

                MatchParseResultDTO? parsedResult = await _tournamentParser.ParseAsync(cleanedHtml, _tournamentEventDownloaderService);

                if (parsedResult != null)
                {
                    return parsedResult.Matches;
                }
                else
                {
                    return new List<Match>();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[PARSE ERROR] {archivePath}: {ex.Message}");
                return null!;
            }
        }
    }
}