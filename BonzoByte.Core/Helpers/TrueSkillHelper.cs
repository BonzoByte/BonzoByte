using Azure.Identity;
using BonzoByte.Core.Models;
using BonzoByte.Core.Models.TrueSkill;
using Moserware.Numerics;
using Moserware.Skills;
using Moserware.Skills.TrueSkill;
using Player = Moserware.Skills.Player;
namespace BonzoByte.Core.Helpers
{
    public static class TrueSkillHelper
    {
        public static readonly string[] AllKeys = ["M", "SM", "GSM", "MS1", "MS2", "MS3", "MS4", "SMS1", "SMS2", "SMS3", "SMS4", "GSMS1", "GSMS2", "GSMS3", "GSMS4"];
        public static readonly string[] SetKeys = ["SM", "SMS1", "SMS2", "SMS3", "SMS4"];
        public static readonly string[] GameKeys = ["GSM", "GSMS1", "GSMS2", "GSMS3", "GSMS4"];
        public static readonly string[] SurfaceKeys = ["S1", "S2", "S3", "S4"];

        public static TrueSkillResult CalculateM(double meanP1, double meanP2, double sdP1, double sdP2, int[]? ranks = null)
        {
            var gi0 = GameInfo.DefaultGameInfo;
            // Ne mutiramo default; kreiramo novi GameInfo
            var gameInfo = new GameInfo(gi0.InitialMean, gi0.InitialStandardDeviation, gi0.Beta, gi0.DynamicsFactor, 0.0);

            var player1 = new Player(1);
            var player2 = new Player(2);

            var ratingP1 = new Rating(meanP1, sdP1);
            var ratingP2 = new Rating(meanP2, sdP2);

            var teams = new List<IDictionary<Player, Rating>>
    {
        new Dictionary<Player, Rating> { { player1, ratingP1 } },
        new Dictionary<Player, Rating> { { player2, ratingP2 } }
    };

            var finalRanks = ranks ?? new[] { 1, 2 };
            var calculator = new TwoPlayerTrueSkillCalculator();
            var newRatings = calculator.CalculateNewRatings(gameInfo, teams, finalRanks);

            var newP1 = newRatings[player1];
            var newP2 = newRatings[player2];

            // Win prob BEZ draw prob u formuli
            double winProbability = CalculateWinProbability(meanP1, meanP2, sdP1, sdP2);

            return new TrueSkillResult
            {
                MeanP1 = newP1.Mean,
                SdP1 = newP1.StandardDeviation,
                MeanP2 = newP2.Mean,
                SdP2 = newP2.StandardDeviation,
                WinProbabilityP1 = winProbability
            };
        }

        public static TrueSkillResult CalculateSM(
            double meanP1, double meanP2, double sdP1, double sdP2,
            int p1, int p2, string res)
        {
            var sets = ParseSets(res, p1, p2);

            var gi0 = GameInfo.DefaultGameInfo;
            var gameInfo = new GameInfo(gi0.InitialMean, gi0.InitialStandardDeviation, gi0.Beta, gi0.DynamicsFactor, 0.0);

            var player1 = new Player(1);
            var player2 = new Player(2);

            var ratingP1 = new Rating(meanP1, sdP1);
            var ratingP2 = new Rating(meanP2, sdP2);

            double winProbability = CalculateWinProbability(meanP1, meanP2, sdP1, sdP2);

            var calculator = new TwoPlayerTrueSkillCalculator();
            foreach (var set in sets)
            {
                var finalRanks = set.WhoWon == 1 ? new[] { 1, 2 } : new[] { 2, 1 };
                var teams = new List<IDictionary<Player, Rating>>
        {
            new Dictionary<Player, Rating> { { player1, ratingP1 } },
            new Dictionary<Player, Rating> { { player2, ratingP2 } }
        };
                var newRatings = calculator.CalculateNewRatings(gameInfo, teams, finalRanks);
                ratingP1 = newRatings[player1];
                ratingP2 = newRatings[player2];
            }

            return new TrueSkillResult
            {
                MeanP1 = ratingP1.Mean,
                SdP1 = ratingP1.StandardDeviation,
                MeanP2 = ratingP2.Mean,
                SdP2 = ratingP2.StandardDeviation,
                WinProbabilityP1 = winProbability
            };
        }

