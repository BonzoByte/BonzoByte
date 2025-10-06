using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Services
{
    /// <summary>
    /// Jednokratni batch runner za povijesne .br HTML arhive:
    /// Working -> (parse+insert+flag) -> Finished | Failed
    /// </summary>
    public sealed class HistoricalOddsBatchRunner
    {
        // Hardkodirano (premjesti u settings kad stigneš)
        private readonly string _workingDir = @"d:\brArchives\MatchOdds\Working";
        private readonly string _finishedDir;
        private readonly string _failedDir;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _maxDegree;

        public HistoricalOddsBatchRunner(
            IServiceScopeFactory scopeFactory,
            int? maxDegreeOfParallelism = null)
        {
            _scopeFactory = scopeFactory;
            _maxDegree = Math.Clamp(maxDegreeOfParallelism ?? Math.Min(Environment.ProcessorCount, 4), 1, 16);

            var root = Path.GetDirectoryName(_workingDir) ?? _workingDir;
            _finishedDir = Path.Combine(root, "Finished");
            _failedDir = Path.Combine(root, "Failed");

            Directory.CreateDirectory(_workingDir);
            Directory.CreateDirectory(_finishedDir);
            Directory.CreateDirectory(_failedDir);
        }

        public async Task RunOnceAsync(CancellationToken ct = default)
        {
            var files = Directory.EnumerateFiles(_workingDir, "*.br", SearchOption.TopDirectoryOnly).ToList();
            Console.WriteLine($"[HistOdds] Found {files.Count} file(s). Parallelism={_maxDegree}");

            await Parallel.ForEachAsync(
                files,
                new ParallelOptions { MaxDegreeOfParallelism = _maxDegree, CancellationToken = ct },
                async (file, token) =>
                {
                    try
                    {
                        int? matchId = TryExtractMatchId(file);
                        if (matchId is null)
                        {
                            Console.WriteLine($"[HistOdds] Skipping (no match id): {Path.GetFileName(file)}");
                            SafeMove(file, _failedDir);
                            return;
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var parser = scope.ServiceProvider.GetRequiredService<MatchOddsParserService>();
                        var flagger = scope.ServiceProvider.GetRequiredService<MatchOddsFlaggerService>();

                        // 1) Parse + insert (parser radi insert + eventualni cache update)
                        var inserted = await parser.ParseOddsFromArchiveAsync(
                            matchTPId: matchId.Value,
                            matchStartLocal: null,    // povijesno: ne prosjeđujemo start
                            isFinished: true,         // povijesne mečeve tretiramo kao završene
                            ct: token
                        );

                        // 2) Flagger (mask/likely switched/…)
                        await flagger.FlagAsync(matchId.Value, isFinished: true, matchStartLocal: null);

                        Console.WriteLine($"[HistOdds] OK {Path.GetFileName(file)} -> rows: {inserted.Count}");

                        // 3) Move -> Finished
                        SafeMove(file, _finishedDir);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"[HistOdds] Canceled: {Path.GetFileName(file)}");
                        // ostavi datoteku u Working
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[HistOdds] FAIL {Path.GetFileName(file)} -> {ex}");
                        SafeMove(file, _failedDir);
                    }
                });

            Console.WriteLine("[HistOdds] Done.");
        }

        private static int? TryExtractMatchId(string path)
        {
            // Uzmi najduži niz znamenki iz naziva fajla (npr. 826423744.br -> 826423744)
            var name = Path.GetFileNameWithoutExtension(path);
            var digits = Regex.Matches(name, @"\d+")
                              .Select(m => m.Value)
                              .OrderByDescending(s => s.Length)
                              .FirstOrDefault();
            if (digits != null && int.TryParse(digits, out var id)) return id;
            return null;
        }

        private static void SafeMove(string src, string destDir, int attempts = 3)
        {
            Directory.CreateDirectory(destDir);
            var fileName = Path.GetFileName(src);
            var dest = Path.Combine(destDir, fileName);

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    if (File.Exists(dest))
                    {
                        var stem = Path.GetFileNameWithoutExtension(fileName);
                        var ext = Path.GetExtension(fileName);
                        dest = Path.Combine(destDir, $"{stem}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}");
                    }
                    File.Move(src, dest);
                    return;
                }
                catch (IOException ex) when (i < attempts - 1)
                {
                    Console.WriteLine($"[HistOdds] Move retry ({i + 1}/{attempts}) for {fileName}: {ex.Message}");
                    Thread.Sleep(120); // kratki backoff
                }
            }

            // ako smo ovdje, pokušaj rename u mjestu
            try
            {
                var stem = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                var fallback = Path.Combine(Path.GetDirectoryName(src)!, $"{stem}_MOVEFAILED_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}");
                File.Move(src, fallback);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HistOdds] Move failed permanently for {fileName} -> {ex.Message}");
            }
        }
    }
}