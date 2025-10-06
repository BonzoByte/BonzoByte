using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Persist;

using Force.DeepCloner;

namespace BonzoByte
{
    public class MatchData
    {
        public long MatchTPId { get; set; }
        public double[] Inputs { get; set; } = Array.Empty<double>(); // dopuštamo NaN kod load-a
        public double Label { get; set; }    // y (0/1)
    }

    public class Calibrator
    {
        public double A { get; set; }
        public double B { get; set; }
        public string Type { get; set; } = "Platt(logit->sigmoid)";
    }

    class Program
    {
        // === KONFIG ===
        static string ConnStr =
            "Data Source=Stanko;Initial Catalog=BonzoByte;Integrated Security=True;Connection Timeout=180000000;TrustServerCertificate=True";

        // VARIJANTE: R0/R1 × {ALL, CLAY, HARD, GRASS}
        // r0_all JE IZVRŠEN — ostavljen zakomentiran (odkomentiraj po potrebi).
        static readonly Dictionary<string, (string Train, string Val, string Test)> VariantViews =
            new(StringComparer.OrdinalIgnoreCase)
            {
                //["r0_all"]   = ("ML.vw_NN_R0_ALL_train",   "ML.vw_NN_R0_ALL_val",   "ML.vw_NN_R0_ALL_test"),
                ["r1_all"] = ("ML.vw_NN_R1_ALL_train", "ML.vw_NN_R1_ALL_val", "ML.vw_NN_R1_ALL_test"),
                ["r0_clay"] = ("ML.vw_NN_R0_CLAY_train", "ML.vw_NN_R0_CLAY_val", "ML.vw_NN_R0_CLAY_test"),
                ["r1_clay"] = ("ML.vw_NN_R1_CLAY_train", "ML.vw_NN_R1_CLAY_val", "ML.vw_NN_R1_CLAY_test"),
                ["r0_hard"] = ("ML.vw_NN_R0_HARD_train", "ML.vw_NN_R0_HARD_val", "ML.vw_NN_R0_HARD_test"),
                ["r1_hard"] = ("ML.vw_NN_R1_HARD_train", "ML.vw_NN_R1_HARD_val", "ML.vw_NN_R1_HARD_test"),
                ["r0_grass"] = ("ML.vw_NN_R0_GRASS_train", "ML.vw_NN_R0_GRASS_val", "ML.vw_NN_R0_GRASS_test"),
                ["r1_grass"] = ("ML.vw_NN_R1_GRASS_train", "ML.vw_NN_R1_GRASS_val", "ML.vw_NN_R1_GRASS_test"),
            };

        // Fiksni whitelist i redoslijed (40) — slim set po našoj SHAP selekciji
        static readonly string[] FeatureWhitelist = new[]
        {
            "WP_avg_fav","z_rel","WP_range","DaysSinceLossDiff","AgeDiff",
            "GameWinRateEffSurfDiff","WP_avg_entropy","WinRateTotalSmoothedDiff","WP_std","EffSurfFocusFavDiff",
            "z_avg_fav","TSScale_avg","EffSurfMatchesLogFavDiff","DaysSinceWinDiff","FavSurfaceUplift",
            "WinRateEffSurfDiff","dZ_H2HM_M_abs","PrizeLog","RestDaysFavMinusOpp","OppSurfaceUplift",
            "SetWinRateEffSurfDiff","RecencySlope_MminusY","EffSurfWeight","FormEffSurfCompositeDiff","dWP_H2HM_M_abs",
            "z_std_fav","WinRateEffSurfSmoothedDiff","FormCompositeDiff","SurfaceUpliftDiff","SeasonMonth_sin",
            "RecencySlopeEff_MminusY","H2H_conflict_weighted","AgeDiff_sq","SeasonMonth_cos","IsSurf4",
            "H2HWinRateSmoothedDiff","RecencySlope_WminusM","StreakDiff","IsSurf2","RecencySlopeEff_WminusM"
        };

        // Heavy-tail za winsor (clip 1.–99. percentil po trainu)
        static readonly HashSet<string> WinsorCols = new(StringComparer.OrdinalIgnoreCase)
        {
            "RestDaysFavMinusOpp","DaysSinceWinDiff","DaysSinceLossDiff","StreakDiff"
        };

        // Kamo spremamo modele/feature/kalibrator/metricse
        static readonly string SaveDir = @"c:\bb_nn_out";