        public static TrueSkillResult CalculateGSM(
            double meanP1, double meanP2, double sdP1, double sdP2,
            int p1, int p2, string res)
        {
            var sets = ParseSets(res, p1, p2);

            var gi0 = GameInfo.DefaultGameInfo;
            var gameInfo = new GameInfo(gi0.InitialMean, gi0.InitialStandardDeviation, gi0.Beta, gi0.DynamicsFactor, 0.0);

            var player1 = new Player(1);
            var player2 = new Player(2);

            var ratingP1 = new Rating(meanP1, sdP1);
            var ratingP2 = new Rating(meanP2, sdP2);

            double winProbability = CalculateWinProbability(meanP1, meanP2, sdP1, sdP2);

            // Raspakiraj igre kronološki
            var gameSeq = new List<int[]>();
            foreach (var set in sets)
            {
                double p1Step = set.GamesP1 > 0 ? 1.0 / (set.GamesP1 + 1) : 0;
                double p2Step = set.GamesP2 > 0 ? 1.0 / (set.GamesP2 + 1) : 0;

                var temp = new List<(double pos, int[] rank)>();
                double acc = 0;
                for (int j = 0; j < set.GamesP1; j++) { acc += p1Step; temp.Add((acc, new[] { 1, 2 })); }
                acc = 0;
                for (int j = 0; j < set.GamesP2; j++) { acc += p2Step; temp.Add((acc, new[] { 2, 1 })); }

                foreach (var g in temp.OrderBy(t => t.pos)) gameSeq.Add(g.rank);
            }

            var calculator = new TwoPlayerTrueSkillCalculator();
            foreach (var finalRanks in gameSeq)
            {
                var teams = new List<IDictionary<Player, Rating>>
        {
            new Dictionary<Player, Rating> { { player1, ratingP1 } },
            new Dictionary<Player, Rating> { { player2, ratingP2 } }
        };
                var newRatings = calculator.CalculateNewRatings(gameInfo, teams, finalRanks);
                ratingP1 = newRatings[player1];
                ratingP2 = newRatings[player2];
            }

            return new TrueSkillResult
            {
                MeanP1 = ratingP1.Mean,
                SdP1 = ratingP1.StandardDeviation,
                MeanP2 = ratingP2.Mean,
                SdP2 = ratingP2.StandardDeviation,
                WinProbabilityP1 = winProbability
            };
        }


        public static double CalculateWinProbability(double mean1, double mean2, double sd1, double sd2)
        {
            var gi0 = GameInfo.DefaultGameInfo;
            double numerator = mean1 - mean2;
            double sigmaSquaredSum = sd1 * sd1 + sd2 * sd2;
            double denom = Math.Sqrt(2 * gi0.Beta * gi0.Beta + sigmaSquaredSum);
            return GaussianDistribution.CumulativeTo(numerator / denom);
        }

        public static List<SetWonBy> ParseSets(string? resultDetails, int player1Id, int player2Id)
        {
            var sets = new List<SetWonBy>();
            if (string.IsNullOrWhiteSpace(resultDetails)) return sets;

            string resDet = resultDetails.Replace(" (", "").Trim();
            if (resDet.Replace(" ", "").Replace("(", "").Replace(")", "").Length <= 3) return sets;

            string[] rawSets = resDet.Split(" ");
            foreach (var raw in rawSets)
            {
                string cleaned = raw;

                while (cleaned.Contains('('))
                {
                    int start = cleaned.IndexOf('(');
                    int end = cleaned.IndexOf(')', start);
                    if (end > start)
                        cleaned = cleaned.Remove(start, end - start + 1).Trim();
                    else
                        break;
                }

                cleaned = cleaned.Replace(")", "").Trim();
                cleaned = cleaned.Replace("-", "");

                // Normalize tiebreaks
                if (cleaned.StartsWith("7") && cleaned.EndsWith("6")) cleaned = "76";
                else if (cleaned.StartsWith("6") && cleaned.EndsWith("7")) cleaned = "67";

                int gamesP1 = 0, gamesP2 = 0, whoWon = 0, by = 0, total = 0;

                try
                {
                    if (cleaned.Length == 2)
                    {
                        gamesP1 = int.Parse(cleaned[0].ToString());
                        gamesP2 = int.Parse(cleaned[1].ToString());
                    }
                    else if (cleaned.Length == 3)
                    {
                        gamesP1 = int.Parse(cleaned[..2]);
                        gamesP2 = int.Parse(cleaned[2..]);
                    }
                    else if (cleaned.Length == 4)
                    {
                        gamesP1 = int.Parse(cleaned[..2]);
                        gamesP2 = int.Parse(cleaned[2..]);
                    }

                    total = gamesP1 + gamesP2;
                    if (gamesP1 > gamesP2) whoWon = 1;
                    else if (gamesP2 > gamesP1) whoWon = 2;

                    by = Math.Abs(gamesP1 - gamesP2);
                }
                catch
                {
                    continue;
                }

                if (by != 0)
                {
                    sets.Add(new SetWonBy
                    {
                        Set = cleaned,
                        Player1Id = player1Id,
                        Player2Id = player2Id,
                        WhoWon = whoWon,
                        By = by,
                        Games = total,
                        GamesP1 = gamesP1,
                        GamesP2 = gamesP2
                    });
                }
            }

            return sets;
        }
    }
}