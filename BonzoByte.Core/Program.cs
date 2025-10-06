using BonzoByte.Core.Configs;
using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.DAL.Repositories;
using BonzoByte.Core.DTOs;
using BonzoByte.Core.Helpers;
using BonzoByte.Core.Models;
using BonzoByte.Core.Services;
using BonzoByte.Core.Services.Interfaces;
using BonzoByte.Core.SQLGen;
using BonzoByte.ML;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BonzoByte.Core
{
    internal class Program
    {
        static string connectionString = "Data Source=Stanko;Initial Catalog=BonzoByte;Integrated Security=True;Connection Timeout=180000000;TrustServerCertificate=True";
        public static async Task Main(string[] args)
        {
#if DEBUG
            // Hook first-chance FormatException logging as early as possible (once)
            if (!FirstChanceHook.IsHooked)
            {
                AppDomain.CurrentDomain.FirstChanceException += FirstChanceHook.Handler;
                FirstChanceHook.IsHooked = true;
            }
#endif
            try
            {
                using var host = CreateHostBuilder(args).Build();
                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;

                await InitializeReferenceDataAsync(services);

                var missingDates = GetMissingArchiveDates(services);
                await DownloadMissingArchivesAsync(services, missingDates);

                await ParseAllMatchPagesAsync(services);

                Console.WriteLine("\n✅ DONE downloading");

                //await GenerateBrotliArchivesForDailyMatches(services);

                Console.WriteLine("\n✅ PARSING COMPLETE");

                //await HistoricalOddsBatchRunner(services);
            }
            catch (Exception ex)
            {
                WriteError("Fatal error in Main", ex);
            }
        }

        private static async Task DailyMatchExporter()
        {
            var exporter = new DailyMatchesExporter(connectionString, "dbo.GetDailyMatchesForDate");

            var start = new DateOnly(1990, 1, 1);
            var end = DailyMatchesExporter.GetLocalTodayEuropeZagreb();

            await exporter.RunAsync(start, end, async (date, reader) =>
            {
                string aaa;
                aaa = "";
            });
        }

        private static async Task TournamentMatchesExporterAsync()
        {
            var exporter = new TournamentMatchesExporter(connectionString, "dbo.GetTournamentMatches");

            // 1) Dohvati listu TE ID-ova (vidi opciju A ili B dolje)
            var teIds = await GetAllTournamentEventIdsAsync(connectionString, CancellationToken.None);

            // 2) Pokreni export
            await exporter.RunAsync(
                teIds,
                async (teId, reader) =>
                {
                    // TODO: mapiraj reader -> DTO i snimi /tournaments/{teId}.br
                    // Primjer čitanja:
                    // var rows = new List<YourTournamentMatchDto>();
                    // while (await reader.ReadAsync())
                    // {
                    //     var matchId = reader.GetInt32(reader.GetOrdinal("MatchTPId"));
                    //     // ...mapiraj ostale kolone...
                    //     rows.Add(dto);
                    // }
                    // // serialize rows -> json -> brotli -> file ...
                },
                sortDirection: "ASC",
                ct: CancellationToken.None
            );
        }

        private static async Task<List<int>> GetAllTournamentEventIdsAsync(string cs, CancellationToken ct)
        {
            var ids = new List<int>();
            using var conn = new SqlConnection(cs);
            await conn.OpenAsync(ct);

            // Ako želiš SVE evente iz tablice TournamentEvent, promijeni izvor u dbo.TournamentEvent
            var sql = "SELECT DISTINCT TournamentEventTPId FROM dbo.Match ORDER BY DateTime";
            using var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text, CommandTimeout = 0 };
            using var r = await cmd.ExecuteReaderAsync(ct);
            while (await r.ReadAsync(ct))
                ids.Add(r.GetInt32(0));

            return ids;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    // Config bindings
                    services.Configure<MongoSettings>(context.Configuration.GetSection("MongoSettings"));
                    services.Configure<ScrapingResultsSettings>(context.Configuration.GetSection("ScrapingResults"));
                    services.Configure<TrueskillSettings>(context.Configuration.GetSection("Trueskill"));
                    services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<MongoSettings>>().Value);
                    services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ScrapingResultsSettings>>().Value);
                    services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<TrueskillSettings>>().Value);

                    // DB connection
                    services.AddTransient<IDbConnection>(sp =>
                        new SqlConnection(context.Configuration.GetConnectionString("DefaultConnection")));

                    // Repositories
                    services.AddScoped<ICountryRepository, CountryRepository>();
                    services.AddScoped<IBookieRepository, BookieRepository>();
                    services.AddScoped<IPlaysRepository, PlaysRepository>();
                    services.AddScoped<IRoundRepository, RoundRepository>();
                    services.AddScoped<ISurfaceRepository, SurfaceRepository>();
                    services.AddScoped<ITournamentLevelRepository, TournamentLevelRepository>();
                    services.AddScoped<ITournamentTypeRepository, TournamentTypeRepository>();
                    services.AddScoped<ITournamentEventRepository, TournamentEventRepository>();
                    services.AddScoped<IPlayerRepository, PlayerRepository>();
                    services.AddScoped<IMatchRepository, MatchRepository>();
                    services.AddScoped<IMatchActiveRepository, MatchActiveRepository>();
                    services.AddScoped<IMatchOddsRepository, MatchOddsRepository>();
                    services.AddScoped<IMatchActiveOddsRepository, MatchActiveOddsRepository>();

                    // Core services
                    services.AddHttpClient<MatchPageDownloaderService>();
                    services.AddSingleton<MatchDateService>();
                    services.AddSingleton<ResultArchiveManager>();
                    services.AddSingleton<ResultScraperScheduler>();
                    services.AddSingleton<ScrapingPathResolver>();
                    services.AddSingleton<BrotliArchiveScanner>();
                    services.AddSingleton<MissingArchiveDetectorService>();
                    services.AddSingleton<IReferenceDataService, ReferenceDataService>();
                    services.AddSingleton<MatchPageParserService>();
                    services.AddScoped<MatchTournamentParser>();
                    services.AddSingleton<PlayerHtmlParser>();
                    services.AddHttpClient<TournamentEventDownloaderService>();
                    services.AddSingleton<TournamentParserHelper>();
                    services.AddSingleton<MatchDetailsDownloaderService>();

                    // 🔄 PROMJENA: ova dva neka budu Scoped (ovise o Scoped repo-ima)
                    services.AddScoped<MatchOddsParserService>();
                    services.AddScoped<MatchOddsFlaggerService>();

                    services.AddSingleton(sp => new NNRouterScorer(context.Configuration.GetConnectionString("DefaultConnection")!));
                });

        private static async Task InitializeReferenceDataAsync(IServiceProvider services)
        {
            var referenceData = services.GetRequiredService<IReferenceDataService>();
            Console.WriteLine("📚 Loading reference data...");
            await referenceData.LoadAllAsync();
            Console.WriteLine("📚 Reference data loaded.");
        }

        private static List<DateTime> GetMissingArchiveDates(IServiceProvider services)
        {
            var detector = services.GetRequiredService<MissingArchiveDetectorService>();
            var missingKeys = detector.GetMissingArchiveKeys();

            Console.WriteLine("📦 Missing archive keys:");
            foreach (var key in missingKeys)
                Console.WriteLine(" - " + key);

            var missingDates = missingKeys
                .Select(key =>
                {
                    var parts = key.Split('_');
                    if (parts.Length != 4) return (DateTime?)null;

                    var dateStr = $"{parts[0]}-{parts[1]}-{parts[2]}";
                    return DateTime.TryParse(dateStr, out var dt) ? dt : (DateTime?)null;
                })
                .Where(dt => dt.HasValue)
                .Select(dt => dt!.Value.Date)
                .Distinct()
                .OrderBy(dt => dt)
                .ToList();

            Console.WriteLine("📅 Missing archive dates:");
            foreach (var dt in missingDates)
                Console.WriteLine($" - {dt:yyyy-MM-dd}");

            return missingDates;
        }

        private static async Task DownloadMissingArchivesAsync(IServiceProvider services, List<DateTime> missingDates)
        {
            var downloader = services.GetRequiredService<MatchPageDownloaderService>();
            await downloader.DownloadMatchPagesAsync(missingDates);
        }

        private static async Task ParseAllMatchPagesAsync(IServiceProvider services)
        {
            var parser = services.GetRequiredService<MatchPageParserService>();
            var scrapingSettings = services.GetRequiredService<ScrapingResultsSettings>();
            var matchPagesPath = "d:\\brArchives\\Results\\Working\\";
            var _reference = services.GetRequiredService<IReferenceDataService>();

            string outputFolder = @"d:\brArchives\Results\Finished\";
            Directory.CreateDirectory(outputFolder);

            var allBrFiles = Directory.GetFiles(matchPagesPath, "*.br", SearchOption.AllDirectories);
            Console.WriteLine($"\n🔍 Found {allBrFiles.Length} .br match pages to parse");

            // 1) Grupiraj po datumu iz naziva fajla (yyyy_MM_dd)
            var dateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var full in allBrFiles)
            {
                var nameNoExt = Path.GetFileNameWithoutExtension(full); // npr. "2025_07_27_1"
                var dateKey = Regex.Replace(nameNoExt, @"_(\d+)$", "");  // -> "2025_07_27"
                dateKeys.Add(dateKey);
            }

            foreach (var dateKey in dateKeys)
            {
                try
                {
                    var dayliMatches = new List<Models.Match>();

                    // 2) Učitaj sve partove 1..4 (postoji mogućnost da neki ne postoje)
                    for (int j = 1; j <= 4; j++)
                    {
                        var fileFullName = Path.Combine(matchPagesPath, $"{dateKey}_{j}.br");
                        if (!File.Exists(fileFullName))
                            continue;

                        var matchesFromTournament = await parser.ParseTournamentEventsFromArchive(fileFullName, dayliMatches);
                        if (matchesFromTournament != null)
                            dayliMatches.AddRange(matchesFromTournament);

                        File.Move(fileFullName, Path.Combine(outputFolder, Path.GetFileName(fileFullName)), true);
                    }

                    // 3) Filtriraj po datumu iz dateKey
                    if (!DateTime.TryParseExact(dateKey, "yyyy_MM_dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                    {
                        Console.WriteLine($"[!] Skipping {dateKey}: invalid date format.");
                        continue;
                    }

                    var beforeCount = dayliMatches.Count;
                    dayliMatches = dayliMatches
                        .Where(m => m.DateTime.HasValue && m.DateTime.Value.Date == parsedDate.Date)
                        .ToList();

                    // 4) Ukloni duplikate po MatchTPId (zadrži zadnji po vremenu ako želiš)
                    dayliMatches = dayliMatches
                        .Where(m => m.MatchTPId.HasValue)
                        .GroupBy(m => m.MatchTPId!.Value)
                        .Select(g => g
                            .OrderByDescending(x => x.DateTime ?? DateTime.MinValue)
                            .First())
                        .ToList();

                    Console.WriteLine($"📅 {dateKey}: loaded={beforeCount}, filtered={dayliMatches.Count}");
                }
                catch (Exception ex)
                {
                    string aaa;
                    aaa = "";
                }
            }
        }

        private static void WriteError(string title, Exception ex)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {title}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ForegroundColor = prev;
        }
    }

    /// <summary>
    /// First-chance hook za diagnostiku FormatException-a.
    /// Aktivira se u DEBUG buildu na ulazu u Main.
    /// </summary>
    internal static class FirstChanceHook
    {
        public static bool IsHooked { get; set; }

        public static void Handler(object? sender, FirstChanceExceptionEventArgs e)
        {
            // Filtriramo samo FormatException da konzola ne poludi
            if (e.Exception is FormatException fe)
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"[FirstChance] FormatException: {fe.Message}");
                Console.WriteLine(fe.StackTrace);
                Console.ForegroundColor = prev;
            }
        }
    }
}