        static void Main(string[] args)
        {
            Directory.CreateDirectory(SaveDir);

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // Default: sve osim r0_all (jer je odrađen). Proslijediš argse po želji.
            var defaultVariants = new[] { "r1_all", "r0_clay", "r1_clay", "r0_hard", "r1_hard", "r0_grass", "r1_grass" };
            var variants = (args.Length == 0)
                ? defaultVariants
                : args.Select(a => a.Trim().ToLowerInvariant()).ToArray();

            foreach (var variant in variants)
            {
                if (!VariantViews.TryGetValue(variant, out var v))
                {
                    Console.WriteLine($"Nepoznata varijanta: {variant}. Dozvoljeno: {string.Join(",", VariantViews.Keys)}");
                    continue;
                }

                try
                {
                    TrainAndSave(variant, v.Train, v.Val, v.Test);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{variant}] ERROR: {ex.Message}");
                }

                Console.WriteLine(new string('-', 80));
            }

            Console.WriteLine("Gotovo.");
        }

        static void TrainAndSave(string variantName, string tblTrain, string tblVal, string tblTest)
        {
            Console.WriteLine($">>> VARIJANTA: {variantName.ToUpperInvariant()}");

            var tStart = DateTime.UtcNow;

            var dfTrain = LoadData(tblTrain);
            var dfVal = LoadData(tblVal);
            var dfTest = LoadData(tblTest);

            // provjeri featuru listu (uzima samo whitelist u tom poretku)
            var featureNames = FeatureWhitelist
                .Where(fn => dfTrain.FeatureNames.Contains(fn, StringComparer.OrdinalIgnoreCase))
                .ToList();

            // warn na nedostajuće
            var missing = FeatureWhitelist.Where(fn => !featureNames.Contains(fn, StringComparer.OrdinalIgnoreCase)).ToList();
            if (missing.Count > 0)
                Console.WriteLine($"[WARN] {missing.Count} feature(a) nedostaje u viewu (npr. {string.Join(", ", missing.Take(5))})");

            // re-mapaj redoslijed inputa svih setova prema featureNames
            RemapTo(featureNames, dfTrain);
            RemapTo(featureNames, dfVal);
            RemapTo(featureNames, dfTest);

            Console.WriteLine($"Train: {dfTrain.Rows.Count:N0}  | Val: {dfVal.Rows.Count:N0}  | Test: {dfTest.Rows.Count:N0}  | #feat={featureNames.Count}");
            Console.WriteLine($"y-mean Train={dfTrain.Rows.Average(r => r.Label):F3}  Val={dfVal.Rows.Average(r => r.Label):F3}  Test={dfTest.Rows.Average(r => r.Label):F3}");

            // ===== Preprocess: train-median imput + winsor (1–99%) =====
            var trainMatrix = dfTrain.Rows.Select(r => r.Inputs).ToArray();
            var valMatrix = dfVal.Rows.Select(r => r.Inputs).ToArray();
            var testMatrix = dfTest.Rows.Select(r => r.Inputs).ToArray();

            var med = ColumnMedians(trainMatrix);
            var (p01, p99) = WinsorLimits(trainMatrix, featureNames);

            ImputeAndWinsor(trainMatrix, med, p01, p99, featureNames);
            ImputeAndWinsor(valMatrix, med, p01, p99, featureNames);
            ImputeAndWinsor(testMatrix, med, p01, p99, featureNames);

            AssertFinite(trainMatrix, "train");
            AssertFinite(valMatrix, "val");
            AssertFinite(testMatrix, "test");

            // setovi za Encog
            var Xtrain = new BasicMLDataSet(
                trainMatrix,
                dfTrain.Rows.Select(r => new double[] { r.Label }).ToArray());
            var Xval = new BasicMLDataSet(
                valMatrix,
                dfVal.Rows.Select(r => new double[] { r.Label }).ToArray());

            // ===== Mreža 32-8 TANH + Sigmoid =====
            int inputN = featureNames.Count;
            var net = new BasicNetwork();
            net.AddLayer(new BasicLayer(null, true, inputN));
            net.AddLayer(new BasicLayer(new ActivationTANH(), true, 32));
            net.AddLayer(new BasicLayer(new ActivationTANH(), true, 8));
            net.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            net.Structure.FinalizeStructure();
            net.Reset();

            // ===== Trening (RPROP) + early stopping po val Brier =====
            var trainer = new ResilientPropagation(net, Xtrain);

            // "Pametan" patience po veličini seta/varijanti
            int patience = variantName.Contains("grass", StringComparison.OrdinalIgnoreCase) ? 20
                         : variantName.StartsWith("r1_", StringComparison.OrdinalIgnoreCase) ? 30
                         : 50;
            int wait = 0, maxEpoch = 10000;
            double bestVal = double.PositiveInfinity;
            BasicNetwork bestNet = null!;

            for (int epoch = 1; epoch <= maxEpoch; epoch++)
            {
                trainer.Iteration();
                double trainBrier = trainer.Error;
                double valBrier = Brier(net, valMatrix, dfVal.Rows.Select(r => r.Label).ToArray());
                Console.WriteLine($"[{variantName}] Epoch {epoch,4} | train Brier {trainBrier:F6} | val Brier {valBrier:F6}");

                if (valBrier + 1e-8 < bestVal) { bestVal = valBrier; bestNet = net.DeepClone(); wait = 0; }
                else if (++wait >= patience) { Console.WriteLine($"[{variantName}] Early stopping."); break; }
            }
            trainer.FinishTraining();
            var finalNet = bestNet ?? net;

            // ===== RAW evaluacija na TEST =====
            var rawVal = Predict(finalNet, valMatrix);
            var rawTest = Predict(finalNet, testMatrix);

            var (brierValRaw, aucValRaw, eceValRaw) = Metrics(dfVal.Rows.Select(r => r.Label).ToArray(), rawVal);
            var (brierTestRaw, aucTestRaw, eceTestRaw) = Metrics(dfTest.Rows.Select(r => r.Label).ToArray(), rawTest);

            Console.WriteLine($"[{variantName}] RAW  Validation -> Brier: {brierValRaw:F6} | AUC: {aucValRaw:F4} | ECE: {eceValRaw:F3}");
            Console.WriteLine($"[{variantName}] RAW  Test       -> Brier: {brierTestRaw:F6} | AUC: {aucTestRaw:F4} | ECE: {eceTestRaw:F3}");

            // ===== Platt kalibracija na VAL =====
            var calib = FitPlatt(rawVal, dfVal.Rows.Select(r => r.Label));
            var calVal = Calibrate(rawVal, calib);
            var calTest = Calibrate(rawTest, calib);

            var (brierValCal, aucValCal, eceValCal) = Metrics(dfVal.Rows.Select(r => r.Label).ToArray(), calVal);
            var (brierTestCal, aucTestCal, eceTestCal) = Metrics(dfTest.Rows.Select(r => r.Label).ToArray(), calTest);

            Console.WriteLine($"[{variantName}] CAL  Validation -> Brier: {brierValCal:F6} | AUC: {aucValCal:F4} | ECE: {eceValCal:F3}");
            Console.WriteLine($"[{variantName}] CAL  Test       -> Brier: {brierTestCal:F6} | AUC: {aucTestCal:F4} | ECE: {eceTestCal:F3}");

            // ===== Accuracy@p≥t (test, kalibrirano) =====
            var yTest = dfTest.Rows.Select(r => r.Label).ToArray();
            var (acc60, n60) = AccuracyAt(yTest, calTest, 0.60);
            var (acc70, n70) = AccuracyAt(yTest, calTest, 0.70);
            var (acc80, n80) = AccuracyAt(yTest, calTest, 0.80);
            Console.WriteLine($"[{variantName}] ACC@p≥0.60: {acc60:P2} on {n60:N0} | @0.70: {acc70:P2} on {n70:N0} | @0.80: {acc80:P2} on {n80:N0}");

            // ===== Save =====
            var outDir = Path.Combine(SaveDir, variantName);
            Directory.CreateDirectory(outDir);

            var modelPath = Path.Combine(outDir, $"trainedNN_slim40_{variantName}.en");
            var featPath = Path.Combine(outDir, $"nn_features_slim40_{variantName}.txt");
            var calibPath = Path.Combine(outDir, $"nn_calibrator_slim40_{variantName}.json");
            var metricsPath = Path.Combine(outDir, $"nn_metrics_slim40_{variantName}.json");

            EncogDirectoryPersistence.SaveObject(new FileInfo(modelPath), finalNet);
            File.WriteAllLines(featPath, featureNames);
            File.WriteAllText(calibPath, JsonSerializer.Serialize(calib, new JsonSerializerOptions { WriteIndented = true }));

            var metricsObj = new
            {
                variant = variantName,
                startedUtc = tStart,
                finishedUtc = DateTime.UtcNow,
                nTrain = dfTrain.Rows.Count,
                nVal = dfVal.Rows.Count,
                nTest = dfTest.Rows.Count,
                feats = featureNames.Count,
                raw = new
                {
                    val = new { brier = brierValRaw, auc = aucValRaw, ece = eceValRaw },
                    test = new { brier = brierTestRaw, auc = aucTestRaw, ece = eceTestRaw }
                },
                cal = new
                {
                    val = new { brier = brierValCal, auc = aucValCal, ece = eceValCal },
                    test = new { brier = brierTestCal, auc = aucTestCal, ece = eceTestCal }
                },
                acc_at = new
                {
                    p60 = new { acc = acc60, n = n60 },
                    p70 = new { acc = acc70, n = n70 },
                    p80 = new { acc = acc80, n = n80 }
                }
            };
            File.WriteAllText(metricsPath, JsonSerializer.Serialize(metricsObj, new JsonSerializerOptions { WriteIndented = true }));
            AppendMetricsCsv(Path.Combine(SaveDir, "nn_metrics_summary.csv"), variantName, metricsObj);

            Console.WriteLine($"[{variantName}] Saved model:      {modelPath}");
            Console.WriteLine($"[{variantName}] Saved features:   {featPath}");
            Console.WriteLine($"[{variantName}] Saved calibrator: {calibPath}");
            Console.WriteLine($"[{variantName}] Saved metrics:    {metricsPath}");
        }

