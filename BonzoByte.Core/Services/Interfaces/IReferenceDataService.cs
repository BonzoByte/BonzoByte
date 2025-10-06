using BonzoByte.Core.DTOs;
using BonzoByte.Core.Models;
using System.Security.Cryptography;

namespace BonzoByte.Core.Services.Interfaces
{
    public interface IReferenceDataService
    {
        // ID-based lookup
        IReadOnlyDictionary<int, Country> Countries { get; }
        IDictionary<int, Bookie> Bookies { get; }
        IReadOnlyDictionary<int, Plays> Plays { get; }
        IReadOnlyDictionary<int, Round> Rounds { get; }
        IReadOnlyDictionary<int, Surface> Surfaces { get; }
        IReadOnlyDictionary<int, TournamentLevel> TournamentLevels { get; }
        IReadOnlyDictionary<int, TournamentType> TournamentTypes { get; }
        IDictionary<int, TournamentEvent> TournamentEvents { get; }
        IDictionary<int, Player> Players { get; }
        IDictionary<int, Match> Matches { get; }
        IDictionary<int, IReadOnlyDictionary<int, MatchOdds>> MatchOddsByMatch { get; }
        IEnumerable<MatchOdds> GetOddsForMatch(int matchTPId);

        // Custom mape za lookup po drugim ključevima
        IReadOnlyDictionary<string, Country> CountriesByName { get; }
        IReadOnlyDictionary<string, Bookie> BookiesByName { get; }
        IReadOnlyDictionary<int, Bookie> BookiesById { get; }
        IReadOnlyDictionary<string, Country> CountriesByISO2 { get; }
        IReadOnlyDictionary<string, Country> CountriesByISO3 { get; }
        IReadOnlyDictionary<string, Surface> SurfacesByLabel { get; }
        IReadOnlyDictionary<string, TournamentType> TournamentTypesByName { get; }
        IReadOnlyDictionary<string, TournamentLevel> TournamentLevelsByName { get; }

        int GetNextAvailableBookieId();
        Task LoadAllAsync();

        void AddCountry(Country country);
        void AddBookie(Bookie bookie);
        void AddPlays(Plays plays);
        void AddRound(Round round);
        void AddSurface(Surface surface);
        void AddTournamentLevel(TournamentLevel tournamentLevel);
        void AddTournamentType(TournamentType tournamentType);
        void AddTournamentEvent(TournamentEvent tournamentEvent);
        void AddPlayer(Player player);
        bool TryGetPlayer(int playerTPId, out Player? player);
        void AddMatch(Match match);
        (MatchOdds? bestP1, MatchOdds? bestP2) GetBestOddsBySide(int matchTPId);
        (double? avgP1, double? avgP2) GetAverageOdds(int matchTPId);
        void AddOrUpdateMatchOdds(MatchOdds matchOdds);
        IEnumerable<MatchOdds> GetLatestMatchOddsByBookie(int mid);
        (Bookie bookie, bool isNew) GetOrAddBookieByName(string bookieName);
        Task TournamentEventBrotliArchivesAsync(int tournamentEventTPId, TournamentEventMongoDTO dto, CancellationToken ct = default);
    }
}