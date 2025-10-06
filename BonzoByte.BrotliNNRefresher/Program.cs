using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BonzoByte.BrotliNNRefresher
{
    internal class Program
    {
        // ====== HARD-CODED SETTINGS ======
        private const string SqlConn =
            "Data Source=Stanko;Initial Catalog=BonzoByte;Integrated Security=True;Connection Timeout=180000000;TrustServerCertificate=True";

        // Root folderi (obrađujemo oba)
        private static readonly string MatchDetailsRoot = @"d:\BrotliArchives\MatchDetails";
        private static readonly string DailyMatchesRoot = @"d:\BrotliArchives\DayliMatches";

        // Paralelizacija (po potrebi promijeni)
        private const int MaxThreads = 8;

        static int Main(string[] args)
        {
            try
            {
                Log.Info("Loading NN map from SQL...");
                var map = MapLoader.FromSql(SqlConn);                  // Map: MatchTPId -> Pw01 ([0..1])
                Log.Info($"Map loaded: {map.Count:N0} entries.");

                var files = new List<string>(capacity: 8192);
                if (Directory.Exists(MatchDetailsRoot))
                    files.AddRange(Directory.EnumerateFiles(MatchDetailsRoot, "*.br", SearchOption.AllDirectories));
                if (Directory.Exists(DailyMatchesRoot))
                    files.AddRange(Directory.EnumerateFiles(DailyMatchesRoot, "*.br", SearchOption.AllDirectories));

                Log.Info($"Found .br files: {files.Count:N0}");

                int ok = 0, skip = 0, fail = 0, updatedToNull = 0, updatedToValue = 0;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                Parallel.ForEach(
                    files,
                    new ParallelOptions { MaxDegreeOfParallelism = MaxThreads },
                    path =>
                    {
                        try
                        {
                            string json = BrotliUtil.ReadAsString(path);
                            JsonNode? root = JsonNode.Parse(json);
                            if (root is null)
                            {
                                Interlocked.Increment(ref fail);
                                Log.Err($"Invalid JSON root: {path}");
                                return;
                            }

                            bool changed = false;
                            bool anySetToNull = false;
                            bool anySetToValue = false;

                            if (root is JsonObject obj) // MatchDetails: jedan meč po fajlu
                            {
                                if (!IdParse.TryFromFilename(path, out long id))
                                {
                                    Interlocked.Increment(ref fail);
                                    Log.Err($"No id in filename: {path}");
                                    return;
                                }

                                if (map.TryGetValue(id, out var rec))
                                {
                                    // postavi stvarne vrijednosti
                                    if (JsonPatch.ApplyWpAndBet(obj, rec)) { changed = true; anySetToValue = true; }
                                }
                                else
                                {
                                    // NEMAMO NN → oba WP polja na null, who2Bet=0
                                    if (JsonPatch.NullWpAndBet(obj)) { changed = true; anySetToNull = true; }
                                }
                            }
                            else if (root is JsonArray arr) // DailyMatches: više mečeva u nizu
                            {
                                int localChanges = 0, localNulls = 0, localValues = 0;

                                foreach (var item in arr)
                                {
                                    if (item is not JsonObject o) continue;
                                    var id = ReadMatchId(o);
                                    if (id is null) continue; // bez ID-a ne diramo

                                    if (map.TryGetValue(id.Value, out var rec))
                                    {
                                        if (JsonPatch.ApplyWpAndBet(o, rec)) { localChanges++; localValues++; }
                                    }
                                    else
                                    {
                                        if (JsonPatch.NullWpAndBet(o)) { localChanges++; localNulls++; }
                                    }
                                }

                                if (localChanges > 0)
                                {
                                    changed = true;
                                    if (localNulls > 0) anySetToNull = true;
                                    if (localValues > 0) anySetToValue = true;
                                }
                            }
                            else
                            {
                                Interlocked.Increment(ref fail);
                                Log.Err($"Unsupported JSON root (not object/array): {path}");
                                return;
                            }

                            if (!changed)
                            {
                                Interlocked.Increment(ref skip);
                                return;
                            }

                            // Zapiši natrag
                            string updated = root.ToJsonString(JsonPatch.Jopts);
                            BrotliUtil.WriteString(path, updated);
                            Interlocked.Increment(ref ok);
                            if (anySetToNull) Interlocked.Increment(ref updatedToNull);
                            if (anySetToValue) Interlocked.Increment(ref updatedToValue);
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Increment(ref fail);
                            Log.Err($"{path}: {ex.Message}");
                        }
                    });

                sw.Stop();
                Log.Info($"DONE in {sw.Elapsed}. ok={ok:N0} skip={skip:N0} fail={fail:N0} | setToValue={updatedToValue:N0} setToNull={updatedToNull:N0}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        private static long? ReadMatchId(JsonObject o)
        {
            try
            {
                var idNode = o["matchTPId"];
                if (idNode is JsonValue jv)
                {
                    if (jv.TryGetValue<long>(out var lid)) return lid;
                    if (jv.TryGetValue<double>(out var d)) return checked((long)d);
                    if (jv.TryGetValue<string>(out var s) &&
                        long.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var l2))
                        return l2;
                }
                return null;
            }
            catch { return null; }
        }
    }

    internal record NnRecord(float Pw01); // Pw u [0..1]

    internal static class MapLoader
    {
        // Robustan loader: radi bez obzira je li MatchTPId INT/BIGINT i koji je tip WinProbabilityNN
        public static Dictionary<long, NnRecord> FromSql(string connStr)
        {
            var map = new Dictionary<long, NnRecord>(capacity: 2_200_000);
            using var conn = new SqlConnection(connStr);
            conn.Open();

            const string sql = @"
                SELECT m.MatchTPId, m.WinProbabilityNN
                FROM dbo.[Match] m
                WHERE m.IsFinished = 1 AND m.WinProbabilityNN IS NOT NULL;";

            using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 0 };
            using var rdr = cmd.ExecuteReader(System.Data.CommandBehavior.SequentialAccess);

            while (rdr.Read())
            {
                object idObj = rdr.GetValue(0);
                if (idObj == DBNull.Value) continue;

                long id = idObj switch
                {
                    long l => l,
                    int i => i,
                    short s => s,
                    decimal d => (long)d,
                    double d => (long)d,
                    float f => (long)f,
                    string s => long.Parse(s, CultureInfo.InvariantCulture),
                    _ => Convert.ToInt64(idObj, CultureInfo.InvariantCulture)
                };

                object pwObj = rdr.GetValue(1);
                if (pwObj == DBNull.Value) continue;

                double pwDouble = pwObj switch
                {
                    double d => d,
                    float f => f,
                    decimal d => (double)d,
                    string s => double.Parse(s, CultureInfo.InvariantCulture),
                    _ => Convert.ToDouble(pwObj, CultureInfo.InvariantCulture)
                };

                float pw = (float)Math.Clamp(pwDouble, 0.0, 1.0);
                map[id] = new NnRecord(pw);
            }
            return map;
        }
    }

    internal static class JsonPatch
    {
        internal static readonly JsonSerializerOptions Jopts = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Ako imamo NN → upiši vrijednosti; inače → null
        public static bool ApplyWpAndBet(JsonObject obj, NnRecord rec)
        {
            // target numbers
            double p1 = Math.Round(rec.Pw01 * 100.0, 2, MidpointRounding.AwayFromZero);
            double p2 = Math.Round(100.0 - p1, 2, MidpointRounding.AwayFromZero);

            bool changed = false;
            changed |= SetNumberOrNull(obj, "winProbabilityPlayer1NN", p1);
            changed |= SetNumberOrNull(obj, "winProbabilityPlayer2NN", p2);
            changed |= SetNumberOrNull(obj, "who2Bet", 0); // uvijek 0 za sada

            return changed;
        }

        public static bool NullWpAndBet(JsonObject obj)
        {
            bool changed = false;
            changed |= SetNumberOrNull(obj, "winProbabilityPlayer1NN", null);
            changed |= SetNumberOrNull(obj, "winProbabilityPlayer2NN", null);
            changed |= SetNumberOrNull(obj, "who2Bet", 0); // i dalje 0
            return changed;
        }

        // Postavi numeric value ili null (eksplicitno "key": null u JSON-u), vrati true ako je bilo promjene
        private static bool SetNumberOrNull(JsonObject obj, string key, double? value)
        {
            obj.TryGetPropertyValue(key, out JsonNode? oldNode);

            if (value is null)
            {
                // Ako je već literalni null, ništa ne mijenjamo
                if (oldNode is JsonValue jv && jv.ToJsonString() == "null")
                    return false;

                // U svim ostalim slučajevima eksplicitno postavi na null
                obj[key] = JsonValue.Create((double?)null);
                return true;
            }
            else
            {
                // Postavljamo broj; ako već postoji isti broj, preskoči
                double? oldNum = TryReadNumber(oldNode);
                if (oldNum is double on && NearlyEqual(on, value.Value))
                    return false;

                obj[key] = value.Value;
                return true;
            }
        }

        private static double? TryReadNumber(JsonNode? node)
        {
            try
            {
                if (node is null) return null;
                if (node is JsonValue jv)
                {
                    if (jv.TryGetValue<double>(out var d)) return d;
                    if (jv.TryGetValue<string>(out var s) &&
                        double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                        return v;
                }
                return null;
            }
            catch { return null; }
        }

        private static bool NearlyEqual(double a, double b, double eps = 1e-9)
            => Math.Abs(a - b) <= eps;
    }

    internal static class BrotliUtil
    {
        public static string ReadAsString(string path)
        {
            using var fs = File.OpenRead(path);
            using var bs = new BrotliStream(fs, CompressionMode.Decompress, leaveOpen: false);
            using var ms = new MemoryStream();
            bs.CopyTo(ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static void WriteString(string path, string content)
        {
            string tmp = path + ".tmp";
            using (var fs = File.Create(tmp))
            using (var bs = new BrotliStream(fs, CompressionMode.Compress, leaveOpen: false))
            using (var sw = new StreamWriter(bs, new UTF8Encoding(false)))
                sw.Write(content);

            if (File.Exists(path)) File.Delete(path);
            File.Move(tmp, path);
        }
    }

    internal static class IdParse
    {
        private static readonly Regex Digits = new("(\\d+)", RegexOptions.Compiled);

        public static bool TryFromFilename(string path, out long id)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var m = Digits.Match(name);
            if (m.Success && long.TryParse(m.Groups[1].Value, out id)) return true;
            id = 0;
            return false;
        }
    }

    internal static class Log
    {
        public static void Info(string msg) =>
            Console.WriteLine(DateTime.UtcNow.ToString("O") + "\tINFO\t" + msg);

        public static void Err(string msg) =>
            Console.Error.WriteLine(DateTime.UtcNow.ToString("O") + "\tERR \t" + msg);
    }
}