        // ===== Load & helpers =====
        static (List<string> FeatureNames, List<MatchData> Rows) LoadData(string table)
        {
            using var con = new SqlConnection(ConnStr);
            con.Open();

            string sql = $@"SELECT * FROM {table} WITH (NOLOCK);";
            using var cmd = new SqlCommand(sql, con) { CommandTimeout = 0 };
            using var rdr = cmd.ExecuteReader();

            var schema = rdr.GetSchemaTable()!;
            var columns = schema.Rows.Cast<DataRow>().Select(r => (string)r["ColumnName"]).ToList();

            int yIdx = columns.FindIndex(c => c.Equals("y", StringComparison.OrdinalIgnoreCase));
            if (yIdx < 0) throw new Exception("Kolona 'y' nije pronađena!");

            var colIndex = columns
                .Select((name, idx) => new { name, idx })
                .ToDictionary(x => x.name, x => x.idx, StringComparer.OrdinalIgnoreCase);

            int idxMatch = colIndex.ContainsKey("MatchTPId") ? colIndex["MatchTPId"] : -1;

            var rows = new List<MatchData>(capacity: 100000);
            while (rdr.Read())
            {
                long matchId = (idxMatch >= 0)
                    ? Convert.ToInt64(rdr.GetValue(idxMatch), CultureInfo.InvariantCulture)
                    : 0L;

                var rec = new double[columns.Count];
                for (int i = 0; i < columns.Count; i++)
                {
                    object v = rdr.GetValue(i);
                    if (i == yIdx) continue;
                    rec[i] = v == DBNull.Value ? double.NaN : Convert.ToDouble(v, CultureInfo.InvariantCulture);
                }

                double y = Convert.ToDouble(rdr.GetValue(yIdx), CultureInfo.InvariantCulture);
                rows.Add(new MatchData { MatchTPId = matchId, Inputs = rec, Label = y });
            }

            return (columns, rows);
        }

