using BonzoByte.Core.Services.Interfaces;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Persist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BonzoByte.ML
{
    public sealed class NNRouterScorer
    {
        // ===== Konfig =====
        private const string BaseDir = @"c:\bb_nn_out";   // gdje su varijantni folderi
        // blend kontrola
        private const bool ENABLE_R1_BLEND = true;
        private const double BLEND_W = 0.25;       // težina R1 u logit blendu
        private const double BLEND_LOW = 0.55;       // siva zona
        private const double BLEND_HIGH = 0.65;

        private readonly string _connStr;
        private readonly ICore40FeatureExtractor _feats;

        // modeli i kalibratori po varijanti
        private readonly Dictionary<string, BasicNetwork> _nets = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, object> _locks = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Calibrator?> _calibs = new(StringComparer.OrdinalIgnoreCase);

        // kanonski poredak featurea
        private readonly List<string> _featureNames;

        public NNRouterScorer(string connStr)
        {
            _connStr = connStr;

            // 1) Učitaj kanonsku listu featova iz r0_all
            string featsPath = Path.Combine(BaseDir, "r0_all", "nn_features_slim40_r0_all.txt");
            if (!File.Exists(featsPath))
                throw new FileNotFoundException($"Missing feature list: {featsPath}");
            _featureNames = File.ReadAllLines(featsPath)
                                .Select(s => s.Trim())
                                .Where(s => s.Length > 0)
                                .ToList();

            _feats = new Core40FeatureExtractor(connStr, _featureNames);

            // 2) Učitaj modele/kalibratore po varijantama (što postoji)
            var variants = new[]
            {
                "r0_all","r0_clay","r0_hard",           // bazne koje koristimo sigurno
                "r1_all","r1_clay","r1_hard"            // opcijski za blend
                // "r0_grass","r1_grass" -> preskačemo (slabi)
            };

            foreach (var v in variants)
                TryLoadBundle(v);
        }

        private void TryLoadBundle(string variant)
        {
            string dir = Path.Combine(BaseDir, variant);
            string modelPath = Path.Combine(dir, $"trainedNN_slim40_{variant}.en");
            string calibPath = Path.Combine(dir, $"nn_calibrator_slim40_{variant}.json");

            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"[NNRouter] WARN: model missing for {variant} -> {modelPath}");
                return;
            }

            try
            {
                var net = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(modelPath));
                _nets[variant] = net;
                _locks[variant] = new object();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NNRouter] ERROR: failed to load model {variant}: {ex.Message}");
                return;
            }

            Calibrator? calib = null;
            if (File.Exists(calibPath))
            {
                try
                {
                    calib = System.Text.Json.JsonSerializer.Deserialize<Calibrator>(File.ReadAllText(calibPath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NNRouter] WARN: failed to load calibrator {variant}: {ex.Message}");
                }
            }
            _calibs[variant] = calib;
        }

        // ===== Public scoring =====
        public async Task<double> ScoreAndUpdateAsync(int matchTPId, CancellationToken ct = default)
        {
            var fv = await _feats.BuildAsync(matchTPId, ct);
            if (fv == null)
            {
                Console.WriteLine($"[NNRouter] No base data for MatchTPId={matchTPId}");
                return 0.0;
            }

            // 1) Odaberi bazni R0 po podlozi
            string baseVar = BaseVariantForSurface(fv.SurfaceId);
            if (!_nets.ContainsKey(baseVar))
            {
                Console.WriteLine($"[NNRouter] WARN: base model {baseVar} missing, fallback to r0_all.");
                baseVar = "r0_all";
            }

            double pBase = CalibratedPredict(baseVar, fv.Values);

            // 2) (Opcionalno) R1 blend ako imamo H2H≥1 i model postoji i base je u sivoj zoni
            double pFinal = pBase;
            if (ENABLE_R1_BLEND && fv.H2HMatches >= 1 && pBase >= BLEND_LOW && pBase <= BLEND_HIGH)
            {
                string r1Var = R1VariantForSurface(fv.SurfaceId);
                if (_nets.ContainsKey(r1Var))
                {
                    double pR1 = CalibratedPredict(r1Var, fv.Values);
                    pFinal = BlendLogit(pBase, pR1, BLEND_W);
                }
            }

            await UpdateMatchWinProbabilityAsync(matchTPId, pFinal, ct);
            return pFinal;
        }

        private static string BaseVariantForSurface(int surfaceId)
            => surfaceId switch
            {
                2 => "r0_clay",
                4 => "r0_hard",
                // 3 (grass) i 1 (unknown) -> ALL
                _ => "r0_all"
            };

        private static string R1VariantForSurface(int surfaceId)
            => surfaceId switch
            {
                2 => "r1_clay",
                4 => "r1_hard",
                _ => "r1_all"
            };

        // ===== Core helpers =====
        private double CalibratedPredict(string variant, double[] x)
        {
            double pRaw = Predict(variant, x);
            return Calibrate(variant, pRaw);
        }

        private double Predict(string variant, double[] x)
        {
            if (!_nets.TryGetValue(variant, out var net))
            {
                // ne bi se trebalo dogoditi nakon routing fallbacka, ali budimo otporni
                net = _nets["r0_all"];
            }
            var lk = _locks[variant];
            double y;
            lock (lk) { y = net.Compute(new BasicMLData(x))[0]; }
            if (y < 1e-9) y = 1e-9; else if (y > 1 - 1e-9) y = 1 - 1e-9;
            return y;
        }

        private double Calibrate(string variant, double pRaw)
        {
            if (!_calibs.TryGetValue(variant, out var c) || c == null) return pRaw;
            double pr = Math.Clamp(pRaw, 1e-12, 1 - 1e-12);
            double logit = Math.Log(pr / (1 - pr));
            double z = c.A + c.B * logit;
            return 1.0 / (1.0 + Math.Exp(-z));
        }

        private static double BlendLogit(double pA, double pB, double wB)
        {
            double wA = 1.0 - wB;
            double la = Math.Log(pA / (1 - pA));
            double lb = Math.Log(pB / (1 - pB));
            double l = wA * la + wB * lb;
            return 1.0 / (1.0 + Math.Exp(-l));
        }

        private async Task<int> UpdateMatchWinProbabilityAsync(int matchTPId, double pCal, CancellationToken ct)
        {
            await using var con = new SqlConnection(_connStr);
            await con.OpenAsync(ct);

            const string sql = @"UPDATE dbo.Match SET WinProbabilityNN = @p WHERE MatchTPId = @mid;";
            await using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@p", SqlDbType.Float).Value = pCal;
            cmd.Parameters.Add("@mid", SqlDbType.Int).Value = matchTPId;
            cmd.CommandTimeout = 0;

            return await cmd.ExecuteNonQueryAsync(ct);
        }

        private sealed class Calibrator
        {
            public double A { get; set; }
            public double B { get; set; }
            public string Type { get; set; } = "Platt";
        }
    }
}