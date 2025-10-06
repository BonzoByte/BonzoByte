using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.DAL.Repositories;
using BonzoByte.Core.DTOs;
using BonzoByte.Core.Helpers;
using BonzoByte.Core.Models;
using BonzoByte.Core.Services.Interfaces;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly IDbConnection _connection;
        private readonly ICountryRepository _countryRepository;
        private IBookieRepository _bookieRepository;
        private readonly IPlaysRepository _playsRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly ISurfaceRepository _surfaceRepository;
        private readonly ITournamentLevelRepository _tournamentLevelRepository;
        private readonly ITournamentTypeRepository _tournamentTypeRepository;
        private ITournamentEventRepository _tournamentEventRepository;
        private IPlayerRepository _playerRepository;
        private IMatchRepository _matchRepository;
        private IMatchOddsRepository _matchOddsRepository;
        private readonly ScrapingPathResolver _scrapingPathResolver;

        // ==== Mutable backing fields (većinom single-threaded load; iznimke ispod) ====

        private readonly Dictionary<int, Country> _countries = new();
        private readonly Dictionary<int, Plays> _plays = new();
        private readonly Dictionary<int, Round> _rounds = new();
        private readonly Dictionary<int, Surface> _surfaces = new();
        private readonly Dictionary<int, TournamentLevel> _tournamentLevels = new();
        private readonly Dictionary<int, TournamentType> _tournamentTypes = new();
        private Dictionary<int, TournamentEvent> _tournamentEvents = new();
        private Dictionary<int, Player> _players = new();
        private Dictionary<int, Models.Match> _matches = new();

        // ==== THREAD-SAFE: Bookies i by-name lookups ====

        private ConcurrentDictionary<int, Bookie> _bookies = new();
        private readonly ConcurrentDictionary<string, Bookie> _bookiesByName =
            new(StringComparer.InvariantCultureIgnoreCase);
        private readonly ConcurrentDictionary<int, Bookie> _bookiesById = new();

        // Atomarni ID generator (seed-a se iz baze u LoadAllAsync)
        private int _nextBookieId = 0;

        // ==== THREAD-SAFE: najnoviji MatchOdds per (MatchTPId, BookieId) ====

        private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, MatchOdds>> _matchOddsByMatch =
            new();

        // ==== Public read-only viewovi ====

        public IReadOnlyDictionary<int, Country> Countries => _countries;
        public IDictionary<int, Bookie> Bookies => new ReadOnlyDictionary<int, Bookie>(_bookies);
        public IReadOnlyDictionary<int, Plays> Plays => _plays;
        public IReadOnlyDictionary<int, Round> Rounds => _rounds;
        public IReadOnlyDictionary<int, Surface> Surfaces => _surfaces;
        public IReadOnlyDictionary<int, TournamentLevel> TournamentLevels => _tournamentLevels;
        public IReadOnlyDictionary<int, TournamentType> TournamentTypes => _tournamentTypes;
        public IDictionary<int, TournamentEvent> TournamentEvents => _tournamentEvents;
        public IDictionary<int, Player> Players => _players;
        public IDictionary<int, Models.Match> Matches => _matches;

        public IDictionary<int, IReadOnlyDictionary<int, MatchOdds>> MatchOddsByMatch =>
            _matchOddsByMatch.ToDictionary(
                kv => kv.Key,
                kv => (IReadOnlyDictionary<int, MatchOdds>)new ReadOnlyDictionary<int, MatchOdds>(kv.Value));

        public IReadOnlyDictionary<string, Country> CountriesByName => _countriesByName;
        public IReadOnlyDictionary<string, Country> CountriesByISO2 => _countriesByISO2;
        public IReadOnlyDictionary<string, Country> CountriesByISO3 => _countriesByISO3;
        public IReadOnlyDictionary<string, Surface> SurfacesByLabel => _surfacesByLabel;
        public IReadOnlyDictionary<string, TournamentType> TournamentTypesByName => _tournamentTypesByName;
        public IReadOnlyDictionary<string, TournamentLevel> TournamentLevelsByName => _tournamentLevelsByName;
        public IReadOnlyDictionary<string, Bookie> BookiesByName => new ReadOnlyDictionary<string, Bookie>(_bookiesByName);
        public IReadOnlyDictionary<int, Bookie> BookiesById => new ReadOnlyDictionary<int, Bookie>(_bookiesById);

        // By-name lookups (ostaju obični jer se pune jednom tijekom loada)
        private readonly Dictionary<string, Country> _countriesByName = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, Country> _countriesByISO2 = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, Country> _countriesByISO3 = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, Surface> _surfacesByLabel = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, TournamentType> _tournamentTypesByName = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, TournamentLevel> _tournamentLevelsByName = new(StringComparer.InvariantCultureIgnoreCase);

        public ReferenceDataService(
            IDbConnection connection,
            ICountryRepository countryRepository,
            IBookieRepository bookieRepository,
            IPlaysRepository playsRepository,
            IRoundRepository roundRepository,
            ISurfaceRepository surfaceRepository,
            ITournamentLevelRepository tournamentLevelRepository,
            ITournamentTypeRepository tournamentTypeRepository,
            ITournamentEventRepository tournamentEventRepository,
            IPlayerRepository playerRepository,
            IMatchRepository matchRepository,
            IMatchOddsRepository matchOddsRepository,
            ScrapingPathResolver scrapingPathResolver)
        {
            _connection = connection;
            _countryRepository = countryRepository;
            _bookieRepository = bookieRepository;
            _playsRepository = playsRepository;
            _roundRepository = roundRepository;
            _surfaceRepository = surfaceRepository;
            _tournamentLevelRepository = tournamentLevelRepository;
            _tournamentTypeRepository = tournamentTypeRepository;
            _tournamentEventRepository = tournamentEventRepository;
            _playerRepository = playerRepository;
            _matchRepository = matchRepository;
            _matchOddsRepository = matchOddsRepository;
            _scrapingPathResolver = scrapingPathResolver;
        }

        // === BOOKIES: thread-safe ID i get-or-add po imenu ===

        /// <summary>
        /// Vraća novi jedinstveni BookieId bez enumeriranja kolekcija (thread-safe).
        /// </summary>
        public int GetNextAvailableBookieId() => Interlocked.Increment(ref _nextBookieId);

        /// <summary>
        /// Idempotentno dohvaća postojeću kladionicu po imenu ili atomarno kreira novu.
        /// Vraća (bookie, isNew).
        /// </summary>
        public (Bookie bookie, bool isNew) GetOrAddBookieByName(string bookieName)
        {
            var name = NormalizeName(bookieName);
            Bookie? created = null;

            var existing = _bookiesByName.GetOrAdd(name, _ =>
            {
                var id = GetNextAvailableBookieId();
                var b = new Bookie { BookieId = id, BookieName = name };

                _bookies[id] = b;   // int -> bookie
                _bookiesById[id] = b;   // ✅ BY-ID MAPA

                created = b;
                return b;
            });

            return (existing, created is not null);
        }


        // === LOAD ===

        public async Task LoadAllAsync()
        {
            var countries = await _countryRepository.GetAllCountriesAsync();
            Console.WriteLine($"🌍 Loaded countries: {countries.Count()}");
            _countries.Clear();
            foreach (var c in countries.Where(c => c.CountryTPId.HasValue))
                _countries[c.CountryTPId!.Value] = c;

            _countriesByName.Clear();
            foreach (var c in _countries.Values.Where(c => !string.IsNullOrWhiteSpace(c.CountryFull)))
                _countriesByName[c.CountryFull!.Trim()] = c;

            _countriesByISO2.Clear();
            foreach (var c in _countries.Values.Where(c => !string.IsNullOrWhiteSpace(c.CountryISO2)))
                _countriesByISO2[c.CountryISO2!.Trim()] = c;

            _countriesByISO3.Clear();
            foreach (var c in _countries.Values.Where(c => !string.IsNullOrWhiteSpace(c.CountryISO3)))
                _countriesByISO3[c.CountryISO3!.Trim()] = c;

            var bookies = await _bookieRepository.GetAllBookiesAsync();
            _bookies.Clear();
            _bookiesByName.Clear();
            _bookiesById.Clear();

            var maxId = 0;
            foreach (var b in bookies.Where(b => b.BookieId.HasValue))
            {
                var id = b.BookieId!.Value;

                _bookies[id] = b;                     // int -> bookie
                _bookiesById[id] = b;                 // ✅ POPUNI BY-ID MAPU

                if (!string.IsNullOrWhiteSpace(b.BookieName))
                    _bookiesByName[b.BookieName!.Trim()] = b;

                if (id > maxId) maxId = id;
            }
            Interlocked.Exchange(ref _nextBookieId, maxId);

            var plays = await _playsRepository.GetAllPlaysAsync();
            _plays.Clear();
            foreach (var p in plays.Where(p => p.PlaysId.HasValue))
                _plays[p.PlaysId!.Value] = p;

            var rounds = await _roundRepository.GetAllRoundsAsync();
            _rounds.Clear();
            foreach (var r in rounds.Where(r => r.RoundId.HasValue))
                _rounds[r.RoundId!.Value] = r;

            var surfaces = await _surfaceRepository.GetAllSurfacesAsync();
            _surfaces.Clear();
            foreach (var s in surfaces.Where(s => s.SurfaceId.HasValue))
                _surfaces[s.SurfaceId!.Value] = s;

            _surfacesByLabel.Clear();
            foreach (var s in _surfaces.Values.Where(s => !string.IsNullOrWhiteSpace(s.SurfaceName)))
                _surfacesByLabel[s.SurfaceName!.Trim()] = s;

            var tournamentLevels = await _tournamentLevelRepository.GetAllTournamentLevelsAsync();
            _tournamentLevels.Clear();
            foreach (var l in tournamentLevels.Where(l => l.TournamentLevelId.HasValue))
                _tournamentLevels[l.TournamentLevelId!.Value] = l;

            _tournamentLevelsByName.Clear();
            foreach (var l in _tournamentLevels.Values.Where(l => !string.IsNullOrWhiteSpace(l.TournamentLevelName)))
                _tournamentLevelsByName[l.TournamentLevelName!.Trim()] = l;

            var tournamentTypes = await _tournamentTypeRepository.GetAllTournamentTypesAsync();
            _tournamentTypes.Clear();
            foreach (var t in tournamentTypes.Where(t => t.TournamentTypeId.HasValue))
                _tournamentTypes[t.TournamentTypeId!.Value] = t;

            _tournamentTypesByName.Clear();
            foreach (var t in _tournamentTypes.Values.Where(t => !string.IsNullOrWhiteSpace(t.TournamentTypeName)))
                _tournamentTypesByName[t.TournamentTypeName!.Trim()] = t;

            var tournamentEvents = await _tournamentEventRepository.GetAllTournamentEventsAsync();
            _tournamentEvents.Clear();
            foreach (var e in tournamentEvents.Where(e => e.TournamentEventTPId.HasValue))
                _tournamentEvents[e.TournamentEventTPId!.Value] = e;

            var players = await _playerRepository.GetAllPlayersAsync();
            _players.Clear();
            foreach (var p in players.Where(p => p.PlayerTPId.HasValue))
                _players[p.PlayerTPId!.Value] = p;

            var matches = await _matchRepository.GetAllMatchesAsync();
            _matches.Clear();
            foreach (var m in matches.Where(m => m.MatchTPId.HasValue))
                _matches[m.MatchTPId!.Value] = m;
        }

        // === Brotli arhive (ne diramo) ===

        public async Task playerDetailsBrotliArchivesAsync(int playerTPId, PlayerDetailsDTO dto, CancellationToken ct = default)
        {
            Directory.CreateDirectory(@"D:\BrotliArchives\PlayerDetails");

            var outPath = Path.Combine(@"D:\BrotliArchives\PlayerDetails", $"{playerTPId}.br");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };

            ct.ThrowIfCancellationRequested();

            await using var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await using var brotli = new BrotliStream(fs, CompressionLevel.Optimal, leaveOpen: false);

            await JsonSerializer.SerializeAsync(brotli, dto, jsonOptions, ct);
            await brotli.FlushAsync(ct);
        }

        public async Task TournamentEventBrotliArchivesAsync(int tournamentEventTPId, TournamentEventMongoDTO dto, CancellationToken ct = default)
        {
            Directory.CreateDirectory(@"D:\BrotliArchives\TournamentEventDetails");

            var outPath = Path.Combine(@"D:\BrotliArchives\TournamentEventDetails", $"{tournamentEventTPId}.br");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };

            await using var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await using var brotli = new BrotliStream(fs, CompressionLevel.Optimal, leaveOpen: false);

            await JsonSerializer.SerializeAsync(brotli, dto, jsonOptions, ct);
            await brotli.FlushAsync(ct);
        }

        // === Add* helperi (ostavljeni radi kompatibilnosti; Bookie je thread-safe) ===

        public void AddCountry(Country country)
        {
            if (country.CountryTPId is not int id) return;
            _countries[id] = country;
        }

        public void AddBookie(Bookie bookie)
        {
            if (bookie.BookieId is not int id) return;

            // Upis u obje mape
            _bookies[id] = bookie;
            if (!string.IsNullOrWhiteSpace(bookie.BookieName))
                _bookiesByName[bookie.BookieName.Trim()] = bookie;

            // Održi generator barem na max(id)
            int current;
            do
            {
                current = Volatile.Read(ref _nextBookieId);
                if (id <= current) break;
            } while (Interlocked.CompareExchange(ref _nextBookieId, id, current) != current);
        }

        public void AddPlays(Plays plays)
        {
            if (plays.PlaysId is not int id) return;
            _plays[id] = plays;
        }

        public void AddRound(Round round)
        {
            if (round.RoundId is not int id) return;
            _rounds[id] = round;
        }

        public void AddSurface(Surface surface)
        {
            if (surface.SurfaceId is not int id) return;
            _surfaces[id] = surface;
        }

        public void AddTournamentLevel(TournamentLevel tournamentLevel)
        {
            if (tournamentLevel.TournamentLevelId is not int id) return;
            _tournamentLevels[id] = tournamentLevel;
        }

        public void AddTournamentType(TournamentType tournamentType)
        {
            if (tournamentType.TournamentTypeId is not int id) return;
            _tournamentTypes[id] = tournamentType;
        }

        public void AddTournamentEvent(TournamentEvent tournamentEvent)
        {
            if (tournamentEvent.TournamentEventTPId is not int id) return;
            _tournamentEvents[id] = tournamentEvent;
        }

        public bool TryGetPlayer(int playerTPId, out Player? player) => _players.TryGetValue(playerTPId, out player);

        public void AddPlayer(Player player)
        {
            if (player.PlayerTPId is not int id) return;
            _players[id] = player;
        }

        public void AddMatch(Models.Match match)
        {
            if (match.MatchTPId is not int id) return;
            _matches[id] = match;
        }

        // === MatchOdds cache (THREAD-SAFE) ===

        public IEnumerable<MatchOdds> GetOddsForMatch(int matchTPId)
        {
            if (_matchOddsByMatch.TryGetValue(matchTPId, out var byBookie))
                return byBookie.Values.ToList(); // snapshot
            return Enumerable.Empty<MatchOdds>();
        }

        public (MatchOdds? bestP1, MatchOdds? bestP2) GetBestOddsBySide(int matchTPId)
        {
            var all = GetOddsForMatch(matchTPId).ToList();

            MatchOdds? bestP1 = all
                .Where(o => o.Player1Odds.HasValue)
                .OrderByDescending(o => o.Player1Odds!.Value)
                .FirstOrDefault();

            MatchOdds? bestP2 = all
                .Where(o => o.Player2Odds.HasValue)
                .OrderByDescending(o => o.Player2Odds!.Value)
                .FirstOrDefault();

            return (bestP1, bestP2);
        }

        public (double? avgP1, double? avgP2) GetAverageOdds(int matchTPId)
        {
            var all = GetOddsForMatch(matchTPId).ToList();
            if (all.Count == 0) return (null, null);

            var p1s = all.Where(o => o.Player1Odds.HasValue).Select(o => o.Player1Odds!.Value).ToList();
            var p2s = all.Where(o => o.Player2Odds.HasValue).Select(o => o.Player2Odds!.Value).ToList();

            double? avg1 = p1s.Count > 0 ? (double)p1s.Average() : (double?)null;
            double? avg2 = p2s.Count > 0 ? (double)p2s.Average() : (double?)null;
            return (avg1, avg2);
        }

        public void AddOrUpdateMatchOdds(MatchOdds matchOdds)
        {
            if (matchOdds.MatchTPId is not int mid || matchOdds.BookieId is not int bid) return;

            var byBookie = _matchOddsByMatch.GetOrAdd(mid, _ => new ConcurrentDictionary<int, MatchOdds>());

            static DateTime MinDT(MatchOdds mo) => mo.DateTime ?? DateTime.MinValue;

            byBookie.AddOrUpdate(
                bid,
                matchOdds,
                (_, prev) => MinDT(matchOdds) >= MinDT(prev) ? matchOdds : prev
            );
        }

        public IReadOnlyList<MatchOdds> GetLatestMatchOddsByBookie(int matchTPId)
        {
            if (_matchOddsByMatch.TryGetValue(matchTPId, out var byBookie))
                return byBookie.Values.ToList();
            return Array.Empty<MatchOdds>();
        }

        IEnumerable<MatchOdds> IReferenceDataService.GetLatestMatchOddsByBookie(int mid) => GetLatestMatchOddsByBookie(mid);

        // === Helpers ===

        private static string NormalizeName(string name)
        {
            var n = (name ?? string.Empty).Trim();
            n = Regex.Replace(n, @"\s+", " ");
            return n;
        }
    }
}