        static void RemapTo(List<string> orderedFeatNames, (List<string> FeatureNames, List<MatchData> Rows) df)
        {
            var colIndex = df.FeatureNames
                .Select((name, idx) => new { name, idx })
                .ToDictionary(x => x.name, x => x.idx, StringComparer.OrdinalIgnoreCase);

            foreach (var r in df.Rows)
            {
                var x = new double[orderedFeatNames.Count];
                for (int i = 0; i < orderedFeatNames.Count; i++)
                {
                    var fn = orderedFeatNames[i];
                    if (colIndex.TryGetValue(fn, out int idx))
                        x[i] = r.Inputs[idx];
                    else
                        x[i] = double.NaN; // nedostajuće -> imputiramo kasnije
                }
                r.Inputs = x;
            }
        }

        static double[] ColumnMedians(double[][] X)
        {
            int p = X[0].Length;
            var med = new double[p];
            for (int j = 0; j < p; j++)
            {
                var col = X.Select(r => r[j]).Where(v => !double.IsNaN(v) && !double.IsInfinity(v)).OrderBy(v => v).ToArray();
                med[j] = col.Length == 0 ? 0.0 : Percentile(col, 50.0);
            }
            return med;
        }

        static (double[] p01, double[] p99) WinsorLimits(double[][] X, List<string> featNames)
        {
            int p = X[0].Length;
            var p01 = Enumerable.Repeat(double.NegativeInfinity, p).ToArray();
            var p99 = Enumerable.Repeat(double.PositiveInfinity, p).ToArray();

            for (int j = 0; j < p; j++)
            {
                if (!WinsorCols.Contains(featNames[j])) continue;
                var col = X.Select(r => r[j]).Where(v => !double.IsNaN(v) && !double.IsInfinity(v)).OrderBy(v => v).ToArray();
                if (col.Length == 0) { p01[j] = double.NegativeInfinity; p99[j] = double.PositiveInfinity; continue; }
                p01[j] = Percentile(col, 1.0);
                p99[j] = Percentile(col, 99.0);
            }
            return (p01, p99);
        }

