// BonzoByte.BatchScore – batch scoring konzola
// Requires: reference na BonzoByte.ML (NNRouterScorer/Core40FeatureExtractor/FeatureVector)

using BonzoByte.ML;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace BonzoByte.BatchScore
{
    internal class Program
    {
        // === CONFIG ===
        private static readonly string ConnectionString =
            "Data Source=Stanko;Initial Catalog=BonzoByte;Integrated Security=True;Connection Timeout=180000000;TrustServerCertificate=True";

        // gdje su modeli (NNRouterScorer ga već očekuje u c:\bb_nn_out\...)
        // private static readonly string InputDir = @"c:\bb_nn_out\"; // (samo informativno)

        // defaultni parametri (možeš overrideati kroz CLI)
        private const string DefaultMode = "all"; // all | null | date
        private const int DefaultMax = 0;     // 0 = bez limita
        private const int DefaultDop = 6;     // paralelni stupanj (pazi na DB i CPU)
        private static readonly string? DefaultSinceIsoDate = null; // npr. "2024-01-01"

        // Usage:
        //   BonzoByte.BatchScore.exe [--mode all|null|date] [--since YYYY-MM-DD] [--max N] [--dop K]
        // Primjeri:
        //   BonzoByte.BatchScore.exe --mode null --dop 8
        //   BonzoByte.BatchScore.exe --mode date --since 2024-01-01 --max 200000 --dop 6

        private static async Task<int> Main(string[] args)
        {
            var (mode, sinceDate, maxRows, dop) = ParseArgs(args);

            Console.WriteLine($"[BatchScore] mode={mode} since={(sinceDate.HasValue ? sinceDate.Value.ToString("yyyy-MM-dd") : "-")} max={(maxRows <= 0 ? "∞" : maxRows)} dop={dop}");

            // graceful cancel (Ctrl+C)
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

            // 1) Učitaj listu mečeva za scoring
            var ids = await GetMatchIdsAsync(ConnectionString, mode, sinceDate, maxRows, cts.Token);
            Console.WriteLine($"[BatchScore] Total matches to score: {ids.Count:N0}");

            if (ids.Count == 0) return 0;

            // 2) Router (učitava modele/kalibratore) – koristi c:\bb_nn_out\...
            var router = new NNRouterScorer(ConnectionString);

            // 3) Paralelno scoranje
            var sw = Stopwatch.StartNew();
            int ok = 0, fail = 0;
            long lastLogged = 0;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = dop,
                CancellationToken = cts.Token
            };

            try
            {
                await Parallel.ForEachAsync(ids, options, async (matchId, ct) =>
                {
                    try
                    {
                        await RetryAsync(async () =>
                        {
                            await router.ScoreAndUpdateAsync(matchId, ct);
                        }, maxAttempts: 3, initialDelayMs: 200, ct);

                        Interlocked.Increment(ref ok);
                    }
                    catch (OperationCanceledException) { /* bubbling up */ }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref fail);
                        if (fail <= 20) // ne spamaj konzolu
                            Console.WriteLine($"[WARN] MatchTPId={matchId} failed: {ex.Message}");
                    }

                    // simple progress throttle
                    var done = ok + fail;
                    if (done % 5000 == 0 && done != Interlocked.Read(ref lastLogged))
                    {
                        Interlocked.Exchange(ref lastLogged, done);
                        Console.WriteLine($"[BatchScore] {done:N0}/{ids.Count:N0} ({(100.0 * done / Math.Max(1, ids.Count)):F1}%)  ok={ok:N0}  fail={fail:N0}  eta~{Eta(sw.Elapsed, done, ids.Count)}");
                    }
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[BatchScore] Cancelled by user.");
            }

            sw.Stop();
            Console.WriteLine($"[BatchScore] DONE in {sw.Elapsed}. OK={ok:N0}  FAIL={fail:N0}");
            return 0;
        }

        // === SQL selection ===
        private static async Task<List<int>> GetMatchIdsAsync(string connStr, string mode, DateTime? since, int max, CancellationToken ct)
        {
            using var con = new SqlConnection(connStr);
            await con.OpenAsync(ct);

            string sql;
            var cmd = new SqlCommand { Connection = con, CommandTimeout = 0 };

            // Napomena: “IsFinished=1” – ako želiš score-at i buduće mečeve, ukloni taj filter.
            if (mode.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                sql = @"SELECT {0} MatchTPId
                        FROM dbo.Match WITH (NOLOCK)
                        WHERE IsFinished = 1 AND WinProbabilityNN IS NULL
                        ORDER BY MatchTPId;";
            }
            else if (mode.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                sql = @"SELECT {0} MatchTPId
                        FROM dbo.Match WITH (NOLOCK)
                        WHERE IsFinished = 1 AND DateTime >= @since
                        ORDER BY MatchTPId;";
                cmd.Parameters.Add("@since", SqlDbType.DateTime2).Value = since ?? DateTime.UtcNow.Date;
            }
            else // "all"
            {
                sql = @"SELECT {0} MatchTPId
                        FROM dbo.Match WITH (NOLOCK)
                        WHERE IsFinished = 1
                        ORDER BY MatchTPId;";
            }

            string top = (max > 0) ? $"TOP ({max})" : "";
            cmd.CommandText = string.Format(sql, top);

            var ids = new List<int>(capacity: Math.Max(1024, max > 0 ? max : 1024));
            using (var rd = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, ct))
            {
                while (await rd.ReadAsync(ct))
                {
                    ids.Add(rd.GetInt32(0));
                }
            }
            return ids;
        }

        // === Small retry helper (za transient DB/IO) ===
        private static async Task RetryAsync(Func<Task> operation, int maxAttempts, int initialDelayMs, CancellationToken ct)
        {
            int attempt = 0;
            int delay = initialDelayMs;

            while (true)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    await operation();
                    return;
                }
                catch when (++attempt < maxAttempts)
                {
                    await Task.Delay(delay, ct);
                    delay = Math.Min(delay * 2, 5000);
                }
            }
        }

        private static Task TaskDelay(int ms, CancellationToken ct) => Task.Delay(ms, ct);

        // === ETA utility ===
        private static string Eta(TimeSpan elapsed, int done, int total)
        {
            if (done == 0) return "n/a";
            double rate = done / Math.Max(0.001, elapsed.TotalSeconds);
            double remain = (total - done) / Math.Max(0.001, rate);
            return TimeSpan.FromSeconds(remain).ToString(@"hh\:mm\:ss");
        }

        // === Argument parsing ===
        private static (string mode, DateTime? since, int max, int dop) ParseArgs(string[] args)
        {
            string mode = DefaultMode;
            int max = DefaultMax;
            int dop = DefaultDop;
            DateTime? since = null;

            if (!string.IsNullOrEmpty(DefaultSinceIsoDate) && DateTime.TryParse(DefaultSinceIsoDate, out var d0))
                since = d0;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--mode":
                        if (i + 1 < args.Length) mode = args[++i];
                        break;
                    case "--since":
                        if (i + 1 < args.Length && DateTime.TryParse(args[++i], out var d)) since = d;
                        break;
                    case "--max":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out var m)) max = m;
                        break;
                    case "--dop":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out var k)) dop = Math.Max(1, k);
                        break;
                }
            }
            // sanity
            if (!(mode.Equals("all", StringComparison.OrdinalIgnoreCase)
               || mode.Equals("null", StringComparison.OrdinalIgnoreCase)
               || mode.Equals("date", StringComparison.OrdinalIgnoreCase)))
            {
                mode = DefaultMode;
            }
            if (!mode.Equals("date", StringComparison.OrdinalIgnoreCase)) since = null;
            return (mode, since, max, dop);
        }
    }
}