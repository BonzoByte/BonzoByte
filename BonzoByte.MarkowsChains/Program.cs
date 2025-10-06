using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace TennisProbability
{
    class Program
    {
        /*
------------------------------------------------------------------------------------------
        Tennis Probability Calculator Application

        This console application simulates and analyzes different aspects of a tennis match 
        between two players, A and B, based on their respective probabilities of winning points 
        while serving.

        The application calculates the probabilities of various scenarios, such as:
        - Winning a regular game
        - Winning a tiebreak game
        - Winning a tiebreak set
        - Winning an advantage set
        - Winning a "best of 2 sets" match (all tiebreak or advantage set)
        - Winning a "best of 3 sets" match (all tiebreak or advantage set)

        Additionally, it also calculates the probabilities of different score lines for both 
        players in a regular game, a tiebreak game, a tiebreak set, and an advantage set. 

        The analysis takes into account various factors such as serve and return efficiencies 
        and how these influence the chances of winning in different game, set, and match formats. 

        This simulation can be useful for tennis enthusiasts, coaches, and statisticians who want 
        to understand how different point-winning probabilities could affect the overall outcomes 
        of a tennis match.

        Please note: The core assumptions of the simulation and calculations are based on standard 
        tennis rules and the input probabilities provided for both the players.
        ------------------------------------------------------------------------------------------
        */

        static void Main(string[] args)
        {

            // These probabilities represent the chances of Player A and Player B winning a point while serving in a game. 
            double probAOnServe = 0.62;
            double probBOnServe = 0.60;

            // Current score in points for Player A in the ongoing game. Initialized to 0 at the start.
            int currentPointScoreA = 0;

            // Current score in points for Player B in the ongoing game. Initialized to 0 at the start.
            int currentPointScoreB = 0;

            // Current score in games for Player A in the ongoing set. Initialized to 0 at the start.
            int currentGameScoreA = 0;

            // Current score in games for Player B in the ongoing set. Initialized to 0 at the start.
            int currentGameScoreB = 0;

            // Current score in points for Player A during a tiebreak game. Initialized to 0 at the start.
            // Only relevant when a tiebreak game is in progress.
            int currentTiebreakPointScoreA = 0;

            // Current score in points for Player B during a tiebreak game. Initialized to 0 at the start.
            // Only relevant when a tiebreak game is in progress.
            int currentTiebreakPointScoreB = 0;

            // Current score in sets for Player A in the match. Initialized to 0 at the start.
            int currentSetScoreA = 0;

            // Current score in sets for Player B in the match. Initialized to 0 at the start.
            int currentSetScoreB = 0;

            // Calculate the probabilities of Player A and Player B winning a regular game. 
            var (gameWinProbA, gameWinProbB) = CalculateGameProbabilities(probAOnServe, probBOnServe);

            // Calculate the probabilities of both players winning on serve and on return during a tiebreak game.
            var (tiebreakWinOnServe, tiebreakWinOnReturn) = CalculateTiebreakProbabilities(probAOnServe, probBOnServe);

            // Calculate the probabilities of both players winning a tiebreak set when either serving or returning.
            var (winningTiebreakSetOnServe, winningTiebreakSetOnReturn) = CalculateWinningTiebreakSet(tiebreakWinOnServe[0, 0], tiebreakWinOnReturn[0, 0], gameWinProbA[0, 0], gameWinProbB[0, 0]);

            // Calculate the probabilities of both players winning an advantage set when either serving or returning.
            var (winningAdvantageSetOnServe, winningAdvantageSetOnReturn) = CalculateWinningAdvantageSet(gameWinProbA[0, 0], gameWinProbB[0, 0]);

            // Calculate the probabilities of both players winning all tiebreak set matches in a "best of 2 sets" match.
            var winningAnAllTiebreakSetMatchBestOf2Sets = CalculateWinningAllTiebreakSetMatchBestOf2Sets(winningTiebreakSetOnServe[0, 0], winningTiebreakSetOnReturn[0, 0]);

            // Calculate the probabilities of both players winning all tiebreak set matches in a "best of 3 sets" match.
            var winningAnAllTiebreakSetMatchBestOf3Sets = CalculateWinningAllTiebreakSetMatchBestOf3Sets(winningTiebreakSetOnServe[0, 0], winningTiebreakSetOnReturn[0, 0]);

            // Calculate the probabilities of both players winning an advantage set match in a "best of 2 sets" match.
            var winningAnAdvantageSetMatchBestOf2Sets = CalculateWinningAdvantageSetMatchBestOf2Sets(winningAdvantageSetOnServe[0, 0], winningTiebreakSetOnReturn[0, 0]);

            // Calculate the probabilities of both players winning an advantage set match in a "best of 3 sets" match.
            var winningAnAdvantageSetMatchBestOf3Sets = CalculateWinningAdvantageSetMatchBestOf3Sets(winningAdvantageSetOnServe[0, 0], winningTiebreakSetOnReturn[0, 0]);

            // Calculate the probabilities of each possible scoreline for both players in a regular game.
            var (scoreLineProbabilitiesInAGameForPlayerA, scoreLineProbabilitiesInAGameForPlayerB) = CalculateScoreLineProbabilitiesInAGame(probAOnServe, probBOnServe);

            // Calculate the probabilities of each possible scoreline for both players in a tiebreak game.
            var (scoreLineProbabilitiesInATiebreakForPlayerA, scoreLineProbabilitiesInATiebreakForPlayerB) = CalculateTiebreakGameProbabilities(probAOnServe, probBOnServe);

            // Calculate the probabilities of each possible scoreline for both players in a tiebreak set.
            var (scoreLineProbabilitiesInATiebreakSetForPlayerA, scoreLineProbabilitiesInATiebreakSetForPlayerB) = CalculateTiebreakSetScoreProbabilities(tiebreakWinOnServe[0, 0], tiebreakWinOnReturn[0, 0], gameWinProbA, gameWinProbB);

            // Calculate the probabilities of each possible scoreline for both players in an advantage set.
            var (scoreLineProbabilitiesInAAdvantageSetForPlayerA, scoreLineProbabilitiesInAAdvantageSetForPlayerB) = CalculateAdvantageSetScoreProbabilities(tiebreakWinOnServe[0, 0], tiebreakWinOnReturn[0, 0], gameWinProbA, gameWinProbB);

            // Calculate the probabilities of each possible scoreline for both players in a best-of-5 all tiebreak set match.
            var (probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerA, probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerB) = CalculateWinningAllTiebreakSetMatchBestOf5Sets(winningTiebreakSetOnServe[0, 0], winningTiebreakSetOnReturn[0, 0]);

            // Calculate the probabilities of each possible scoreline for both players in a best-of-5 advantage set match.
            var (probabilitiesOfWinningAdvantageSetMatchBestOf5SetsForPlayerA, probabilitiesOfWinningAdvantageSetMatchBestOf5SetsForPlayerB) = CalculateAdvantageSetMatchBestOf5(winningTiebreakSetOnServe[0, 0], winningTiebreakSetOnReturn[0, 0], winningAdvantageSetOnServe[0, 0], winningTiebreakSetOnReturn[0, 0]);

            // Print the results
            PrintResults(gameWinProbA, gameWinProbB, tiebreakWinOnServe, tiebreakWinOnReturn, winningTiebreakSetOnServe, winningTiebreakSetOnReturn, winningAdvantageSetOnServe, winningAdvantageSetOnReturn, winningAnAllTiebreakSetMatchBestOf2Sets, winningAnAllTiebreakSetMatchBestOf3Sets, winningAnAdvantageSetMatchBestOf2Sets, winningAnAdvantageSetMatchBestOf3Sets, scoreLineProbabilitiesInAGameForPlayerA, scoreLineProbabilitiesInAGameForPlayerB, scoreLineProbabilitiesInATiebreakForPlayerA, scoreLineProbabilitiesInATiebreakForPlayerB, scoreLineProbabilitiesInATiebreakSetForPlayerA, scoreLineProbabilitiesInATiebreakSetForPlayerB, scoreLineProbabilitiesInAAdvantageSetForPlayerA, scoreLineProbabilitiesInAAdvantageSetForPlayerB, probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerA, probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerB, probabilitiesOfWinningAdvantageSetMatchBestOf5SetsForPlayerA, probabilitiesOfWinningAdvantageSetMatchBestOf5SetsForPlayerB);
        }

        /// <summary>
        /// Calculates the probability of winning a tennis game for both player A and player B,
        /// given the probabilities of each player winning a point while serving.
        /// </summary>
        /// <param name="probAOnServe">Probability of player A winning a point while serving.</param>
        /// <param name="probBOnServe">Probability of player B winning a point while serving.</param>
        /// <returns>
        /// Two 2D arrays representing the game-winning probabilities for each possible score. 
        /// The first array represents the probabilities for player A and the second array for player B.
        /// Each index in the array represents a possible score, and the value at that index is the calculated probability.
        /// </returns>
        /// <remarks>
        /// This method uses dynamic programming to calculate the game-winning probabilities. 
        /// It iterates over each possible score and updates the probability based on the serving player and 
        /// the outcomes of subsequent points.
        /// </remarks>
        static (double[,], double[,]) CalculateGameProbabilities(double probAOnServe, double probBOnServe)
        {
            // Create matrices to store the game-winning probabilities for both players
            double[,] probA = new double[5, 5];
            double[,] probB = new double[5, 5];

            // Iterate over both players (player 0 = player A, player 1 = player B)
            for (int player = 0; player < 2; player++)
            {
                // Determine the serving player's probability of winning a point (p) and losing a point (q)
                double p = player == 0 ? probAOnServe : probBOnServe;
                double q = 1 - p;

                // Select the correct probability matrix for the current player
                double[,] prob = player == 0 ? probA : probB;

                // Set the initial game-winning probabilities for situations where the game outcome is already decided
                prob[4, 0] = prob[4, 1] = prob[4, 2] = 1; // Player has already won
                prob[0, 4] = prob[1, 4] = prob[2, 4] = 0; // Player has already lost

                // Calculate the probability for the deuce situation (40-40)
                prob[3, 3] = p * p / (p * p + q * q);

                // Calculate probabilities for all other game scores using dynamic programming
                for (int i = 3; i >= 0; i--)
                {
                    for (int j = 3; j >= 0; j--)
                    {
                        if (i == 3 && j == 3) continue; // Skip the deuce situation which was already calculated
                        prob[i, j] = p * prob[i + 1, j] + q * prob[i, j + 1]; // Use previous results to calculate the current probability
                    }
                }
            }

            // Return the matrices containing the game-winning probabilities for players A and B
            return (probA, probB);
        }

        /// <summary>
        /// Calculates the probability of winning a tennis tiebreak for both player A and player B,
        /// given the probabilities of each player winning a point while serving and returning.
        /// </summary>
        /// <param name="probAOnServe">Probability of player A winning a point while serving.</param>
        /// <param name="probBOnServe">Probability of player B winning a point while serving.</param>
        /// <returns>
        /// Two 2D arrays representing the tiebreak-winning probabilities for each possible score while serving and returning.
        /// The first array represents the probabilities while serving, and the second array for while returning.
        /// Each index in the array represents a possible score, and the value at that index is the calculated probability.
        /// </returns>
        /// <remarks>
        /// This method uses dynamic programming to calculate the tiebreak-winning probabilities. 
        /// It iterates over each possible score and updates the probability based on the serving player, the returning player and 
        /// the outcomes of subsequent points.
        /// </remarks>
        static (double[,], double[,]) CalculateTiebreakProbabilities(double probAOnServe, double probBOnServe)
        {
            // Create matrices to store the tiebreak-winning probabilities for both players while serving and returning
            double[,] tiebreakWinOnServe = new double[8, 8];
            double[,] tiebreakWinOnReturn = new double[8, 8];

            // Calculate the probability of each player winning a point while returning serve
            double probAOnReturnB = 1 - probBOnServe;
            double probBOnReturnA = 1 - probAOnServe;

            // Set the initial tiebreak-winning probabilities for the situation where both players have 6 points
            tiebreakWinOnServe[6, 6] = tiebreakWinOnReturn[6, 6] = (probAOnServe * probAOnReturnB) / (probBOnServe * probBOnReturnA + probAOnServe * probAOnReturnB);

            // Initialize the probabilities for situations where one player reaches 7 points
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 7; j >= 0; j--)
                {
                    if (i == 7) // Player A wins the tiebreak
                    {
                        tiebreakWinOnServe[i, j] = 1;
                        tiebreakWinOnReturn[i, j] = 1;
                    }
                    else if (j == 7) // Player B wins the tiebreak
                    {
                        tiebreakWinOnServe[i, j] = 0;
                        tiebreakWinOnReturn[i, j] = 0;
                    }
                }
            }

            // Calculate probabilities for each possible score combination in tiebreak using dynamic programming
            for (int i = 6; i >= 0; i--)
            {
                for (int j = 6; j >= 0; j--)
                {
                    if (!(i == 6 && j == 6)) // Skip the initial condition which was already calculated
                    {
                        bool isEven = ((i + j) % 2) == 0;
                        // Update the probabilities based on whether the total score is even or odd
                        if (isEven)
                        {
                            tiebreakWinOnServe[i, j] = probAOnServe * tiebreakWinOnReturn[i + 1, j] + probBOnReturnA * tiebreakWinOnReturn[i, j + 1];
                            tiebreakWinOnReturn[i, j] = probAOnReturnB * tiebreakWinOnServe[i + 1, j] + probBOnServe * tiebreakWinOnServe[i, j + 1];
                        }
                        else
                        {
                            tiebreakWinOnServe[i, j] = probAOnServe * tiebreakWinOnServe[i + 1, j] + probBOnReturnA * tiebreakWinOnServe[i, j + 1];
                            tiebreakWinOnReturn[i, j] = probAOnReturnB * tiebreakWinOnReturn[i + 1, j] + probBOnServe * tiebreakWinOnReturn[i, j + 1];
                        }
                    }
                }
            }

            // Return the matrices containing the tiebreak-winning probabilities for both players while serving and returning
            return (tiebreakWinOnServe, tiebreakWinOnReturn);
        }

        /// <summary>
        /// Calculates the probability of winning a tiebreak set for both player A and player B,
        /// given the probabilities of winning a tiebreak while serving and returning, and the game winning probabilities.
        /// </summary>
        /// <param name="tiebreakWinOnServe">The probability of winning a tiebreak while serving.</param>
        /// <param name="tiebreakWinOnReturn">The probability of winning a tiebreak while returning.</param>
        /// <param name="gameWinProbA">The probability of player A winning a game.</param>
        /// <param name="gameWinProbB">The probability of player B winning a game.</param>
        /// <returns>
        /// Two 2D arrays representing the tiebreak set-winning probabilities for both players while serving and returning.
        /// The first array represents the probabilities while serving, and the second array for while returning.
        /// Each index in the array represents a possible score, and the value at that index is the calculated probability.
        /// </returns>
        /// <remarks>
        /// This method iterates over each possible score and calculates the probabilities based on the current score, 
        /// whether the player is serving or returning, and the outcomes of subsequent games.
        /// </remarks>
        public static (double[,], double[,]) CalculateWinningTiebreakSet(double tiebreakWinOnServe, double tiebreakWinOnReturn, double gameWinProbA, double gameWinProbB)
        {
            // Create matrices to store the tiebreak set-winning probabilities for both players while serving and returning
            double[,] winningTiebreakSetOnServe = new double[8, 8];
            double[,] winningTiebreakSetOnReturn = new double[8, 8];

            // Iterate over each possible score
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    // Set the tiebreak set-winning probabilities based on the current score
                    if (i == 6 && j == 6)
                    {
                        winningTiebreakSetOnServe[i, j] = tiebreakWinOnServe;
                        winningTiebreakSetOnReturn[i, j] = tiebreakWinOnReturn;
                    }
                    else if ((i == 6 && j == 7) || (i == 5 && j == 7))
                    {
                        winningTiebreakSetOnServe[i, j] = 0;
                        winningTiebreakSetOnReturn[i, j] = 0;
                    }
                    else if ((i == 7 && j == 5) || (i == 7 && j == 6))
                    {
                        winningTiebreakSetOnServe[i, j] = 1;
                        winningTiebreakSetOnReturn[i, j] = 1;
                    }
                    else if (i == 6 && j >= 0 && j <= 4)
                    {
                        winningTiebreakSetOnServe[i, j] = 1;
                        winningTiebreakSetOnReturn[i, j] = 1;
                    }
                    else if (j == 6 && i >= 0 && i <= 4)
                    {
                        winningTiebreakSetOnServe[i, j] = 0;
                        winningTiebreakSetOnReturn[i, j] = 0;
                    }
                }
            }

            // Update the tiebreak set-winning probabilities for certain scores based on the game winning probabilities
            winningTiebreakSetOnServe[5, 6] = gameWinProbA * winningTiebreakSetOnServe[6, 6] + (1 - gameWinProbA) * winningTiebreakSetOnServe[5, 7];
            winningTiebreakSetOnReturn[5, 6] = (1 - gameWinProbB) * winningTiebreakSetOnReturn[6, 6] + gameWinProbB * winningTiebreakSetOnReturn[5, 7];
            winningTiebreakSetOnServe[6, 5] = gameWinProbA * winningTiebreakSetOnServe[7, 5] + (1 - gameWinProbA) * winningTiebreakSetOnServe[6, 6];
            winningTiebreakSetOnReturn[6, 5] = (1 - gameWinProbB) * winningTiebreakSetOnReturn[7, 5] + gameWinProbB * winningTiebreakSetOnReturn[6, 6];

            // Calculate the tiebreak set-winning probabilities for the remaining scores using dynamic programming
            for (int i = 5; i >= 0; i--)
            {
                for (int j = 5; j >= 0; j--)
                {
                    winningTiebreakSetOnServe[i, j] = gameWinProbA * winningTiebreakSetOnReturn[i + 1, j] + (1 - gameWinProbA) * winningTiebreakSetOnReturn[i, j + 1];
                    winningTiebreakSetOnReturn[i, j] = (1 - gameWinProbB) * winningTiebreakSetOnServe[i + 1, j] + gameWinProbB * winningTiebreakSetOnServe[i, j + 1];
                }
            }

            // Return the tiebreak set-winning probabilities for both players while serving and returning
            return (winningTiebreakSetOnServe, winningTiebreakSetOnReturn);
        }

        /// <summary>
        /// Calculates the probability of winning an advantage set for both player A and player B,
        /// given the game winning probabilities.
        /// </summary>
        /// <param name="gameWinProbA">The probability of player A winning a game.</param>
        /// <param name="gameWinProbB">The probability of player B winning a game.</param>
        /// <returns>
        /// Two 2D arrays representing the advantage set-winning probabilities for both players while serving and returning.
        /// The first array represents the probabilities while serving, and the second array for while returning.
        /// Each index in the array represents a possible score, and the value at that index is the calculated probability.
        /// </returns>
        /// <remarks>
        /// This method iterates over each possible score and calculates the probabilities based on the current score, 
        /// whether the player is serving or returning, and the outcomes of subsequent games.
        /// </remarks>
        public static (double[,], double[,]) CalculateWinningAdvantageSet(double gameWinProbA, double gameWinProbB)
        {
            // Create matrices to store the advantage set-winning probabilities for both players while serving and returning
            double[,] advantageSetOnServe = new double[7, 7];
            double[,] advantageSetOnReturn = new double[7, 7];

            // Initialize the last rows and columns of the matrices
            for (int i = 0; i < 6; i++)
            {
                advantageSetOnServe[i, 6] = 0;
                advantageSetOnServe[6, i] = 1;
                advantageSetOnReturn[i, 6] = 0;
                advantageSetOnReturn[6, i] = 1;
            }

            // Calculate the advantage set-winning probabilities for the remaining scores using dynamic programming
            for (int i = 5; i >= 0; i--)
            {
                for (int j = 5; j >= 0; j--)
                {
                    if (i == 5 && j == 5)
                    {
                        advantageSetOnServe[5, 5] = (gameWinProbA * (1 - gameWinProbB)) / (gameWinProbA * (1 - gameWinProbB) + (1 - gameWinProbA) * gameWinProbB);
                        advantageSetOnReturn[5, 5] = advantageSetOnServe[5, 5];
                    }
                    else
                    {
                        advantageSetOnServe[i, j] = gameWinProbA * advantageSetOnReturn[i + 1, j] + (1 - gameWinProbA) * advantageSetOnReturn[i, j + 1];
                        advantageSetOnReturn[i, j] = (1 - gameWinProbB) * advantageSetOnServe[i + 1, j] + gameWinProbB * advantageSetOnServe[i, j + 1];
                    }
                }
            }

            // Return the advantage set-winning probabilities for both players while serving and returning
            return (advantageSetOnServe, advantageSetOnReturn);
        }

        /// <summary>
        /// Calculates the probabilities of each player winning a tiebreak set match (best of 2 sets), given their probabilities of winning a tiebreak set on serve and return.
        /// </summary>
        /// <param name="winningTiebreakSetOnServe">The probability of the player winning a tiebreak set while serving.</param>
        /// <param name="winningTiebreakSetOnReturn">The probability of the player winning a tiebreak set while returning.</param>
        /// <returns>
        /// A 3x3 matrix representing the match-winning probabilities for the player when the score is 6:6 in sets.
        /// Each cell in the matrix corresponds to a possible score (sets won by player A, sets won by player B),
        /// and the value in the cell is the calculated probability of the player winning the match from that score.
        /// </returns>
        /// <remarks>
        /// This method first hardcodes the probabilities for the cases where a player has already won or lost the match.
        /// It then calculates the probabilities for the remaining scores using dynamic programming.
        /// </remarks>
        public static double[,] CalculateWinningAllTiebreakSetMatchBestOf2Sets(double winningTiebreakSetOnServe, double winningTiebreakSetOnReturn)
        {
            // Create a matrix to store the match-winning probabilities
            double[,] allTiebreakSetMatch = new double[3, 3];

            // Initialize the matrix cells for the cases where a player has already won or lost the match
            allTiebreakSetMatch[2, 0] = 1;
            allTiebreakSetMatch[0, 2] = 0;
            allTiebreakSetMatch[2, 1] = 1;
            allTiebreakSetMatch[1, 2] = 0;

            // Calculate the match-winning probabilities for the remaining scores using the probabilities of winning a tiebreak set on serve and return
            allTiebreakSetMatch[1, 1] = winningTiebreakSetOnServe;
            allTiebreakSetMatch[1, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 1];
            allTiebreakSetMatch[0, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 2];
            allTiebreakSetMatch[0, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 1];

            // Return the matrix of match-winning probabilities
            return allTiebreakSetMatch;
        }

        /// <summary>
        /// Calculates the probabilities of each player winning an all tiebreak set match (best of 3 sets), given their probabilities of winning a tiebreak set on serve and return.
        /// </summary>
        /// <param name="winningTiebreakSetOnServe">The probability of the player winning a tiebreak set while serving.</param>
        /// <param name="winningTiebreakSetOnReturn">The probability of the player winning a tiebreak set while returning.</param>
        /// <returns>
        /// A 4x4 matrix representing the match-winning probabilities for the player when the score is 6:6 in sets.
        /// Each cell in the matrix corresponds to a possible score (sets won by player A, sets won by player B),
        /// and the value in the cell is the calculated probability of the player winning the match from that score.
        /// </returns>
        /// <remarks>
        /// This method first hardcodes the probabilities for the cases where a player has already won or lost the match.
        /// It then calculates the probabilities for the remaining scores using dynamic programming.
        /// </remarks>
        public static double[,] CalculateWinningAllTiebreakSetMatchBestOf3Sets(double winningTiebreakSetOnServe, double winningTiebreakSetOnReturn)
        {
            // Create a matrix to store the match-winning probabilities
            double[,] allTiebreakSetMatch = new double[4, 4];

            // Initialize the matrix cells for the cases where a player has already won or lost the match
            allTiebreakSetMatch[3, 0] = 1;
            allTiebreakSetMatch[0, 3] = 0;
            allTiebreakSetMatch[3, 1] = 1;
            allTiebreakSetMatch[1, 3] = 0;
            allTiebreakSetMatch[3, 2] = 1;
            allTiebreakSetMatch[2, 3] = 0;

            // Calculate the match-winning probabilities for the remaining scores using the probabilities of winning a tiebreak set on serve and return
            allTiebreakSetMatch[2, 2] = winningTiebreakSetOnServe;
            allTiebreakSetMatch[2, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[3, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[2, 2];
            allTiebreakSetMatch[1, 2] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 2] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 3];
            allTiebreakSetMatch[1, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 2];
            allTiebreakSetMatch[2, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[3, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[2, 1];
            allTiebreakSetMatch[0, 2] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 2] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 3];
            allTiebreakSetMatch[1, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 1];
            allTiebreakSetMatch[0, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 2];
            allTiebreakSetMatch[0, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 1];

            // Return the matrix of match-winning probabilities
            return allTiebreakSetMatch;
        }

        /// <summary>
        /// Calculates the probabilities of each player winning an advantage set match (best of 2 sets), given their probabilities of winning an advantage set on serve and a tiebreak set on return.
        /// </summary>
        /// <param name="winningAdvantageSetOnServe">The probability of the player winning an advantage set while serving.</param>
        /// <param name="winningTiebreakSetOnReturn">The probability of the player winning a tiebreak set while returning.</param>
        /// <returns>
        /// A 3x3 matrix representing the match-winning probabilities for the player when the score is 6:6 in sets.
        /// Each cell in the matrix corresponds to a possible score (sets won by player A, sets won by player B),
        /// and the value in the cell is the calculated probability of the player winning the match from that score.
        /// </returns>
        /// <remarks>
        /// This method first hardcodes the probabilities for the cases where a player has already won or lost the match.
        /// It then calculates the probabilities for the remaining scores using dynamic programming.
        /// </remarks>
        public static double[,] CalculateWinningAdvantageSetMatchBestOf2Sets(double winningAdvantageSetOnServe, double winningTiebreakSetOnReturn)
        {
            // Create a matrix to store the match-winning probabilities
            double[,] allTiebreakSetMatch = new double[3, 3];

            // Initialize the matrix cells for the cases where a player has already won or lost the match
            allTiebreakSetMatch[2, 0] = 1;
            allTiebreakSetMatch[0, 2] = 0;
            allTiebreakSetMatch[2, 1] = 1;
            allTiebreakSetMatch[1, 2] = 0;

            // Calculate the match-winning probabilities for the remaining scores using the probabilities of winning an advantage set on serve and a tiebreak set on return
            allTiebreakSetMatch[1, 1] = winningAdvantageSetOnServe;
            allTiebreakSetMatch[1, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 1];
            allTiebreakSetMatch[0, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 2];
            allTiebreakSetMatch[0, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 1];

            // Return the matrix of match-winning probabilities
            return allTiebreakSetMatch;
        }

        /// <summary>
        /// Calculates the probabilities of each player winning an advantage set match (best of 3 sets), given their probabilities of winning an advantage set on serve and a tiebreak set on return.
        /// </summary>
        /// <param name="winningAdvantageSetOnServe">The probability of the player winning an advantage set while serving.</param>
        /// <param name="winningTiebreakSetOnReturn">The probability of the player winning a tiebreak set while returning.</param>
        /// <returns>
        /// A 4x4 matrix representing the match-winning probabilities for the player when the score is 6:6 in sets.
        /// Each cell in the matrix corresponds to a possible score (sets won by player A, sets won by player B),
        /// and the value in the cell is the calculated probability of the player winning the match from that score.
        /// </returns>
        /// <remarks>
        /// This method first hardcodes the probabilities for the cases where a player has already won or lost the match.
        /// It then calculates the probabilities for the remaining scores using dynamic programming.
        /// </remarks>
        public static double[,] CalculateWinningAdvantageSetMatchBestOf3Sets(double winningAdvantageSetOnServe, double winningTiebreakSetOnReturn)
        {
            // Create a matrix to store the match-winning probabilities
            double[,] allTiebreakSetMatch = new double[4, 4];

            // Initialize the matrix cells for the cases where a player has already won or lost the match
            allTiebreakSetMatch[3, 0] = 1;
            allTiebreakSetMatch[0, 3] = 0;
            allTiebreakSetMatch[3, 1] = 1;
            allTiebreakSetMatch[1, 3] = 0;
            allTiebreakSetMatch[3, 2] = 1;
            allTiebreakSetMatch[2, 3] = 0;

            // Calculate the match-winning probabilities for the remaining scores using the probabilities of winning an advantage set on serve and a tiebreak set on return
            allTiebreakSetMatch[2, 2] = winningAdvantageSetOnServe;
            allTiebreakSetMatch[2, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[3, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[2, 2];
            allTiebreakSetMatch[1, 2] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 2] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 3];
            allTiebreakSetMatch[1, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 2];
            allTiebreakSetMatch[2, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[3, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[2, 1];
            allTiebreakSetMatch[0, 2] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 2] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 3];
            allTiebreakSetMatch[1, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[2, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[1, 1];
            allTiebreakSetMatch[0, 1] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 1] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 2];
            allTiebreakSetMatch[0, 0] = winningTiebreakSetOnReturn * allTiebreakSetMatch[1, 0] + (1 - winningTiebreakSetOnReturn) * allTiebreakSetMatch[0, 1];

            // Return the matrix of match-winning probabilities
            return allTiebreakSetMatch;
        }

        /// <summary>
        /// Calculates the scoreline probabilities for each player in a tennis game, given their serving probabilities.
        /// </summary>
        /// <param name="probAOnServe">The probability of Player A winning a point while serving.</param>
        /// <param name="probBOnServe">The probability of Player B winning a point while serving.</param>
        /// <returns>
        /// Two 7x7 matrices representing the scoreline probabilities for each player.
        /// Each cell in the matrices corresponds to a possible score (points won by player A, points won by player B),
        /// and the value in the cell is the calculated probability of reaching that score.
        /// </returns>
        /// <remarks>
        /// This method first initializes the matrices with base cases.
        /// It then calculates the probabilities for the remaining scores using dynamic programming.
        /// </remarks>
        public static (double[,], double[,]) CalculateScoreLineProbabilitiesInAGame(double probAOnServe, double probBOnServe)
        {
            // Initialize probability matrices for players A and B
            double[,] probMatrixA = new double[7, 7];
            double[,] probMatrixB = new double[7, 7];
            double[,] probA = new double[5, 5];
            double[,] probB = new double[5, 5];

            // Set initial values for probability matrices based on base cases
            for (int i = 2; i < 7; i++)
            {
                probMatrixA[i, 0] = probMatrixB[i, 0] = probMatrixA[0, i] = probMatrixB[0, i] = i - 2;
            }

            // Calculate probabilities for each possible scoreline
            for (int i = 2; i <= 6; i++)
            {
                for (int j = 2; j <= 6; j++)
                {
                    // Base case: both players start the game with zero points
                    if (i == 2 & j == 2)
                    {
                        probMatrixA[2, 2] = probMatrixB[2, 2] = 1;
                    }
                    // Calculate probabilities for all other scorelines, excluding deuce and advantage scenarios
                    else if (!(i == 6 & j == 5) & !(i == 5 && j == 6) & !(i == 6 & j == 6))
                    {
                        // Conditions and probability calculations for Player A's serve scenarios
                        if ((probMatrixA[i, 0] == 4 && probMatrixA[0, j] >= 0 && probMatrixA[0, j] <= 2) ||
                            (probMatrixA[0, j] == 0 && probMatrixA[i, 0] >= 1 && probMatrixA[i, 0] <= 4) ||
                            (probMatrixA[i, 0] >= 3 && probMatrixA[0, j] >= 3 && probMatrixA[i, 0] == probMatrixA[0, j] + 1) ||
                            (probMatrixA[i, 0] >= 3 && probMatrixA[0, j] >= 3 && probMatrixA[i, 0] == probMatrixA[0, j] + 2))
                        {
                            probMatrixB[i, j] = probBOnServe * probMatrixB[i - 1, j];
                            probMatrixA[i, j] = probAOnServe * probMatrixA[i - 1, j];
                        }
                        // Conditions and probability calculations for Player B's serve scenarios
                        else if ((probMatrixA[0, j] == 4 && probMatrixA[i, 0] >= 0 && probMatrixA[i, 0] <= 2) ||
                            (probMatrixA[i, 0] == 0 && probMatrixA[0, j] >= 1 && probMatrixA[0, j] <= 4) ||
                            (probMatrixA[i, 0] >= 3 && probMatrixA[0, j] >= 3 && probMatrixA[0, j] == probMatrixA[i, 0] + 1) ||
                            (probMatrixA[i, 0] >= 3 && probMatrixA[0, j] >= 3 && probMatrixA[0, j] == probMatrixA[i, 0] + 2))
                        {
                            probMatrixB[i, j] = (1 - probBOnServe) * probMatrixB[i, j - 1];
                            probMatrixA[i, j] = (1 - probAOnServe) * probMatrixA[i, j - 1];
                        }
                        // Conditions and probability calculations for deuce and advantage scenarios
                        else if ((probMatrixA[i, 0] >= 1 && probMatrixA[i, 0] <= 3 && probMatrixA[0, j] >= 1 && probMatrixA[0, j] <= 3) ||
                            (probMatrixA[i, 0] >= 4 && probMatrixA[0, j] >= 4 && probMatrixA[i, 0] == probMatrixA[0, j]))
                        {
                            probMatrixB[i, j] = probBOnServe * probMatrixB[i - 1, j] + (1 - probBOnServe) * probMatrixB[i, j - 1];
                            probMatrixA[i, j] = probAOnServe * probMatrixA[i - 1, j] + (1 - probAOnServe) * probMatrixA[i, j - 1];
                        }
                        // If none of the above conditions are met, set the probability for this scoreline to zero
                        else
                        {
                            probMatrixA[i, j] = probMatrixB[i, j] = 0;
                        }
                    }
                }
            }

            // This block of code is for copying data from the larger 7x7 matrices probMatrixA and probMatrixB 
            // into the smaller 5x5 matrices probA and probB, respectively. 
            // The offsets of +2 in the source matrices (i.e., probMatrixA[i + 2, j + 2]) are used to skip the 
            // first two rows and columns. The result is a condensed representation of the original matrices,
            // containing only the calculated probabilities of interest.
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    probA[i, j] = probMatrixA[i + 2, j + 2];
                    probB[i, j] = probMatrixB[i + 2, j + 2];
                }
            }

            // Return the calculated scoreline probability matrices for both players
            return (probA, probB);
        }

        /// <summary>
        /// Calculates the probabilities of winning a tiebreak game for two players (A and B), given their individual probabilities of winning a point while serving.
        /// </summary>
        /// <param name="pA">The probability of player A winning a point while serving.</param>
        /// <param name="pB">The probability of player B winning a point while serving.</param>
        /// <returns>
        /// Two 8x8 matrices representing the game-winning probabilities for players A and B for all possible scores in a tiebreak game.
        /// Each cell in the matrix corresponds to a possible score (points won by player A, points won by player B),
        /// and the value in the cell is the calculated probability of the player winning the game from that score.
        /// </returns>
        /// <remarks>
        /// This method calculates the probabilities using dynamic programming. The probabilities depend on the current score and the server,
        /// which changes after every two points until the score reaches 6:6, after which the server changes after every point.
        /// The method first initializes the probabilities for the case where both players have 0 points.
        /// It then hardcodes the probabilities for the cases where a player has already won or lost the game.
        /// Finally, it calculates the probabilities for the remaining scores.
        /// </remarks>
        public static (double[,], double[,]) CalculateTiebreakGameProbabilities(double pA, double pB)
        {
            // The complementary probabilities of winning a point while serving for players A and B.
            var qB = 1 - pB;
            var qA = 1 - pA;

            // The maximum number of points a player can score in a tiebreak game (8).
            int maxPoints = 8;

            // Matrices for storing the game-winning probabilities for players A and B.
            // Each cell in the matrix corresponds to a possible score (points won by player A, points won by player B).
            double[,] probA = new double[maxPoints, maxPoints];
            double[,] probB = new double[maxPoints, maxPoints];

            // Calculate the probabilities for all possible scores.
            for (int g = 0; g < maxPoints; g++)
            {
                for (int h = 0; h < maxPoints; h++)
                {
                    // Initialization for the case when both players have 0 points.
                    // The probabilities are set to 1 since a player cannot lose the game without the opponent scoring any points.
                    if (g == 0 && h == 0)
                    {
                        probA[g, h] = 1;
                        probB[g, h] = 1;
                    }
                    // For cases where the game is already decided (one player has 7 points and the other has less than 7 points), the probabilities are set to 0.
                    else if ((g == 7 && h == 7) || (g == 7 && h == 6) || (g == 6 && h == 7))
                    {
                        probA[g, h] = probB[g, h] = 0;
                    }
                    // Calculate the probabilities for the cases where one player has 7 points and the other has less than 7 points.
                    else if (g == 7)
                    {
                        probA[g, h] = probA[g - 1, h] * ((h == 0 || h == 3 || h == 4) ? qB : pA);
                        probB[g, h] = probB[g - 1, h] * ((h == 0 || h == 3 || h == 4) ? pA : qB);
                    }
                    else if (h == 7)
                    {
                        probA[g, h] = probA[g, h - 1] * ((g == 0 || g == 3 || g == 4) ? pB : qA);
                        probB[g, h] = probB[g, h - 1] * ((g == 0 || g == 3 || g == 4) ? qA : pB);
                    }
                    // Calculate the probabilities for all other scores.
                    else
                    {
                        // The server changes after every two points until the score reaches 6:6, after which the server changes after every point.
                        // Therefore, we calculate the probabilities differently depending on whether the total number of points is odd or even.
                        if (g > 0)
                        {
                            var isOdd = ((g - 1 + h) % 4 == 1 || (g - 1 + h) % 4 == 2);
                            probA[g, h] += (isOdd ? qB : pA) * probA[g - 1, h];
                            probB[g, h] += (isOdd ? pA : qB) * probB[g - 1, h];
                        }
                        if (h > 0)
                        {
                            var isOdd = ((g + h - 1) % 4 == 1 || (g + h - 1) % 4 == 2);
                            probA[g, h] += (isOdd ? pB : qA) * probA[g, h - 1];
                            probB[g, h] += (isOdd ? qA : pB) * probB[g, h - 1];
                        }
                    }
                }
            }

            // Return the matrices containing the calculated game-winning probabilities for players A and B.
            return (probA, probB);
        }

        /// <summary>
        /// Calculates the probability of each possible score in a tennis tie-break game.
        /// The method takes into account the server and receiver game winning probabilities 
        /// and calculates all possible outcomes of a tie-break.
        /// </summary>
        /// <param name="tbs">Probability of player A (server) winning a point on serve.</param>
        /// <param name="tbr">Probability of player B (receiver) winning a point on serve.</param>
        /// <param name="gameWinProbA">2D array representing the probability of player A winning a game while serving, based on the current score.</param>
        /// <param name="gameWinProbB">2D array representing the probability of player B winning a game while serving, based on the current score.</param>
        /// <returns>
        /// Two 2D arrays representing the probability of each possible score in the tie-break game. 
        /// The first array is for the server (player A) and the second one for the receiver (player B).
        /// Each index in the array represents a possible score, and the value at that index is the calculated probability.
        /// </returns>
        /// <remarks>
        /// This method uses a Markov chain model to calculate the probabilities. It iterates over each possible score and updates 
        /// the probability based on the current server and the outcome of the previous point.
        /// For example, if the current server is player A and he wins the point, the new probability for that score is the 
        /// probability of the previous score times the probability of player A winning a point on serve. 
        /// Similar logic applies when the server loses the point.
        /// </remarks>
        public static (double[,], double[,]) CalculateTiebreakSetScoreProbabilities(double tbs, double tbr, double[,] gameWinProbA, double[,] gameWinProbB)
        {
            // The maximum number of points.
            int maxPoints = 8;

            // Matrices for storing winning probabilities for players A and B.
            double[,] probA = new double[maxPoints, maxPoints];
            double[,] probB = new double[maxPoints, maxPoints];

            // Winning probabilities for a game on serve for both players. They are also used to calculate the losing probabilities.
            double pgA = gameWinProbA[0, 0];
            double pgB = gameWinProbB[0, 0];

            // Losing probabilities for a game on serve for both players.
            double qgA = 1 - gameWinProbA[0, 0];
            double qgB = 1 - gameWinProbB[0, 0];

            // We calculate the winning probability for every possible combination of points that can be in a game. 
            // This is done by iterating through all possible combinations of points for both players.
            for (int j = 0; j <= maxPoints; j++)
            {
                for (int i = 0; i <= maxPoints; i++)
                {
                    // Check if we have a situation where the game can end with the current combination of points.
                    // In a tie-break, the game can end if a player has 7 points and the other player has less than 7 points.
                    if ((i == 7 && j == 6) || (i == 6 && j == 7))
                    {
                        // The probability of player A winning in this situation is the probability of player A winning in the previous situation (i-1, j) times the probability of player A winning a point on serve.
                        // The probability of player B winning in this situation is the probability of player B winning in the previous situation (i-1, j) times the probability of player B winning a point on serve.
                        if (i == 7 && j == 6)
                        {
                            probA[i, j] = probA[i - 1, j] * (tbs);
                            probB[i, j] = probB[i - 1, j] * (tbr);
                        }
                        if (i == 6 && j == 7)
                        {
                            probA[i, j] = probA[i, j - 1] * (1 - tbs);
                            probB[i, j] = probB[i, j - 1] * (1 - tbr);
                        }
                    }
                    // Initial condition. At the start of the game, the probabilities are equal.
                    else if (i == 0 && j == 0)
                    {
                        probA[i, j] = probB[i, j] = 1;
                    }
                    // Check the case when player A is serving and wins the point.
                    // We calculate the probabilities for players A and B for this situation based on the previous probabilities and the winning and losing probabilities on serve.
                    else if ((i == 1 && j == 0) || (i == 3 && j == 0) || (i == 5 && j == 0) || (i == 6 && j == 1) || (i == 6 && j == 3) || (i == 6 && j == 5))
                    {
                        probA[i, j] = probA[i - 1, j] * pgA;
                        probB[i, j] = probB[i - 1, j] * qgB;
                    }
                    // Other case when player A is serving, but loses the point.
                    else if ((i == 2 && j == 0) || (i == 4 && j == 0) || (i == 6 && j == 0) || (i == 6 && j == 2) || (i == 6 && j == 4) || (i == 7 && j == 5))
                    {
                        probA[i, j] = probA[i - 1, j] * qgB;
                        probB[i, j] = probB[i - 1, j] * pgA;
                    }
                    // Check the case when player B is serving and wins the point.
                    else if ((i == 0 && j == 1) || (i == 0 && j == 3) || (i == 0 && j == 5) || (i == 1 && j == 6) || (i == 3 && j == 6) || (i == 5 && j == 6))
                    {
                        probA[i, j] = probA[i, j - 1] * qgA;
                        probB[i, j] = probB[i, j - 1] * pgB;
                    }
                    // Other case when player B is serving, but loses the point.
                    else if ((i == 0 && j == 2) || (i == 0 && j == 4) || (i == 0 && j == 6) || (i == 2 && j == 6) || (i == 4 && j == 6) || (i == 5 && j == 7))
                    {
                        probA[i, j] = probA[i, j - 1] * pgB;
                        probB[i, j] = probB[i, j - 1] * qgA;
                    }
                    // General case when player A is serving.
                    else if ((i == 2 && j == 1) || (i == 4 && j == 1) || (i == 1 && j == 2) || (i == 3 && j == 2) || (i == 5 && j == 2) || (i == 2 && j == 3) || (i == 4 && j == 3) || (i == 1 && j == 4) || (i == 3 && j == 4) || (i == 5 && j == 4) || (i == 2 && j == 5) || (i == 4 && j == 5))
                    {
                        probA[i, j] = probA[i - 1, j] * pgA + probA[i, j - 1] * qgA;
                        probB[i, j] = probB[i - 1, j] * qgB + probB[i, j - 1] * pgB;
                    }
                    // General case when player B is serving.
                    else if ((i == 1 && j == 1) || (i == 3 && j == 1) || (i == 5 && j == 1) || (i == 2 && j == 2) || (i == 4 && j == 2) || (i == 1 && j == 3) || (i == 3 && j == 3) || (i == 5 && j == 3) || (i == 2 && j == 4) || (i == 4 && j == 4) || (i == 1 && j == 5) || (i == 3 && j == 5) || (i == 5 && j == 5) || (i == 6 && j == 6))
                    {
                        probA[i, j] = probA[i - 1, j] * qgB + probA[i, j - 1] * pgB;
                        probB[i, j] = probB[i - 1, j] * pgA + probB[i, j - 1] * qgA;
                    }
                }
            }
            // Return the matrices containing the probabilities for players A and B.
            return (probA, probB);
        }

        /// <summary>
        /// CalculateAdvantageSetScoreProbabilities Method
        /// This method calculates the probabilities of winning all possible scorelines in an advantage set for two players: Player A and Player B.
        /// The method takes as input the probabilities of Player A winning a game when serving and when returning. The result of the function is two 2D arrays containing the probabilities of each possible scoreline for Player A and Player B, respectively.
        /// The function covers all possible scenarios and scorelines, considering that the set format is always an advantage set. 
        /// It is particularly useful in in-depth analysis of tennis matches where a conventional scoring system is used (advantage sets) instead of tiebreak sets. This can be crucial in grand slam matches where in some cases advantage sets are used in the final set.
        /// </summary>
        /// <param name="tbs">The probability of Player A winning a point when serving.</param>
        /// <param name="tbr">The probability of Player A winning a point when returning.</param>
        /// <param name="gameWinProbA">2D array containing the probabilities of Player A winning a game when serving at all possible scorelines.</param>
        /// <param name="gameWinProbB">2D array containing the probabilities of Player B winning a game when serving at all possible scorelines.</param>
        /// <returns>A tuple containing two 2D arrays of probabilities for each possible scoreline for Player A and Player B, respectively.</returns>
        public static (double[,], double[,]) CalculateAdvantageSetScoreProbabilities(double tbs, double tbr, double[,] gameWinProbA, double[,] gameWinProbB)
        {
            // The maximum number of points.
            int maxPoints = 13;

            // Matrices for storing winning probabilities for players A and B.
            double[,] probA = new double[maxPoints + 1, maxPoints + 1];
            double[,] probB = new double[maxPoints + 1, maxPoints + 1];

            // Winning probabilities for a game on serve for both players. They are also used to calculate the losing probabilities.
            double pgA = gameWinProbA[0, 0];
            double pgB = gameWinProbB[0, 0];

            // Losing probabilities for a game on serve for both players.
            double qgA = 1 - gameWinProbA[0, 0];
            double qgB = 1 - gameWinProbB[0, 0];

            // We calculate the winning probability for every possible combination of points that can be in a game. 
            // This is done by iterating through all possible combinations of points for both players.
            for (int j = 0; j <= maxPoints; j++)
            {
                for (int i = 0; i <= maxPoints; i++)
                {
                    if (i == 5 && j == 6)
                    {
                        string aaa = "";
                    }
                    if (i == 13 && j == 13)
                    { break; }
                    // Check if we have a situation where the game can end with the current combination of points.
                    // In a tie-break in an advantage set is played when the result is 12:12, the game can end if a player has 13 points and the other player has less than 13 points.
                    if ((i == 13 && j == 12) || (i == 12 && j == 13))
                    {
                        // The probability of player A winning in this situation is the probability of player A winning in the previous situation (i-1, j) times the probability of player A winning a point on serve.
                        // The probability of player B winning in this situation is the probability of player B winning in the previous situation (i-1, j) times the probability of player B winning a point on serve.
                        if (i == 13 && j == 12)
                        {
                            probA[i, j] = probA[i - 1, j] * (tbs);
                            probB[i, j] = probB[i - 1, j] * (tbr);
                        }
                        if (i == 12 && j == 13)
                        {
                            probA[i, j] = probA[i, j - 1] * (1 - tbs);
                            probB[i, j] = probB[i, j - 1] * (1 - tbr);
                        }
                    }
                    // Check if we have a situation where the game can end with the current combination of points.
                    // In a advantage set, the game continues until a player has 2 more points than the other after both have reached 6 points.
                    // Dodajte logiku za obračun vjerojatnosti od 6:6 do 12:12
                    else if (i > 6 && j > 6 && i <= maxPoints && j <= maxPoints)
                    {
                        if (Math.Abs(i - j) < 2) // The game is still undecided.
                        {
                            probA[i, j] = (i - 1 >= 0 ? probA[i - 1, j] * pgA : 0) + (j - 1 >= 0 ? probA[i, j - 1] * qgA : 0);
                            probB[i, j] = (i - 1 >= 0 ? probB[i - 1, j] * qgB : 0) + (j - 1 >= 0 ? probB[i, j - 1] * pgB : 0);
                        }
                        else if (i - j == 2) // Player A has won two games in a row.
                        {
                            probA[i, j] = (i - 1 >= 0 ? probA[i - 1, j] * pgA : 0);
                        }
                        else if (j - i == 2) // Player B has won two games in a row.
                        {
                            probB[i, j] = (j - 1 >= 0 ? probB[i, j - 1] * pgB : 0);
                        }
                        // Condition for 6:7, player B wins the point while player A is serving.
                        else if (i == 6 && j == 7)
                        {
                            probA[i, j] = (i - 1 >= 0 ? probA[i - 1, j] * qgA : 0);
                            probB[i, j] = (j - 1 >= 0 ? probB[i, j - 1] * pgB : 0);
                        }
                        // Condition for 7:6, player A wins the point while player B is serving.
                        else if (i == 7 && j == 6)
                        {
                            probA[i, j] = (i - 1 >= 0 ? probA[i - 1, j] * pgA : 0);
                            probB[i, j] = (j - 1 >= 0 ? probB[i, j - 1] * qgB : 0);
                        }
                    }
                    // Initial condition. At the start of the game, the probabilities are equal.
                    else if (i == 0 && j == 0)
                    {
                        probA[i, j] = probB[i, j] = 1;
                    }
                    // Check the case when player A is serving and wins the point.
                    // We calculate the probabilities for players A and B for this situation based on the previous probabilities and the winning and losing probabilities on serve.
                    else if ((i == 1 && j == 0) || (i == 3 && j == 0) || (i == 5 && j == 0) || (i == 6 && j == 1) || (i == 6 && j == 3) || (i == 6 && j == 5) ||
                             (i == 8 && j == 7) || (i == 10 && j == 9) || (i == 12 && j == 11))
                    {
                        probA[i, j] = probA[i - 1, j] * pgA;
                        probB[i, j] = probB[i - 1, j] * qgB;
                    }

                    // Other case when player A is serving, but loses the point.
                    else if ((i == 2 && j == 0) || (i == 4 && j == 0) || (i == 6 && j == 0) || (i == 6 && j == 2) || (i == 6 && j == 4) || (i == 7 && j == 5) ||
                             (i == 7 && j == 6) || (i == 9 && j == 8) || (i == 11 && j == 10))
                    {
                        probA[i, j] = probA[i - 1, j] * qgB;
                        probB[i, j] = probB[i - 1, j] * pgA;
                    }

                    // Check the case when player B is serving and wins the point.
                    else if ((i == 0 && j == 1) || (i == 0 && j == 3) || (i == 0 && j == 5) || (i == 1 && j == 6) || (i == 3 && j == 6) || (i == 5 && j == 6) ||
                             (i == 7 && j == 8) || (i == 9 && j == 10) || (i == 11 && j == 12))
                    {
                        probA[i, j] = probA[i, j - 1] * qgA;
                        probB[i, j] = probB[i, j - 1] * pgB;
                    }

                    // Other case when player B is serving, but loses the point.
                    else if ((i == 0 && j == 2) || (i == 0 && j == 4) || (i == 0 && j == 6) || (i == 2 && j == 6) || (i == 4 && j == 6) || (i == 5 && j == 7) ||
                             (i == 6 && j == 8) || (i == 8 && j == 10) || (i == 10 && j == 12))
                    {
                        probA[i, j] = probA[i, j - 1] * pgB;
                        probB[i, j] = probB[i, j - 1] * qgA;
                    }

                    // General case when player A is serving.
                    else if ((i == 2 && j == 1) || (i == 4 && j == 1) || (i == 1 && j == 2) || (i == 3 && j == 2) || (i == 5 && j == 2) || (i == 2 && j == 3) || (i == 4 && j == 3) || (i == 1 && j == 4) || (i == 3 && j == 4) || (i == 5 && j == 4) || (i == 2 && j == 5) || (i == 4 && j == 5) ||
                             (i == 6 && j == 7) || (i == 7 && j == 6) || (i == 8 && j == 7) || (i == 7 && j == 8) || (i == 9 && j == 8) || (i == 8 && j == 9) || (i == 10 && j == 9) || (i == 9 && j == 10) || (i == 11 && j == 10) || (i == 10 && j == 11) || (i == 11 && j == 12) || (i == 12 && j == 11))
                    {
                        probA[i, j] = probA[i - 1, j] * pgA + probA[i, j - 1] * qgA;
                        probB[i, j] = probB[i - 1, j] * qgB + probB[i, j - 1] * pgB;
                    }

                    // General case when player B is serving.

                    else if ((i == 1 && j == 1) || (i == 3 && j == 1) || (i == 5 && j == 1) || (i == 2 && j == 2) || (i == 4 && j == 2) || (i == 1 && j == 3) || (i == 3 && j == 3) || (i == 5 && j == 3) || (i == 2 && j == 4) || (i == 4 && j == 4) || (i == 1 && j == 5) || (i == 3 && j == 5) || (i == 5 && j == 5) ||
                             (i == 6 && j == 6) || (i == 7 && j == 7) || (i == 8 && j == 8) || (i == 9 && j == 9) || (i == 10 && j == 10) || (i == 11 && j == 11) || (i == 12 && j == 12))
                    {
                        probA[i, j] = probA[i - 1, j] * qgB + probA[i, j - 1] * pgB;
                        probB[i, j] = probB[i - 1, j] * pgA + probB[i, j - 1] * qgA;
                    }
                }
            }
            // Return the matrices containing the probabilities for players A and B.
            return (probA, probB);
        }

        /// <summary>
        /// CalculateWinningAllTiebreakSetMatchBestOf5Sets Method
        /// This method calculates the probabilities of winning a best of 5 sets match for all possible
        /// scorelines where all sets are tiebreak sets, for two players: Player A and Player B.
        /// The method takes as input the probabilities of Player A winning a tiebreak set when serving and 
        /// when returning. The result of the function is two 2D arrays containing the probabilities of 
        /// each possible scoreline for Player A and Player B, respectively.
        /// The function covers all the possible scenarios and scorelines, considering that the set format 
        /// is always a tiebreak set.
        /// This method helps to understand the dynamics of a match when only tiebreak sets are considered, 
        /// providing a comprehensive perspective of the different outcomes in a best of 5 sets match. 
        /// It's useful for an in-depth analysis where the serving strength of a player plays a vital role.
        /// </summary>
        /// <param name="winningTiebreakSetOnServe">The probability of Player A winning a tiebreak set when serving.</param>
        /// <param name="winningTiebreakSetOnReturn">The probability of Player A winning a tiebreak set when returning.</param>
        /// <returns>A tuple containing two 2D arrays of probabilities for each possible scoreline for Player A and Player B, respectively.</returns>
        public static (double[,], double[,]) CalculateWinningAllTiebreakSetMatchBestOf5Sets(double winningTiebreakSetOnServe, double winningTiebreakSetOnReturn)
        {
            // The number of sets required to win the match.
            int setsToWin = 3;

            // The 2D arrays representing the probabilities of each possible scoreline for Player A and Player B.
            double[,] probA = new double[setsToWin + 1, setsToWin + 1];
            double[,] probB = new double[setsToWin + 1, setsToWin + 1];

            // Initialize the starting condition where both players have won 0 sets.
            probA[0, 0] = 1;
            probB[0, 0] = 1;

            // Calculate the probability for all possible scorelines.
            for (int e = 0; e <= setsToWin; e++)
            {
                for (int f = 0; f <= setsToWin; f++)
                {
                    // If Player A has won at least one set...
                    if (e > 0)
                    {
                        // Previous probability of the scoreline before Player A won the current set.
                        // If Player B has already won the match, this probability becomes 0.
                        double prevProbA = (f < setsToWin) ? probA[e - 1, f] : 0;

                        // Update the probabilities after Player A wins a set on serve and Player B loses a set on return.
                        probA[e, f] += prevProbA * winningTiebreakSetOnServe;
                        probB[e, f] += prevProbA * (1 - winningTiebreakSetOnServe);
                    }

                    // If Player B has won at least one set...
                    if (f > 0)
                    {
                        // Previous probability of the scoreline before Player B won the current set.
                        // If Player A has already won the match, this probability becomes 0.
                        double prevProbB = (e < setsToWin) ? probA[e, f - 1] : 0;

                        // Update the probabilities after Player B wins a set on return and Player A loses a set on serve.
                        probA[e, f] += prevProbB * (1 - winningTiebreakSetOnReturn);
                        probB[e, f] += prevProbB * winningTiebreakSetOnReturn;
                    }
                }
            }

            // Return the probabilities of all possible scorelines for Player A and Player B.
            return (probA, probB);
        }

        /// <summary>
        /// CalculateAdvantageSetMatchBestOf5 Method
        /// This method calculates the probabilities of winning a best of 5 sets match for all possible
        /// scorelines where the first 4 sets are tiebreak sets and the last set is an advantage set, 
        /// for two players: Player A and Player B.
        /// The method takes as input the probabilities of Player A winning a tiebreak set and an advantage
        /// set when serving and returning.The result of the function is two 2D arrays containing the
        /// probabilities of each possible scoreline for Player A and Player B, respectively.
        /// The function covers all the possible scenarios and scorelines, considering the specific rule
        /// where the first four sets are tiebreak sets and the last set is an advantage set.
        /// This method adds depth to the analysis of a tennis match by taking into account different
        /// set formats, providing a more comprehensive perspective of the different outcomes in a
        /// best of 5 sets match.
        /// </summary>
        /// <param name="winningTiebreakSetOnServe">The probability of Player A winning a tiebreak set when serving.</param>
        /// <param name="winningAdvantageSetOnServe">The probability of Player A winning an advantage set when serving.</param>
        /// <param name="winningTiebreakSetOnReturn">The probability of Player A winning a tiebreak set when returning.</param>
        /// <param name="winningAdvantageSetOnReturn">The probability of Player A winning an advantage set when returning.</param>
        /// <returns>A tuple containing two 2D arrays of probabilities for each possible scoreline for Player A and Player B, respectively.</returns>
        public static (double[,], double[,]) CalculateAdvantageSetMatchBestOf5(double winningTiebreakSetOnServe, double winningAdvantageSetOnServe, double winningTiebreakSetOnReturn, double winningAdvantageSetOnReturn)
        {
            // The number of sets required to win the match.
            int setsToWin = 3;

            // 2D arrays representing the probabilities of reaching each possible scoreline for Player A and Player B.
            double[,] probA = new double[setsToWin + 1, setsToWin + 1];
            double[,] probB = new double[setsToWin + 1, setsToWin + 1];

            // Nested loops to iterate through all possible set scores.
            for (int k = 0; k <= setsToWin; k++)
            {
                for (int l = 0; l <= setsToWin; l++)
                {
                    // Base case: The match start, no sets won by either player.
                    if (k == 0 && l == 0)
                    {
                        probA[k, l] = 0;
                        probB[k, l] = 0;
                    }
                    // First row: Player A has won sets, Player B has not.
                    else if (k > 0 && l == 0)
                    {
                        probA[k, l] = winningTiebreakSetOnServe * probA[k - 1, l];
                        probB[k, l] = (1 - winningTiebreakSetOnServe) * probB[k - 1, l];
                    }
                    // First column: Player B has won sets, Player A has not.
                    else if (l > 0 && k == 0)
                    {
                        probA[k, l] = (1 - winningTiebreakSetOnReturn) * probA[k, l - 1];
                        probB[k, l] = winningTiebreakSetOnReturn * probB[k, l - 1];
                    }
                    // Middle squares: Both players have won sets, but no one has yet won the match.
                    else if (k > 0 && l > 0 && k <= 2 && l <= 2)
                    {
                        probA[k, l] = winningTiebreakSetOnServe * probA[k - 1, l] + (1 - winningTiebreakSetOnReturn) * probA[k, l - 1];
                        probB[k, l] = (1 - winningTiebreakSetOnServe) * probB[k - 1, l] + winningTiebreakSetOnReturn * probB[k, l - 1];
                    }
                    // Last row (excluding the last square): Player A is one set away from winning the match.
                    else if (k == 3 && l <= 2)
                    {
                        probA[k, l] = winningAdvantageSetOnServe * probA[k - 1, l];
                        probB[k, l] = (1 - winningAdvantageSetOnReturn) * probB[k - 1, l];
                    }
                    // Last column (excluding the last square): Player B is one set away from winning the match.
                    else if (l == 3 && k <= 2)
                    {
                        probA[k, l] = (1 - winningAdvantageSetOnServe) * probA[k, l - 1];
                        probB[k, l] = winningAdvantageSetOnReturn * probB[k, l - 1];
                    }
                }
            }

            // Return the calculated probabilities.
            return (probA, probB);
        }

        /// <summary>
        /// PrintResults method.
        /// This method prints the probabilities calculated from the various methods
        /// for winning a game, tiebreak, set, and match, as well as probabilities of 
        /// reaching various scorelines, for both Player A and Player B.
        /// The method uses the PrintProbabilityTable helper function to print out each 
        /// probability table.
        /// </summary>
        /// <param name="gameWinProbA">Game win probabilities for Player A.</param>
        /// <param name="gameWinProbB">Game win probabilities for Player B.</param>
        /// <param name="tiebreakWinOnServe">Tiebreak win probabilities for the serving player.</param>
        /// <param name="tiebreakWinOnReturn">Tiebreak win probabilities for the returning player.</param>
        /// <param name="winningTiebreakSetOnServe">Winning tiebreak set probabilities for the serving player.</param>
        /// <param name="winningTiebreakSetOnReturn">Winning tiebreak set probabilities for the returning player.</param>
        /// <param name="winningAdvantageSetOnServe">Winning advantage set probabilities for the serving player.</param>
        /// <param name="winningAdvantageSetOnReturn">Winning advantage set probabilities for the returning player.</param>
        /// <param name="winningAnAllTiebreakSetMatchBestOf2Sets">Winning probabilities of an all-tiebreak set match (best of 2 sets).</param>
        /// <param name="winningAnAllTiebreakSetMatchBestOf3Sets">Winning probabilities of an all-tiebreak set match (best of 3 sets).</param>
        /// <param name="winningAnAdvantageSetMatchBestOf2Sets">Winning probabilities of an advantage set match (best of 2 sets).</param>
        /// <param name="winningAnAdvantageSetMatchBestOf3Sets">Winning probabilities of an advantage set match (best of 3 sets).</param>
        /// <param name="scoreLineProbabilitiesInAGameForPlayerA">Scoreline probabilities in a game for Player A.</param>
        /// <param name="scoreLineProbabilitiesInAGameForPlayerB">Scoreline probabilities in a game for Player B.</param>
        /// <param name="CalculateTiebreakGameProbabilitiesForA">Tiebreak scoreline probabilities for Player A.</param>
        /// <param name="CalculateTiebreakGameProbabilitiesForB">Tiebreak scoreline probabilities for Player B.</param>
        /// <param name="scoreLineProbabilitiesInATiebreakSetForPlayerA">Tiebreak set scoreline probabilities for Player A.</param>
        /// <param name="scoreLineProbabilitiesInATiebreakSetForPlayerB">Tiebreak set scoreline probabilities for Player B.</param>
        /// <param name="scoreLineProbabilitiesInAAdvantageSetForPlayerA">Advantage set scoreline probabilities for Player A.</param>
        /// <param name="scoreLineProbabilitiesInAAdvantageSetForPlayerB">Advantage set scoreline probabilities for Player B.</param>
        /// <param name="probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerA">Winning probabilities of an all-tiebreak set match (best of 5 sets) for Player A.</param>
        /// <param name="probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerB">Winning probabilities of an all-tiebreak set match (best of 5 sets) for Player B.</param>
        static void PrintResults(double[,] gameWinProbA, double[,] gameWinProbB, double[,] tiebreakWinOnServe, double[,] tiebreakWinOnReturn, double[,] winningTiebreakSetOnServe, double[,] winningTiebreakSetOnReturn, double[,] winningAdvantageSetOnServe, double[,] winningAdvantageSetOnReturn, double[,] winningAnAllTiebreakSetMatchBestOf2Sets, double[,] winningAnAllTiebreakSetMatchBestOf3Sets, double[,] winningAnAdvantageSetMatchBestOf2Sets, double[,] winningAnAdvantageSetMatchBestOf3Sets, double[,] scoreLineProbabilitiesInAGameForPlayerA, double[,] scoreLineProbabilitiesInAGameForPlayerB, double[,] CalculateTiebreakGameProbabilitiesForA, double[,] CalculateTiebreakGameProbabilitiesForB, double[,] scoreLineProbabilitiesInATiebreakSetForPlayerA, double[,] scoreLineProbabilitiesInATiebreakSetForPlayerB, double[,] scoreLineProbabilitiesInAAdvantageSetForPlayerA, double[,] scoreLineProbabilitiesInAAdvantageSetForPlayerB, double[,] probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerA, double[,] probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerB, double[,] probabilitiesOfWinningAdvantageSetMatchBestOf5SetsForPlayerA, double[,] probabilitiesOfWinningAdvantageSetMatchBestOf5SetsForPlayerB)
        {
            // Print game winning probabilities for player A and player B
            Console.WriteLine("Winning a game on serve player A:");
            PrintProbabilityTable(gameWinProbA);
            Console.WriteLine("Winning a game on serve player B:");
            PrintProbabilityTable(gameWinProbB);

            // Print tiebreak game winning probabilities for both players
            Console.WriteLine("Winning a tiebreak game on serve:");
            PrintProbabilityTable(tiebreakWinOnServe);
            Console.WriteLine("Winning a tiebreak game on return:");
            PrintProbabilityTable(tiebreakWinOnReturn);

            // Print winning tiebreak set probabilities for both players
            Console.WriteLine("\nWinning a tiebreak set on serve:");
            PrintProbabilityTable(winningTiebreakSetOnServe);
            Console.WriteLine("\nWinning a tiebreak set on return:");
            PrintProbabilityTable(winningTiebreakSetOnReturn);

            // Print winning advantage set probabilities for both players
            Console.WriteLine("\nWinning a advantage set on serve:");
            PrintProbabilityTable(winningAdvantageSetOnServe);
            Console.WriteLine("\nWinning a advantage set on return:");
            PrintProbabilityTable(winningAdvantageSetOnReturn);

            // Print winning probabilities of all tiebreak set match for both players in a match played to the best of 2 sets
            Console.WriteLine("\nWinning probabilities of all tiebreak set match for both players in a match played to the best of 2 sets:");
            PrintProbabilityTable(winningAnAllTiebreakSetMatchBestOf2Sets);

            // Print winning probabilities of all tiebreak set match for both players in a match played to the best of 3 sets
            Console.WriteLine("\nWinning probabilities of all tiebreak set match for both players in a match played to the best of 3 sets:");
            PrintProbabilityTable(winningAnAllTiebreakSetMatchBestOf3Sets);

            // Print winning probabilities of advantage set match for both players in a match played to the best of 2 sets
            Console.WriteLine("\nWinning probabilities of advantage set match for both players in a match played to the best of 2 sets:");
            PrintProbabilityTable(winningAnAdvantageSetMatchBestOf2Sets);

            // Print winning probabilities of advantage set match for both players in a match played to the best of 3 sets
            Console.WriteLine("\nWinning probabilities of advantage set match for both players in a match played to the best of 3 sets:");
            PrintProbabilityTable(winningAnAdvantageSetMatchBestOf3Sets);

            // Print winning probabilities of reaching various score lines in a game for player A
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a game for player A:");
            PrintProbabilityTable(scoreLineProbabilitiesInAGameForPlayerA);

            // Print winning probabilities of reaching various score lines in a game for player B
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a game for player B:");
            PrintProbabilityTable(scoreLineProbabilitiesInAGameForPlayerB);

            // Print winning probabilities of reaching various score lines in a tiebreak for player A
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a tiebreak for player A:");
            PrintProbabilityTable(CalculateTiebreakGameProbabilitiesForA);

            // Print winning probabilities of reaching various score lines in a tiebreak for player B
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a tiebreak for player B:");
            PrintProbabilityTable(CalculateTiebreakGameProbabilitiesForB);

            // Print winning probabilities of reaching various score lines in a tiebreak set for player A
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a tiebreak set for player A:");
            PrintProbabilityTable(scoreLineProbabilitiesInATiebreakSetForPlayerA);

            // Print winning probabilities of reaching various score lines in a tiebreak set for player B
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a tiebreak set for player B:");
            PrintProbabilityTable(scoreLineProbabilitiesInATiebreakSetForPlayerB);

            // Print winning probabilities of reaching various score lines in a advantage set for player A
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a advantage set for player A:");
            PrintProbabilityTable(scoreLineProbabilitiesInAAdvantageSetForPlayerA);

            // Print winning probabilities of reaching various score lines in a advantage set for player B
            Console.WriteLine("\nWinning probabilities of reaching various score lines in a advantage set for player B:");
            PrintProbabilityTable(scoreLineProbabilitiesInAAdvantageSetForPlayerB);

            // Print winning probabilities of of each possible scoreline for player A a best-of-5 all tiebreak set match
            Console.WriteLine("\nwinning probabilities of of each possible scoreline for player A a best-of-5 all tiebreak set match:");
            PrintProbabilityTable(probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerA);

            // Print winning probabilities of of each possible scoreline for player B a best-of-5 all tiebreak set match
            Console.WriteLine("\nwinning probabilities of of each possible scoreline for player B a best-of-5 all tiebreak set match:");
            PrintProbabilityTable(probabilitiesOfWinningAllTiebreakSetMatchBestOf5SetsForPlayerB);
        }

        /// <summary>
        /// PrintProbabilityTable method.
        /// This helper method takes a 2D array of probabilities and prints it out 
        /// in a tabular format. Each probability is printed with 3 decimal places.
        /// </summary>
        /// <param name="table">A 2D array of probabilities to print.</param>
        static void PrintProbabilityTable(double[,] table)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    Console.Write($"{table[i, j]:F3}\t");
                }
                Console.WriteLine();
            }
        }
    }
}