        static void ImputeAndWinsor(double[][] X, double[] med, double[] p01, double[] p99, List<string> featNames)
        {
            int n = X.Length; int p = X[0].Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < p; j++)
                {
                    double v = X[i][j];
                    if (double.IsNaN(v) || double.IsInfinity(v)) v = med[j];
                    if (v < p01[j]) v = p01[j];
                    if (v > p99[j]) v = p99[j];
                    X[i][j] = v;
                }
            }
        }

        static void AssertFinite(double[][] X, string tag)
        {
            for (int i = 0; i < X.Length; i++)
                for (int j = 0; j < X[i].Length; j++)
                    if (double.IsNaN(X[i][j]) || double.IsInfinity(X[i][j]))
                        throw new Exception($"[{tag}] non-finite at row {i}, col {j}");
        }

        static double Percentile(double[] sorted, double p)
        {
            if (sorted.Length == 0) return 0.0;
            double rank = (p / 100.0) * (sorted.Length - 1);
            int lo = (int)Math.Floor(rank);
            int hi = (int)Math.Ceiling(rank);
            if (lo == hi) return sorted[lo];
            double w = rank - lo;
            return sorted[lo] * (1 - w) + sorted[hi] * w;
        }

        static double Brier(BasicNetwork net, double[][] X, double[] y)
        {
            double err = 0; int n = y.Length;
            for (int i = 0; i < n; i++)
            {
                var o = net.Compute(new BasicMLData(X[i]))[0];
                double d = o - y[i];
                err += d * d;
            }
            return err / Math.Max(1, n);
        }

        static double[] Predict(BasicNetwork net, double[][] X)
        {
            var p = new double[X.Length];
            for (int i = 0; i < X.Length; i++)
                p[i] = net.Compute(new BasicMLData(X[i]))[0];
            return p;
        }

        static (double Brier, double AUC, double ECE) Metrics(double[] y, double[] p)
        {
            double brier = 0.0; int n = y.Length;
            for (int i = 0; i < n; i++) { double d = p[i] - y[i]; brier += d * d; }
            brier /= Math.Max(1, n);

            double auc = RocAuc(y, p);
            double ece = ECE(y, p, 12);
            return (brier, auc, ece);
        }

        static double RocAuc(double[] yTrue, double[] yScore)
        {
            int n = yTrue.Length;
            var idx = Enumerable.Range(0, n).ToArray();
            Array.Sort(idx, (i, j) => yScore[i].CompareTo(yScore[j]));

            var ranks = new double[n];
            int i0 = 0;
            while (i0 < n)
            {
                int i1 = i0 + 1;
                while (i1 < n && yScore[idx[i1]] == yScore[idx[i0]]) i1++;
                double avgRank = (i0 + 1 + i1) / 2.0;
                for (int k = i0; k < i1; k++) ranks[idx[k]] = avgRank;
                i0 = i1;
            }

            double nPos = 0, sumPos = 0;
            for (int i = 0; i < n; i++)
                if (yTrue[i] > 0.5) { nPos++; sumPos += ranks[i]; }
            double nNeg = n - nPos;
            if (nPos == 0 || nNeg == 0) return 0.5;

            return (sumPos - nPos * (nPos + 1) / 2.0) / (nPos * nNeg);
        }

        static double ECE(double[] y, double[] p, int bins)
        {
            int n = y.Length;
            double ece = 0.0;
            for (int b = 0; b < bins; b++)
            {
                double lo = (double)b / bins;
                double hi = (double)(b + 1) / bins;
                var idx = Enumerable.Range(0, n).Where(i => p[i] >= lo && p[i] < hi).ToArray();
                if (idx.Length == 0) continue;
                double conf = idx.Select(i => p[i]).Average();
                double acc = idx.Select(i => y[i]).Average();
                ece += ((double)idx.Length / n) * Math.Abs(acc - conf);
            }
            return ece;
        }

        // Platt calibration
        static Calibrator FitPlatt(double[] pRaw, IEnumerable<double> yEnum, int iters = 50)
        {
            var y = yEnum.ToArray();
            int n = pRaw.Length;
            var x = new double[n];

            for (int i = 0; i < n; i++)
            {
                double pr = Math.Clamp(pRaw[i], 1e-6, 1 - 1e-6);
                x[i] = Math.Log(pr / (1 - pr)); // logit
            }

            double A = 0.0, B = 1.0;
            for (int it = 0; it < iters; it++)
            {
                double gA = 0, gB = 0, hAA = 1e-6, hAB = 0, hBB = 1e-6;
                for (int i = 0; i < n; i++)
                {
                    double z = A + B * x[i];
                    double q = 1.0 / (1.0 + Math.Exp(-z));
                    double diff = q - y[i];
                    gA += diff;
                    gB += diff * x[i];
                    double w = q * (1 - q);
                    hAA += w;
                    hAB += w * x[i];
                    hBB += w * x[i] * x[i];
                }
                double det = hAA * hBB - hAB * hAB;
                if (Math.Abs(det) < 1e-12) break;
                double dA = (-gA * hBB + gB * hAB) / det;
                double dB = (-gB * hAA + gA * hAB) / det;

                A += dA; B += dB;
                if (Math.Abs(dA) + Math.Abs(dB) < 1e-6) break;
            }
            return new Calibrator { A = A, B = B };
        }

        static double[] Calibrate(double[] pRaw, Calibrator c)
        {
            int n = pRaw.Length;
            var p = new double[n];
            for (int i = 0; i < n; i++)
            {
                double pr = Math.Clamp(pRaw[i], 1e-12, 1 - 1e-12);
                double x = Math.Log(pr / (1 - pr));
                double z = c.A + c.B * x;
                p[i] = 1.0 / (1.0 + Math.Exp(-z));
            }
            return p;
        }

        // Accuracy @ p-threshold
        static (double acc, int n) AccuracyAt(double[] y, double[] p, double thr = 0.6)
        {
            var idx = Enumerable.Range(0, y.Length).Where(i => p[i] >= thr).ToArray();
            if (idx.Length == 0) return (double.NaN, 0);
            double acc = idx.Select(i => ((p[i] >= 0.5 ? 1.0 : 0.0) == y[i]) ? 1.0 : 0.0).Average();
            return (acc, idx.Length);
        }

        // Append metrics to a global CSV
        static void AppendMetricsCsv(string csvPath, string variant, object metricsObj)
        {
            // minimalist: samo glavne brojke iz metricsObj-a
            var json = JsonSerializer.Serialize(metricsObj);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var calTest = root.GetProperty("cal").GetProperty("test");
            double auc = calTest.GetProperty("auc").GetDouble();
            double brier = calTest.GetProperty("brier").GetDouble();
            double ece = calTest.GetProperty("ece").GetDouble();

            var acc_at = root.GetProperty("acc_at");
            double acc60 = acc_at.GetProperty("p60").GetProperty("acc").GetDouble();
            int n60 = acc_at.GetProperty("p60").GetProperty("n").GetInt32();
            double acc70 = acc_at.GetProperty("p70").GetProperty("acc").GetDouble();
            int n70 = acc_at.GetProperty("p70").GetProperty("n").GetInt32();
            double acc80 = acc_at.GetProperty("p80").GetProperty("acc").GetDouble();
            int n80 = acc_at.GetProperty("p80").GetProperty("n").GetInt32();

            bool writeHeader = !File.Exists(csvPath);
            using var sw = new StreamWriter(csvPath, append: true);
            if (writeHeader)
                sw.WriteLine("variant,auc_cal_test,brier_cal_test,ece_cal_test,acc_p60,n_p60,acc_p70,n_p70,acc_p80,n_p80,finishedUtc");
            sw.WriteLine($"{variant},{auc:F6},{brier:F6},{ece:F6},{acc60:F6},{n60},{acc70:F6},{n70},{acc80:F6},{n80},{DateTime.UtcNow:O}");
        }
    }
}