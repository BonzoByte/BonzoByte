using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Persist;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace BonzoByte.Score
{
    public class ScoreRow
    {
        public long MatchId { get; set; }
        public int? LeftPlayerId { get; set; }
        public int? RightPlayerId { get; set; }
        public int Orientation { get; set; } = 0; // 0=kanonski red (Left=Player1), 1=druga orijentacija
        public string CvSplit { get; set; } = "prod";
        public bool? Y { get; set; }
        public double PRaw { get; set; }
        public double? PCal { get; set; }
        public string ModelVersion { get; set; } = "CORE40_v1";
        public DateTime ScoredAtUtc { get; set; }
    }

    public class Calibrator
    {
        public double A { get; set; }
        public double B { get; set; }
        public string Type { get; set; } = "Platt";
    }

    class Program
    {
        // ------------- KONFIG -------------
        static string ConnStr =
            "Data Source=Stanko;Initial Catalog=BonzoByte;Integrated Security=True;TrustServerCertificate=True;Connection Timeout=180";

        // koliko redova grupirati u jedan BulkCopy
        static int BatchSize = 100_000;

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // Arg0: split all|train|val|test|prod (default all)
            string splitArg = (args.Length > 0 ? args[0] : "all").Trim().ToLowerInvariant();
            if (splitArg is not ("all" or "train" or "val" or "test" or "prod"))
            {
                Console.WriteLine("Usage: BonzoByte.Score.exe [all|train|val|test|prod] [variant: all|k2|k5|router] [mode: prod|eval]");
                return;
            }

            // Arg1: variant all|k2|k5|router (default all)
            string variant = (args.Length > 1 ? args[1] : "all").Trim().ToLowerInvariant();
            if (variant is not ("all" or "k2" or "k5" or "router"))
            {
                Console.WriteLine("Variant must be one of: all|k2|k5|router");
                return;
            }

            // Arg2: mode prod|eval (default prod)
            string mode = (args.Length > 2 ? args[2] : "prod").Trim().ToLowerInvariant();
            bool evalMode = mode == "eval";
            bool routerMode = variant == "router";

            // odaberi izvorni view po splitu
            string sourceView = splitArg switch
            {
                "train" => "[ML].[vw_CORE40_train]",
                "val" => "[ML].[vw_CORE40_val]",
                "test" => "[ML].[vw_CORE40_test]",
                // "prod" nema poseban view — koristimo v1_split i filtriramo Orientation=0 (kanonski red)
                _ => "[ML].[vw_CORE40_v1_split]"
            };

            Console.WriteLine($"Split={splitArg} | Variant={variant} | Mode={mode}");
            Console.WriteLine($"Source view: {sourceView}");

            // --- UČITAJ MODELE I KALIBRATORE ---
            var nets = new Dictionary<string, BasicNetwork>(StringComparer.OrdinalIgnoreCase);
            var calibs = new Dictionary<string, Calibrator?>(StringComparer.OrdinalIgnoreCase);

            string[] toLoad = routerMode ? new[] { "all", "k2", "k5" } : new[] { variant };
            foreach (var v in toLoad)
            {
                string mp = $@"c:\trainedNN_core40_{v}.en";
                string fp = $@"c:\nn_features_core40_{v}.txt";
                string cp = $@"c:\nn_calibrator_core40_{v}.json";

                if (!File.Exists(mp)) throw new FileNotFoundException("Model not found", mp);
                if (!File.Exists(fp)) throw new FileNotFoundException("Features not found", fp);

                var net = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(mp));
                nets[v] = net;

                Calibrator? calib = null;
                if (File.Exists(cp))
                {
                    try
                    {
                        calib = JsonSerializer.Deserialize<Calibrator>(File.ReadAllText(cp));
                        Console.WriteLine($"Loaded calibrator for {v}: {cp} (A={calib!.A:F4}, B={calib.B:F4})");
                    }
                    catch
                    {
                        Console.WriteLine($"[WARN] Failed to parse calibrator {cp} for {v}. Using raw p.");
                    }
                }
                else
                {
                    Console.WriteLine($"[INFO] No calibrator file for {v}. Using raw p.");
                }
                calibs[v] = calib;

                Console.WriteLine($"Loaded model {v}: {mp} (inputs={net.Structure.Flat.InputCount})");
            }

            // Koristimo feature listu od ALL kao kanonsku (pretpostavka: isti redoslijed u svim treninzima)
            string featureListPath = File.Exists(@"c:\nn_features_core40_all.txt")
                                     ? @"c:\nn_features_core40_all.txt"
                                     : $@"c:\nn_features_core40_{toLoad[0]}.txt";

            var featureNames = File.ReadAllLines(featureListPath)
                                   .Select(s => s.Trim())
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .ToList();

            // Ako je 'Orientation' završio u feature listi slučajno, ukloni ga ako treba da bi se count poklopio s modelom
            bool removedOrientation = false;
            foreach (var kv in nets)
            {
                int expected = kv.Value.Structure.Flat.InputCount;
                if (featureNames.Count != expected &&
                    featureNames.Any(f => f.Equals("Orientation", StringComparison.OrdinalIgnoreCase)) &&
                    featureNames.Count - 1 == expected)
                {
                    featureNames = featureNames.Where(f => !f.Equals("Orientation", StringComparison.OrdinalIgnoreCase)).ToList();
                    removedOrientation = true;
                }
                if (featureNames.Count != expected)
                    throw new Exception($"Feature count mismatch for model '{kv.Key}': model expects {expected}, feature list has {featureNames.Count}");
            }
            if (removedOrientation) Console.WriteLine("[INFO] Removed 'Orientation' from feature list to match model inputs.");

            Console.WriteLine($"Features: {featureNames.Count}  (file: {featureListPath})");

            // --- Pre-clean ---
            if (routerMode)
            {
                // u router modu brišemo sve tri varijante za zadani split
                PreCleanScoresMulti(splitArg, new[] { "CORE40_v1_all", "CORE40_v1_k2", "CORE40_v1_k5" }, evalMode);
            }
            else
            {
                string modelVersionSingle = $"CORE40_v1_{variant}" + (evalMode ? "_eval" : "");
                PreCleanScores(splitArg, modelVersionSingle);
            }

            // --- SELECT s pre-match iskustvom igrača ---
            // Uvijek joinamo dbo.Match da dobijemo p1/p2 pre-match brojeve (router ih koristi; ostali ih ignoriraju)
            string selectSql = evalMode
                ? $@"
SELECT v.*, 
       (ISNULL(m.Player1WinsTotal,0)+ISNULL(m.Player1LossesTotal,0)) AS p1_matches,
       (ISNULL(m.Player2WinsTotal,0)+ISNULL(m.Player2LossesTotal,0)) AS p2_matches
FROM {sourceView} v
JOIN dbo.Match m ON m.MatchTPId = v.MatchTPId;"
                : $@"
SELECT v.*, 
       (ISNULL(m.Player1WinsTotal,0)+ISNULL(m.Player1LossesTotal,0)) AS p1_matches,
       (ISNULL(m.Player2WinsTotal,0)+ISNULL(m.Player2LossesTotal,0)) AS p2_matches
FROM {sourceView} v
JOIN dbo.Match m ON m.MatchTPId = v.MatchTPId
WHERE v.Orientation = 0;"; // prod: kanonski red (Left=Player1)

            Console.WriteLine("Scoring started...");

            long totalRead = 0, totalInserted = 0;
            var batch = new List<ScoreRow>(BatchSize);

            using var conRead = new SqlConnection(ConnStr);
            conRead.Open();

            using var cmd = new SqlCommand(selectSql, conRead) { CommandTimeout = 0 };
            using var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult);

            // mapiraj ordinale
            var ord = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var schema = rdr.GetSchemaTable();
            var cols = schema!.Rows.Cast<DataRow>().Select(r => (string)r["ColumnName"]).ToList();
            foreach (var c in cols) ord[c] = rdr.GetOrdinal(c);

            // provjera da sve feature kolone postoje
            var missing = featureNames.Where(f => !ord.ContainsKey(f)).ToList();
            if (missing.Count > 0)
                throw new Exception("Missing feature columns in view: " + string.Join(", ", missing));

            // helperi
            static double ReadDouble(IDataRecord r, int i)
                => (i < 0 || r.IsDBNull(i)) ? 0.0 : Convert.ToDouble(r.GetValue(i), CultureInfo.InvariantCulture);
            static int ReadInt(IDataRecord r, int i)
                => (i < 0 || r.IsDBNull(i)) ? 0 : Convert.ToInt32(r.GetValue(i), CultureInfo.InvariantCulture);

            // kandidati za ID i meta imena (što god postoji u viewu)
            string matchIdCol = new[] { "MatchTPId", "MatchId", "match_id" }.FirstOrDefault(ord.ContainsKey)
                                ?? throw new Exception("Match Id column not found (MatchTPId/MatchId/match_id).");
            string yCol = new[] { "y", "Y" }.FirstOrDefault(ord.ContainsKey);      // opcionalno
            string cvCol = new[] { "cv_split" }.FirstOrDefault(ord.ContainsKey);   // opcionalno
            string orientationCol = ord.ContainsKey("Orientation") ? "Orientation" : null;
            int iP1M = ord.ContainsKey("p1_matches") ? ord["p1_matches"] : -1;
            int iP2M = ord.ContainsKey("p2_matches") ? ord["p2_matches"] : -1;

            while (rdr.Read())
            {
                totalRead++;

                long matchId = Convert.ToInt64(rdr.GetValue(ord[matchIdCol]), CultureInfo.InvariantCulture);

                int orientation = (orientationCol != null && !rdr.IsDBNull(ord[orientationCol]))
                    ? Convert.ToInt32(rdr.GetValue(ord[orientationCol]))
                    : 0;

                bool? y = (yCol != null && !rdr.IsDBNull(ord[yCol]))
                            ? Convert.ToInt32(rdr.GetValue(ord[yCol])) != 0
                            : (bool?)null;

                string cv = (cvCol != null && !rdr.IsDBNull(ord[cvCol]))
                                ? Convert.ToString(rdr.GetValue(ord[cvCol]))!
                                : splitArg; // ako nema kolone, koristi traženi split

                // pripremi NN ulaz
                var x = new double[featureNames.Count];
                for (int i = 0; i < featureNames.Count; i++)
                    x[i] = ReadDouble(rdr, ord[featureNames[i]]);

                // --- Router odluka ---
                string chosen = variant;
                if (routerMode)
                {
                    int p1m = ReadInt(rdr, iP1M);
                    int p2m = ReadInt(rdr, iP2M);
                    chosen = (p1m >= 5 && p2m >= 5) ? "k5"
                           : (p1m >= 2 && p2m >= 2) ? "k2"
                           : "all";
                }

                var netChosen = nets[chosen];
                var calibChosen = calibs[chosen];

                // predikcija
                double p = netChosen.Compute(new BasicMLData(x))[0];
                p = Math.Min(1 - 1e-9, Math.Max(1e-9, p)); // clamp

                double pCal = p;
                if (calibChosen != null)
                {
                    double pr = Math.Clamp(p, 1e-12, 1 - 1e-12);
                    double logit = Math.Log(pr / (1 - pr));
                    double z = calibChosen.A + calibChosen.B * logit;
                    pCal = 1.0 / (1.0 + Math.Exp(-z));
                }

                // ModelVersion zapisujemo po stvarno korištenom modelu
                string modelVersion = $"CORE40_v1_{chosen}" + (evalMode ? "_eval" : "");

                batch.Add(new ScoreRow
                {
                    MatchId = matchId,
                    LeftPlayerId = null,   // popunit ćemo UPDATE-om iz dbo.Match
                    RightPlayerId = null,
                    Orientation = orientation,
                    CvSplit = cv,
                    Y = y,
                    PRaw = p,
                    PCal = pCal,
                    ModelVersion = modelVersion,
                    ScoredAtUtc = DateTime.UtcNow
                });

                if (batch.Count >= BatchSize)
                {
                    BulkInsertScores(batch);
                    totalInserted += batch.Count;
                    Console.WriteLine($"Inserted {totalInserted:N0} / Read {totalRead:N0}");
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                BulkInsertScores(batch);
                totalInserted += batch.Count;
                batch.Clear();
            }

            Console.WriteLine($"DONE. Read={totalRead:N0}, Inserted={totalInserted:N0}");

            // dopuni Player1Id/Player2Id iz dbo.Match
            // (ograničimo na ovaj split; modelVersion je raznolik u routeru, pa ne filtriramo po njemu)
            FillPlayersFromMatch(splitArg);
        }

        // Briše samo jedan modelVersion za zadani split
        static void PreCleanScores(string splitArg, string modelVersion)
        {
            using var con = new SqlConnection(ConnStr);
            con.Open();

            string sql = (splitArg == "all")
                ? @"DELETE FROM ML.Core40_Scores WHERE ModelVersion=@ver AND cv_split IN ('train','val','test','prod')"
                : @"DELETE FROM ML.Core40_Scores WHERE ModelVersion=@ver AND cv_split=@cv";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@ver", modelVersion);
            if (splitArg != "all") cmd.Parameters.AddWithValue("@cv", splitArg);
            cmd.CommandTimeout = 0;
            int n = cmd.ExecuteNonQuery();
            Console.WriteLine($"PreClean => deleted {n} rows for version {modelVersion}, split={splitArg}");
        }

        // Router pre-clean: briše više verzija za zadani split
        static void PreCleanScoresMulti(string splitArg, IEnumerable<string> modelVersions, bool evalMode)
        {
            using var con = new SqlConnection(ConnStr);
            con.Open();

            string suffix = evalMode ? "_eval" : "";
            var versions = modelVersions.Select(v => v + suffix).ToArray();

            string sql = (splitArg == "all")
                ? $@"DELETE FROM ML.Core40_Scores WHERE ModelVersion IN ({string.Join(",", versions.Select((_, i) => "@v" + i))})
                     AND cv_split IN ('train','val','test','prod')"
                : $@"DELETE FROM ML.Core40_Scores WHERE ModelVersion IN ({string.Join(",", versions.Select((_, i) => "@v" + i))})
                     AND cv_split=@cv";

            using var cmd = new SqlCommand(sql, con);
            for (int i = 0; i < versions.Length; i++)
                cmd.Parameters.AddWithValue("@v" + i, versions[i]);
            if (splitArg != "all") cmd.Parameters.AddWithValue("@cv", splitArg);
            cmd.CommandTimeout = 0;

            int n = cmd.ExecuteNonQuery();
            Console.WriteLine($"PreClean (router) => deleted {n} rows for versions [{string.Join(", ", versions)}], split={splitArg}");
        }

        static void BulkInsertScores(List<ScoreRow> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("MatchId", typeof(long));
            dt.Columns.Add("LeftPlayerId", typeof(int));
            dt.Columns.Add("RightPlayerId", typeof(int));
            dt.Columns.Add("Orientation", typeof(int));
            dt.Columns.Add("cv_split", typeof(string));
            dt.Columns.Add("y", typeof(bool));
            dt.Columns.Add("P_Raw", typeof(decimal));
            dt.Columns.Add("P_Cal", typeof(decimal));
            dt.Columns.Add("ModelVersion", typeof(string));
            dt.Columns.Add("ScoredAtUtc", typeof(DateTime));

            foreach (var r in rows)
            {
                var row = dt.NewRow();
                row["MatchId"] = r.MatchId;
                row["LeftPlayerId"] = (object?)r.LeftPlayerId ?? DBNull.Value;
                row["RightPlayerId"] = (object?)r.RightPlayerId ?? DBNull.Value;
                row["Orientation"] = r.Orientation;
                row["cv_split"] = r.CvSplit;
                row["y"] = (object?)r.Y ?? DBNull.Value;
                row["P_Raw"] = (decimal)r.PRaw;
                row["P_Cal"] = r.PCal.HasValue ? (object)(decimal)r.PCal.Value : DBNull.Value;
                row["ModelVersion"] = r.ModelVersion;
                row["ScoredAtUtc"] = r.ScoredAtUtc;
                dt.Rows.Add(row);
            }

            using var con = new SqlConnection(ConnStr);
            con.Open();

            using var bulk = new SqlBulkCopy(con)
            {
                DestinationTableName = "ML.Core40_Scores",
                BatchSize = Math.Max(1, rows.Count)
            };

            bulk.ColumnMappings.Add("MatchId", "MatchId");
            bulk.ColumnMappings.Add("LeftPlayerId", "LeftPlayerId");
            bulk.ColumnMappings.Add("RightPlayerId", "RightPlayerId");
            bulk.ColumnMappings.Add("Orientation", "Orientation");
            bulk.ColumnMappings.Add("cv_split", "cv_split");
            bulk.ColumnMappings.Add("y", "y");
            bulk.ColumnMappings.Add("P_Raw", "P_Raw");
            bulk.ColumnMappings.Add("P_Cal", "P_Cal");
            bulk.ColumnMappings.Add("ModelVersion", "ModelVersion");
            bulk.ColumnMappings.Add("ScoredAtUtc", "ScoredAtUtc");

            bulk.WriteToServer(dt);
        }

        // Nakon inserta dopuni Player1TPId/Player2TPId
        static void FillPlayersFromMatch(string splitArg)
        {
            using var con = new SqlConnection(ConnStr);
            con.Open();

            string sql = @"
                UPDATE s
                SET s.LeftPlayerId  = m.Player1TPId,
                    s.RightPlayerId = m.Player2TPId
                FROM ML.Core40_Scores s
                JOIN dbo.Match m
                  ON m.MatchTPId = s.MatchId
                WHERE (s.LeftPlayerId IS NULL OR s.RightPlayerId IS NULL)
                  AND (@cv = 'all' OR s.cv_split = @cv);";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@cv", splitArg);
            cmd.CommandTimeout = 0;

            int n = cmd.ExecuteNonQuery();
            Console.WriteLine($"Filled Left/Right from dbo.Match: {n} rows");
        }
    }
}