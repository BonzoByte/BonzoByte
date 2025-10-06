using BonzoByte.Core.Models;
using BonzoByte.Core.Models.TrueSkill;
using Moserware.Skills;

namespace BonzoByte.Core.Helpers
{
    public static class TrueSkillGranularUpdater
    {
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

        /// <summary>
        /// Approximates the likely order of games won within each set for granular TrueSkill updates.
        /// This is based on a uniform distribution of game wins throughout the set.
        /// </summary>
        public static List<GameWonRank> GenerateGameRanks(List<SetWonBy> sets, out double totalGamesPlayed)
        {
            var result = new List<GameWonRank>();
            totalGamesPlayed = 0;

            foreach (var s in sets)
            {
                totalGamesPlayed += s.Games;

                double p1Game = s.GamesP1 > 0 ? 1.0 / (s.GamesP1 + 1) : 0;
                double p2Game = s.GamesP2 > 0 ? 1.0 / (s.GamesP2 + 1) : 0;

                double p1GamePos = 0;
                double p2GamePos = 0;

                for (int j = 0; j < s.GamesP1; j++)
                {
                    p1GamePos += p1Game;
                    result.Add(new GameWonRank
                    {
                        GamePos = p1GamePos,
                        Rank = [1, 2]
                    });
                }

                for (int j = 0; j < s.GamesP2; j++)
                {
                    p2GamePos += p2Game;
                    result.Add(new GameWonRank
                    {
                        GamePos = p2GamePos,
                        Rank = [2, 1]
                    });
                }
            }

            return result.OrderBy(g => g.GamePos).ToList();
        }

        public static void InjectSetAndGameStats(Match match)
        {
            var sets = ParseSets(match.ResultDetails, match.Player1TPId!.Value, match.Player2TPId!.Value);
            if (sets.Count == 0) return;

            int setsWonP1 = 0, setsWonP2 = 0;
            int gamesWonP1 = 0, gamesWonP2 = 0;

            foreach (var s in sets)
            {
                if (s.WhoWon == 1) setsWonP1++;
                else if (s.WhoWon == 2) setsWonP2++;

                gamesWonP1 += s.GamesP1;
                gamesWonP2 += s.GamesP2;
            }

            match.P1SetsWon = setsWonP1;
            match.P2SetsWon = setsWonP2;
            match.P1SetsLoss = setsWonP2;
            match.P2SetsLoss = setsWonP1;

            match.P1GamesWon = gamesWonP1;
            match.P2GamesWon = gamesWonP2;
            match.P1GamesLoss = gamesWonP2;
            match.P2GamesLoss = gamesWonP1;
        }
    }
}