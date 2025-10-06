using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.DTOs;
using BonzoByte.Core.Helpers;
using BonzoByte.Core.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace BonzoByte.Core.DAL.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly IDbConnection _connection;

        public MatchRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Match>> GetAllMatchesAsync()
        {
            var Matches = new List<Match>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllMatches";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                try
                {
                    var MatchTPId = reader["MatchTPId"];
                    var TournamentEventTPId = reader["TournamentEventTPId"];
                    var DateTime = reader["DateTime"];
                    var Player1TPId = reader["Player1TPId"];
                    var Player2TPId = reader["Player2TPId"];
                    var Player1Seed = reader["Player1Seed"];
                    var Player2Seed = reader["Player2Seed"];
                    var Result = reader["Result"];
                    var ResultDetails = reader["ResultDetails"];
                    var Player1Odds = reader["Player1Odds"];
                    var Player2Odds = reader["Player2Odds"];
                    var Player1Percentage = reader["Player1Percentage"];
                    var Player2Percentage = reader["Player2Percentage"];
                    var SurfaceId = reader["SurfaceId"];
                    var RoundId = reader["RoundId"];
                    var Player1TrueSkillMeanM = reader["Player1TrueSkillMeanM"];
                    var Player1TrueSkillStandardDeviationM = reader["Player1TrueSkillStandardDeviationM"];
                    var Player2TrueSkillMeanM = reader["Player2TrueSkillMeanM"];
                    var Player2TrueSkillStandardDeviationM = reader["Player2TrueSkillStandardDeviationM"];
                    var Player1TrueSkillMeanOldM = reader["Player1TrueSkillMeanOldM"];
                    var Player1TrueSkillStandardDeviationOldM = reader["Player1TrueSkillStandardDeviationOldM"];
                    var Player2TrueSkillMeanOldM = reader["Player2TrueSkillMeanOldM"];
                    var Player2TrueSkillStandardDeviationOldM = reader["Player2TrueSkillStandardDeviationOldM"];
                    var WinProbabilityPlayer1M = reader["WinProbabilityPlayer1M"];
                    var Player1TrueSkillMeanSM = reader["Player1TrueSkillMeanSM"];
                    var Player1TrueSkillStandardDeviationSM = reader["Player1TrueSkillStandardDeviationSM"];
                    var Player2TrueSkillMeanSM = reader["Player2TrueSkillMeanSM"];
                    var Player2TrueSkillStandardDeviationSM = reader["Player2TrueSkillStandardDeviationSM"];
                    var Player1TrueSkillMeanOldSM = reader["Player1TrueSkillMeanOldSM"];
                    var Player1TrueSkillStandardDeviationOldSM = reader["Player1TrueSkillStandardDeviationOldSM"];
                    var Player2TrueSkillMeanOldSM = reader["Player2TrueSkillMeanOldSM"];
                    var Player2TrueSkillStandardDeviationOldSM = reader["Player2TrueSkillStandardDeviationOldSM"];
                    var WinProbabilityPlayer1SM = reader["WinProbabilityPlayer1SM"];
                    var Player1TrueSkillMeanGSM = reader["Player1TrueSkillMeanGSM"];
                    var Player1TrueSkillStandardDeviationGSM = reader["Player1TrueSkillStandardDeviationGSM"];
                    var Player2TrueSkillMeanGSM = reader["Player2TrueSkillMeanGSM"];
                    var Player2TrueSkillStandardDeviationGSM = reader["Player2TrueSkillStandardDeviationGSM"];
                    var Player1TrueSkillMeanOldGSM = reader["Player1TrueSkillMeanOldGSM"];
                    var Player1TrueSkillStandardDeviationOldGSM = reader["Player1TrueSkillStandardDeviationOldGSM"];
                    var Player2TrueSkillMeanOldGSM = reader["Player2TrueSkillMeanOldGSM"];
                    var Player2TrueSkillStandardDeviationOldGSM = reader["Player2TrueSkillStandardDeviationOldGSM"];
                    var WinProbabilityPlayer1GSM = reader["WinProbabilityPlayer1GSM"];
                    var Player1TrueSkillMeanMS1 = reader["Player1TrueSkillMeanMS1"];
                    var Player1TrueSkillStandardDeviationMS1 = reader["Player1TrueSkillStandardDeviationMS1"];
                    var Player2TrueSkillMeanMS1 = reader["Player2TrueSkillMeanMS1"];
                    var Player2TrueSkillStandardDeviationMS1 = reader["Player2TrueSkillStandardDeviationMS1"];
                    var Player1TrueSkillMeanOldMS1 = reader["Player1TrueSkillMeanOldMS1"];
                    var Player1TrueSkillStandardDeviationOldMS1 = reader["Player1TrueSkillStandardDeviationOldMS1"];
                    var Player2TrueSkillMeanOldMS1 = reader["Player2TrueSkillMeanOldMS1"];
                    var Player2TrueSkillStandardDeviationOldMS1 = reader["Player2TrueSkillStandardDeviationOldMS1"];
                    var WinProbabilityPlayer1MS1 = reader["WinProbabilityPlayer1MS1"];
                    var Player1TrueSkillMeanSMS1 = reader["Player1TrueSkillMeanSMS1"];
                    var Player1TrueSkillStandardDeviationSMS1 = reader["Player1TrueSkillStandardDeviationSMS1"];
                    var Player2TrueSkillMeanSMS1 = reader["Player2TrueSkillMeanSMS1"];
                    var Player2TrueSkillStandardDeviationSMS1 = reader["Player2TrueSkillStandardDeviationSMS1"];
                    var Player1TrueSkillMeanOldSMS1 = reader["Player1TrueSkillMeanOldSMS1"];
                    var Player1TrueSkillStandardDeviationOldSMS1 = reader["Player1TrueSkillStandardDeviationOldSMS1"];
                    var Player2TrueSkillMeanOldSMS1 = reader["Player2TrueSkillMeanOldSMS1"];
                    var Player2TrueSkillStandardDeviationOldSMS1 = reader["Player2TrueSkillStandardDeviationOldSMS1"];
                    var WinProbabilityPlayer1SMS1 = reader["WinProbabilityPlayer1SMS1"];
                    var Player1TrueSkillMeanGSMS1 = reader["Player1TrueSkillMeanGSMS1"];
                    var Player1TrueSkillStandardDeviationGSMS1 = reader["Player1TrueSkillStandardDeviationGSMS1"];
                    var Player2TrueSkillMeanGSMS1 = reader["Player2TrueSkillMeanGSMS1"];
                    var Player2TrueSkillStandardDeviationGSMS1 = reader["Player2TrueSkillStandardDeviationGSMS1"];
                    var Player1TrueSkillMeanOldGSMS1 = reader["Player1TrueSkillMeanOldGSMS1"];
                    var Player1TrueSkillStandardDeviationOldGSMS1 = reader["Player1TrueSkillStandardDeviationOldGSMS1"];
                    var Player2TrueSkillMeanOldGSMS1 = reader["Player2TrueSkillMeanOldGSMS1"];
                    var Player2TrueSkillStandardDeviationOldGSMS1 = reader["Player2TrueSkillStandardDeviationOldGSMS1"];
                    var WinProbabilityPlayer1GSMS1 = reader["WinProbabilityPlayer1GSMS1"];
                    var Player1TrueSkillMeanMS2 = reader["Player1TrueSkillMeanMS2"];
                    var Player1TrueSkillStandardDeviationMS2 = reader["Player1TrueSkillStandardDeviationMS2"];
                    var Player2TrueSkillMeanMS2 = reader["Player2TrueSkillMeanMS2"];
                    var Player2TrueSkillStandardDeviationMS2 = reader["Player2TrueSkillStandardDeviationMS2"];
                    var Player1TrueSkillMeanOldMS2 = reader["Player1TrueSkillMeanOldMS2"];
                    var Player1TrueSkillStandardDeviationOldMS2 = reader["Player1TrueSkillStandardDeviationOldMS2"];
                    var Player2TrueSkillMeanOldMS2 = reader["Player2TrueSkillMeanOldMS2"];
                    var Player2TrueSkillStandardDeviationOldMS2 = reader["Player2TrueSkillStandardDeviationOldMS2"];
                    var WinProbabilityPlayer1MS2 = reader["WinProbabilityPlayer1MS2"];
                    var Player1TrueSkillMeanSMS2 = reader["Player1TrueSkillMeanSMS2"];
                    var Player1TrueSkillStandardDeviationSMS2 = reader["Player1TrueSkillStandardDeviationSMS2"];
                    var Player2TrueSkillMeanSMS2 = reader["Player2TrueSkillMeanSMS2"];
                    var Player2TrueSkillStandardDeviationSMS2 = reader["Player2TrueSkillStandardDeviationSMS2"];
                    var Player1TrueSkillMeanOldSMS2 = reader["Player1TrueSkillMeanOldSMS2"];
                    var Player1TrueSkillStandardDeviationOldSMS2 = reader["Player1TrueSkillStandardDeviationOldSMS2"];
                    var Player2TrueSkillMeanOldSMS2 = reader["Player2TrueSkillMeanOldSMS2"];
                    var Player2TrueSkillStandardDeviationOldSMS2 = reader["Player2TrueSkillStandardDeviationOldSMS2"];
                    var WinProbabilityPlayer1SMS2 = reader["WinProbabilityPlayer1SMS2"];
                    var Player1TrueSkillMeanGSMS2 = reader["Player1TrueSkillMeanGSMS2"];
                    var Player1TrueSkillStandardDeviationGSMS2 = reader["Player1TrueSkillStandardDeviationGSMS2"];
                    var Player2TrueSkillMeanGSMS2 = reader["Player2TrueSkillMeanGSMS2"];
                    var Player2TrueSkillStandardDeviationGSMS2 = reader["Player2TrueSkillStandardDeviationGSMS2"];
                    var Player1TrueSkillMeanOldGSMS2 = reader["Player1TrueSkillMeanOldGSMS2"];
                    var Player1TrueSkillStandardDeviationOldGSMS2 = reader["Player1TrueSkillStandardDeviationOldGSMS2"];
                    var Player2TrueSkillMeanOldGSMS2 = reader["Player2TrueSkillMeanOldGSMS2"];
                    var Player2TrueSkillStandardDeviationOldGSMS2 = reader["Player2TrueSkillStandardDeviationOldGSMS2"];
                    var WinProbabilityPlayer1GSMS2 = reader["WinProbabilityPlayer1GSMS2"];
                    var Player1TrueSkillMeanMS3 = reader["Player1TrueSkillMeanMS3"];
                    var Player1TrueSkillStandardDeviationMS3 = reader["Player1TrueSkillStandardDeviationMS3"];
                    var Player2TrueSkillMeanMS3 = reader["Player2TrueSkillMeanMS3"];
                    var Player2TrueSkillStandardDeviationMS3 = reader["Player2TrueSkillStandardDeviationMS3"];
                    var Player1TrueSkillMeanOldMS3 = reader["Player1TrueSkillMeanOldMS3"];
                    var Player1TrueSkillStandardDeviationOldMS3 = reader["Player1TrueSkillStandardDeviationOldMS3"];
                    var Player2TrueSkillMeanOldMS3 = reader["Player2TrueSkillMeanOldMS3"];
                    var Player2TrueSkillStandardDeviationOldMS3 = reader["Player2TrueSkillStandardDeviationOldMS3"];
                    var WinProbabilityPlayer1MS3 = reader["WinProbabilityPlayer1MS3"];
                    var Player1TrueSkillMeanSMS3 = reader["Player1TrueSkillMeanSMS3"];
                    var Player1TrueSkillStandardDeviationSMS3 = reader["Player1TrueSkillStandardDeviationSMS3"];
                    var Player2TrueSkillMeanSMS3 = reader["Player2TrueSkillMeanSMS3"];
                    var Player2TrueSkillStandardDeviationSMS3 = reader["Player2TrueSkillStandardDeviationSMS3"];
                    var Player1TrueSkillMeanOldSMS3 = reader["Player1TrueSkillMeanOldSMS3"];
                    var Player1TrueSkillStandardDeviationOldSMS3 = reader["Player1TrueSkillStandardDeviationOldSMS3"];
                    var Player2TrueSkillMeanOldSMS3 = reader["Player2TrueSkillMeanOldSMS3"];
                    var Player2TrueSkillStandardDeviationOldSMS3 = reader["Player2TrueSkillStandardDeviationOldSMS3"];
                    var WinProbabilityPlayer1SMS3 = reader["WinProbabilityPlayer1SMS3"];
                    var Player1TrueSkillMeanGSMS3 = reader["Player1TrueSkillMeanGSMS3"];
                    var Player1TrueSkillStandardDeviationGSMS3 = reader["Player1TrueSkillStandardDeviationGSMS3"];
                    var Player2TrueSkillMeanGSMS3 = reader["Player2TrueSkillMeanGSMS3"];
                    var Player2TrueSkillStandardDeviationGSMS3 = reader["Player2TrueSkillStandardDeviationGSMS3"];
                    var Player1TrueSkillMeanOldGSMS3 = reader["Player1TrueSkillMeanOldGSMS3"];
                    var Player1TrueSkillStandardDeviationOldGSMS3 = reader["Player1TrueSkillStandardDeviationOldGSMS3"];
                    var Player2TrueSkillMeanOldGSMS3 = reader["Player2TrueSkillMeanOldGSMS3"];
                    var Player2TrueSkillStandardDeviationOldGSMS3 = reader["Player2TrueSkillStandardDeviationOldGSMS3"];
                    var WinProbabilityPlayer1GSMS3 = reader["WinProbabilityPlayer1GSMS3"];
                    var Player1TrueSkillMeanMS4 = reader["Player1TrueSkillMeanMS4"];
                    var Player1TrueSkillStandardDeviationMS4 = reader["Player1TrueSkillStandardDeviationMS4"];
                    var Player2TrueSkillMeanMS4 = reader["Player2TrueSkillMeanMS4"];
                    var Player2TrueSkillStandardDeviationMS4 = reader["Player2TrueSkillStandardDeviationMS4"];
                    var Player1TrueSkillMeanOldMS4 = reader["Player1TrueSkillMeanOldMS4"];
                    var Player1TrueSkillStandardDeviationOldMS4 = reader["Player1TrueSkillStandardDeviationOldMS4"];
                    var Player2TrueSkillMeanOldMS4 = reader["Player2TrueSkillMeanOldMS4"];
                    var Player2TrueSkillStandardDeviationOldMS4 = reader["Player2TrueSkillStandardDeviationOldMS4"];
                    var WinProbabilityPlayer1MS4 = reader["WinProbabilityPlayer1MS4"];
                    var Player1TrueSkillMeanSMS4 = reader["Player1TrueSkillMeanSMS4"];
                    var Player1TrueSkillStandardDeviationSMS4 = reader["Player1TrueSkillStandardDeviationSMS4"];
                    var Player2TrueSkillMeanSMS4 = reader["Player2TrueSkillMeanSMS4"];
                    var Player2TrueSkillStandardDeviationSMS4 = reader["Player2TrueSkillStandardDeviationSMS4"];
                    var Player1TrueSkillMeanOldSMS4 = reader["Player1TrueSkillMeanOldSMS4"];
                    var Player1TrueSkillStandardDeviationOldSMS4 = reader["Player1TrueSkillStandardDeviationOldSMS4"];
                    var Player2TrueSkillMeanOldSMS4 = reader["Player2TrueSkillMeanOldSMS4"];
                    var Player2TrueSkillStandardDeviationOldSMS4 = reader["Player2TrueSkillStandardDeviationOldSMS4"];
                    var WinProbabilityPlayer1SMS4 = reader["WinProbabilityPlayer1SMS4"];
                    var Player1TrueSkillMeanGSMS4 = reader["Player1TrueSkillMeanGSMS4"];
                    var Player1TrueSkillStandardDeviationGSMS4 = reader["Player1TrueSkillStandardDeviationGSMS4"];
                    var Player2TrueSkillMeanGSMS4 = reader["Player2TrueSkillMeanGSMS4"];
                    var Player2TrueSkillStandardDeviationGSMS4 = reader["Player2TrueSkillStandardDeviationGSMS4"];
                    var Player1TrueSkillMeanOldGSMS4 = reader["Player1TrueSkillMeanOldGSMS4"];
                    var Player1TrueSkillStandardDeviationOldGSMS4 = reader["Player1TrueSkillStandardDeviationOldGSMS4"];
                    var Player2TrueSkillMeanOldGSMS4 = reader["Player2TrueSkillMeanOldGSMS4"];
                    var Player2TrueSkillStandardDeviationOldGSMS4 = reader["Player2TrueSkillStandardDeviationOldGSMS4"];
                    var WinProbabilityPlayer1GSMS4 = reader["WinProbabilityPlayer1GSMS4"];
                    var Player1WinsTotal = reader["Player1WinsTotal"];
                    var Player1LossesTotal = reader["Player1LossesTotal"];
                    var Player1WinsLastYear = reader["Player1WinsLastYear"];
                    var Player1LossesLastYear = reader["Player1LossesLastYear"];
                    var Player1WinsLastMonth = reader["Player1WinsLastMonth"];
                    var Player1LossesLastMonth = reader["Player1LossesLastMonth"];
                    var Player1WinsLastWeek = reader["Player1WinsLastWeek"];
                    var Player1LossesLastWeek = reader["Player1LossesLastWeek"];
                    var Player2WinsTotal = reader["Player2WinsTotal"];
                    var Player2LossesTotal = reader["Player2LossesTotal"];
                    var Player2WinsLastYear = reader["Player2WinsLastYear"];
                    var Player2LossesLastYear = reader["Player2LossesLastYear"];
                    var Player2WinsLastMonth = reader["Player2WinsLastMonth"];
                    var Player2LossesLastMonth = reader["Player2LossesLastMonth"];
                    var Player2WinsLastWeek = reader["Player2WinsLastWeek"];
                    var Player2LossesLastWeek = reader["Player2LossesLastWeek"];
                    var Player1WinsTotalS1 = reader["Player1WinsTotalS1"];
                    var Player1LossesTotalS1 = reader["Player1LossesTotalS1"];
                    var Player1WinsLastYearS1 = reader["Player1WinsLastYearS1"];
                    var Player1LossesLastYearS1 = reader["Player1LossesLastYearS1"];
                    var Player1WinsLastMonthS1 = reader["Player1WinsLastMonthS1"];
                    var Player1LossesLastMonthS1 = reader["Player1LossesLastMonthS1"];
                    var Player1WinsLastWeekS1 = reader["Player1WinsLastWeekS1"];
                    var Player1LossesLastWeekS1 = reader["Player1LossesLastWeekS1"];
                    var Player2WinsTotalS1 = reader["Player2WinsTotalS1"];
                    var Player2LossesTotalS1 = reader["Player2LossesTotalS1"];
                    var Player2WinsLastYearS1 = reader["Player2WinsLastYearS1"];
                    var Player2LossesLastYearS1 = reader["Player2LossesLastYearS1"];
                    var Player2WinsLastMonthS1 = reader["Player2WinsLastMonthS1"];
                    var Player2LossesLastMonthS1 = reader["Player2LossesLastMonthS1"];
                    var Player2WinsLastWeekS1 = reader["Player2WinsLastWeekS1"];
                    var Player2LossesLastWeekS1 = reader["Player2LossesLastWeekS1"];
                    var Player1WinsTotalS2 = reader["Player1WinsTotalS2"];
                    var Player1LossesTotalS2 = reader["Player1LossesTotalS2"];
                    var Player1WinsLastYearS2 = reader["Player1WinsLastYearS2"];
                    var Player1LossesLastYearS2 = reader["Player1LossesLastYearS2"];
                    var Player1WinsLastMonthS2 = reader["Player1WinsLastMonthS2"];
                    var Player1LossesLastMonthS2 = reader["Player1LossesLastMonthS2"];
                    var Player1WinsLastWeekS2 = reader["Player1WinsLastWeekS2"];
                    var Player1LossesLastWeekS2 = reader["Player1LossesLastWeekS2"];
                    var Player2WinsTotalS2 = reader["Player2WinsTotalS2"];
                    var Player2LossesTotalS2 = reader["Player2LossesTotalS2"];
                    var Player2WinsLastYearS2 = reader["Player2WinsLastYearS2"];
                    var Player2LossesLastYearS2 = reader["Player2LossesLastYearS2"];
                    var Player2WinsLastMonthS2 = reader["Player2WinsLastMonthS2"];
                    var Player2LossesLastMonthS2 = reader["Player2LossesLastMonthS2"];
                    var Player2WinsLastWeekS2 = reader["Player2WinsLastWeekS2"];
                    var Player2LossesLastWeekS2 = reader["Player2LossesLastWeekS2"];
                    var Player1WinsTotalS3 = reader["Player1WinsTotalS3"];
                    var Player1LossesTotalS3 = reader["Player1LossesTotalS3"];
                    var Player1WinsLastYearS3 = reader["Player1WinsLastYearS3"];
                    var Player1LossesLastYearS3 = reader["Player1LossesLastYearS3"];
                    var Player1WinsLastMonthS3 = reader["Player1WinsLastMonthS3"];
                    var Player1LossesLastMonthS3 = reader["Player1LossesLastMonthS3"];
                    var Player1WinsLastWeekS3 = reader["Player1WinsLastWeekS3"];
                    var Player1LossesLastWeekS3 = reader["Player1LossesLastWeekS3"];
                    var Player2WinsTotalS3 = reader["Player2WinsTotalS3"];
                    var Player2LossesTotalS3 = reader["Player2LossesTotalS3"];
                    var Player2WinsLastYearS3 = reader["Player2WinsLastYearS3"];
                    var Player2LossesLastYearS3 = reader["Player2LossesLastYearS3"];
                    var Player2WinsLastMonthS3 = reader["Player2WinsLastMonthS3"];
                    var Player2LossesLastMonthS3 = reader["Player2LossesLastMonthS3"];
                    var Player2WinsLastWeekS3 = reader["Player2WinsLastWeekS3"];
                    var Player2LossesLastWeekS3 = reader["Player2LossesLastWeekS3"];
                    var Player1WinsTotalS4 = reader["Player1WinsTotalS4"];
                    var Player1LossesTotalS4 = reader["Player1LossesTotalS4"];
                    var Player1WinsLastYearS4 = reader["Player1WinsLastYearS4"];
                    var Player1LossesLastYearS4 = reader["Player1LossesLastYearS4"];
                    var Player1WinsLastMonthS4 = reader["Player1WinsLastMonthS4"];
                    var Player1LossesLastMonthS4 = reader["Player1LossesLastMonthS4"];
                    var Player1WinsLastWeekS4 = reader["Player1WinsLastWeekS4"];
                    var Player1LossesLastWeekS4 = reader["Player1LossesLastWeekS4"];
                    var Player2WinsTotalS4 = reader["Player2WinsTotalS4"];
                    var Player2LossesTotalS4 = reader["Player2LossesTotalS4"];
                    var Player2WinsLastYearS4 = reader["Player2WinsLastYearS4"];
                    var Player2LossesLastYearS4 = reader["Player2LossesLastYearS4"];
                    var Player2WinsLastMonthS4 = reader["Player2WinsLastMonthS4"];
                    var Player2LossesLastMonthS4 = reader["Player2LossesLastMonthS4"];
                    var Player2WinsLastWeekS4 = reader["Player2WinsLastWeekS4"];
                    var Player2LossesLastWeekS4 = reader["Player2LossesLastWeekS4"];
                    var Player1WinsSetsTotal = reader["Player1WinsSetsTotal"];
                    var Player1LossesSetsTotal = reader["Player1LossesSetsTotal"];
                    var Player1WinsSetsLastYear = reader["Player1WinsSetsLastYear"];
                    var Player1LossesSetsLastYear = reader["Player1LossesSetsLastYear"];
                    var Player1WinsSetsLastMonth = reader["Player1WinsSetsLastMonth"];
                    var Player1LossesSetsLastMonth = reader["Player1LossesSetsLastMonth"];
                    var Player1WinsSetsLastWeek = reader["Player1WinsSetsLastWeek"];
                    var Player1LossesSetsLastWeek = reader["Player1LossesSetsLastWeek"];
                    var Player2WinsSetsTotal = reader["Player2WinsSetsTotal"];
                    var Player2LossesSetsTotal = reader["Player2LossesSetsTotal"];
                    var Player2WinsSetsLastYear = reader["Player2WinsSetsLastYear"];
                    var Player2LossesSetsLastYear = reader["Player2LossesSetsLastYear"];
                    var Player2WinsSetsLastMonth = reader["Player2WinsSetsLastMonth"];
                    var Player2LossesSetsLastMonth = reader["Player2LossesSetsLastMonth"];
                    var Player2WinsSetsLastWeek = reader["Player2WinsSetsLastWeek"];
                    var Player2LossesSetsLastWeek = reader["Player2LossesSetsLastWeek"];
                    var Player1WinsSetsTotalS1 = reader["Player1WinsSetsTotalS1"];
                    var Player1LossesSetsTotalS1 = reader["Player1LossesSetsTotalS1"];
                    var Player1WinsSetsLastYearS1 = reader["Player1WinsSetsLastYearS1"];
                    var Player1LossesSetsLastYearS1 = reader["Player1LossesSetsLastYearS1"];
                    var Player1WinsSetsLastMonthS1 = reader["Player1WinsSetsLastMonthS1"];
                    var Player1LossesSetsLastMonthS1 = reader["Player1LossesSetsLastMonthS1"];
                    var Player1WinsSetsLastWeekS1 = reader["Player1WinsSetsLastWeekS1"];
                    var Player1LossesSetsLastWeekS1 = reader["Player1LossesSetsLastWeekS1"];
                    var Player2WinsSetsTotalS1 = reader["Player2WinsSetsTotalS1"];
                    var Player2LossesSetsTotalS1 = reader["Player2LossesSetsTotalS1"];
                    var Player2WinsSetsLastYearS1 = reader["Player2WinsSetsLastYearS1"];
                    var Player2LossesSetsLastYearS1 = reader["Player2LossesSetsLastYearS1"];
                    var Player2WinsSetsLastMonthS1 = reader["Player2WinsSetsLastMonthS1"];
                    var Player2LossesSetsLastMonthS1 = reader["Player2LossesSetsLastMonthS1"];
                    var Player2WinsSetsLastWeekS1 = reader["Player2WinsSetsLastWeekS1"];
                    var Player2LossesSetsLastWeekS1 = reader["Player2LossesSetsLastWeekS1"];
                    var Player1WinsSetsTotalS2 = reader["Player1WinsSetsTotalS2"];
                    var Player1LossesSetsTotalS2 = reader["Player1LossesSetsTotalS2"];
                    var Player1WinsSetsLastYearS2 = reader["Player1WinsSetsLastYearS2"];
                    var Player1LossesSetsLastYearS2 = reader["Player1LossesSetsLastYearS2"];
                    var Player1WinsSetsLastMonthS2 = reader["Player1WinsSetsLastMonthS2"];
                    var Player1LossesSetsLastMonthS2 = reader["Player1LossesSetsLastMonthS2"];
                    var Player1WinsSetsLastWeekS2 = reader["Player1WinsSetsLastWeekS2"];
                    var Player1LossesSetsLastWeekS2 = reader["Player1LossesSetsLastWeekS2"];
                    var Player2WinsSetsTotalS2 = reader["Player2WinsSetsTotalS2"];
                    var Player2LossesSetsTotalS2 = reader["Player2LossesSetsTotalS2"];
                    var Player2WinsSetsLastYearS2 = reader["Player2WinsSetsLastYearS2"];
                    var Player2LossesSetsLastYearS2 = reader["Player2LossesSetsLastYearS2"];
                    var Player2WinsSetsLastMonthS2 = reader["Player2WinsSetsLastMonthS2"];
                    var Player2LossesSetsLastMonthS2 = reader["Player2LossesSetsLastMonthS2"];
                    var Player2WinsSetsLastWeekS2 = reader["Player2WinsSetsLastWeekS2"];
                    var Player2LossesSetsLastWeekS2 = reader["Player2LossesSetsLastWeekS2"];
                    var Player1WinsSetsTotalS3 = reader["Player1WinsSetsTotalS3"];
                    var Player1LossesSetsTotalS3 = reader["Player1LossesSetsTotalS3"];
                    var Player1WinsSetsLastYearS3 = reader["Player1WinsSetsLastYearS3"];
                    var Player1LossesSetsLastYearS3 = reader["Player1LossesSetsLastYearS3"];
                    var Player1WinsSetsLastMonthS3 = reader["Player1WinsSetsLastMonthS3"];
                    var Player1LossesSetsLastMonthS3 = reader["Player1LossesSetsLastMonthS3"];
                    var Player1WinsSetsLastWeekS3 = reader["Player1WinsSetsLastWeekS3"];
                    var Player1LossesSetsLastWeekS3 = reader["Player1LossesSetsLastWeekS3"];
                    var Player2WinsSetsTotalS3 = reader["Player2WinsSetsTotalS3"];
                    var Player2LossesSetsTotalS3 = reader["Player2LossesSetsTotalS3"];
                    var Player2WinsSetsLastYearS3 = reader["Player2WinsSetsLastYearS3"];
                    var Player2LossesSetsLastYearS3 = reader["Player2LossesSetsLastYearS3"];
                    var Player2WinsSetsLastMonthS3 = reader["Player2WinsSetsLastMonthS3"];
                    var Player2LossesSetsLastMonthS3 = reader["Player2LossesSetsLastMonthS3"];
                    var Player2WinsSetsLastWeekS3 = reader["Player2WinsSetsLastWeekS3"];
                    var Player2LossesSetsLastWeekS3 = reader["Player2LossesSetsLastWeekS3"];
                    var Player1WinsSetsTotalS4 = reader["Player1WinsSetsTotalS4"];
                    var Player1LossesSetsTotalS4 = reader["Player1LossesSetsTotalS4"];
                    var Player1WinsSetsLastYearS4 = reader["Player1WinsSetsLastYearS4"];
                    var Player1LossesSetsLastYearS4 = reader["Player1LossesSetsLastYearS4"];
                    var Player1WinsSetsLastMonthS4 = reader["Player1WinsSetsLastMonthS4"];
                    var Player1LossesSetsLastMonthS4 = reader["Player1LossesSetsLastMonthS4"];
                    var Player1WinsSetsLastWeekS4 = reader["Player1WinsSetsLastWeekS4"];
                    var Player1LossesSetsLastWeekS4 = reader["Player1LossesSetsLastWeekS4"];
                    var Player2WinsSetsTotalS4 = reader["Player2WinsSetsTotalS4"];
                    var Player2LossesSetsTotalS4 = reader["Player2LossesSetsTotalS4"];
                    var Player2WinsSetsLastYearS4 = reader["Player2WinsSetsLastYearS4"];
                    var Player2LossesSetsLastYearS4 = reader["Player2LossesSetsLastYearS4"];
                    var Player2WinsSetsLastMonthS4 = reader["Player2WinsSetsLastMonthS4"];
                    var Player2LossesSetsLastMonthS4 = reader["Player2LossesSetsLastMonthS4"];
                    var Player2WinsSetsLastWeekS4 = reader["Player2WinsSetsLastWeekS4"];
                    var Player2LossesSetsLastWeekS4 = reader["Player2LossesSetsLastWeekS4"];
                    var Player1WinsGamesTotal = reader["Player1WinsGamesTotal"];
                    var Player1LossesGamesTotal = reader["Player1LossesGamesTotal"];
                    var Player1WinsGamesLastYear = reader["Player1WinsGamesLastYear"];
                    var Player1LossesGamesLastYear = reader["Player1LossesGamesLastYear"];
                    var Player1WinsGamesLastMonth = reader["Player1WinsGamesLastMonth"];
                    var Player1LossesGamesLastMonth = reader["Player1LossesGamesLastMonth"];
                    var Player1WinsGamesLastWeek = reader["Player1WinsGamesLastWeek"];
                    var Player1LossesGamesLastWeek = reader["Player1LossesGamesLastWeek"];
                    var Player2WinsGamesTotal = reader["Player2WinsGamesTotal"];
                    var Player2LossesGamesTotal = reader["Player2LossesGamesTotal"];
                    var Player2WinsGamesLastYear = reader["Player2WinsGamesLastYear"];
                    var Player2LossesGamesLastYear = reader["Player2LossesGamesLastYear"];
                    var Player2WinsGamesLastMonth = reader["Player2WinsGamesLastMonth"];
                    var Player2LossesGamesLastMonth = reader["Player2LossesGamesLastMonth"];
                    var Player2WinsGamesLastWeek = reader["Player2WinsGamesLastWeek"];
                    var Player2LossesGamesLastWeek = reader["Player2LossesGamesLastWeek"];
                    var Player1WinsGamesTotalS1 = reader["Player1WinsGamesTotalS1"];
                    var Player1LossesGamesTotalS1 = reader["Player1LossesGamesTotalS1"];
                    var Player1WinsGamesLastYearS1 = reader["Player1WinsGamesLastYearS1"];
                    var Player1LossesGamesLastYearS1 = reader["Player1LossesGamesLastYearS1"];
                    var Player1WinsGamesLastMonthS1 = reader["Player1WinsGamesLastMonthS1"];
                    var Player1LossesGamesLastMonthS1 = reader["Player1LossesGamesLastMonthS1"];
                    var Player1WinsGamesLastWeekS1 = reader["Player1WinsGamesLastWeekS1"];
                    var Player1LossesGamesLastWeekS1 = reader["Player1LossesGamesLastWeekS1"];
                    var Player2WinsGamesTotalS1 = reader["Player2WinsGamesTotalS1"];
                    var Player2LossesGamesTotalS1 = reader["Player2LossesGamesTotalS1"];
                    var Player2WinsGamesLastYearS1 = reader["Player2WinsGamesLastYearS1"];
                    var Player2LossesGamesLastYearS1 = reader["Player2LossesGamesLastYearS1"];
                    var Player2WinsGamesLastMonthS1 = reader["Player2WinsGamesLastMonthS1"];
                    var Player2LossesGamesLastMonthS1 = reader["Player2LossesGamesLastMonthS1"];
                    var Player2WinsGamesLastWeekS1 = reader["Player2WinsGamesLastWeekS1"];
                    var Player2LossesGamesLastWeekS1 = reader["Player2LossesGamesLastWeekS1"];
                    var Player1WinsGamesTotalS2 = reader["Player1WinsGamesTotalS2"];
                    var Player1LossesGamesTotalS2 = reader["Player1LossesGamesTotalS2"];
                    var Player1WinsGamesLastYearS2 = reader["Player1WinsGamesLastYearS2"];
                    var Player1LossesGamesLastYearS2 = reader["Player1LossesGamesLastYearS2"];
                    var Player1WinsGamesLastMonthS2 = reader["Player1WinsGamesLastMonthS2"];
                    var Player1LossesGamesLastMonthS2 = reader["Player1LossesGamesLastMonthS2"];
                    var Player1WinsGamesLastWeekS2 = reader["Player1WinsGamesLastWeekS2"];
                    var Player1LossesGamesLastWeekS2 = reader["Player1LossesGamesLastWeekS2"];
                    var Player2WinsGamesTotalS2 = reader["Player2WinsGamesTotalS2"];
                    var Player2LossesGamesTotalS2 = reader["Player2LossesGamesTotalS2"];
                    var Player2WinsGamesLastYearS2 = reader["Player2WinsGamesLastYearS2"];
                    var Player2LossesGamesLastYearS2 = reader["Player2LossesGamesLastYearS2"];
                    var Player2WinsGamesLastMonthS2 = reader["Player2WinsGamesLastMonthS2"];
                    var Player2LossesGamesLastMonthS2 = reader["Player2LossesGamesLastMonthS2"];
                    var Player2WinsGamesLastWeekS2 = reader["Player2WinsGamesLastWeekS2"];
                    var Player2LossesGamesLastWeekS2 = reader["Player2LossesGamesLastWeekS2"];
                    var Player1WinsGamesTotalS3 = reader["Player1WinsGamesTotalS3"];
                    var Player1LossesGamesTotalS3 = reader["Player1LossesGamesTotalS3"];
                    var Player1WinsGamesLastYearS3 = reader["Player1WinsGamesLastYearS3"];
                    var Player1LossesGamesLastYearS3 = reader["Player1LossesGamesLastYearS3"];
                    var Player1WinsGamesLastMonthS3 = reader["Player1WinsGamesLastMonthS3"];
                    var Player1LossesGamesLastMonthS3 = reader["Player1LossesGamesLastMonthS3"];
                    var Player1WinsGamesLastWeekS3 = reader["Player1WinsGamesLastWeekS3"];
                    var Player1LossesGamesLastWeekS3 = reader["Player1LossesGamesLastWeekS3"];
                    var Player2WinsGamesTotalS3 = reader["Player2WinsGamesTotalS3"];
                    var Player2LossesGamesTotalS3 = reader["Player2LossesGamesTotalS3"];
                    var Player2WinsGamesLastYearS3 = reader["Player2WinsGamesLastYearS3"];
                    var Player2LossesGamesLastYearS3 = reader["Player2LossesGamesLastYearS3"];
                    var Player2WinsGamesLastMonthS3 = reader["Player2WinsGamesLastMonthS3"];
                    var Player2LossesGamesLastMonthS3 = reader["Player2LossesGamesLastMonthS3"];
                    var Player2WinsGamesLastWeekS3 = reader["Player2WinsGamesLastWeekS3"];
                    var Player2LossesGamesLastWeekS3 = reader["Player2LossesGamesLastWeekS3"];
                    var Player1WinsGamesTotalS4 = reader["Player1WinsGamesTotalS4"];
                    var Player1LossesGamesTotalS4 = reader["Player1LossesGamesTotalS4"];
                    var Player1WinsGamesLastYearS4 = reader["Player1WinsGamesLastYearS4"];
                    var Player1LossesGamesLastYearS4 = reader["Player1LossesGamesLastYearS4"];
                    var Player1WinsGamesLastMonthS4 = reader["Player1WinsGamesLastMonthS4"];
                    var Player1LossesGamesLastMonthS4 = reader["Player1LossesGamesLastMonthS4"];
                    var Player1WinsGamesLastWeekS4 = reader["Player1WinsGamesLastWeekS4"];
                    var Player1LossesGamesLastWeekS4 = reader["Player1LossesGamesLastWeekS4"];
                    var Player2WinsGamesTotalS4 = reader["Player2WinsGamesTotalS4"];
                    var Player2LossesGamesTotalS4 = reader["Player2LossesGamesTotalS4"];
                    var Player2WinsGamesLastYearS4 = reader["Player2WinsGamesLastYearS4"];
                    var Player2LossesGamesLastYearS4 = reader["Player2LossesGamesLastYearS4"];
                    var Player2WinsGamesLastMonthS4 = reader["Player2WinsGamesLastMonthS4"];
                    var Player2LossesGamesLastMonthS4 = reader["Player2LossesGamesLastMonthS4"];
                    var Player2WinsGamesLastWeekS4 = reader["Player2WinsGamesLastWeekS4"];
                    var Player2LossesGamesLastWeekS4 = reader["Player2LossesGamesLastWeekS4"];
                    var Player1DaysSinceLastWin = reader["Player1DaysSinceLastWin"];
                    var Player2DaysSinceLastWin = reader["Player2DaysSinceLastWin"];
                    var Player1DaysSinceLastWinS1 = reader["Player1DaysSinceLastWinS1"];
                    var Player2DaysSinceLastWinS1 = reader["Player2DaysSinceLastWinS1"];
                    var Player1DaysSinceLastWinS2 = reader["Player1DaysSinceLastWinS2"];
                    var Player2DaysSinceLastWinS2 = reader["Player2DaysSinceLastWinS2"];
                    var Player1DaysSinceLastWinS3 = reader["Player1DaysSinceLastWinS3"];
                    var Player2DaysSinceLastWinS3 = reader["Player2DaysSinceLastWinS3"];
                    var Player1DaysSinceLastWinS4 = reader["Player1DaysSinceLastWinS4"];
                    var Player2DaysSinceLastWinS4 = reader["Player2DaysSinceLastWinS4"];
                    var Player1DaysSinceLastLoss = reader["Player1DaysSinceLastLoss"];
                    var Player2DaysSinceLastLoss = reader["Player2DaysSinceLastLoss"];
                    var Player1DaysSinceLastLossS1 = reader["Player1DaysSinceLastLossS1"];
                    var Player2DaysSinceLastLossS1 = reader["Player2DaysSinceLastLossS1"];
                    var Player1DaysSinceLastLossS2 = reader["Player1DaysSinceLastLossS2"];
                    var Player2DaysSinceLastLossS2 = reader["Player2DaysSinceLastLossS2"];
                    var Player1DaysSinceLastLossS3 = reader["Player1DaysSinceLastLossS3"];
                    var Player2DaysSinceLastLossS3 = reader["Player2DaysSinceLastLossS3"];
                    var Player1DaysSinceLastLossS4 = reader["Player1DaysSinceLastLossS4"];
                    var Player2DaysSinceLastLossS4 = reader["Player2DaysSinceLastLossS4"];
                    var Player1TotalWinsAsFavourite = reader["Player1TotalWinsAsFavourite"];
                    var Player2TotalWinsAsFavourite = reader["Player2TotalWinsAsFavourite"];
                    var Player1TotalWinsAsUnderdog = reader["Player1TotalWinsAsUnderdog"];
                    var Player2TotalWinsAsUnderdog = reader["Player2TotalWinsAsUnderdog"];
                    var Player1TotalLossesAsFavourite = reader["Player1TotalLossesAsFavourite"];
                    var Player2TotalLossesAsFavourite = reader["Player2TotalLossesAsFavourite"];
                    var Player1TotalLossesAsUnderdog = reader["Player1TotalLossesAsUnderdog"];
                    var Player2TotalLossesAsUnderdog = reader["Player2TotalLossesAsUnderdog"];
                    var Player1AverageWinningProbabilityAtWonAsFavourite = reader["Player1AverageWinningProbabilityAtWonAsFavourite"];
                    var Player2AverageWinningProbabilityAtWonAsFavourite = reader["Player2AverageWinningProbabilityAtWonAsFavourite"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdog = reader["Player1AverageWinningProbabilityAtWonAsUnderdog"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdog = reader["Player2AverageWinningProbabilityAtWonAsUnderdog"];
                    var Player1AverageWinningProbabilityAtLossAsFavourite = reader["Player1AverageWinningProbabilityAtLossAsFavourite"];
                    var Player2AverageWinningProbabilityAtLossAsFavourite = reader["Player2AverageWinningProbabilityAtLossAsFavourite"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdog = reader["Player1AverageWinningProbabilityAtLossAsUnderdog"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdog = reader["Player2AverageWinningProbabilityAtLossAsUnderdog"];
                    var Player1TotalWinsAsFavouriteLastYear = reader["Player1TotalWinsAsFavouriteLastYear"];
                    var Player2TotalWinsAsFavouriteLastYear = reader["Player2TotalWinsAsFavouriteLastYear"];
                    var Player1TotalWinsAsUnderdogLastYear = reader["Player1TotalWinsAsUnderdogLastYear"];
                    var Player2TotalWinsAsUnderdogLastYear = reader["Player2TotalWinsAsUnderdogLastYear"];
                    var Player1TotalLossesAsFavouriteLastYear = reader["Player1TotalLossesAsFavouriteLastYear"];
                    var Player2TotalLossesAsFavouriteLastYear = reader["Player2TotalLossesAsFavouriteLastYear"];
                    var Player1TotalLossesAsUnderdogLastYear = reader["Player1TotalLossesAsUnderdogLastYear"];
                    var Player2TotalLossesAsUnderdogLastYear = reader["Player2TotalLossesAsUnderdogLastYear"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastYear"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastYear"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastYear"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastYear"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastYear"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastYear"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastYear"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastYear"];
                    var Player1TotalWinsAsFavouriteLastMonth = reader["Player1TotalWinsAsFavouriteLastMonth"];
                    var Player2TotalWinsAsFavouriteLastMonth = reader["Player2TotalWinsAsFavouriteLastMonth"];
                    var Player1TotalWinsAsUnderdogLastMonth = reader["Player1TotalWinsAsUnderdogLastMonth"];
                    var Player2TotalWinsAsUnderdogLastMonth = reader["Player2TotalWinsAsUnderdogLastMonth"];
                    var Player1TotalLossesAsFavouriteLastMonth = reader["Player1TotalLossesAsFavouriteLastMonth"];
                    var Player2TotalLossesAsFavouriteLastMonth = reader["Player2TotalLossesAsFavouriteLastMonth"];
                    var Player1TotalLossesAsUnderdogLastMonth = reader["Player1TotalLossesAsUnderdogLastMonth"];
                    var Player2TotalLossesAsUnderdogLastMonth = reader["Player2TotalLossesAsUnderdogLastMonth"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth"];
                    var Player1TotalWinsAsFavouriteLastWeek = reader["Player1TotalWinsAsFavouriteLastWeek"];
                    var Player2TotalWinsAsFavouriteLastWeek = reader["Player2TotalWinsAsFavouriteLastWeek"];
                    var Player1TotalWinsAsUnderdogLastWeek = reader["Player1TotalWinsAsUnderdogLastWeek"];
                    var Player2TotalWinsAsUnderdogLastWeek = reader["Player2TotalWinsAsUnderdogLastWeek"];
                    var Player1TotalLossesAsFavouriteLastWeek = reader["Player1TotalLossesAsFavouriteLastWeek"];
                    var Player2TotalLossesAsFavouriteLastWeek = reader["Player2TotalLossesAsFavouriteLastWeek"];
                    var Player1TotalLossesAsUnderdogLastWeek = reader["Player1TotalLossesAsUnderdogLastWeek"];
                    var Player2TotalLossesAsUnderdogLastWeek = reader["Player2TotalLossesAsUnderdogLastWeek"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek"];
                    var Player1H2H = reader["Player1H2H"];
                    var Player2H2H = reader["Player2H2H"];
                    var Player1H2HOld = reader["Player1H2HOld"];
                    var Player2H2HOld = reader["Player2H2HOld"];
                    var Player1H2HTrueSkillMeanM = reader["Player1H2HTrueSkillMeanM"];
                    var Player1H2HTrueSkillStandardDeviationM = reader["Player1H2HTrueSkillStandardDeviationM"];
                    var Player2H2HTrueSkillMeanM = reader["Player2H2HTrueSkillMeanM"];
                    var Player2H2HTrueSkillStandardDeviationM = reader["Player2H2HTrueSkillStandardDeviationM"];
                    var Player1H2HTrueSkillMeanOldM = reader["Player1H2HTrueSkillMeanOldM"];
                    var Player1H2HTrueSkillStandardDeviationOldM = reader["Player1H2HTrueSkillStandardDeviationOldM"];
                    var Player2H2HTrueSkillMeanOldM = reader["Player2H2HTrueSkillMeanOldM"];
                    var Player2H2HTrueSkillStandardDeviationOldM = reader["Player2H2HTrueSkillStandardDeviationOldM"];
                    var WinProbabilityPlayer1H2HM = reader["WinProbabilityPlayer1H2HM"];
                    var Player1H2HTrueSkillMeanSM = reader["Player1H2HTrueSkillMeanSM"];
                    var Player1H2HTrueSkillStandardDeviationSM = reader["Player1H2HTrueSkillStandardDeviationSM"];
                    var Player2H2HTrueSkillMeanSM = reader["Player2H2HTrueSkillMeanSM"];
                    var Player2H2HTrueSkillStandardDeviationSM = reader["Player2H2HTrueSkillStandardDeviationSM"];
                    var Player1H2HTrueSkillMeanOldSM = reader["Player1H2HTrueSkillMeanOldSM"];
                    var Player1H2HTrueSkillStandardDeviationOldSM = reader["Player1H2HTrueSkillStandardDeviationOldSM"];
                    var Player2H2HTrueSkillMeanOldSM = reader["Player2H2HTrueSkillMeanOldSM"];
                    var Player2H2HTrueSkillStandardDeviationOldSM = reader["Player2H2HTrueSkillStandardDeviationOldSM"];
                    var WinProbabilityPlayer1H2HSM = reader["WinProbabilityPlayer1H2HSM"];
                    var Player1H2HTrueSkillMeanGSM = reader["Player1H2HTrueSkillMeanGSM"];
                    var Player1H2HTrueSkillStandardDeviationGSM = reader["Player1H2HTrueSkillStandardDeviationGSM"];
                    var Player2H2HTrueSkillMeanGSM = reader["Player2H2HTrueSkillMeanGSM"];
                    var Player2H2HTrueSkillStandardDeviationGSM = reader["Player2H2HTrueSkillStandardDeviationGSM"];
                    var Player1H2HTrueSkillMeanOldGSM = reader["Player1H2HTrueSkillMeanOldGSM"];
                    var Player1H2HTrueSkillStandardDeviationOldGSM = reader["Player1H2HTrueSkillStandardDeviationOldGSM"];
                    var Player2H2HTrueSkillMeanOldGSM = reader["Player2H2HTrueSkillMeanOldGSM"];
                    var Player2H2HTrueSkillStandardDeviationOldGSM = reader["Player2H2HTrueSkillStandardDeviationOldGSM"];
                    var WinProbabilityPlayer1H2HGSM = reader["WinProbabilityPlayer1H2HGSM"];
                    var Player1H2HS1 = reader["Player1H2HS1"];
                    var Player2H2HS1 = reader["Player2H2HS1"];
                    var Player1H2HOldS1 = reader["Player1H2HOldS1"];
                    var Player2H2HOldS1 = reader["Player2H2HOldS1"];
                    var Player1H2HTrueSkillMeanMS1 = reader["Player1H2HTrueSkillMeanMS1"];
                    var Player1H2HTrueSkillStandardDeviationMS1 = reader["Player1H2HTrueSkillStandardDeviationMS1"];
                    var Player2H2HTrueSkillMeanMS1 = reader["Player2H2HTrueSkillMeanMS1"];
                    var Player2H2HTrueSkillStandardDeviationMS1 = reader["Player2H2HTrueSkillStandardDeviationMS1"];
                    var Player1H2HTrueSkillMeanOldMS1 = reader["Player1H2HTrueSkillMeanOldMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldMS1 = reader["Player1H2HTrueSkillStandardDeviationOldMS1"];
                    var Player2H2HTrueSkillMeanOldMS1 = reader["Player2H2HTrueSkillMeanOldMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldMS1 = reader["Player2H2HTrueSkillStandardDeviationOldMS1"];
                    var WinProbabilityPlayer1H2HMS1 = reader["WinProbabilityPlayer1H2HMS1"];
                    var Player1H2HTrueSkillMeanSMS1 = reader["Player1H2HTrueSkillMeanSMS1"];
                    var Player1H2HTrueSkillStandardDeviationSMS1 = reader["Player1H2HTrueSkillStandardDeviationSMS1"];
                    var Player2H2HTrueSkillMeanSMS1 = reader["Player2H2HTrueSkillMeanSMS1"];
                    var Player2H2HTrueSkillStandardDeviationSMS1 = reader["Player2H2HTrueSkillStandardDeviationSMS1"];
                    var Player1H2HTrueSkillMeanOldSMS1 = reader["Player1H2HTrueSkillMeanOldSMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldSMS1"];
                    var Player2H2HTrueSkillMeanOldSMS1 = reader["Player2H2HTrueSkillMeanOldSMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldSMS1"];
                    var WinProbabilityPlayer1H2HSMS1 = reader["WinProbabilityPlayer1H2HSMS1"];
                    var Player1H2HTrueSkillMeanGSMS1 = reader["Player1H2HTrueSkillMeanGSMS1"];
                    var Player1H2HTrueSkillStandardDeviationGSMS1 = reader["Player1H2HTrueSkillStandardDeviationGSMS1"];
                    var Player2H2HTrueSkillMeanGSMS1 = reader["Player2H2HTrueSkillMeanGSMS1"];
                    var Player2H2HTrueSkillStandardDeviationGSMS1 = reader["Player2H2HTrueSkillStandardDeviationGSMS1"];
                    var Player1H2HTrueSkillMeanOldGSMS1 = reader["Player1H2HTrueSkillMeanOldGSMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS1"];
                    var Player2H2HTrueSkillMeanOldGSMS1 = reader["Player2H2HTrueSkillMeanOldGSMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS1"];
                    var WinProbabilityPlayer1H2HGSMS1 = reader["WinProbabilityPlayer1H2HGSMS1"];
                    var Player1H2HS2 = reader["Player1H2HS2"];
                    var Player2H2HS2 = reader["Player2H2HS2"];
                    var Player1H2HOldS2 = reader["Player1H2HOldS2"];
                    var Player2H2HOldS2 = reader["Player2H2HOldS2"];
                    var Player1H2HTrueSkillMeanMS2 = reader["Player1H2HTrueSkillMeanMS2"];
                    var Player1H2HTrueSkillStandardDeviationMS2 = reader["Player1H2HTrueSkillStandardDeviationMS2"];
                    var Player2H2HTrueSkillMeanMS2 = reader["Player2H2HTrueSkillMeanMS2"];
                    var Player2H2HTrueSkillStandardDeviationMS2 = reader["Player2H2HTrueSkillStandardDeviationMS2"];
                    var Player1H2HTrueSkillMeanOldMS2 = reader["Player1H2HTrueSkillMeanOldMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldMS2 = reader["Player1H2HTrueSkillStandardDeviationOldMS2"];
                    var Player2H2HTrueSkillMeanOldMS2 = reader["Player2H2HTrueSkillMeanOldMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldMS2 = reader["Player2H2HTrueSkillStandardDeviationOldMS2"];
                    var WinProbabilityPlayer1H2HMS2 = reader["WinProbabilityPlayer1H2HMS2"];
                    var Player1H2HTrueSkillMeanSMS2 = reader["Player1H2HTrueSkillMeanSMS2"];
                    var Player1H2HTrueSkillStandardDeviationSMS2 = reader["Player1H2HTrueSkillStandardDeviationSMS2"];
                    var Player2H2HTrueSkillMeanSMS2 = reader["Player2H2HTrueSkillMeanSMS2"];
                    var Player2H2HTrueSkillStandardDeviationSMS2 = reader["Player2H2HTrueSkillStandardDeviationSMS2"];
                    var Player1H2HTrueSkillMeanOldSMS2 = reader["Player1H2HTrueSkillMeanOldSMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldSMS2"];
                    var Player2H2HTrueSkillMeanOldSMS2 = reader["Player2H2HTrueSkillMeanOldSMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldSMS2"];
                    var WinProbabilityPlayer1H2HSMS2 = reader["WinProbabilityPlayer1H2HSMS2"];
                    var Player1H2HTrueSkillMeanGSMS2 = reader["Player1H2HTrueSkillMeanGSMS2"];
                    var Player1H2HTrueSkillStandardDeviationGSMS2 = reader["Player1H2HTrueSkillStandardDeviationGSMS2"];
                    var Player2H2HTrueSkillMeanGSMS2 = reader["Player2H2HTrueSkillMeanGSMS2"];
                    var Player2H2HTrueSkillStandardDeviationGSMS2 = reader["Player2H2HTrueSkillStandardDeviationGSMS2"];
                    var Player1H2HTrueSkillMeanOldGSMS2 = reader["Player1H2HTrueSkillMeanOldGSMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS2"];
                    var Player2H2HTrueSkillMeanOldGSMS2 = reader["Player2H2HTrueSkillMeanOldGSMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS2"];
                    var WinProbabilityPlayer1H2HGSMS2 = reader["WinProbabilityPlayer1H2HGSMS2"];
                    var Player1H2HS3 = reader["Player1H2HS3"];
                    var Player2H2HS3 = reader["Player2H2HS3"];
                    var Player1H2HOldS3 = reader["Player1H2HOldS3"];
                    var Player2H2HOldS3 = reader["Player2H2HOldS3"];
                    var Player1H2HTrueSkillMeanMS3 = reader["Player1H2HTrueSkillMeanMS3"];
                    var Player1H2HTrueSkillStandardDeviationMS3 = reader["Player1H2HTrueSkillStandardDeviationMS3"];
                    var Player2H2HTrueSkillMeanMS3 = reader["Player2H2HTrueSkillMeanMS3"];
                    var Player2H2HTrueSkillStandardDeviationMS3 = reader["Player2H2HTrueSkillStandardDeviationMS3"];
                    var Player1H2HTrueSkillMeanOldMS3 = reader["Player1H2HTrueSkillMeanOldMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldMS3 = reader["Player1H2HTrueSkillStandardDeviationOldMS3"];
                    var Player2H2HTrueSkillMeanOldMS3 = reader["Player2H2HTrueSkillMeanOldMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldMS3 = reader["Player2H2HTrueSkillStandardDeviationOldMS3"];
                    var WinProbabilityPlayer1H2HMS3 = reader["WinProbabilityPlayer1H2HMS3"];
                    var Player1H2HTrueSkillMeanSMS3 = reader["Player1H2HTrueSkillMeanSMS3"];
                    var Player1H2HTrueSkillStandardDeviationSMS3 = reader["Player1H2HTrueSkillStandardDeviationSMS3"];
                    var Player2H2HTrueSkillMeanSMS3 = reader["Player2H2HTrueSkillMeanSMS3"];
                    var Player2H2HTrueSkillStandardDeviationSMS3 = reader["Player2H2HTrueSkillStandardDeviationSMS3"];
                    var Player1H2HTrueSkillMeanOldSMS3 = reader["Player1H2HTrueSkillMeanOldSMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldSMS3"];
                    var Player2H2HTrueSkillMeanOldSMS3 = reader["Player2H2HTrueSkillMeanOldSMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldSMS3"];
                    var WinProbabilityPlayer1H2HSMS3 = reader["WinProbabilityPlayer1H2HSMS3"];
                    var Player1H2HTrueSkillMeanGSMS3 = reader["Player1H2HTrueSkillMeanGSMS3"];
                    var Player1H2HTrueSkillStandardDeviationGSMS3 = reader["Player1H2HTrueSkillStandardDeviationGSMS3"];
                    var Player2H2HTrueSkillMeanGSMS3 = reader["Player2H2HTrueSkillMeanGSMS3"];
                    var Player2H2HTrueSkillStandardDeviationGSMS3 = reader["Player2H2HTrueSkillStandardDeviationGSMS3"];
                    var Player1H2HTrueSkillMeanOldGSMS3 = reader["Player1H2HTrueSkillMeanOldGSMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS3"];
                    var Player2H2HTrueSkillMeanOldGSMS3 = reader["Player2H2HTrueSkillMeanOldGSMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS3"];
                    var WinProbabilityPlayer1H2HGSMS3 = reader["WinProbabilityPlayer1H2HGSMS3"];
                    var Player1H2HS4 = reader["Player1H2HS4"];
                    var Player2H2HS4 = reader["Player2H2HS4"];
                    var Player1H2HOldS4 = reader["Player1H2HOldS4"];
                    var Player2H2HOldS4 = reader["Player2H2HOldS4"];
                    var Player1H2HTrueSkillMeanMS4 = reader["Player1H2HTrueSkillMeanMS4"];
                    var Player1H2HTrueSkillStandardDeviationMS4 = reader["Player1H2HTrueSkillStandardDeviationMS4"];
                    var Player2H2HTrueSkillMeanMS4 = reader["Player2H2HTrueSkillMeanMS4"];
                    var Player2H2HTrueSkillStandardDeviationMS4 = reader["Player2H2HTrueSkillStandardDeviationMS4"];
                    var Player1H2HTrueSkillMeanOldMS4 = reader["Player1H2HTrueSkillMeanOldMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldMS4 = reader["Player1H2HTrueSkillStandardDeviationOldMS4"];
                    var Player2H2HTrueSkillMeanOldMS4 = reader["Player2H2HTrueSkillMeanOldMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldMS4 = reader["Player2H2HTrueSkillStandardDeviationOldMS4"];
                    var WinProbabilityPlayer1H2HMS4 = reader["WinProbabilityPlayer1H2HMS4"];
                    var Player1H2HTrueSkillMeanSMS4 = reader["Player1H2HTrueSkillMeanSMS4"];
                    var Player1H2HTrueSkillStandardDeviationSMS4 = reader["Player1H2HTrueSkillStandardDeviationSMS4"];
                    var Player2H2HTrueSkillMeanSMS4 = reader["Player2H2HTrueSkillMeanSMS4"];
                    var Player2H2HTrueSkillStandardDeviationSMS4 = reader["Player2H2HTrueSkillStandardDeviationSMS4"];
                    var Player1H2HTrueSkillMeanOldSMS4 = reader["Player1H2HTrueSkillMeanOldSMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldSMS4"];
                    var Player2H2HTrueSkillMeanOldSMS4 = reader["Player2H2HTrueSkillMeanOldSMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldSMS4"];
                    var WinProbabilityPlayer1H2HSMS4 = reader["WinProbabilityPlayer1H2HSMS4"];
                    var Player1H2HTrueSkillMeanGSMS4 = reader["Player1H2HTrueSkillMeanGSMS4"];
                    var Player1H2HTrueSkillStandardDeviationGSMS4 = reader["Player1H2HTrueSkillStandardDeviationGSMS4"];
                    var Player2H2HTrueSkillMeanGSMS4 = reader["Player2H2HTrueSkillMeanGSMS4"];
                    var Player2H2HTrueSkillStandardDeviationGSMS4 = reader["Player2H2HTrueSkillStandardDeviationGSMS4"];
                    var Player1H2HTrueSkillMeanOldGSMS4 = reader["Player1H2HTrueSkillMeanOldGSMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS4"];
                    var Player2H2HTrueSkillMeanOldGSMS4 = reader["Player2H2HTrueSkillMeanOldGSMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS4"];
                    var WinProbabilityPlayer1H2HGSMS4 = reader["WinProbabilityPlayer1H2HGSMS4"];
                    var Player1Streak = reader["Player1Streak"];
                    var Player2Streak = reader["Player2Streak"];
                    var Player1StreakS1 = reader["Player1StreakS1"];
                    var Player2StreakS1 = reader["Player2StreakS1"];
                    var Player1StreakS2 = reader["Player1StreakS2"];
                    var Player2StreakS2 = reader["Player2StreakS2"];
                    var Player1StreakS3 = reader["Player1StreakS3"];
                    var Player2StreakS3 = reader["Player2StreakS3"];
                    var Player1StreakS4 = reader["Player1StreakS4"];
                    var Player2StreakS4 = reader["Player2StreakS4"];
                    var P1SetsWon = reader["P1SetsWon"];
                    var P2SetsWon = reader["P2SetsWon"];
                    var P1GamesWon = reader["P1GamesWon"];
                    var P2GamesWon = reader["P2GamesWon"];
                    var P1SetsLoss = reader["P1SetsLoss"];
                    var P2SetsLoss = reader["P2SetsLoss"];
                    var P1GamesLoss = reader["P1GamesLoss"];
                    var P2GamesLoss = reader["P2GamesLoss"];
                    var WinProbabilityNN = reader["WinProbabilityNN"];
                    var IsFinished = 1; // dodati nakon dodavanja atrbuta u bazu reader["IsFinished"];
                }
                catch (Exception ex)
                {
                    string aaa;
                    aaa = "";
                }

                var Match = new Match
                {
                    MatchTPId = reader["MatchTPId"] as int?,
                    TournamentEventTPId = reader["TournamentEventTPId"] as int?,
                    DateTime = reader["DateTime"] as DateTime?,
                    Player1TPId = reader["Player1TPId"] as int?,
                    Player2TPId = reader["Player2TPId"] as int?,
                    Player1Seed = reader["Player1Seed"] as string,
                    Player2Seed = reader["Player2Seed"] as string,
                    Result = reader["Result"] as string,
                    ResultDetails = reader["ResultDetails"] as string,
                    Player1Odds = reader["Player1Odds"] as double?,
                    Player2Odds = reader["Player2Odds"] as double?,
                    Player1Percentage = reader["Player1Percentage"] as double?,
                    Player2Percentage = reader["Player2Percentage"] as double?,
                    SurfaceId = reader["SurfaceId"] as int?,
                    RoundId = reader["RoundId"] as int?,
                    Player1TrueSkillMeanM = reader["Player1TrueSkillMeanM"] as double?,
                    Player1TrueSkillStandardDeviationM = reader["Player1TrueSkillStandardDeviationM"] as double?,
                    Player2TrueSkillMeanM = reader["Player2TrueSkillMeanM"] as double?,
                    Player2TrueSkillStandardDeviationM = reader["Player2TrueSkillStandardDeviationM"] as double?,
                    Player1TrueSkillMeanOldM = reader["Player1TrueSkillMeanOldM"] as double?,
                    Player1TrueSkillStandardDeviationOldM = reader["Player1TrueSkillStandardDeviationOldM"] as double?,
                    Player2TrueSkillMeanOldM = reader["Player2TrueSkillMeanOldM"] as double?,
                    Player2TrueSkillStandardDeviationOldM = reader["Player2TrueSkillStandardDeviationOldM"] as double?,
                    WinProbabilityPlayer1M = reader["WinProbabilityPlayer1M"] as double?,
                    Player1TrueSkillMeanSM = reader["Player1TrueSkillMeanSM"] as double?,
                    Player1TrueSkillStandardDeviationSM = reader["Player1TrueSkillStandardDeviationSM"] as double?,
                    Player2TrueSkillMeanSM = reader["Player2TrueSkillMeanSM"] as double?,
                    Player2TrueSkillStandardDeviationSM = reader["Player2TrueSkillStandardDeviationSM"] as double?,
                    Player1TrueSkillMeanOldSM = reader["Player1TrueSkillMeanOldSM"] as double?,
                    Player1TrueSkillStandardDeviationOldSM = reader["Player1TrueSkillStandardDeviationOldSM"] as double?,
                    Player2TrueSkillMeanOldSM = reader["Player2TrueSkillMeanOldSM"] as double?,
                    Player2TrueSkillStandardDeviationOldSM = reader["Player2TrueSkillStandardDeviationOldSM"] as double?,
                    WinProbabilityPlayer1SM = reader["WinProbabilityPlayer1SM"] as double?,
                    Player1TrueSkillMeanGSM = reader["Player1TrueSkillMeanGSM"] as double?,
                    Player1TrueSkillStandardDeviationGSM = reader["Player1TrueSkillStandardDeviationGSM"] as double?,
                    Player2TrueSkillMeanGSM = reader["Player2TrueSkillMeanGSM"] as double?,
                    Player2TrueSkillStandardDeviationGSM = reader["Player2TrueSkillStandardDeviationGSM"] as double?,
                    Player1TrueSkillMeanOldGSM = reader["Player1TrueSkillMeanOldGSM"] as double?,
                    Player1TrueSkillStandardDeviationOldGSM = reader["Player1TrueSkillStandardDeviationOldGSM"] as double?,
                    Player2TrueSkillMeanOldGSM = reader["Player2TrueSkillMeanOldGSM"] as double?,
                    Player2TrueSkillStandardDeviationOldGSM = reader["Player2TrueSkillStandardDeviationOldGSM"] as double?,
                    WinProbabilityPlayer1GSM = reader["WinProbabilityPlayer1GSM"] as double?,
                    Player1TrueSkillMeanMS1 = reader["Player1TrueSkillMeanMS1"] as double?,
                    Player1TrueSkillStandardDeviationMS1 = reader["Player1TrueSkillStandardDeviationMS1"] as double?,
                    Player2TrueSkillMeanMS1 = reader["Player2TrueSkillMeanMS1"] as double?,
                    Player2TrueSkillStandardDeviationMS1 = reader["Player2TrueSkillStandardDeviationMS1"] as double?,
                    Player1TrueSkillMeanOldMS1 = reader["Player1TrueSkillMeanOldMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldMS1 = reader["Player1TrueSkillStandardDeviationOldMS1"] as double?,
                    Player2TrueSkillMeanOldMS1 = reader["Player2TrueSkillMeanOldMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldMS1 = reader["Player2TrueSkillStandardDeviationOldMS1"] as double?,
                    WinProbabilityPlayer1MS1 = reader["WinProbabilityPlayer1MS1"] as double?,
                    Player1TrueSkillMeanSMS1 = reader["Player1TrueSkillMeanSMS1"] as double?,
                    Player1TrueSkillStandardDeviationSMS1 = reader["Player1TrueSkillStandardDeviationSMS1"] as double?,
                    Player2TrueSkillMeanSMS1 = reader["Player2TrueSkillMeanSMS1"] as double?,
                    Player2TrueSkillStandardDeviationSMS1 = reader["Player2TrueSkillStandardDeviationSMS1"] as double?,
                    Player1TrueSkillMeanOldSMS1 = reader["Player1TrueSkillMeanOldSMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS1 = reader["Player1TrueSkillStandardDeviationOldSMS1"] as double?,
                    Player2TrueSkillMeanOldSMS1 = reader["Player2TrueSkillMeanOldSMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS1 = reader["Player2TrueSkillStandardDeviationOldSMS1"] as double?,
                    WinProbabilityPlayer1SMS1 = reader["WinProbabilityPlayer1SMS1"] as double?,
                    Player1TrueSkillMeanGSMS1 = reader["Player1TrueSkillMeanGSMS1"] as double?,
                    Player1TrueSkillStandardDeviationGSMS1 = reader["Player1TrueSkillStandardDeviationGSMS1"] as double?,
                    Player2TrueSkillMeanGSMS1 = reader["Player2TrueSkillMeanGSMS1"] as double?,
                    Player2TrueSkillStandardDeviationGSMS1 = reader["Player2TrueSkillStandardDeviationGSMS1"] as double?,
                    Player1TrueSkillMeanOldGSMS1 = reader["Player1TrueSkillMeanOldGSMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS1 = reader["Player1TrueSkillStandardDeviationOldGSMS1"] as double?,
                    Player2TrueSkillMeanOldGSMS1 = reader["Player2TrueSkillMeanOldGSMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS1 = reader["Player2TrueSkillStandardDeviationOldGSMS1"] as double?,
                    WinProbabilityPlayer1GSMS1 = reader["WinProbabilityPlayer1GSMS1"] as double?,
                    Player1TrueSkillMeanMS2 = reader["Player1TrueSkillMeanMS2"] as double?,
                    Player1TrueSkillStandardDeviationMS2 = reader["Player1TrueSkillStandardDeviationMS2"] as double?,
                    Player2TrueSkillMeanMS2 = reader["Player2TrueSkillMeanMS2"] as double?,
                    Player2TrueSkillStandardDeviationMS2 = reader["Player2TrueSkillStandardDeviationMS2"] as double?,
                    Player1TrueSkillMeanOldMS2 = reader["Player1TrueSkillMeanOldMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldMS2 = reader["Player1TrueSkillStandardDeviationOldMS2"] as double?,
                    Player2TrueSkillMeanOldMS2 = reader["Player2TrueSkillMeanOldMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldMS2 = reader["Player2TrueSkillStandardDeviationOldMS2"] as double?,
                    WinProbabilityPlayer1MS2 = reader["WinProbabilityPlayer1MS2"] as double?,
                    Player1TrueSkillMeanSMS2 = reader["Player1TrueSkillMeanSMS2"] as double?,
                    Player1TrueSkillStandardDeviationSMS2 = reader["Player1TrueSkillStandardDeviationSMS2"] as double?,
                    Player2TrueSkillMeanSMS2 = reader["Player2TrueSkillMeanSMS2"] as double?,
                    Player2TrueSkillStandardDeviationSMS2 = reader["Player2TrueSkillStandardDeviationSMS2"] as double?,
                    Player1TrueSkillMeanOldSMS2 = reader["Player1TrueSkillMeanOldSMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS2 = reader["Player1TrueSkillStandardDeviationOldSMS2"] as double?,
                    Player2TrueSkillMeanOldSMS2 = reader["Player2TrueSkillMeanOldSMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS2 = reader["Player2TrueSkillStandardDeviationOldSMS2"] as double?,
                    WinProbabilityPlayer1SMS2 = reader["WinProbabilityPlayer1SMS2"] as double?,
                    Player1TrueSkillMeanGSMS2 = reader["Player1TrueSkillMeanGSMS2"] as double?,
                    Player1TrueSkillStandardDeviationGSMS2 = reader["Player1TrueSkillStandardDeviationGSMS2"] as double?,
                    Player2TrueSkillMeanGSMS2 = reader["Player2TrueSkillMeanGSMS2"] as double?,
                    Player2TrueSkillStandardDeviationGSMS2 = reader["Player2TrueSkillStandardDeviationGSMS2"] as double?,
                    Player1TrueSkillMeanOldGSMS2 = reader["Player1TrueSkillMeanOldGSMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS2 = reader["Player1TrueSkillStandardDeviationOldGSMS2"] as double?,
                    Player2TrueSkillMeanOldGSMS2 = reader["Player2TrueSkillMeanOldGSMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS2 = reader["Player2TrueSkillStandardDeviationOldGSMS2"] as double?,
                    WinProbabilityPlayer1GSMS2 = reader["WinProbabilityPlayer1GSMS2"] as double?,
                    Player1TrueSkillMeanMS3 = reader["Player1TrueSkillMeanMS3"] as double?,
                    Player1TrueSkillStandardDeviationMS3 = reader["Player1TrueSkillStandardDeviationMS3"] as double?,
                    Player2TrueSkillMeanMS3 = reader["Player2TrueSkillMeanMS3"] as double?,
                    Player2TrueSkillStandardDeviationMS3 = reader["Player2TrueSkillStandardDeviationMS3"] as double?,
                    Player1TrueSkillMeanOldMS3 = reader["Player1TrueSkillMeanOldMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldMS3 = reader["Player1TrueSkillStandardDeviationOldMS3"] as double?,
                    Player2TrueSkillMeanOldMS3 = reader["Player2TrueSkillMeanOldMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldMS3 = reader["Player2TrueSkillStandardDeviationOldMS3"] as double?,
                    WinProbabilityPlayer1MS3 = reader["WinProbabilityPlayer1MS3"] as double?,
                    Player1TrueSkillMeanSMS3 = reader["Player1TrueSkillMeanSMS3"] as double?,
                    Player1TrueSkillStandardDeviationSMS3 = reader["Player1TrueSkillStandardDeviationSMS3"] as double?,
                    Player2TrueSkillMeanSMS3 = reader["Player2TrueSkillMeanSMS3"] as double?,
                    Player2TrueSkillStandardDeviationSMS3 = reader["Player2TrueSkillStandardDeviationSMS3"] as double?,
                    Player1TrueSkillMeanOldSMS3 = reader["Player1TrueSkillMeanOldSMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS3 = reader["Player1TrueSkillStandardDeviationOldSMS3"] as double?,
                    Player2TrueSkillMeanOldSMS3 = reader["Player2TrueSkillMeanOldSMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS3 = reader["Player2TrueSkillStandardDeviationOldSMS3"] as double?,
                    WinProbabilityPlayer1SMS3 = reader["WinProbabilityPlayer1SMS3"] as double?,
                    Player1TrueSkillMeanGSMS3 = reader["Player1TrueSkillMeanGSMS3"] as double?,
                    Player1TrueSkillStandardDeviationGSMS3 = reader["Player1TrueSkillStandardDeviationGSMS3"] as double?,
                    Player2TrueSkillMeanGSMS3 = reader["Player2TrueSkillMeanGSMS3"] as double?,
                    Player2TrueSkillStandardDeviationGSMS3 = reader["Player2TrueSkillStandardDeviationGSMS3"] as double?,
                    Player1TrueSkillMeanOldGSMS3 = reader["Player1TrueSkillMeanOldGSMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS3 = reader["Player1TrueSkillStandardDeviationOldGSMS3"] as double?,
                    Player2TrueSkillMeanOldGSMS3 = reader["Player2TrueSkillMeanOldGSMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS3 = reader["Player2TrueSkillStandardDeviationOldGSMS3"] as double?,
                    WinProbabilityPlayer1GSMS3 = reader["WinProbabilityPlayer1GSMS3"] as double?,
                    Player1TrueSkillMeanMS4 = reader["Player1TrueSkillMeanMS4"] as double?,
                    Player1TrueSkillStandardDeviationMS4 = reader["Player1TrueSkillStandardDeviationMS4"] as double?,
                    Player2TrueSkillMeanMS4 = reader["Player2TrueSkillMeanMS4"] as double?,
                    Player2TrueSkillStandardDeviationMS4 = reader["Player2TrueSkillStandardDeviationMS4"] as double?,
                    Player1TrueSkillMeanOldMS4 = reader["Player1TrueSkillMeanOldMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldMS4 = reader["Player1TrueSkillStandardDeviationOldMS4"] as double?,
                    Player2TrueSkillMeanOldMS4 = reader["Player2TrueSkillMeanOldMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldMS4 = reader["Player2TrueSkillStandardDeviationOldMS4"] as double?,
                    WinProbabilityPlayer1MS4 = reader["WinProbabilityPlayer1MS4"] as double?,
                    Player1TrueSkillMeanSMS4 = reader["Player1TrueSkillMeanSMS4"] as double?,
                    Player1TrueSkillStandardDeviationSMS4 = reader["Player1TrueSkillStandardDeviationSMS4"] as double?,
                    Player2TrueSkillMeanSMS4 = reader["Player2TrueSkillMeanSMS4"] as double?,
                    Player2TrueSkillStandardDeviationSMS4 = reader["Player2TrueSkillStandardDeviationSMS4"] as double?,
                    Player1TrueSkillMeanOldSMS4 = reader["Player1TrueSkillMeanOldSMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS4 = reader["Player1TrueSkillStandardDeviationOldSMS4"] as double?,
                    Player2TrueSkillMeanOldSMS4 = reader["Player2TrueSkillMeanOldSMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS4 = reader["Player2TrueSkillStandardDeviationOldSMS4"] as double?,
                    WinProbabilityPlayer1SMS4 = reader["WinProbabilityPlayer1SMS4"] as double?,
                    Player1TrueSkillMeanGSMS4 = reader["Player1TrueSkillMeanGSMS4"] as double?,
                    Player1TrueSkillStandardDeviationGSMS4 = reader["Player1TrueSkillStandardDeviationGSMS4"] as double?,
                    Player2TrueSkillMeanGSMS4 = reader["Player2TrueSkillMeanGSMS4"] as double?,
                    Player2TrueSkillStandardDeviationGSMS4 = reader["Player2TrueSkillStandardDeviationGSMS4"] as double?,
                    Player1TrueSkillMeanOldGSMS4 = reader["Player1TrueSkillMeanOldGSMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS4 = reader["Player1TrueSkillStandardDeviationOldGSMS4"] as double?,
                    Player2TrueSkillMeanOldGSMS4 = reader["Player2TrueSkillMeanOldGSMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS4 = reader["Player2TrueSkillStandardDeviationOldGSMS4"] as double?,
                    WinProbabilityPlayer1GSMS4 = reader["WinProbabilityPlayer1GSMS4"] as double?,
                    Player1WinsTotal = reader["Player1WinsTotal"] as int?,
                    Player1LossesTotal = reader["Player1LossesTotal"] as int?,
                    Player1WinsLastYear = reader["Player1WinsLastYear"] as int?,
                    Player1LossesLastYear = reader["Player1LossesLastYear"] as int?,
                    Player1WinsLastMonth = reader["Player1WinsLastMonth"] as int?,
                    Player1LossesLastMonth = reader["Player1LossesLastMonth"] as int?,
                    Player1WinsLastWeek = reader["Player1WinsLastWeek"] as int?,
                    Player1LossesLastWeek = reader["Player1LossesLastWeek"] as int?,
                    Player2WinsTotal = reader["Player2WinsTotal"] as int?,
                    Player2LossesTotal = reader["Player2LossesTotal"] as int?,
                    Player2WinsLastYear = reader["Player2WinsLastYear"] as int?,
                    Player2LossesLastYear = reader["Player2LossesLastYear"] as int?,
                    Player2WinsLastMonth = reader["Player2WinsLastMonth"] as int?,
                    Player2LossesLastMonth = reader["Player2LossesLastMonth"] as int?,
                    Player2WinsLastWeek = reader["Player2WinsLastWeek"] as int?,
                    Player2LossesLastWeek = reader["Player2LossesLastWeek"] as int?,
                    Player1WinsTotalS1 = reader["Player1WinsTotalS1"] as int?,
                    Player1LossesTotalS1 = reader["Player1LossesTotalS1"] as int?,
                    Player1WinsLastYearS1 = reader["Player1WinsLastYearS1"] as int?,
                    Player1LossesLastYearS1 = reader["Player1LossesLastYearS1"] as int?,
                    Player1WinsLastMonthS1 = reader["Player1WinsLastMonthS1"] as int?,
                    Player1LossesLastMonthS1 = reader["Player1LossesLastMonthS1"] as int?,
                    Player1WinsLastWeekS1 = reader["Player1WinsLastWeekS1"] as int?,
                    Player1LossesLastWeekS1 = reader["Player1LossesLastWeekS1"] as int?,
                    Player2WinsTotalS1 = reader["Player2WinsTotalS1"] as int?,
                    Player2LossesTotalS1 = reader["Player2LossesTotalS1"] as int?,
                    Player2WinsLastYearS1 = reader["Player2WinsLastYearS1"] as int?,
                    Player2LossesLastYearS1 = reader["Player2LossesLastYearS1"] as int?,
                    Player2WinsLastMonthS1 = reader["Player2WinsLastMonthS1"] as int?,
                    Player2LossesLastMonthS1 = reader["Player2LossesLastMonthS1"] as int?,
                    Player2WinsLastWeekS1 = reader["Player2WinsLastWeekS1"] as int?,
                    Player2LossesLastWeekS1 = reader["Player2LossesLastWeekS1"] as int?,
                    Player1WinsTotalS2 = reader["Player1WinsTotalS2"] as int?,
                    Player1LossesTotalS2 = reader["Player1LossesTotalS2"] as int?,
                    Player1WinsLastYearS2 = reader["Player1WinsLastYearS2"] as int?,
                    Player1LossesLastYearS2 = reader["Player1LossesLastYearS2"] as int?,
                    Player1WinsLastMonthS2 = reader["Player1WinsLastMonthS2"] as int?,
                    Player1LossesLastMonthS2 = reader["Player1LossesLastMonthS2"] as int?,
                    Player1WinsLastWeekS2 = reader["Player1WinsLastWeekS2"] as int?,
                    Player1LossesLastWeekS2 = reader["Player1LossesLastWeekS2"] as int?,
                    Player2WinsTotalS2 = reader["Player2WinsTotalS2"] as int?,
                    Player2LossesTotalS2 = reader["Player2LossesTotalS2"] as int?,
                    Player2WinsLastYearS2 = reader["Player2WinsLastYearS2"] as int?,
                    Player2LossesLastYearS2 = reader["Player2LossesLastYearS2"] as int?,
                    Player2WinsLastMonthS2 = reader["Player2WinsLastMonthS2"] as int?,
                    Player2LossesLastMonthS2 = reader["Player2LossesLastMonthS2"] as int?,
                    Player2WinsLastWeekS2 = reader["Player2WinsLastWeekS2"] as int?,
                    Player2LossesLastWeekS2 = reader["Player2LossesLastWeekS2"] as int?,
                    Player1WinsTotalS3 = reader["Player1WinsTotalS3"] as int?,
                    Player1LossesTotalS3 = reader["Player1LossesTotalS3"] as int?,
                    Player1WinsLastYearS3 = reader["Player1WinsLastYearS3"] as int?,
                    Player1LossesLastYearS3 = reader["Player1LossesLastYearS3"] as int?,
                    Player1WinsLastMonthS3 = reader["Player1WinsLastMonthS3"] as int?,
                    Player1LossesLastMonthS3 = reader["Player1LossesLastMonthS3"] as int?,
                    Player1WinsLastWeekS3 = reader["Player1WinsLastWeekS3"] as int?,
                    Player1LossesLastWeekS3 = reader["Player1LossesLastWeekS3"] as int?,
                    Player2WinsTotalS3 = reader["Player2WinsTotalS3"] as int?,
                    Player2LossesTotalS3 = reader["Player2LossesTotalS3"] as int?,
                    Player2WinsLastYearS3 = reader["Player2WinsLastYearS3"] as int?,
                    Player2LossesLastYearS3 = reader["Player2LossesLastYearS3"] as int?,
                    Player2WinsLastMonthS3 = reader["Player2WinsLastMonthS3"] as int?,
                    Player2LossesLastMonthS3 = reader["Player2LossesLastMonthS3"] as int?,
                    Player2WinsLastWeekS3 = reader["Player2WinsLastWeekS3"] as int?,
                    Player2LossesLastWeekS3 = reader["Player2LossesLastWeekS3"] as int?,
                    Player1WinsTotalS4 = reader["Player1WinsTotalS4"] as int?,
                    Player1LossesTotalS4 = reader["Player1LossesTotalS4"] as int?,
                    Player1WinsLastYearS4 = reader["Player1WinsLastYearS4"] as int?,
                    Player1LossesLastYearS4 = reader["Player1LossesLastYearS4"] as int?,
                    Player1WinsLastMonthS4 = reader["Player1WinsLastMonthS4"] as int?,
                    Player1LossesLastMonthS4 = reader["Player1LossesLastMonthS4"] as int?,
                    Player1WinsLastWeekS4 = reader["Player1WinsLastWeekS4"] as int?,
                    Player1LossesLastWeekS4 = reader["Player1LossesLastWeekS4"] as int?,
                    Player2WinsTotalS4 = reader["Player2WinsTotalS4"] as int?,
                    Player2LossesTotalS4 = reader["Player2LossesTotalS4"] as int?,
                    Player2WinsLastYearS4 = reader["Player2WinsLastYearS4"] as int?,
                    Player2LossesLastYearS4 = reader["Player2LossesLastYearS4"] as int?,
                    Player2WinsLastMonthS4 = reader["Player2WinsLastMonthS4"] as int?,
                    Player2LossesLastMonthS4 = reader["Player2LossesLastMonthS4"] as int?,
                    Player2WinsLastWeekS4 = reader["Player2WinsLastWeekS4"] as int?,
                    Player2LossesLastWeekS4 = reader["Player2LossesLastWeekS4"] as int?,
                    Player1WinsSetsTotal = reader["Player1WinsSetsTotal"] as int?,
                    Player1LossesSetsTotal = reader["Player1LossesSetsTotal"] as int?,
                    Player1WinsSetsLastYear = reader["Player1WinsSetsLastYear"] as int?,
                    Player1LossesSetsLastYear = reader["Player1LossesSetsLastYear"] as int?,
                    Player1WinsSetsLastMonth = reader["Player1WinsSetsLastMonth"] as int?,
                    Player1LossesSetsLastMonth = reader["Player1LossesSetsLastMonth"] as int?,
                    Player1WinsSetsLastWeek = reader["Player1WinsSetsLastWeek"] as int?,
                    Player1LossesSetsLastWeek = reader["Player1LossesSetsLastWeek"] as int?,
                    Player2WinsSetsTotal = reader["Player2WinsSetsTotal"] as int?,
                    Player2LossesSetsTotal = reader["Player2LossesSetsTotal"] as int?,
                    Player2WinsSetsLastYear = reader["Player2WinsSetsLastYear"] as int?,
                    Player2LossesSetsLastYear = reader["Player2LossesSetsLastYear"] as int?,
                    Player2WinsSetsLastMonth = reader["Player2WinsSetsLastMonth"] as int?,
                    Player2LossesSetsLastMonth = reader["Player2LossesSetsLastMonth"] as int?,
                    Player2WinsSetsLastWeek = reader["Player2WinsSetsLastWeek"] as int?,
                    Player2LossesSetsLastWeek = reader["Player2LossesSetsLastWeek"] as int?,
                    Player1WinsSetsTotalS1 = reader["Player1WinsSetsTotalS1"] as int?,
                    Player1LossesSetsTotalS1 = reader["Player1LossesSetsTotalS1"] as int?,
                    Player1WinsSetsLastYearS1 = reader["Player1WinsSetsLastYearS1"] as int?,
                    Player1LossesSetsLastYearS1 = reader["Player1LossesSetsLastYearS1"] as int?,
                    Player1WinsSetsLastMonthS1 = reader["Player1WinsSetsLastMonthS1"] as int?,
                    Player1LossesSetsLastMonthS1 = reader["Player1LossesSetsLastMonthS1"] as int?,
                    Player1WinsSetsLastWeekS1 = reader["Player1WinsSetsLastWeekS1"] as int?,
                    Player1LossesSetsLastWeekS1 = reader["Player1LossesSetsLastWeekS1"] as int?,
                    Player2WinsSetsTotalS1 = reader["Player2WinsSetsTotalS1"] as int?,
                    Player2LossesSetsTotalS1 = reader["Player2LossesSetsTotalS1"] as int?,
                    Player2WinsSetsLastYearS1 = reader["Player2WinsSetsLastYearS1"] as int?,
                    Player2LossesSetsLastYearS1 = reader["Player2LossesSetsLastYearS1"] as int?,
                    Player2WinsSetsLastMonthS1 = reader["Player2WinsSetsLastMonthS1"] as int?,
                    Player2LossesSetsLastMonthS1 = reader["Player2LossesSetsLastMonthS1"] as int?,
                    Player2WinsSetsLastWeekS1 = reader["Player2WinsSetsLastWeekS1"] as int?,
                    Player2LossesSetsLastWeekS1 = reader["Player2LossesSetsLastWeekS1"] as int?,
                    Player1WinsSetsTotalS2 = reader["Player1WinsSetsTotalS2"] as int?,
                    Player1LossesSetsTotalS2 = reader["Player1LossesSetsTotalS2"] as int?,
                    Player1WinsSetsLastYearS2 = reader["Player1WinsSetsLastYearS2"] as int?,
                    Player1LossesSetsLastYearS2 = reader["Player1LossesSetsLastYearS2"] as int?,
                    Player1WinsSetsLastMonthS2 = reader["Player1WinsSetsLastMonthS2"] as int?,
                    Player1LossesSetsLastMonthS2 = reader["Player1LossesSetsLastMonthS2"] as int?,
                    Player1WinsSetsLastWeekS2 = reader["Player1WinsSetsLastWeekS2"] as int?,
                    Player1LossesSetsLastWeekS2 = reader["Player1LossesSetsLastWeekS2"] as int?,
                    Player2WinsSetsTotalS2 = reader["Player2WinsSetsTotalS2"] as int?,
                    Player2LossesSetsTotalS2 = reader["Player2LossesSetsTotalS2"] as int?,
                    Player2WinsSetsLastYearS2 = reader["Player2WinsSetsLastYearS2"] as int?,
                    Player2LossesSetsLastYearS2 = reader["Player2LossesSetsLastYearS2"] as int?,
                    Player2WinsSetsLastMonthS2 = reader["Player2WinsSetsLastMonthS2"] as int?,
                    Player2LossesSetsLastMonthS2 = reader["Player2LossesSetsLastMonthS2"] as int?,
                    Player2WinsSetsLastWeekS2 = reader["Player2WinsSetsLastWeekS2"] as int?,
                    Player2LossesSetsLastWeekS2 = reader["Player2LossesSetsLastWeekS2"] as int?,
                    Player1WinsSetsTotalS3 = reader["Player1WinsSetsTotalS3"] as int?,
                    Player1LossesSetsTotalS3 = reader["Player1LossesSetsTotalS3"] as int?,
                    Player1WinsSetsLastYearS3 = reader["Player1WinsSetsLastYearS3"] as int?,
                    Player1LossesSetsLastYearS3 = reader["Player1LossesSetsLastYearS3"] as int?,
                    Player1WinsSetsLastMonthS3 = reader["Player1WinsSetsLastMonthS3"] as int?,
                    Player1LossesSetsLastMonthS3 = reader["Player1LossesSetsLastMonthS3"] as int?,
                    Player1WinsSetsLastWeekS3 = reader["Player1WinsSetsLastWeekS3"] as int?,
                    Player1LossesSetsLastWeekS3 = reader["Player1LossesSetsLastWeekS3"] as int?,
                    Player2WinsSetsTotalS3 = reader["Player2WinsSetsTotalS3"] as int?,
                    Player2LossesSetsTotalS3 = reader["Player2LossesSetsTotalS3"] as int?,
                    Player2WinsSetsLastYearS3 = reader["Player2WinsSetsLastYearS3"] as int?,
                    Player2LossesSetsLastYearS3 = reader["Player2LossesSetsLastYearS3"] as int?,
                    Player2WinsSetsLastMonthS3 = reader["Player2WinsSetsLastMonthS3"] as int?,
                    Player2LossesSetsLastMonthS3 = reader["Player2LossesSetsLastMonthS3"] as int?,
                    Player2WinsSetsLastWeekS3 = reader["Player2WinsSetsLastWeekS3"] as int?,
                    Player2LossesSetsLastWeekS3 = reader["Player2LossesSetsLastWeekS3"] as int?,
                    Player1WinsSetsTotalS4 = reader["Player1WinsSetsTotalS4"] as int?,
                    Player1LossesSetsTotalS4 = reader["Player1LossesSetsTotalS4"] as int?,
                    Player1WinsSetsLastYearS4 = reader["Player1WinsSetsLastYearS4"] as int?,
                    Player1LossesSetsLastYearS4 = reader["Player1LossesSetsLastYearS4"] as int?,
                    Player1WinsSetsLastMonthS4 = reader["Player1WinsSetsLastMonthS4"] as int?,
                    Player1LossesSetsLastMonthS4 = reader["Player1LossesSetsLastMonthS4"] as int?,
                    Player1WinsSetsLastWeekS4 = reader["Player1WinsSetsLastWeekS4"] as int?,
                    Player1LossesSetsLastWeekS4 = reader["Player1LossesSetsLastWeekS4"] as int?,
                    Player2WinsSetsTotalS4 = reader["Player2WinsSetsTotalS4"] as int?,
                    Player2LossesSetsTotalS4 = reader["Player2LossesSetsTotalS4"] as int?,
                    Player2WinsSetsLastYearS4 = reader["Player2WinsSetsLastYearS4"] as int?,
                    Player2LossesSetsLastYearS4 = reader["Player2LossesSetsLastYearS4"] as int?,
                    Player2WinsSetsLastMonthS4 = reader["Player2WinsSetsLastMonthS4"] as int?,
                    Player2LossesSetsLastMonthS4 = reader["Player2LossesSetsLastMonthS4"] as int?,
                    Player2WinsSetsLastWeekS4 = reader["Player2WinsSetsLastWeekS4"] as int?,
                    Player2LossesSetsLastWeekS4 = reader["Player2LossesSetsLastWeekS4"] as int?,
                    Player1WinsGamesTotal = reader["Player1WinsGamesTotal"] as int?,
                    Player1LossesGamesTotal = reader["Player1LossesGamesTotal"] as int?,
                    Player1WinsGamesLastYear = reader["Player1WinsGamesLastYear"] as int?,
                    Player1LossesGamesLastYear = reader["Player1LossesGamesLastYear"] as int?,
                    Player1WinsGamesLastMonth = reader["Player1WinsGamesLastMonth"] as int?,
                    Player1LossesGamesLastMonth = reader["Player1LossesGamesLastMonth"] as int?,
                    Player1WinsGamesLastWeek = reader["Player1WinsGamesLastWeek"] as int?,
                    Player1LossesGamesLastWeek = reader["Player1LossesGamesLastWeek"] as int?,
                    Player2WinsGamesTotal = reader["Player2WinsGamesTotal"] as int?,
                    Player2LossesGamesTotal = reader["Player2LossesGamesTotal"] as int?,
                    Player2WinsGamesLastYear = reader["Player2WinsGamesLastYear"] as int?,
                    Player2LossesGamesLastYear = reader["Player2LossesGamesLastYear"] as int?,
                    Player2WinsGamesLastMonth = reader["Player2WinsGamesLastMonth"] as int?,
                    Player2LossesGamesLastMonth = reader["Player2LossesGamesLastMonth"] as int?,
                    Player2WinsGamesLastWeek = reader["Player2WinsGamesLastWeek"] as int?,
                    Player2LossesGamesLastWeek = reader["Player2LossesGamesLastWeek"] as int?,
                    Player1WinsGamesTotalS1 = reader["Player1WinsGamesTotalS1"] as int?,
                    Player1LossesGamesTotalS1 = reader["Player1LossesGamesTotalS1"] as int?,
                    Player1WinsGamesLastYearS1 = reader["Player1WinsGamesLastYearS1"] as int?,
                    Player1LossesGamesLastYearS1 = reader["Player1LossesGamesLastYearS1"] as int?,
                    Player1WinsGamesLastMonthS1 = reader["Player1WinsGamesLastMonthS1"] as int?,
                    Player1LossesGamesLastMonthS1 = reader["Player1LossesGamesLastMonthS1"] as int?,
                    Player1WinsGamesLastWeekS1 = reader["Player1WinsGamesLastWeekS1"] as int?,
                    Player1LossesGamesLastWeekS1 = reader["Player1LossesGamesLastWeekS1"] as int?,
                    Player2WinsGamesTotalS1 = reader["Player2WinsGamesTotalS1"] as int?,
                    Player2LossesGamesTotalS1 = reader["Player2LossesGamesTotalS1"] as int?,
                    Player2WinsGamesLastYearS1 = reader["Player2WinsGamesLastYearS1"] as int?,
                    Player2LossesGamesLastYearS1 = reader["Player2LossesGamesLastYearS1"] as int?,
                    Player2WinsGamesLastMonthS1 = reader["Player2WinsGamesLastMonthS1"] as int?,
                    Player2LossesGamesLastMonthS1 = reader["Player2LossesGamesLastMonthS1"] as int?,
                    Player2WinsGamesLastWeekS1 = reader["Player2WinsGamesLastWeekS1"] as int?,
                    Player2LossesGamesLastWeekS1 = reader["Player2LossesGamesLastWeekS1"] as int?,
                    Player1WinsGamesTotalS2 = reader["Player1WinsGamesTotalS2"] as int?,
                    Player1LossesGamesTotalS2 = reader["Player1LossesGamesTotalS2"] as int?,
                    Player1WinsGamesLastYearS2 = reader["Player1WinsGamesLastYearS2"] as int?,
                    Player1LossesGamesLastYearS2 = reader["Player1LossesGamesLastYearS2"] as int?,
                    Player1WinsGamesLastMonthS2 = reader["Player1WinsGamesLastMonthS2"] as int?,
                    Player1LossesGamesLastMonthS2 = reader["Player1LossesGamesLastMonthS2"] as int?,
                    Player1WinsGamesLastWeekS2 = reader["Player1WinsGamesLastWeekS2"] as int?,
                    Player1LossesGamesLastWeekS2 = reader["Player1LossesGamesLastWeekS2"] as int?,
                    Player2WinsGamesTotalS2 = reader["Player2WinsGamesTotalS2"] as int?,
                    Player2LossesGamesTotalS2 = reader["Player2LossesGamesTotalS2"] as int?,
                    Player2WinsGamesLastYearS2 = reader["Player2WinsGamesLastYearS2"] as int?,
                    Player2LossesGamesLastYearS2 = reader["Player2LossesGamesLastYearS2"] as int?,
                    Player2WinsGamesLastMonthS2 = reader["Player2WinsGamesLastMonthS2"] as int?,
                    Player2LossesGamesLastMonthS2 = reader["Player2LossesGamesLastMonthS2"] as int?,
                    Player2WinsGamesLastWeekS2 = reader["Player2WinsGamesLastWeekS2"] as int?,
                    Player2LossesGamesLastWeekS2 = reader["Player2LossesGamesLastWeekS2"] as int?,
                    Player1WinsGamesTotalS3 = reader["Player1WinsGamesTotalS3"] as int?,
                    Player1LossesGamesTotalS3 = reader["Player1LossesGamesTotalS3"] as int?,
                    Player1WinsGamesLastYearS3 = reader["Player1WinsGamesLastYearS3"] as int?,
                    Player1LossesGamesLastYearS3 = reader["Player1LossesGamesLastYearS3"] as int?,
                    Player1WinsGamesLastMonthS3 = reader["Player1WinsGamesLastMonthS3"] as int?,
                    Player1LossesGamesLastMonthS3 = reader["Player1LossesGamesLastMonthS3"] as int?,
                    Player1WinsGamesLastWeekS3 = reader["Player1WinsGamesLastWeekS3"] as int?,
                    Player1LossesGamesLastWeekS3 = reader["Player1LossesGamesLastWeekS3"] as int?,
                    Player2WinsGamesTotalS3 = reader["Player2WinsGamesTotalS3"] as int?,
                    Player2LossesGamesTotalS3 = reader["Player2LossesGamesTotalS3"] as int?,
                    Player2WinsGamesLastYearS3 = reader["Player2WinsGamesLastYearS3"] as int?,
                    Player2LossesGamesLastYearS3 = reader["Player2LossesGamesLastYearS3"] as int?,
                    Player2WinsGamesLastMonthS3 = reader["Player2WinsGamesLastMonthS3"] as int?,
                    Player2LossesGamesLastMonthS3 = reader["Player2LossesGamesLastMonthS3"] as int?,
                    Player2WinsGamesLastWeekS3 = reader["Player2WinsGamesLastWeekS3"] as int?,
                    Player2LossesGamesLastWeekS3 = reader["Player2LossesGamesLastWeekS3"] as int?,
                    Player1WinsGamesTotalS4 = reader["Player1WinsGamesTotalS4"] as int?,
                    Player1LossesGamesTotalS4 = reader["Player1LossesGamesTotalS4"] as int?,
                    Player1WinsGamesLastYearS4 = reader["Player1WinsGamesLastYearS4"] as int?,
                    Player1LossesGamesLastYearS4 = reader["Player1LossesGamesLastYearS4"] as int?,
                    Player1WinsGamesLastMonthS4 = reader["Player1WinsGamesLastMonthS4"] as int?,
                    Player1LossesGamesLastMonthS4 = reader["Player1LossesGamesLastMonthS4"] as int?,
                    Player1WinsGamesLastWeekS4 = reader["Player1WinsGamesLastWeekS4"] as int?,
                    Player1LossesGamesLastWeekS4 = reader["Player1LossesGamesLastWeekS4"] as int?,
                    Player2WinsGamesTotalS4 = reader["Player2WinsGamesTotalS4"] as int?,
                    Player2LossesGamesTotalS4 = reader["Player2LossesGamesTotalS4"] as int?,
                    Player2WinsGamesLastYearS4 = reader["Player2WinsGamesLastYearS4"] as int?,
                    Player2LossesGamesLastYearS4 = reader["Player2LossesGamesLastYearS4"] as int?,
                    Player2WinsGamesLastMonthS4 = reader["Player2WinsGamesLastMonthS4"] as int?,
                    Player2LossesGamesLastMonthS4 = reader["Player2LossesGamesLastMonthS4"] as int?,
                    Player2WinsGamesLastWeekS4 = reader["Player2WinsGamesLastWeekS4"] as int?,
                    Player2LossesGamesLastWeekS4 = reader["Player2LossesGamesLastWeekS4"] as int?,
                    Player1DaysSinceLastWin = reader["Player1DaysSinceLastWin"] as int?,
                    Player2DaysSinceLastWin = reader["Player2DaysSinceLastWin"] as int?,
                    Player1DaysSinceLastWinS1 = reader["Player1DaysSinceLastWinS1"] as int?,
                    Player2DaysSinceLastWinS1 = reader["Player2DaysSinceLastWinS1"] as int?,
                    Player1DaysSinceLastWinS2 = reader["Player1DaysSinceLastWinS2"] as int?,
                    Player2DaysSinceLastWinS2 = reader["Player2DaysSinceLastWinS2"] as int?,
                    Player1DaysSinceLastWinS3 = reader["Player1DaysSinceLastWinS3"] as int?,
                    Player2DaysSinceLastWinS3 = reader["Player2DaysSinceLastWinS3"] as int?,
                    Player1DaysSinceLastWinS4 = reader["Player1DaysSinceLastWinS4"] as int?,
                    Player2DaysSinceLastWinS4 = reader["Player2DaysSinceLastWinS4"] as int?,
                    Player1DaysSinceLastLoss = reader["Player1DaysSinceLastLoss"] as int?,
                    Player2DaysSinceLastLoss = reader["Player2DaysSinceLastLoss"] as int?,
                    Player1DaysSinceLastLossS1 = reader["Player1DaysSinceLastLossS1"] as int?,
                    Player2DaysSinceLastLossS1 = reader["Player2DaysSinceLastLossS1"] as int?,
                    Player1DaysSinceLastLossS2 = reader["Player1DaysSinceLastLossS2"] as int?,
                    Player2DaysSinceLastLossS2 = reader["Player2DaysSinceLastLossS2"] as int?,
                    Player1DaysSinceLastLossS3 = reader["Player1DaysSinceLastLossS3"] as int?,
                    Player2DaysSinceLastLossS3 = reader["Player2DaysSinceLastLossS3"] as int?,
                    Player1DaysSinceLastLossS4 = reader["Player1DaysSinceLastLossS4"] as int?,
                    Player2DaysSinceLastLossS4 = reader["Player2DaysSinceLastLossS4"] as int?,
                    Player1TotalWinsAsFavourite = reader["Player1TotalWinsAsFavourite"] as int?,
                    Player2TotalWinsAsFavourite = reader["Player2TotalWinsAsFavourite"] as int?,
                    Player1TotalWinsAsUnderdog = reader["Player1TotalWinsAsUnderdog"] as int?,
                    Player2TotalWinsAsUnderdog = reader["Player2TotalWinsAsUnderdog"] as int?,
                    Player1TotalLossesAsFavourite = reader["Player1TotalLossesAsFavourite"] as int?,
                    Player2TotalLossesAsFavourite = reader["Player2TotalLossesAsFavourite"] as int?,
                    Player1TotalLossesAsUnderdog = reader["Player1TotalLossesAsUnderdog"] as int?,
                    Player2TotalLossesAsUnderdog = reader["Player2TotalLossesAsUnderdog"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavourite = reader["Player1AverageWinningProbabilityAtWonAsFavourite"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavourite = reader["Player2AverageWinningProbabilityAtWonAsFavourite"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdog = reader["Player1AverageWinningProbabilityAtWonAsUnderdog"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdog = reader["Player2AverageWinningProbabilityAtWonAsUnderdog"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavourite = reader["Player1AverageWinningProbabilityAtLossAsFavourite"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavourite = reader["Player2AverageWinningProbabilityAtLossAsFavourite"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdog = reader["Player1AverageWinningProbabilityAtLossAsUnderdog"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdog = reader["Player2AverageWinningProbabilityAtLossAsUnderdog"] as double?,
                    Player1TotalWinsAsFavouriteLastYear = reader["Player1TotalWinsAsFavouriteLastYear"] as int?,
                    Player2TotalWinsAsFavouriteLastYear = reader["Player2TotalWinsAsFavouriteLastYear"] as int?,
                    Player1TotalWinsAsUnderdogLastYear = reader["Player1TotalWinsAsUnderdogLastYear"] as int?,
                    Player2TotalWinsAsUnderdogLastYear = reader["Player2TotalWinsAsUnderdogLastYear"] as int?,
                    Player1TotalLossesAsFavouriteLastYear = reader["Player1TotalLossesAsFavouriteLastYear"] as int?,
                    Player2TotalLossesAsFavouriteLastYear = reader["Player2TotalLossesAsFavouriteLastYear"] as int?,
                    Player1TotalLossesAsUnderdogLastYear = reader["Player1TotalLossesAsUnderdogLastYear"] as int?,
                    Player2TotalLossesAsUnderdogLastYear = reader["Player2TotalLossesAsUnderdogLastYear"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastYear"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastYear"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastYear"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastYear"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastYear"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastYear"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastYear"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastYear"] as double?,
                    Player1TotalWinsAsFavouriteLastMonth = reader["Player1TotalWinsAsFavouriteLastMonth"] as int?,
                    Player2TotalWinsAsFavouriteLastMonth = reader["Player2TotalWinsAsFavouriteLastMonth"] as int?,
                    Player1TotalWinsAsUnderdogLastMonth = reader["Player1TotalWinsAsUnderdogLastMonth"] as int?,
                    Player2TotalWinsAsUnderdogLastMonth = reader["Player2TotalWinsAsUnderdogLastMonth"] as int?,
                    Player1TotalLossesAsFavouriteLastMonth = reader["Player1TotalLossesAsFavouriteLastMonth"] as int?,
                    Player2TotalLossesAsFavouriteLastMonth = reader["Player2TotalLossesAsFavouriteLastMonth"] as int?,
                    Player1TotalLossesAsUnderdogLastMonth = reader["Player1TotalLossesAsUnderdogLastMonth"] as int?,
                    Player2TotalLossesAsUnderdogLastMonth = reader["Player2TotalLossesAsUnderdogLastMonth"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth"] as double?,
                    Player1TotalWinsAsFavouriteLastWeek = reader["Player1TotalWinsAsFavouriteLastWeek"] as int?,
                    Player2TotalWinsAsFavouriteLastWeek = reader["Player2TotalWinsAsFavouriteLastWeek"] as int?,
                    Player1TotalWinsAsUnderdogLastWeek = reader["Player1TotalWinsAsUnderdogLastWeek"] as int?,
                    Player2TotalWinsAsUnderdogLastWeek = reader["Player2TotalWinsAsUnderdogLastWeek"] as int?,
                    Player1TotalLossesAsFavouriteLastWeek = reader["Player1TotalLossesAsFavouriteLastWeek"] as int?,
                    Player2TotalLossesAsFavouriteLastWeek = reader["Player2TotalLossesAsFavouriteLastWeek"] as int?,
                    Player1TotalLossesAsUnderdogLastWeek = reader["Player1TotalLossesAsUnderdogLastWeek"] as int?,
                    Player2TotalLossesAsUnderdogLastWeek = reader["Player2TotalLossesAsUnderdogLastWeek"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek"] as double?,
                    Player1H2H = reader["Player1H2H"] as int?,
                    Player2H2H = reader["Player2H2H"] as int?,
                    Player1H2HOld = reader["Player1H2HOld"] as int?,
                    Player2H2HOld = reader["Player2H2HOld"] as int?,
                    Player1H2HTrueSkillMeanM = reader["Player1H2HTrueSkillMeanM"] as double?,
                    Player1H2HTrueSkillStandardDeviationM = reader["Player1H2HTrueSkillStandardDeviationM"] as double?,
                    Player2H2HTrueSkillMeanM = reader["Player2H2HTrueSkillMeanM"] as double?,
                    Player2H2HTrueSkillStandardDeviationM = reader["Player2H2HTrueSkillStandardDeviationM"] as double?,
                    Player1H2HTrueSkillMeanOldM = reader["Player1H2HTrueSkillMeanOldM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldM = reader["Player1H2HTrueSkillStandardDeviationOldM"] as double?,
                    Player2H2HTrueSkillMeanOldM = reader["Player2H2HTrueSkillMeanOldM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldM = reader["Player2H2HTrueSkillStandardDeviationOldM"] as double?,
                    WinProbabilityPlayer1H2HM = reader["WinProbabilityPlayer1H2HM"] as double?,
                    Player1H2HTrueSkillMeanSM = reader["Player1H2HTrueSkillMeanSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationSM = reader["Player1H2HTrueSkillStandardDeviationSM"] as double?,
                    Player2H2HTrueSkillMeanSM = reader["Player2H2HTrueSkillMeanSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationSM = reader["Player2H2HTrueSkillStandardDeviationSM"] as double?,
                    Player1H2HTrueSkillMeanOldSM = reader["Player1H2HTrueSkillMeanOldSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSM = reader["Player1H2HTrueSkillStandardDeviationOldSM"] as double?,
                    Player2H2HTrueSkillMeanOldSM = reader["Player2H2HTrueSkillMeanOldSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSM = reader["Player2H2HTrueSkillStandardDeviationOldSM"] as double?,
                    WinProbabilityPlayer1H2HSM = reader["WinProbabilityPlayer1H2HSM"] as double?,
                    Player1H2HTrueSkillMeanGSM = reader["Player1H2HTrueSkillMeanGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSM = reader["Player1H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player2H2HTrueSkillMeanGSM = reader["Player2H2HTrueSkillMeanGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSM = reader["Player2H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player1H2HTrueSkillMeanOldGSM = reader["Player1H2HTrueSkillMeanOldGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSM = reader["Player1H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    Player2H2HTrueSkillMeanOldGSM = reader["Player2H2HTrueSkillMeanOldGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSM = reader["Player2H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    WinProbabilityPlayer1H2HGSM = reader["WinProbabilityPlayer1H2HGSM"] as double?,
                    Player1H2HS1 = reader["Player1H2HS1"] as int?,
                    Player2H2HS1 = reader["Player2H2HS1"] as int?,
                    Player1H2HOldS1 = reader["Player1H2HOldS1"] as int?,
                    Player2H2HOldS1 = reader["Player2H2HOldS1"] as int?,
                    Player1H2HTrueSkillMeanMS1 = reader["Player1H2HTrueSkillMeanMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS1 = reader["Player1H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player2H2HTrueSkillMeanMS1 = reader["Player2H2HTrueSkillMeanMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS1 = reader["Player2H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player1H2HTrueSkillMeanOldMS1 = reader["Player1H2HTrueSkillMeanOldMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS1 = reader["Player1H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    Player2H2HTrueSkillMeanOldMS1 = reader["Player2H2HTrueSkillMeanOldMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS1 = reader["Player2H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    WinProbabilityPlayer1H2HMS1 = reader["WinProbabilityPlayer1H2HMS1"] as double?,
                    Player1H2HTrueSkillMeanSMS1 = reader["Player1H2HTrueSkillMeanSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS1 = reader["Player1H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player2H2HTrueSkillMeanSMS1 = reader["Player2H2HTrueSkillMeanSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS1 = reader["Player2H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldSMS1 = reader["Player1H2HTrueSkillMeanOldSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldSMS1 = reader["Player2H2HTrueSkillMeanOldSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    WinProbabilityPlayer1H2HSMS1 = reader["WinProbabilityPlayer1H2HSMS1"] as double?,
                    Player1H2HTrueSkillMeanGSMS1 = reader["Player1H2HTrueSkillMeanGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS1 = reader["Player1H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player2H2HTrueSkillMeanGSMS1 = reader["Player2H2HTrueSkillMeanGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS1 = reader["Player2H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS1 = reader["Player1H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS1 = reader["Player2H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    WinProbabilityPlayer1H2HGSMS1 = reader["WinProbabilityPlayer1H2HGSMS1"] as double?,
                    Player1H2HS2 = reader["Player1H2HS2"] as int?,
                    Player2H2HS2 = reader["Player2H2HS2"] as int?,
                    Player1H2HOldS2 = reader["Player1H2HOldS2"] as int?,
                    Player2H2HOldS2 = reader["Player2H2HOldS2"] as int?,
                    Player1H2HTrueSkillMeanMS2 = reader["Player1H2HTrueSkillMeanMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS2 = reader["Player1H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player2H2HTrueSkillMeanMS2 = reader["Player2H2HTrueSkillMeanMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS2 = reader["Player2H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player1H2HTrueSkillMeanOldMS2 = reader["Player1H2HTrueSkillMeanOldMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS2 = reader["Player1H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    Player2H2HTrueSkillMeanOldMS2 = reader["Player2H2HTrueSkillMeanOldMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS2 = reader["Player2H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    WinProbabilityPlayer1H2HMS2 = reader["WinProbabilityPlayer1H2HMS2"] as double?,
                    Player1H2HTrueSkillMeanSMS2 = reader["Player1H2HTrueSkillMeanSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS2 = reader["Player1H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player2H2HTrueSkillMeanSMS2 = reader["Player2H2HTrueSkillMeanSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS2 = reader["Player2H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldSMS2 = reader["Player1H2HTrueSkillMeanOldSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldSMS2 = reader["Player2H2HTrueSkillMeanOldSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    WinProbabilityPlayer1H2HSMS2 = reader["WinProbabilityPlayer1H2HSMS2"] as double?,
                    Player1H2HTrueSkillMeanGSMS2 = reader["Player1H2HTrueSkillMeanGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS2 = reader["Player1H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player2H2HTrueSkillMeanGSMS2 = reader["Player2H2HTrueSkillMeanGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS2 = reader["Player2H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS2 = reader["Player1H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS2 = reader["Player2H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    WinProbabilityPlayer1H2HGSMS2 = reader["WinProbabilityPlayer1H2HGSMS2"] as double?,
                    Player1H2HS3 = reader["Player1H2HS3"] as int?,
                    Player2H2HS3 = reader["Player2H2HS3"] as int?,
                    Player1H2HOldS3 = reader["Player1H2HOldS3"] as int?,
                    Player2H2HOldS3 = reader["Player2H2HOldS3"] as int?,
                    Player1H2HTrueSkillMeanMS3 = reader["Player1H2HTrueSkillMeanMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS3 = reader["Player1H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player2H2HTrueSkillMeanMS3 = reader["Player2H2HTrueSkillMeanMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS3 = reader["Player2H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player1H2HTrueSkillMeanOldMS3 = reader["Player1H2HTrueSkillMeanOldMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS3 = reader["Player1H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    Player2H2HTrueSkillMeanOldMS3 = reader["Player2H2HTrueSkillMeanOldMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS3 = reader["Player2H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    WinProbabilityPlayer1H2HMS3 = reader["WinProbabilityPlayer1H2HMS3"] as double?,
                    Player1H2HTrueSkillMeanSMS3 = reader["Player1H2HTrueSkillMeanSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS3 = reader["Player1H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player2H2HTrueSkillMeanSMS3 = reader["Player2H2HTrueSkillMeanSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS3 = reader["Player2H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldSMS3 = reader["Player1H2HTrueSkillMeanOldSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldSMS3 = reader["Player2H2HTrueSkillMeanOldSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    WinProbabilityPlayer1H2HSMS3 = reader["WinProbabilityPlayer1H2HSMS3"] as double?,
                    Player1H2HTrueSkillMeanGSMS3 = reader["Player1H2HTrueSkillMeanGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS3 = reader["Player1H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player2H2HTrueSkillMeanGSMS3 = reader["Player2H2HTrueSkillMeanGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS3 = reader["Player2H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS3 = reader["Player1H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS3 = reader["Player2H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    WinProbabilityPlayer1H2HGSMS3 = reader["WinProbabilityPlayer1H2HGSMS3"] as double?,
                    Player1H2HS4 = reader["Player1H2HS4"] as int?,
                    Player2H2HS4 = reader["Player2H2HS4"] as int?,
                    Player1H2HOldS4 = reader["Player1H2HOldS4"] as int?,
                    Player2H2HOldS4 = reader["Player2H2HOldS4"] as int?,
                    Player1H2HTrueSkillMeanMS4 = reader["Player1H2HTrueSkillMeanMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS4 = reader["Player1H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player2H2HTrueSkillMeanMS4 = reader["Player2H2HTrueSkillMeanMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS4 = reader["Player2H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player1H2HTrueSkillMeanOldMS4 = reader["Player1H2HTrueSkillMeanOldMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS4 = reader["Player1H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    Player2H2HTrueSkillMeanOldMS4 = reader["Player2H2HTrueSkillMeanOldMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS4 = reader["Player2H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    WinProbabilityPlayer1H2HMS4 = reader["WinProbabilityPlayer1H2HMS4"] as double?,
                    Player1H2HTrueSkillMeanSMS4 = reader["Player1H2HTrueSkillMeanSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS4 = reader["Player1H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player2H2HTrueSkillMeanSMS4 = reader["Player2H2HTrueSkillMeanSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS4 = reader["Player2H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldSMS4 = reader["Player1H2HTrueSkillMeanOldSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldSMS4 = reader["Player2H2HTrueSkillMeanOldSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    WinProbabilityPlayer1H2HSMS4 = reader["WinProbabilityPlayer1H2HSMS4"] as double?,
                    Player1H2HTrueSkillMeanGSMS4 = reader["Player1H2HTrueSkillMeanGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS4 = reader["Player1H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player2H2HTrueSkillMeanGSMS4 = reader["Player2H2HTrueSkillMeanGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS4 = reader["Player2H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS4 = reader["Player1H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS4 = reader["Player2H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    WinProbabilityPlayer1H2HGSMS4 = reader["WinProbabilityPlayer1H2HGSMS4"] as double?,
                    Player1Streak = reader["Player1Streak"] as int?,
                    Player2Streak = reader["Player2Streak"] as int?,
                    Player1StreakS1 = reader["Player1StreakS1"] as int?,
                    Player2StreakS1 = reader["Player2StreakS1"] as int?,
                    Player1StreakS2 = reader["Player1StreakS2"] as int?,
                    Player2StreakS2 = reader["Player2StreakS2"] as int?,
                    Player1StreakS3 = reader["Player1StreakS3"] as int?,
                    Player2StreakS3 = reader["Player2StreakS3"] as int?,
                    Player1StreakS4 = reader["Player1StreakS4"] as int?,
                    Player2StreakS4 = reader["Player2StreakS4"] as int?,
                    P1SetsWon = reader["P1SetsWon"] as int?,
                    P2SetsWon = reader["P2SetsWon"] as int?,
                    P1GamesWon = reader["P1GamesWon"] as int?,
                    P2GamesWon = reader["P2GamesWon"] as int?,
                    P1SetsLoss = reader["P1SetsLoss"] as int?,
                    P2SetsLoss = reader["P2SetsLoss"] as int?,
                    P1GamesLoss = reader["P1GamesLoss"] as int?,
                    P2GamesLoss = reader["P2GamesLoss"] as int?,
                    WinProbabilityNN = reader["WinProbabilityNN"] as double?,
                    IsFinished = true,//reader["IsFinished"]                                                 as bool?,
                };
                Matches.Add(Match);
            }

            return Matches;
        }

        public async Task<IEnumerable<Match>> GetAllMatchesForPlayerAsync(int playerTPId)
        {
            var Matches = new List<Match>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllMatchesForPlayer";
            command.CommandType = CommandType.StoredProcedure;

            var p = command.CreateParameter();
            p.ParameterName = "@playerTPId";
            p.Value = playerTPId;

            command.Parameters.Add(p);

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                try
                {
                    var MatchTPId = reader["MatchTPId"];
                    var TournamentEventTPId = reader["TournamentEventTPId"];
                    var DateTime = reader["DateTime"];
                    var Player1TPId = reader["Player1TPId"];
                    var Player2TPId = reader["Player2TPId"];
                    var Player1Seed = reader["Player1Seed"];
                    var Player2Seed = reader["Player2Seed"];
                    var Result = reader["Result"];
                    var ResultDetails = reader["ResultDetails"];
                    var Player1Odds = reader["Player1Odds"];
                    var Player2Odds = reader["Player2Odds"];
                    var Player1Percentage = reader["Player1Percentage"];
                    var Player2Percentage = reader["Player2Percentage"];
                    var SurfaceId = reader["SurfaceId"];
                    var RoundId = reader["RoundId"];
                    var Player1TrueSkillMeanM = reader["Player1TrueSkillMeanM"];
                    var Player1TrueSkillStandardDeviationM = reader["Player1TrueSkillStandardDeviationM"];
                    var Player2TrueSkillMeanM = reader["Player2TrueSkillMeanM"];
                    var Player2TrueSkillStandardDeviationM = reader["Player2TrueSkillStandardDeviationM"];
                    var Player1TrueSkillMeanOldM = reader["Player1TrueSkillMeanOldM"];
                    var Player1TrueSkillStandardDeviationOldM = reader["Player1TrueSkillStandardDeviationOldM"];
                    var Player2TrueSkillMeanOldM = reader["Player2TrueSkillMeanOldM"];
                    var Player2TrueSkillStandardDeviationOldM = reader["Player2TrueSkillStandardDeviationOldM"];
                    var WinProbabilityPlayer1M = reader["WinProbabilityPlayer1M"];
                    var Player1TrueSkillMeanSM = reader["Player1TrueSkillMeanSM"];
                    var Player1TrueSkillStandardDeviationSM = reader["Player1TrueSkillStandardDeviationSM"];
                    var Player2TrueSkillMeanSM = reader["Player2TrueSkillMeanSM"];
                    var Player2TrueSkillStandardDeviationSM = reader["Player2TrueSkillStandardDeviationSM"];
                    var Player1TrueSkillMeanOldSM = reader["Player1TrueSkillMeanOldSM"];
                    var Player1TrueSkillStandardDeviationOldSM = reader["Player1TrueSkillStandardDeviationOldSM"];
                    var Player2TrueSkillMeanOldSM = reader["Player2TrueSkillMeanOldSM"];
                    var Player2TrueSkillStandardDeviationOldSM = reader["Player2TrueSkillStandardDeviationOldSM"];
                    var WinProbabilityPlayer1SM = reader["WinProbabilityPlayer1SM"];
                    var Player1TrueSkillMeanGSM = reader["Player1TrueSkillMeanGSM"];
                    var Player1TrueSkillStandardDeviationGSM = reader["Player1TrueSkillStandardDeviationGSM"];
                    var Player2TrueSkillMeanGSM = reader["Player2TrueSkillMeanGSM"];
                    var Player2TrueSkillStandardDeviationGSM = reader["Player2TrueSkillStandardDeviationGSM"];
                    var Player1TrueSkillMeanOldGSM = reader["Player1TrueSkillMeanOldGSM"];
                    var Player1TrueSkillStandardDeviationOldGSM = reader["Player1TrueSkillStandardDeviationOldGSM"];
                    var Player2TrueSkillMeanOldGSM = reader["Player2TrueSkillMeanOldGSM"];
                    var Player2TrueSkillStandardDeviationOldGSM = reader["Player2TrueSkillStandardDeviationOldGSM"];
                    var WinProbabilityPlayer1GSM = reader["WinProbabilityPlayer1GSM"];
                    var Player1TrueSkillMeanMS1 = reader["Player1TrueSkillMeanMS1"];
                    var Player1TrueSkillStandardDeviationMS1 = reader["Player1TrueSkillStandardDeviationMS1"];
                    var Player2TrueSkillMeanMS1 = reader["Player2TrueSkillMeanMS1"];
                    var Player2TrueSkillStandardDeviationMS1 = reader["Player2TrueSkillStandardDeviationMS1"];
                    var Player1TrueSkillMeanOldMS1 = reader["Player1TrueSkillMeanOldMS1"];
                    var Player1TrueSkillStandardDeviationOldMS1 = reader["Player1TrueSkillStandardDeviationOldMS1"];
                    var Player2TrueSkillMeanOldMS1 = reader["Player2TrueSkillMeanOldMS1"];
                    var Player2TrueSkillStandardDeviationOldMS1 = reader["Player2TrueSkillStandardDeviationOldMS1"];
                    var WinProbabilityPlayer1MS1 = reader["WinProbabilityPlayer1MS1"];
                    var Player1TrueSkillMeanSMS1 = reader["Player1TrueSkillMeanSMS1"];
                    var Player1TrueSkillStandardDeviationSMS1 = reader["Player1TrueSkillStandardDeviationSMS1"];
                    var Player2TrueSkillMeanSMS1 = reader["Player2TrueSkillMeanSMS1"];
                    var Player2TrueSkillStandardDeviationSMS1 = reader["Player2TrueSkillStandardDeviationSMS1"];
                    var Player1TrueSkillMeanOldSMS1 = reader["Player1TrueSkillMeanOldSMS1"];
                    var Player1TrueSkillStandardDeviationOldSMS1 = reader["Player1TrueSkillStandardDeviationOldSMS1"];
                    var Player2TrueSkillMeanOldSMS1 = reader["Player2TrueSkillMeanOldSMS1"];
                    var Player2TrueSkillStandardDeviationOldSMS1 = reader["Player2TrueSkillStandardDeviationOldSMS1"];
                    var WinProbabilityPlayer1SMS1 = reader["WinProbabilityPlayer1SMS1"];
                    var Player1TrueSkillMeanGSMS1 = reader["Player1TrueSkillMeanGSMS1"];
                    var Player1TrueSkillStandardDeviationGSMS1 = reader["Player1TrueSkillStandardDeviationGSMS1"];
                    var Player2TrueSkillMeanGSMS1 = reader["Player2TrueSkillMeanGSMS1"];
                    var Player2TrueSkillStandardDeviationGSMS1 = reader["Player2TrueSkillStandardDeviationGSMS1"];
                    var Player1TrueSkillMeanOldGSMS1 = reader["Player1TrueSkillMeanOldGSMS1"];
                    var Player1TrueSkillStandardDeviationOldGSMS1 = reader["Player1TrueSkillStandardDeviationOldGSMS1"];
                    var Player2TrueSkillMeanOldGSMS1 = reader["Player2TrueSkillMeanOldGSMS1"];
                    var Player2TrueSkillStandardDeviationOldGSMS1 = reader["Player2TrueSkillStandardDeviationOldGSMS1"];
                    var WinProbabilityPlayer1GSMS1 = reader["WinProbabilityPlayer1GSMS1"];
                    var Player1TrueSkillMeanMS2 = reader["Player1TrueSkillMeanMS2"];
                    var Player1TrueSkillStandardDeviationMS2 = reader["Player1TrueSkillStandardDeviationMS2"];
                    var Player2TrueSkillMeanMS2 = reader["Player2TrueSkillMeanMS2"];
                    var Player2TrueSkillStandardDeviationMS2 = reader["Player2TrueSkillStandardDeviationMS2"];
                    var Player1TrueSkillMeanOldMS2 = reader["Player1TrueSkillMeanOldMS2"];
                    var Player1TrueSkillStandardDeviationOldMS2 = reader["Player1TrueSkillStandardDeviationOldMS2"];
                    var Player2TrueSkillMeanOldMS2 = reader["Player2TrueSkillMeanOldMS2"];
                    var Player2TrueSkillStandardDeviationOldMS2 = reader["Player2TrueSkillStandardDeviationOldMS2"];
                    var WinProbabilityPlayer1MS2 = reader["WinProbabilityPlayer1MS2"];
                    var Player1TrueSkillMeanSMS2 = reader["Player1TrueSkillMeanSMS2"];
                    var Player1TrueSkillStandardDeviationSMS2 = reader["Player1TrueSkillStandardDeviationSMS2"];
                    var Player2TrueSkillMeanSMS2 = reader["Player2TrueSkillMeanSMS2"];
                    var Player2TrueSkillStandardDeviationSMS2 = reader["Player2TrueSkillStandardDeviationSMS2"];
                    var Player1TrueSkillMeanOldSMS2 = reader["Player1TrueSkillMeanOldSMS2"];
                    var Player1TrueSkillStandardDeviationOldSMS2 = reader["Player1TrueSkillStandardDeviationOldSMS2"];
                    var Player2TrueSkillMeanOldSMS2 = reader["Player2TrueSkillMeanOldSMS2"];
                    var Player2TrueSkillStandardDeviationOldSMS2 = reader["Player2TrueSkillStandardDeviationOldSMS2"];
                    var WinProbabilityPlayer1SMS2 = reader["WinProbabilityPlayer1SMS2"];
                    var Player1TrueSkillMeanGSMS2 = reader["Player1TrueSkillMeanGSMS2"];
                    var Player1TrueSkillStandardDeviationGSMS2 = reader["Player1TrueSkillStandardDeviationGSMS2"];
                    var Player2TrueSkillMeanGSMS2 = reader["Player2TrueSkillMeanGSMS2"];
                    var Player2TrueSkillStandardDeviationGSMS2 = reader["Player2TrueSkillStandardDeviationGSMS2"];
                    var Player1TrueSkillMeanOldGSMS2 = reader["Player1TrueSkillMeanOldGSMS2"];
                    var Player1TrueSkillStandardDeviationOldGSMS2 = reader["Player1TrueSkillStandardDeviationOldGSMS2"];
                    var Player2TrueSkillMeanOldGSMS2 = reader["Player2TrueSkillMeanOldGSMS2"];
                    var Player2TrueSkillStandardDeviationOldGSMS2 = reader["Player2TrueSkillStandardDeviationOldGSMS2"];
                    var WinProbabilityPlayer1GSMS2 = reader["WinProbabilityPlayer1GSMS2"];
                    var Player1TrueSkillMeanMS3 = reader["Player1TrueSkillMeanMS3"];
                    var Player1TrueSkillStandardDeviationMS3 = reader["Player1TrueSkillStandardDeviationMS3"];
                    var Player2TrueSkillMeanMS3 = reader["Player2TrueSkillMeanMS3"];
                    var Player2TrueSkillStandardDeviationMS3 = reader["Player2TrueSkillStandardDeviationMS3"];
                    var Player1TrueSkillMeanOldMS3 = reader["Player1TrueSkillMeanOldMS3"];
                    var Player1TrueSkillStandardDeviationOldMS3 = reader["Player1TrueSkillStandardDeviationOldMS3"];
                    var Player2TrueSkillMeanOldMS3 = reader["Player2TrueSkillMeanOldMS3"];
                    var Player2TrueSkillStandardDeviationOldMS3 = reader["Player2TrueSkillStandardDeviationOldMS3"];
                    var WinProbabilityPlayer1MS3 = reader["WinProbabilityPlayer1MS3"];
                    var Player1TrueSkillMeanSMS3 = reader["Player1TrueSkillMeanSMS3"];
                    var Player1TrueSkillStandardDeviationSMS3 = reader["Player1TrueSkillStandardDeviationSMS3"];
                    var Player2TrueSkillMeanSMS3 = reader["Player2TrueSkillMeanSMS3"];
                    var Player2TrueSkillStandardDeviationSMS3 = reader["Player2TrueSkillStandardDeviationSMS3"];
                    var Player1TrueSkillMeanOldSMS3 = reader["Player1TrueSkillMeanOldSMS3"];
                    var Player1TrueSkillStandardDeviationOldSMS3 = reader["Player1TrueSkillStandardDeviationOldSMS3"];
                    var Player2TrueSkillMeanOldSMS3 = reader["Player2TrueSkillMeanOldSMS3"];
                    var Player2TrueSkillStandardDeviationOldSMS3 = reader["Player2TrueSkillStandardDeviationOldSMS3"];
                    var WinProbabilityPlayer1SMS3 = reader["WinProbabilityPlayer1SMS3"];
                    var Player1TrueSkillMeanGSMS3 = reader["Player1TrueSkillMeanGSMS3"];
                    var Player1TrueSkillStandardDeviationGSMS3 = reader["Player1TrueSkillStandardDeviationGSMS3"];
                    var Player2TrueSkillMeanGSMS3 = reader["Player2TrueSkillMeanGSMS3"];
                    var Player2TrueSkillStandardDeviationGSMS3 = reader["Player2TrueSkillStandardDeviationGSMS3"];
                    var Player1TrueSkillMeanOldGSMS3 = reader["Player1TrueSkillMeanOldGSMS3"];
                    var Player1TrueSkillStandardDeviationOldGSMS3 = reader["Player1TrueSkillStandardDeviationOldGSMS3"];
                    var Player2TrueSkillMeanOldGSMS3 = reader["Player2TrueSkillMeanOldGSMS3"];
                    var Player2TrueSkillStandardDeviationOldGSMS3 = reader["Player2TrueSkillStandardDeviationOldGSMS3"];
                    var WinProbabilityPlayer1GSMS3 = reader["WinProbabilityPlayer1GSMS3"];
                    var Player1TrueSkillMeanMS4 = reader["Player1TrueSkillMeanMS4"];
                    var Player1TrueSkillStandardDeviationMS4 = reader["Player1TrueSkillStandardDeviationMS4"];
                    var Player2TrueSkillMeanMS4 = reader["Player2TrueSkillMeanMS4"];
                    var Player2TrueSkillStandardDeviationMS4 = reader["Player2TrueSkillStandardDeviationMS4"];
                    var Player1TrueSkillMeanOldMS4 = reader["Player1TrueSkillMeanOldMS4"];
                    var Player1TrueSkillStandardDeviationOldMS4 = reader["Player1TrueSkillStandardDeviationOldMS4"];
                    var Player2TrueSkillMeanOldMS4 = reader["Player2TrueSkillMeanOldMS4"];
                    var Player2TrueSkillStandardDeviationOldMS4 = reader["Player2TrueSkillStandardDeviationOldMS4"];
                    var WinProbabilityPlayer1MS4 = reader["WinProbabilityPlayer1MS4"];
                    var Player1TrueSkillMeanSMS4 = reader["Player1TrueSkillMeanSMS4"];
                    var Player1TrueSkillStandardDeviationSMS4 = reader["Player1TrueSkillStandardDeviationSMS4"];
                    var Player2TrueSkillMeanSMS4 = reader["Player2TrueSkillMeanSMS4"];
                    var Player2TrueSkillStandardDeviationSMS4 = reader["Player2TrueSkillStandardDeviationSMS4"];
                    var Player1TrueSkillMeanOldSMS4 = reader["Player1TrueSkillMeanOldSMS4"];
                    var Player1TrueSkillStandardDeviationOldSMS4 = reader["Player1TrueSkillStandardDeviationOldSMS4"];
                    var Player2TrueSkillMeanOldSMS4 = reader["Player2TrueSkillMeanOldSMS4"];
                    var Player2TrueSkillStandardDeviationOldSMS4 = reader["Player2TrueSkillStandardDeviationOldSMS4"];
                    var WinProbabilityPlayer1SMS4 = reader["WinProbabilityPlayer1SMS4"];
                    var Player1TrueSkillMeanGSMS4 = reader["Player1TrueSkillMeanGSMS4"];
                    var Player1TrueSkillStandardDeviationGSMS4 = reader["Player1TrueSkillStandardDeviationGSMS4"];
                    var Player2TrueSkillMeanGSMS4 = reader["Player2TrueSkillMeanGSMS4"];
                    var Player2TrueSkillStandardDeviationGSMS4 = reader["Player2TrueSkillStandardDeviationGSMS4"];
                    var Player1TrueSkillMeanOldGSMS4 = reader["Player1TrueSkillMeanOldGSMS4"];
                    var Player1TrueSkillStandardDeviationOldGSMS4 = reader["Player1TrueSkillStandardDeviationOldGSMS4"];
                    var Player2TrueSkillMeanOldGSMS4 = reader["Player2TrueSkillMeanOldGSMS4"];
                    var Player2TrueSkillStandardDeviationOldGSMS4 = reader["Player2TrueSkillStandardDeviationOldGSMS4"];
                    var WinProbabilityPlayer1GSMS4 = reader["WinProbabilityPlayer1GSMS4"];
                    var Player1WinsTotal = reader["Player1WinsTotal"];
                    var Player1LossesTotal = reader["Player1LossesTotal"];
                    var Player1WinsLastYear = reader["Player1WinsLastYear"];
                    var Player1LossesLastYear = reader["Player1LossesLastYear"];
                    var Player1WinsLastMonth = reader["Player1WinsLastMonth"];
                    var Player1LossesLastMonth = reader["Player1LossesLastMonth"];
                    var Player1WinsLastWeek = reader["Player1WinsLastWeek"];
                    var Player1LossesLastWeek = reader["Player1LossesLastWeek"];
                    var Player2WinsTotal = reader["Player2WinsTotal"];
                    var Player2LossesTotal = reader["Player2LossesTotal"];
                    var Player2WinsLastYear = reader["Player2WinsLastYear"];
                    var Player2LossesLastYear = reader["Player2LossesLastYear"];
                    var Player2WinsLastMonth = reader["Player2WinsLastMonth"];
                    var Player2LossesLastMonth = reader["Player2LossesLastMonth"];
                    var Player2WinsLastWeek = reader["Player2WinsLastWeek"];
                    var Player2LossesLastWeek = reader["Player2LossesLastWeek"];
                    var Player1WinsTotalS1 = reader["Player1WinsTotalS1"];
                    var Player1LossesTotalS1 = reader["Player1LossesTotalS1"];
                    var Player1WinsLastYearS1 = reader["Player1WinsLastYearS1"];
                    var Player1LossesLastYearS1 = reader["Player1LossesLastYearS1"];
                    var Player1WinsLastMonthS1 = reader["Player1WinsLastMonthS1"];
                    var Player1LossesLastMonthS1 = reader["Player1LossesLastMonthS1"];
                    var Player1WinsLastWeekS1 = reader["Player1WinsLastWeekS1"];
                    var Player1LossesLastWeekS1 = reader["Player1LossesLastWeekS1"];
                    var Player2WinsTotalS1 = reader["Player2WinsTotalS1"];
                    var Player2LossesTotalS1 = reader["Player2LossesTotalS1"];
                    var Player2WinsLastYearS1 = reader["Player2WinsLastYearS1"];
                    var Player2LossesLastYearS1 = reader["Player2LossesLastYearS1"];
                    var Player2WinsLastMonthS1 = reader["Player2WinsLastMonthS1"];
                    var Player2LossesLastMonthS1 = reader["Player2LossesLastMonthS1"];
                    var Player2WinsLastWeekS1 = reader["Player2WinsLastWeekS1"];
                    var Player2LossesLastWeekS1 = reader["Player2LossesLastWeekS1"];
                    var Player1WinsTotalS2 = reader["Player1WinsTotalS2"];
                    var Player1LossesTotalS2 = reader["Player1LossesTotalS2"];
                    var Player1WinsLastYearS2 = reader["Player1WinsLastYearS2"];
                    var Player1LossesLastYearS2 = reader["Player1LossesLastYearS2"];
                    var Player1WinsLastMonthS2 = reader["Player1WinsLastMonthS2"];
                    var Player1LossesLastMonthS2 = reader["Player1LossesLastMonthS2"];
                    var Player1WinsLastWeekS2 = reader["Player1WinsLastWeekS2"];
                    var Player1LossesLastWeekS2 = reader["Player1LossesLastWeekS2"];
                    var Player2WinsTotalS2 = reader["Player2WinsTotalS2"];
                    var Player2LossesTotalS2 = reader["Player2LossesTotalS2"];
                    var Player2WinsLastYearS2 = reader["Player2WinsLastYearS2"];
                    var Player2LossesLastYearS2 = reader["Player2LossesLastYearS2"];
                    var Player2WinsLastMonthS2 = reader["Player2WinsLastMonthS2"];
                    var Player2LossesLastMonthS2 = reader["Player2LossesLastMonthS2"];
                    var Player2WinsLastWeekS2 = reader["Player2WinsLastWeekS2"];
                    var Player2LossesLastWeekS2 = reader["Player2LossesLastWeekS2"];
                    var Player1WinsTotalS3 = reader["Player1WinsTotalS3"];
                    var Player1LossesTotalS3 = reader["Player1LossesTotalS3"];
                    var Player1WinsLastYearS3 = reader["Player1WinsLastYearS3"];
                    var Player1LossesLastYearS3 = reader["Player1LossesLastYearS3"];
                    var Player1WinsLastMonthS3 = reader["Player1WinsLastMonthS3"];
                    var Player1LossesLastMonthS3 = reader["Player1LossesLastMonthS3"];
                    var Player1WinsLastWeekS3 = reader["Player1WinsLastWeekS3"];
                    var Player1LossesLastWeekS3 = reader["Player1LossesLastWeekS3"];
                    var Player2WinsTotalS3 = reader["Player2WinsTotalS3"];
                    var Player2LossesTotalS3 = reader["Player2LossesTotalS3"];
                    var Player2WinsLastYearS3 = reader["Player2WinsLastYearS3"];
                    var Player2LossesLastYearS3 = reader["Player2LossesLastYearS3"];
                    var Player2WinsLastMonthS3 = reader["Player2WinsLastMonthS3"];
                    var Player2LossesLastMonthS3 = reader["Player2LossesLastMonthS3"];
                    var Player2WinsLastWeekS3 = reader["Player2WinsLastWeekS3"];
                    var Player2LossesLastWeekS3 = reader["Player2LossesLastWeekS3"];
                    var Player1WinsTotalS4 = reader["Player1WinsTotalS4"];
                    var Player1LossesTotalS4 = reader["Player1LossesTotalS4"];
                    var Player1WinsLastYearS4 = reader["Player1WinsLastYearS4"];
                    var Player1LossesLastYearS4 = reader["Player1LossesLastYearS4"];
                    var Player1WinsLastMonthS4 = reader["Player1WinsLastMonthS4"];
                    var Player1LossesLastMonthS4 = reader["Player1LossesLastMonthS4"];
                    var Player1WinsLastWeekS4 = reader["Player1WinsLastWeekS4"];
                    var Player1LossesLastWeekS4 = reader["Player1LossesLastWeekS4"];
                    var Player2WinsTotalS4 = reader["Player2WinsTotalS4"];
                    var Player2LossesTotalS4 = reader["Player2LossesTotalS4"];
                    var Player2WinsLastYearS4 = reader["Player2WinsLastYearS4"];
                    var Player2LossesLastYearS4 = reader["Player2LossesLastYearS4"];
                    var Player2WinsLastMonthS4 = reader["Player2WinsLastMonthS4"];
                    var Player2LossesLastMonthS4 = reader["Player2LossesLastMonthS4"];
                    var Player2WinsLastWeekS4 = reader["Player2WinsLastWeekS4"];
                    var Player2LossesLastWeekS4 = reader["Player2LossesLastWeekS4"];
                    var Player1WinsSetsTotal = reader["Player1WinsSetsTotal"];
                    var Player1LossesSetsTotal = reader["Player1LossesSetsTotal"];
                    var Player1WinsSetsLastYear = reader["Player1WinsSetsLastYear"];
                    var Player1LossesSetsLastYear = reader["Player1LossesSetsLastYear"];
                    var Player1WinsSetsLastMonth = reader["Player1WinsSetsLastMonth"];
                    var Player1LossesSetsLastMonth = reader["Player1LossesSetsLastMonth"];
                    var Player1WinsSetsLastWeek = reader["Player1WinsSetsLastWeek"];
                    var Player1LossesSetsLastWeek = reader["Player1LossesSetsLastWeek"];
                    var Player2WinsSetsTotal = reader["Player2WinsSetsTotal"];
                    var Player2LossesSetsTotal = reader["Player2LossesSetsTotal"];
                    var Player2WinsSetsLastYear = reader["Player2WinsSetsLastYear"];
                    var Player2LossesSetsLastYear = reader["Player2LossesSetsLastYear"];
                    var Player2WinsSetsLastMonth = reader["Player2WinsSetsLastMonth"];
                    var Player2LossesSetsLastMonth = reader["Player2LossesSetsLastMonth"];
                    var Player2WinsSetsLastWeek = reader["Player2WinsSetsLastWeek"];
                    var Player2LossesSetsLastWeek = reader["Player2LossesSetsLastWeek"];
                    var Player1WinsSetsTotalS1 = reader["Player1WinsSetsTotalS1"];
                    var Player1LossesSetsTotalS1 = reader["Player1LossesSetsTotalS1"];
                    var Player1WinsSetsLastYearS1 = reader["Player1WinsSetsLastYearS1"];
                    var Player1LossesSetsLastYearS1 = reader["Player1LossesSetsLastYearS1"];
                    var Player1WinsSetsLastMonthS1 = reader["Player1WinsSetsLastMonthS1"];
                    var Player1LossesSetsLastMonthS1 = reader["Player1LossesSetsLastMonthS1"];
                    var Player1WinsSetsLastWeekS1 = reader["Player1WinsSetsLastWeekS1"];
                    var Player1LossesSetsLastWeekS1 = reader["Player1LossesSetsLastWeekS1"];
                    var Player2WinsSetsTotalS1 = reader["Player2WinsSetsTotalS1"];
                    var Player2LossesSetsTotalS1 = reader["Player2LossesSetsTotalS1"];
                    var Player2WinsSetsLastYearS1 = reader["Player2WinsSetsLastYearS1"];
                    var Player2LossesSetsLastYearS1 = reader["Player2LossesSetsLastYearS1"];
                    var Player2WinsSetsLastMonthS1 = reader["Player2WinsSetsLastMonthS1"];
                    var Player2LossesSetsLastMonthS1 = reader["Player2LossesSetsLastMonthS1"];
                    var Player2WinsSetsLastWeekS1 = reader["Player2WinsSetsLastWeekS1"];
                    var Player2LossesSetsLastWeekS1 = reader["Player2LossesSetsLastWeekS1"];
                    var Player1WinsSetsTotalS2 = reader["Player1WinsSetsTotalS2"];
                    var Player1LossesSetsTotalS2 = reader["Player1LossesSetsTotalS2"];
                    var Player1WinsSetsLastYearS2 = reader["Player1WinsSetsLastYearS2"];
                    var Player1LossesSetsLastYearS2 = reader["Player1LossesSetsLastYearS2"];
                    var Player1WinsSetsLastMonthS2 = reader["Player1WinsSetsLastMonthS2"];
                    var Player1LossesSetsLastMonthS2 = reader["Player1LossesSetsLastMonthS2"];
                    var Player1WinsSetsLastWeekS2 = reader["Player1WinsSetsLastWeekS2"];
                    var Player1LossesSetsLastWeekS2 = reader["Player1LossesSetsLastWeekS2"];
                    var Player2WinsSetsTotalS2 = reader["Player2WinsSetsTotalS2"];
                    var Player2LossesSetsTotalS2 = reader["Player2LossesSetsTotalS2"];
                    var Player2WinsSetsLastYearS2 = reader["Player2WinsSetsLastYearS2"];
                    var Player2LossesSetsLastYearS2 = reader["Player2LossesSetsLastYearS2"];
                    var Player2WinsSetsLastMonthS2 = reader["Player2WinsSetsLastMonthS2"];
                    var Player2LossesSetsLastMonthS2 = reader["Player2LossesSetsLastMonthS2"];
                    var Player2WinsSetsLastWeekS2 = reader["Player2WinsSetsLastWeekS2"];
                    var Player2LossesSetsLastWeekS2 = reader["Player2LossesSetsLastWeekS2"];
                    var Player1WinsSetsTotalS3 = reader["Player1WinsSetsTotalS3"];
                    var Player1LossesSetsTotalS3 = reader["Player1LossesSetsTotalS3"];
                    var Player1WinsSetsLastYearS3 = reader["Player1WinsSetsLastYearS3"];
                    var Player1LossesSetsLastYearS3 = reader["Player1LossesSetsLastYearS3"];
                    var Player1WinsSetsLastMonthS3 = reader["Player1WinsSetsLastMonthS3"];
                    var Player1LossesSetsLastMonthS3 = reader["Player1LossesSetsLastMonthS3"];
                    var Player1WinsSetsLastWeekS3 = reader["Player1WinsSetsLastWeekS3"];
                    var Player1LossesSetsLastWeekS3 = reader["Player1LossesSetsLastWeekS3"];
                    var Player2WinsSetsTotalS3 = reader["Player2WinsSetsTotalS3"];
                    var Player2LossesSetsTotalS3 = reader["Player2LossesSetsTotalS3"];
                    var Player2WinsSetsLastYearS3 = reader["Player2WinsSetsLastYearS3"];
                    var Player2LossesSetsLastYearS3 = reader["Player2LossesSetsLastYearS3"];
                    var Player2WinsSetsLastMonthS3 = reader["Player2WinsSetsLastMonthS3"];
                    var Player2LossesSetsLastMonthS3 = reader["Player2LossesSetsLastMonthS3"];
                    var Player2WinsSetsLastWeekS3 = reader["Player2WinsSetsLastWeekS3"];
                    var Player2LossesSetsLastWeekS3 = reader["Player2LossesSetsLastWeekS3"];
                    var Player1WinsSetsTotalS4 = reader["Player1WinsSetsTotalS4"];
                    var Player1LossesSetsTotalS4 = reader["Player1LossesSetsTotalS4"];
                    var Player1WinsSetsLastYearS4 = reader["Player1WinsSetsLastYearS4"];
                    var Player1LossesSetsLastYearS4 = reader["Player1LossesSetsLastYearS4"];
                    var Player1WinsSetsLastMonthS4 = reader["Player1WinsSetsLastMonthS4"];
                    var Player1LossesSetsLastMonthS4 = reader["Player1LossesSetsLastMonthS4"];
                    var Player1WinsSetsLastWeekS4 = reader["Player1WinsSetsLastWeekS4"];
                    var Player1LossesSetsLastWeekS4 = reader["Player1LossesSetsLastWeekS4"];
                    var Player2WinsSetsTotalS4 = reader["Player2WinsSetsTotalS4"];
                    var Player2LossesSetsTotalS4 = reader["Player2LossesSetsTotalS4"];
                    var Player2WinsSetsLastYearS4 = reader["Player2WinsSetsLastYearS4"];
                    var Player2LossesSetsLastYearS4 = reader["Player2LossesSetsLastYearS4"];
                    var Player2WinsSetsLastMonthS4 = reader["Player2WinsSetsLastMonthS4"];
                    var Player2LossesSetsLastMonthS4 = reader["Player2LossesSetsLastMonthS4"];
                    var Player2WinsSetsLastWeekS4 = reader["Player2WinsSetsLastWeekS4"];
                    var Player2LossesSetsLastWeekS4 = reader["Player2LossesSetsLastWeekS4"];
                    var Player1WinsGamesTotal = reader["Player1WinsGamesTotal"];
                    var Player1LossesGamesTotal = reader["Player1LossesGamesTotal"];
                    var Player1WinsGamesLastYear = reader["Player1WinsGamesLastYear"];
                    var Player1LossesGamesLastYear = reader["Player1LossesGamesLastYear"];
                    var Player1WinsGamesLastMonth = reader["Player1WinsGamesLastMonth"];
                    var Player1LossesGamesLastMonth = reader["Player1LossesGamesLastMonth"];
                    var Player1WinsGamesLastWeek = reader["Player1WinsGamesLastWeek"];
                    var Player1LossesGamesLastWeek = reader["Player1LossesGamesLastWeek"];
                    var Player2WinsGamesTotal = reader["Player2WinsGamesTotal"];
                    var Player2LossesGamesTotal = reader["Player2LossesGamesTotal"];
                    var Player2WinsGamesLastYear = reader["Player2WinsGamesLastYear"];
                    var Player2LossesGamesLastYear = reader["Player2LossesGamesLastYear"];
                    var Player2WinsGamesLastMonth = reader["Player2WinsGamesLastMonth"];
                    var Player2LossesGamesLastMonth = reader["Player2LossesGamesLastMonth"];
                    var Player2WinsGamesLastWeek = reader["Player2WinsGamesLastWeek"];
                    var Player2LossesGamesLastWeek = reader["Player2LossesGamesLastWeek"];
                    var Player1WinsGamesTotalS1 = reader["Player1WinsGamesTotalS1"];
                    var Player1LossesGamesTotalS1 = reader["Player1LossesGamesTotalS1"];
                    var Player1WinsGamesLastYearS1 = reader["Player1WinsGamesLastYearS1"];
                    var Player1LossesGamesLastYearS1 = reader["Player1LossesGamesLastYearS1"];
                    var Player1WinsGamesLastMonthS1 = reader["Player1WinsGamesLastMonthS1"];
                    var Player1LossesGamesLastMonthS1 = reader["Player1LossesGamesLastMonthS1"];
                    var Player1WinsGamesLastWeekS1 = reader["Player1WinsGamesLastWeekS1"];
                    var Player1LossesGamesLastWeekS1 = reader["Player1LossesGamesLastWeekS1"];
                    var Player2WinsGamesTotalS1 = reader["Player2WinsGamesTotalS1"];
                    var Player2LossesGamesTotalS1 = reader["Player2LossesGamesTotalS1"];
                    var Player2WinsGamesLastYearS1 = reader["Player2WinsGamesLastYearS1"];
                    var Player2LossesGamesLastYearS1 = reader["Player2LossesGamesLastYearS1"];
                    var Player2WinsGamesLastMonthS1 = reader["Player2WinsGamesLastMonthS1"];
                    var Player2LossesGamesLastMonthS1 = reader["Player2LossesGamesLastMonthS1"];
                    var Player2WinsGamesLastWeekS1 = reader["Player2WinsGamesLastWeekS1"];
                    var Player2LossesGamesLastWeekS1 = reader["Player2LossesGamesLastWeekS1"];
                    var Player1WinsGamesTotalS2 = reader["Player1WinsGamesTotalS2"];
                    var Player1LossesGamesTotalS2 = reader["Player1LossesGamesTotalS2"];
                    var Player1WinsGamesLastYearS2 = reader["Player1WinsGamesLastYearS2"];
                    var Player1LossesGamesLastYearS2 = reader["Player1LossesGamesLastYearS2"];
                    var Player1WinsGamesLastMonthS2 = reader["Player1WinsGamesLastMonthS2"];
                    var Player1LossesGamesLastMonthS2 = reader["Player1LossesGamesLastMonthS2"];
                    var Player1WinsGamesLastWeekS2 = reader["Player1WinsGamesLastWeekS2"];
                    var Player1LossesGamesLastWeekS2 = reader["Player1LossesGamesLastWeekS2"];
                    var Player2WinsGamesTotalS2 = reader["Player2WinsGamesTotalS2"];
                    var Player2LossesGamesTotalS2 = reader["Player2LossesGamesTotalS2"];
                    var Player2WinsGamesLastYearS2 = reader["Player2WinsGamesLastYearS2"];
                    var Player2LossesGamesLastYearS2 = reader["Player2LossesGamesLastYearS2"];
                    var Player2WinsGamesLastMonthS2 = reader["Player2WinsGamesLastMonthS2"];
                    var Player2LossesGamesLastMonthS2 = reader["Player2LossesGamesLastMonthS2"];
                    var Player2WinsGamesLastWeekS2 = reader["Player2WinsGamesLastWeekS2"];
                    var Player2LossesGamesLastWeekS2 = reader["Player2LossesGamesLastWeekS2"];
                    var Player1WinsGamesTotalS3 = reader["Player1WinsGamesTotalS3"];
                    var Player1LossesGamesTotalS3 = reader["Player1LossesGamesTotalS3"];
                    var Player1WinsGamesLastYearS3 = reader["Player1WinsGamesLastYearS3"];
                    var Player1LossesGamesLastYearS3 = reader["Player1LossesGamesLastYearS3"];
                    var Player1WinsGamesLastMonthS3 = reader["Player1WinsGamesLastMonthS3"];
                    var Player1LossesGamesLastMonthS3 = reader["Player1LossesGamesLastMonthS3"];
                    var Player1WinsGamesLastWeekS3 = reader["Player1WinsGamesLastWeekS3"];
                    var Player1LossesGamesLastWeekS3 = reader["Player1LossesGamesLastWeekS3"];
                    var Player2WinsGamesTotalS3 = reader["Player2WinsGamesTotalS3"];
                    var Player2LossesGamesTotalS3 = reader["Player2LossesGamesTotalS3"];
                    var Player2WinsGamesLastYearS3 = reader["Player2WinsGamesLastYearS3"];
                    var Player2LossesGamesLastYearS3 = reader["Player2LossesGamesLastYearS3"];
                    var Player2WinsGamesLastMonthS3 = reader["Player2WinsGamesLastMonthS3"];
                    var Player2LossesGamesLastMonthS3 = reader["Player2LossesGamesLastMonthS3"];
                    var Player2WinsGamesLastWeekS3 = reader["Player2WinsGamesLastWeekS3"];
                    var Player2LossesGamesLastWeekS3 = reader["Player2LossesGamesLastWeekS3"];
                    var Player1WinsGamesTotalS4 = reader["Player1WinsGamesTotalS4"];
                    var Player1LossesGamesTotalS4 = reader["Player1LossesGamesTotalS4"];
                    var Player1WinsGamesLastYearS4 = reader["Player1WinsGamesLastYearS4"];
                    var Player1LossesGamesLastYearS4 = reader["Player1LossesGamesLastYearS4"];
                    var Player1WinsGamesLastMonthS4 = reader["Player1WinsGamesLastMonthS4"];
                    var Player1LossesGamesLastMonthS4 = reader["Player1LossesGamesLastMonthS4"];
                    var Player1WinsGamesLastWeekS4 = reader["Player1WinsGamesLastWeekS4"];
                    var Player1LossesGamesLastWeekS4 = reader["Player1LossesGamesLastWeekS4"];
                    var Player2WinsGamesTotalS4 = reader["Player2WinsGamesTotalS4"];
                    var Player2LossesGamesTotalS4 = reader["Player2LossesGamesTotalS4"];
                    var Player2WinsGamesLastYearS4 = reader["Player2WinsGamesLastYearS4"];
                    var Player2LossesGamesLastYearS4 = reader["Player2LossesGamesLastYearS4"];
                    var Player2WinsGamesLastMonthS4 = reader["Player2WinsGamesLastMonthS4"];
                    var Player2LossesGamesLastMonthS4 = reader["Player2LossesGamesLastMonthS4"];
                    var Player2WinsGamesLastWeekS4 = reader["Player2WinsGamesLastWeekS4"];
                    var Player2LossesGamesLastWeekS4 = reader["Player2LossesGamesLastWeekS4"];
                    var Player1DaysSinceLastWin = reader["Player1DaysSinceLastWin"];
                    var Player2DaysSinceLastWin = reader["Player2DaysSinceLastWin"];
                    var Player1DaysSinceLastWinS1 = reader["Player1DaysSinceLastWinS1"];
                    var Player2DaysSinceLastWinS1 = reader["Player2DaysSinceLastWinS1"];
                    var Player1DaysSinceLastWinS2 = reader["Player1DaysSinceLastWinS2"];
                    var Player2DaysSinceLastWinS2 = reader["Player2DaysSinceLastWinS2"];
                    var Player1DaysSinceLastWinS3 = reader["Player1DaysSinceLastWinS3"];
                    var Player2DaysSinceLastWinS3 = reader["Player2DaysSinceLastWinS3"];
                    var Player1DaysSinceLastWinS4 = reader["Player1DaysSinceLastWinS4"];
                    var Player2DaysSinceLastWinS4 = reader["Player2DaysSinceLastWinS4"];
                    var Player1DaysSinceLastLoss = reader["Player1DaysSinceLastLoss"];
                    var Player2DaysSinceLastLoss = reader["Player2DaysSinceLastLoss"];
                    var Player1DaysSinceLastLossS1 = reader["Player1DaysSinceLastLossS1"];
                    var Player2DaysSinceLastLossS1 = reader["Player2DaysSinceLastLossS1"];
                    var Player1DaysSinceLastLossS2 = reader["Player1DaysSinceLastLossS2"];
                    var Player2DaysSinceLastLossS2 = reader["Player2DaysSinceLastLossS2"];
                    var Player1DaysSinceLastLossS3 = reader["Player1DaysSinceLastLossS3"];
                    var Player2DaysSinceLastLossS3 = reader["Player2DaysSinceLastLossS3"];
                    var Player1DaysSinceLastLossS4 = reader["Player1DaysSinceLastLossS4"];
                    var Player2DaysSinceLastLossS4 = reader["Player2DaysSinceLastLossS4"];
                    var Player1TotalWinsAsFavourite = reader["Player1TotalWinsAsFavourite"];
                    var Player2TotalWinsAsFavourite = reader["Player2TotalWinsAsFavourite"];
                    var Player1TotalWinsAsUnderdog = reader["Player1TotalWinsAsUnderdog"];
                    var Player2TotalWinsAsUnderdog = reader["Player2TotalWinsAsUnderdog"];
                    var Player1TotalLossesAsFavourite = reader["Player1TotalLossesAsFavourite"];
                    var Player2TotalLossesAsFavourite = reader["Player2TotalLossesAsFavourite"];
                    var Player1TotalLossesAsUnderdog = reader["Player1TotalLossesAsUnderdog"];
                    var Player2TotalLossesAsUnderdog = reader["Player2TotalLossesAsUnderdog"];
                    var Player1AverageWinningProbabilityAtWonAsFavourite = reader["Player1AverageWinningProbabilityAtWonAsFavourite"];
                    var Player2AverageWinningProbabilityAtWonAsFavourite = reader["Player2AverageWinningProbabilityAtWonAsFavourite"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdog = reader["Player1AverageWinningProbabilityAtWonAsUnderdog"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdog = reader["Player2AverageWinningProbabilityAtWonAsUnderdog"];
                    var Player1AverageWinningProbabilityAtLossAsFavourite = reader["Player1AverageWinningProbabilityAtLossAsFavourite"];
                    var Player2AverageWinningProbabilityAtLossAsFavourite = reader["Player2AverageWinningProbabilityAtLossAsFavourite"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdog = reader["Player1AverageWinningProbabilityAtLossAsUnderdog"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdog = reader["Player2AverageWinningProbabilityAtLossAsUnderdog"];
                    var Player1TotalWinsAsFavouriteLastYear = reader["Player1TotalWinsAsFavouriteLastYear"];
                    var Player2TotalWinsAsFavouriteLastYear = reader["Player2TotalWinsAsFavouriteLastYear"];
                    var Player1TotalWinsAsUnderdogLastYear = reader["Player1TotalWinsAsUnderdogLastYear"];
                    var Player2TotalWinsAsUnderdogLastYear = reader["Player2TotalWinsAsUnderdogLastYear"];
                    var Player1TotalLossesAsFavouriteLastYear = reader["Player1TotalLossesAsFavouriteLastYear"];
                    var Player2TotalLossesAsFavouriteLastYear = reader["Player2TotalLossesAsFavouriteLastYear"];
                    var Player1TotalLossesAsUnderdogLastYear = reader["Player1TotalLossesAsUnderdogLastYear"];
                    var Player2TotalLossesAsUnderdogLastYear = reader["Player2TotalLossesAsUnderdogLastYear"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastYear"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastYear"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastYear"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastYear"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastYear"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastYear"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastYear"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastYear"];
                    var Player1TotalWinsAsFavouriteLastMonth = reader["Player1TotalWinsAsFavouriteLastMonth"];
                    var Player2TotalWinsAsFavouriteLastMonth = reader["Player2TotalWinsAsFavouriteLastMonth"];
                    var Player1TotalWinsAsUnderdogLastMonth = reader["Player1TotalWinsAsUnderdogLastMonth"];
                    var Player2TotalWinsAsUnderdogLastMonth = reader["Player2TotalWinsAsUnderdogLastMonth"];
                    var Player1TotalLossesAsFavouriteLastMonth = reader["Player1TotalLossesAsFavouriteLastMonth"];
                    var Player2TotalLossesAsFavouriteLastMonth = reader["Player2TotalLossesAsFavouriteLastMonth"];
                    var Player1TotalLossesAsUnderdogLastMonth = reader["Player1TotalLossesAsUnderdogLastMonth"];
                    var Player2TotalLossesAsUnderdogLastMonth = reader["Player2TotalLossesAsUnderdogLastMonth"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth"];
                    var Player1TotalWinsAsFavouriteLastWeek = reader["Player1TotalWinsAsFavouriteLastWeek"];
                    var Player2TotalWinsAsFavouriteLastWeek = reader["Player2TotalWinsAsFavouriteLastWeek"];
                    var Player1TotalWinsAsUnderdogLastWeek = reader["Player1TotalWinsAsUnderdogLastWeek"];
                    var Player2TotalWinsAsUnderdogLastWeek = reader["Player2TotalWinsAsUnderdogLastWeek"];
                    var Player1TotalLossesAsFavouriteLastWeek = reader["Player1TotalLossesAsFavouriteLastWeek"];
                    var Player2TotalLossesAsFavouriteLastWeek = reader["Player2TotalLossesAsFavouriteLastWeek"];
                    var Player1TotalLossesAsUnderdogLastWeek = reader["Player1TotalLossesAsUnderdogLastWeek"];
                    var Player2TotalLossesAsUnderdogLastWeek = reader["Player2TotalLossesAsUnderdogLastWeek"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek"];
                    var Player1H2H = reader["Player1H2H"];
                    var Player2H2H = reader["Player2H2H"];
                    var Player1H2HOld = reader["Player1H2HOld"];
                    var Player2H2HOld = reader["Player2H2HOld"];
                    var Player1H2HTrueSkillMeanM = reader["Player1H2HTrueSkillMeanM"];
                    var Player1H2HTrueSkillStandardDeviationM = reader["Player1H2HTrueSkillStandardDeviationM"];
                    var Player2H2HTrueSkillMeanM = reader["Player2H2HTrueSkillMeanM"];
                    var Player2H2HTrueSkillStandardDeviationM = reader["Player2H2HTrueSkillStandardDeviationM"];
                    var Player1H2HTrueSkillMeanOldM = reader["Player1H2HTrueSkillMeanOldM"];
                    var Player1H2HTrueSkillStandardDeviationOldM = reader["Player1H2HTrueSkillStandardDeviationOldM"];
                    var Player2H2HTrueSkillMeanOldM = reader["Player2H2HTrueSkillMeanOldM"];
                    var Player2H2HTrueSkillStandardDeviationOldM = reader["Player2H2HTrueSkillStandardDeviationOldM"];
                    var WinProbabilityPlayer1H2HM = reader["WinProbabilityPlayer1H2HM"];
                    var Player1H2HTrueSkillMeanSM = reader["Player1H2HTrueSkillMeanSM"];
                    var Player1H2HTrueSkillStandardDeviationSM = reader["Player1H2HTrueSkillStandardDeviationSM"];
                    var Player2H2HTrueSkillMeanSM = reader["Player2H2HTrueSkillMeanSM"];
                    var Player2H2HTrueSkillStandardDeviationSM = reader["Player2H2HTrueSkillStandardDeviationSM"];
                    var Player1H2HTrueSkillMeanOldSM = reader["Player1H2HTrueSkillMeanOldSM"];
                    var Player1H2HTrueSkillStandardDeviationOldSM = reader["Player1H2HTrueSkillStandardDeviationOldSM"];
                    var Player2H2HTrueSkillMeanOldSM = reader["Player2H2HTrueSkillMeanOldSM"];
                    var Player2H2HTrueSkillStandardDeviationOldSM = reader["Player2H2HTrueSkillStandardDeviationOldSM"];
                    var WinProbabilityPlayer1H2HSM = reader["WinProbabilityPlayer1H2HSM"];
                    var Player1H2HTrueSkillMeanGSM = reader["Player1H2HTrueSkillMeanGSM"];
                    var Player1H2HTrueSkillStandardDeviationGSM = reader["Player1H2HTrueSkillStandardDeviationGSM"];
                    var Player2H2HTrueSkillMeanGSM = reader["Player2H2HTrueSkillMeanGSM"];
                    var Player2H2HTrueSkillStandardDeviationGSM = reader["Player2H2HTrueSkillStandardDeviationGSM"];
                    var Player1H2HTrueSkillMeanOldGSM = reader["Player1H2HTrueSkillMeanOldGSM"];
                    var Player1H2HTrueSkillStandardDeviationOldGSM = reader["Player1H2HTrueSkillStandardDeviationOldGSM"];
                    var Player2H2HTrueSkillMeanOldGSM = reader["Player2H2HTrueSkillMeanOldGSM"];
                    var Player2H2HTrueSkillStandardDeviationOldGSM = reader["Player2H2HTrueSkillStandardDeviationOldGSM"];
                    var WinProbabilityPlayer1H2HGSM = reader["WinProbabilityPlayer1H2HGSM"];
                    var Player1H2HS1 = reader["Player1H2HS1"];
                    var Player2H2HS1 = reader["Player2H2HS1"];
                    var Player1H2HOldS1 = reader["Player1H2HOldS1"];
                    var Player2H2HOldS1 = reader["Player2H2HOldS1"];
                    var Player1H2HTrueSkillMeanMS1 = reader["Player1H2HTrueSkillMeanMS1"];
                    var Player1H2HTrueSkillStandardDeviationMS1 = reader["Player1H2HTrueSkillStandardDeviationMS1"];
                    var Player2H2HTrueSkillMeanMS1 = reader["Player2H2HTrueSkillMeanMS1"];
                    var Player2H2HTrueSkillStandardDeviationMS1 = reader["Player2H2HTrueSkillStandardDeviationMS1"];
                    var Player1H2HTrueSkillMeanOldMS1 = reader["Player1H2HTrueSkillMeanOldMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldMS1 = reader["Player1H2HTrueSkillStandardDeviationOldMS1"];
                    var Player2H2HTrueSkillMeanOldMS1 = reader["Player2H2HTrueSkillMeanOldMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldMS1 = reader["Player2H2HTrueSkillStandardDeviationOldMS1"];
                    var WinProbabilityPlayer1H2HMS1 = reader["WinProbabilityPlayer1H2HMS1"];
                    var Player1H2HTrueSkillMeanSMS1 = reader["Player1H2HTrueSkillMeanSMS1"];
                    var Player1H2HTrueSkillStandardDeviationSMS1 = reader["Player1H2HTrueSkillStandardDeviationSMS1"];
                    var Player2H2HTrueSkillMeanSMS1 = reader["Player2H2HTrueSkillMeanSMS1"];
                    var Player2H2HTrueSkillStandardDeviationSMS1 = reader["Player2H2HTrueSkillStandardDeviationSMS1"];
                    var Player1H2HTrueSkillMeanOldSMS1 = reader["Player1H2HTrueSkillMeanOldSMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldSMS1"];
                    var Player2H2HTrueSkillMeanOldSMS1 = reader["Player2H2HTrueSkillMeanOldSMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldSMS1"];
                    var WinProbabilityPlayer1H2HSMS1 = reader["WinProbabilityPlayer1H2HSMS1"];
                    var Player1H2HTrueSkillMeanGSMS1 = reader["Player1H2HTrueSkillMeanGSMS1"];
                    var Player1H2HTrueSkillStandardDeviationGSMS1 = reader["Player1H2HTrueSkillStandardDeviationGSMS1"];
                    var Player2H2HTrueSkillMeanGSMS1 = reader["Player2H2HTrueSkillMeanGSMS1"];
                    var Player2H2HTrueSkillStandardDeviationGSMS1 = reader["Player2H2HTrueSkillStandardDeviationGSMS1"];
                    var Player1H2HTrueSkillMeanOldGSMS1 = reader["Player1H2HTrueSkillMeanOldGSMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS1"];
                    var Player2H2HTrueSkillMeanOldGSMS1 = reader["Player2H2HTrueSkillMeanOldGSMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS1"];
                    var WinProbabilityPlayer1H2HGSMS1 = reader["WinProbabilityPlayer1H2HGSMS1"];
                    var Player1H2HS2 = reader["Player1H2HS2"];
                    var Player2H2HS2 = reader["Player2H2HS2"];
                    var Player1H2HOldS2 = reader["Player1H2HOldS2"];
                    var Player2H2HOldS2 = reader["Player2H2HOldS2"];
                    var Player1H2HTrueSkillMeanMS2 = reader["Player1H2HTrueSkillMeanMS2"];
                    var Player1H2HTrueSkillStandardDeviationMS2 = reader["Player1H2HTrueSkillStandardDeviationMS2"];
                    var Player2H2HTrueSkillMeanMS2 = reader["Player2H2HTrueSkillMeanMS2"];
                    var Player2H2HTrueSkillStandardDeviationMS2 = reader["Player2H2HTrueSkillStandardDeviationMS2"];
                    var Player1H2HTrueSkillMeanOldMS2 = reader["Player1H2HTrueSkillMeanOldMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldMS2 = reader["Player1H2HTrueSkillStandardDeviationOldMS2"];
                    var Player2H2HTrueSkillMeanOldMS2 = reader["Player2H2HTrueSkillMeanOldMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldMS2 = reader["Player2H2HTrueSkillStandardDeviationOldMS2"];
                    var WinProbabilityPlayer1H2HMS2 = reader["WinProbabilityPlayer1H2HMS2"];
                    var Player1H2HTrueSkillMeanSMS2 = reader["Player1H2HTrueSkillMeanSMS2"];
                    var Player1H2HTrueSkillStandardDeviationSMS2 = reader["Player1H2HTrueSkillStandardDeviationSMS2"];
                    var Player2H2HTrueSkillMeanSMS2 = reader["Player2H2HTrueSkillMeanSMS2"];
                    var Player2H2HTrueSkillStandardDeviationSMS2 = reader["Player2H2HTrueSkillStandardDeviationSMS2"];
                    var Player1H2HTrueSkillMeanOldSMS2 = reader["Player1H2HTrueSkillMeanOldSMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldSMS2"];
                    var Player2H2HTrueSkillMeanOldSMS2 = reader["Player2H2HTrueSkillMeanOldSMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldSMS2"];
                    var WinProbabilityPlayer1H2HSMS2 = reader["WinProbabilityPlayer1H2HSMS2"];
                    var Player1H2HTrueSkillMeanGSMS2 = reader["Player1H2HTrueSkillMeanGSMS2"];
                    var Player1H2HTrueSkillStandardDeviationGSMS2 = reader["Player1H2HTrueSkillStandardDeviationGSMS2"];
                    var Player2H2HTrueSkillMeanGSMS2 = reader["Player2H2HTrueSkillMeanGSMS2"];
                    var Player2H2HTrueSkillStandardDeviationGSMS2 = reader["Player2H2HTrueSkillStandardDeviationGSMS2"];
                    var Player1H2HTrueSkillMeanOldGSMS2 = reader["Player1H2HTrueSkillMeanOldGSMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS2"];
                    var Player2H2HTrueSkillMeanOldGSMS2 = reader["Player2H2HTrueSkillMeanOldGSMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS2"];
                    var WinProbabilityPlayer1H2HGSMS2 = reader["WinProbabilityPlayer1H2HGSMS2"];
                    var Player1H2HS3 = reader["Player1H2HS3"];
                    var Player2H2HS3 = reader["Player2H2HS3"];
                    var Player1H2HOldS3 = reader["Player1H2HOldS3"];
                    var Player2H2HOldS3 = reader["Player2H2HOldS3"];
                    var Player1H2HTrueSkillMeanMS3 = reader["Player1H2HTrueSkillMeanMS3"];
                    var Player1H2HTrueSkillStandardDeviationMS3 = reader["Player1H2HTrueSkillStandardDeviationMS3"];
                    var Player2H2HTrueSkillMeanMS3 = reader["Player2H2HTrueSkillMeanMS3"];
                    var Player2H2HTrueSkillStandardDeviationMS3 = reader["Player2H2HTrueSkillStandardDeviationMS3"];
                    var Player1H2HTrueSkillMeanOldMS3 = reader["Player1H2HTrueSkillMeanOldMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldMS3 = reader["Player1H2HTrueSkillStandardDeviationOldMS3"];
                    var Player2H2HTrueSkillMeanOldMS3 = reader["Player2H2HTrueSkillMeanOldMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldMS3 = reader["Player2H2HTrueSkillStandardDeviationOldMS3"];
                    var WinProbabilityPlayer1H2HMS3 = reader["WinProbabilityPlayer1H2HMS3"];
                    var Player1H2HTrueSkillMeanSMS3 = reader["Player1H2HTrueSkillMeanSMS3"];
                    var Player1H2HTrueSkillStandardDeviationSMS3 = reader["Player1H2HTrueSkillStandardDeviationSMS3"];
                    var Player2H2HTrueSkillMeanSMS3 = reader["Player2H2HTrueSkillMeanSMS3"];
                    var Player2H2HTrueSkillStandardDeviationSMS3 = reader["Player2H2HTrueSkillStandardDeviationSMS3"];
                    var Player1H2HTrueSkillMeanOldSMS3 = reader["Player1H2HTrueSkillMeanOldSMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldSMS3"];
                    var Player2H2HTrueSkillMeanOldSMS3 = reader["Player2H2HTrueSkillMeanOldSMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldSMS3"];
                    var WinProbabilityPlayer1H2HSMS3 = reader["WinProbabilityPlayer1H2HSMS3"];
                    var Player1H2HTrueSkillMeanGSMS3 = reader["Player1H2HTrueSkillMeanGSMS3"];
                    var Player1H2HTrueSkillStandardDeviationGSMS3 = reader["Player1H2HTrueSkillStandardDeviationGSMS3"];
                    var Player2H2HTrueSkillMeanGSMS3 = reader["Player2H2HTrueSkillMeanGSMS3"];
                    var Player2H2HTrueSkillStandardDeviationGSMS3 = reader["Player2H2HTrueSkillStandardDeviationGSMS3"];
                    var Player1H2HTrueSkillMeanOldGSMS3 = reader["Player1H2HTrueSkillMeanOldGSMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS3"];
                    var Player2H2HTrueSkillMeanOldGSMS3 = reader["Player2H2HTrueSkillMeanOldGSMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS3"];
                    var WinProbabilityPlayer1H2HGSMS3 = reader["WinProbabilityPlayer1H2HGSMS3"];
                    var Player1H2HS4 = reader["Player1H2HS4"];
                    var Player2H2HS4 = reader["Player2H2HS4"];
                    var Player1H2HOldS4 = reader["Player1H2HOldS4"];
                    var Player2H2HOldS4 = reader["Player2H2HOldS4"];
                    var Player1H2HTrueSkillMeanMS4 = reader["Player1H2HTrueSkillMeanMS4"];
                    var Player1H2HTrueSkillStandardDeviationMS4 = reader["Player1H2HTrueSkillStandardDeviationMS4"];
                    var Player2H2HTrueSkillMeanMS4 = reader["Player2H2HTrueSkillMeanMS4"];
                    var Player2H2HTrueSkillStandardDeviationMS4 = reader["Player2H2HTrueSkillStandardDeviationMS4"];
                    var Player1H2HTrueSkillMeanOldMS4 = reader["Player1H2HTrueSkillMeanOldMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldMS4 = reader["Player1H2HTrueSkillStandardDeviationOldMS4"];
                    var Player2H2HTrueSkillMeanOldMS4 = reader["Player2H2HTrueSkillMeanOldMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldMS4 = reader["Player2H2HTrueSkillStandardDeviationOldMS4"];
                    var WinProbabilityPlayer1H2HMS4 = reader["WinProbabilityPlayer1H2HMS4"];
                    var Player1H2HTrueSkillMeanSMS4 = reader["Player1H2HTrueSkillMeanSMS4"];
                    var Player1H2HTrueSkillStandardDeviationSMS4 = reader["Player1H2HTrueSkillStandardDeviationSMS4"];
                    var Player2H2HTrueSkillMeanSMS4 = reader["Player2H2HTrueSkillMeanSMS4"];
                    var Player2H2HTrueSkillStandardDeviationSMS4 = reader["Player2H2HTrueSkillStandardDeviationSMS4"];
                    var Player1H2HTrueSkillMeanOldSMS4 = reader["Player1H2HTrueSkillMeanOldSMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldSMS4"];
                    var Player2H2HTrueSkillMeanOldSMS4 = reader["Player2H2HTrueSkillMeanOldSMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldSMS4"];
                    var WinProbabilityPlayer1H2HSMS4 = reader["WinProbabilityPlayer1H2HSMS4"];
                    var Player1H2HTrueSkillMeanGSMS4 = reader["Player1H2HTrueSkillMeanGSMS4"];
                    var Player1H2HTrueSkillStandardDeviationGSMS4 = reader["Player1H2HTrueSkillStandardDeviationGSMS4"];
                    var Player2H2HTrueSkillMeanGSMS4 = reader["Player2H2HTrueSkillMeanGSMS4"];
                    var Player2H2HTrueSkillStandardDeviationGSMS4 = reader["Player2H2HTrueSkillStandardDeviationGSMS4"];
                    var Player1H2HTrueSkillMeanOldGSMS4 = reader["Player1H2HTrueSkillMeanOldGSMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS4"];
                    var Player2H2HTrueSkillMeanOldGSMS4 = reader["Player2H2HTrueSkillMeanOldGSMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS4"];
                    var WinProbabilityPlayer1H2HGSMS4 = reader["WinProbabilityPlayer1H2HGSMS4"];
                    var Player1Streak = reader["Player1Streak"];
                    var Player2Streak = reader["Player2Streak"];
                    var Player1StreakS1 = reader["Player1StreakS1"];
                    var Player2StreakS1 = reader["Player2StreakS1"];
                    var Player1StreakS2 = reader["Player1StreakS2"];
                    var Player2StreakS2 = reader["Player2StreakS2"];
                    var Player1StreakS3 = reader["Player1StreakS3"];
                    var Player2StreakS3 = reader["Player2StreakS3"];
                    var Player1StreakS4 = reader["Player1StreakS4"];
                    var Player2StreakS4 = reader["Player2StreakS4"];
                    var P1SetsWon = reader["P1SetsWon"];
                    var P2SetsWon = reader["P2SetsWon"];
                    var P1GamesWon = reader["P1GamesWon"];
                    var P2GamesWon = reader["P2GamesWon"];
                    var P1SetsLoss = reader["P1SetsLoss"];
                    var P2SetsLoss = reader["P2SetsLoss"];
                    var P1GamesLoss = reader["P1GamesLoss"];
                    var P2GamesLoss = reader["P2GamesLoss"];
                    var WinProbabilityNN = reader["WinProbabilityNN"];
                    var IsFinished = 1; // dodati nakon dodavanja atrbuta u bazu reader["IsFinished"];
                }
                catch (Exception ex)
                {
                    string aaa;
                    aaa = "";
                }

                var Match = new Match
                {
                    MatchTPId = reader["MatchTPId"] as int?,
                    TournamentEventTPId = reader["TournamentEventTPId"] as int?,
                    DateTime = reader["DateTime"] as DateTime?,
                    Player1TPId = reader["Player1TPId"] as int?,
                    Player2TPId = reader["Player2TPId"] as int?,
                    Player1Seed = reader["Player1Seed"] as string,
                    Player2Seed = reader["Player2Seed"] as string,
                    Result = reader["Result"] as string,
                    ResultDetails = reader["ResultDetails"] as string,
                    Player1Odds = reader["Player1Odds"] as double?,
                    Player2Odds = reader["Player2Odds"] as double?,
                    Player1Percentage = reader["Player1Percentage"] as double?,
                    Player2Percentage = reader["Player2Percentage"] as double?,
                    SurfaceId = reader["SurfaceId"] as int?,
                    RoundId = reader["RoundId"] as int?,
                    Player1TrueSkillMeanM = reader["Player1TrueSkillMeanM"] as double?,
                    Player1TrueSkillStandardDeviationM = reader["Player1TrueSkillStandardDeviationM"] as double?,
                    Player2TrueSkillMeanM = reader["Player2TrueSkillMeanM"] as double?,
                    Player2TrueSkillStandardDeviationM = reader["Player2TrueSkillStandardDeviationM"] as double?,
                    Player1TrueSkillMeanOldM = reader["Player1TrueSkillMeanOldM"] as double?,
                    Player1TrueSkillStandardDeviationOldM = reader["Player1TrueSkillStandardDeviationOldM"] as double?,
                    Player2TrueSkillMeanOldM = reader["Player2TrueSkillMeanOldM"] as double?,
                    Player2TrueSkillStandardDeviationOldM = reader["Player2TrueSkillStandardDeviationOldM"] as double?,
                    WinProbabilityPlayer1M = reader["WinProbabilityPlayer1M"] as double?,
                    Player1TrueSkillMeanSM = reader["Player1TrueSkillMeanSM"] as double?,
                    Player1TrueSkillStandardDeviationSM = reader["Player1TrueSkillStandardDeviationSM"] as double?,
                    Player2TrueSkillMeanSM = reader["Player2TrueSkillMeanSM"] as double?,
                    Player2TrueSkillStandardDeviationSM = reader["Player2TrueSkillStandardDeviationSM"] as double?,
                    Player1TrueSkillMeanOldSM = reader["Player1TrueSkillMeanOldSM"] as double?,
                    Player1TrueSkillStandardDeviationOldSM = reader["Player1TrueSkillStandardDeviationOldSM"] as double?,
                    Player2TrueSkillMeanOldSM = reader["Player2TrueSkillMeanOldSM"] as double?,
                    Player2TrueSkillStandardDeviationOldSM = reader["Player2TrueSkillStandardDeviationOldSM"] as double?,
                    WinProbabilityPlayer1SM = reader["WinProbabilityPlayer1SM"] as double?,
                    Player1TrueSkillMeanGSM = reader["Player1TrueSkillMeanGSM"] as double?,
                    Player1TrueSkillStandardDeviationGSM = reader["Player1TrueSkillStandardDeviationGSM"] as double?,
                    Player2TrueSkillMeanGSM = reader["Player2TrueSkillMeanGSM"] as double?,
                    Player2TrueSkillStandardDeviationGSM = reader["Player2TrueSkillStandardDeviationGSM"] as double?,
                    Player1TrueSkillMeanOldGSM = reader["Player1TrueSkillMeanOldGSM"] as double?,
                    Player1TrueSkillStandardDeviationOldGSM = reader["Player1TrueSkillStandardDeviationOldGSM"] as double?,
                    Player2TrueSkillMeanOldGSM = reader["Player2TrueSkillMeanOldGSM"] as double?,
                    Player2TrueSkillStandardDeviationOldGSM = reader["Player2TrueSkillStandardDeviationOldGSM"] as double?,
                    WinProbabilityPlayer1GSM = reader["WinProbabilityPlayer1GSM"] as double?,
                    Player1TrueSkillMeanMS1 = reader["Player1TrueSkillMeanMS1"] as double?,
                    Player1TrueSkillStandardDeviationMS1 = reader["Player1TrueSkillStandardDeviationMS1"] as double?,
                    Player2TrueSkillMeanMS1 = reader["Player2TrueSkillMeanMS1"] as double?,
                    Player2TrueSkillStandardDeviationMS1 = reader["Player2TrueSkillStandardDeviationMS1"] as double?,
                    Player1TrueSkillMeanOldMS1 = reader["Player1TrueSkillMeanOldMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldMS1 = reader["Player1TrueSkillStandardDeviationOldMS1"] as double?,
                    Player2TrueSkillMeanOldMS1 = reader["Player2TrueSkillMeanOldMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldMS1 = reader["Player2TrueSkillStandardDeviationOldMS1"] as double?,
                    WinProbabilityPlayer1MS1 = reader["WinProbabilityPlayer1MS1"] as double?,
                    Player1TrueSkillMeanSMS1 = reader["Player1TrueSkillMeanSMS1"] as double?,
                    Player1TrueSkillStandardDeviationSMS1 = reader["Player1TrueSkillStandardDeviationSMS1"] as double?,
                    Player2TrueSkillMeanSMS1 = reader["Player2TrueSkillMeanSMS1"] as double?,
                    Player2TrueSkillStandardDeviationSMS1 = reader["Player2TrueSkillStandardDeviationSMS1"] as double?,
                    Player1TrueSkillMeanOldSMS1 = reader["Player1TrueSkillMeanOldSMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS1 = reader["Player1TrueSkillStandardDeviationOldSMS1"] as double?,
                    Player2TrueSkillMeanOldSMS1 = reader["Player2TrueSkillMeanOldSMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS1 = reader["Player2TrueSkillStandardDeviationOldSMS1"] as double?,
                    WinProbabilityPlayer1SMS1 = reader["WinProbabilityPlayer1SMS1"] as double?,
                    Player1TrueSkillMeanGSMS1 = reader["Player1TrueSkillMeanGSMS1"] as double?,
                    Player1TrueSkillStandardDeviationGSMS1 = reader["Player1TrueSkillStandardDeviationGSMS1"] as double?,
                    Player2TrueSkillMeanGSMS1 = reader["Player2TrueSkillMeanGSMS1"] as double?,
                    Player2TrueSkillStandardDeviationGSMS1 = reader["Player2TrueSkillStandardDeviationGSMS1"] as double?,
                    Player1TrueSkillMeanOldGSMS1 = reader["Player1TrueSkillMeanOldGSMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS1 = reader["Player1TrueSkillStandardDeviationOldGSMS1"] as double?,
                    Player2TrueSkillMeanOldGSMS1 = reader["Player2TrueSkillMeanOldGSMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS1 = reader["Player2TrueSkillStandardDeviationOldGSMS1"] as double?,
                    WinProbabilityPlayer1GSMS1 = reader["WinProbabilityPlayer1GSMS1"] as double?,
                    Player1TrueSkillMeanMS2 = reader["Player1TrueSkillMeanMS2"] as double?,
                    Player1TrueSkillStandardDeviationMS2 = reader["Player1TrueSkillStandardDeviationMS2"] as double?,
                    Player2TrueSkillMeanMS2 = reader["Player2TrueSkillMeanMS2"] as double?,
                    Player2TrueSkillStandardDeviationMS2 = reader["Player2TrueSkillStandardDeviationMS2"] as double?,
                    Player1TrueSkillMeanOldMS2 = reader["Player1TrueSkillMeanOldMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldMS2 = reader["Player1TrueSkillStandardDeviationOldMS2"] as double?,
                    Player2TrueSkillMeanOldMS2 = reader["Player2TrueSkillMeanOldMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldMS2 = reader["Player2TrueSkillStandardDeviationOldMS2"] as double?,
                    WinProbabilityPlayer1MS2 = reader["WinProbabilityPlayer1MS2"] as double?,
                    Player1TrueSkillMeanSMS2 = reader["Player1TrueSkillMeanSMS2"] as double?,
                    Player1TrueSkillStandardDeviationSMS2 = reader["Player1TrueSkillStandardDeviationSMS2"] as double?,
                    Player2TrueSkillMeanSMS2 = reader["Player2TrueSkillMeanSMS2"] as double?,
                    Player2TrueSkillStandardDeviationSMS2 = reader["Player2TrueSkillStandardDeviationSMS2"] as double?,
                    Player1TrueSkillMeanOldSMS2 = reader["Player1TrueSkillMeanOldSMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS2 = reader["Player1TrueSkillStandardDeviationOldSMS2"] as double?,
                    Player2TrueSkillMeanOldSMS2 = reader["Player2TrueSkillMeanOldSMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS2 = reader["Player2TrueSkillStandardDeviationOldSMS2"] as double?,
                    WinProbabilityPlayer1SMS2 = reader["WinProbabilityPlayer1SMS2"] as double?,
                    Player1TrueSkillMeanGSMS2 = reader["Player1TrueSkillMeanGSMS2"] as double?,
                    Player1TrueSkillStandardDeviationGSMS2 = reader["Player1TrueSkillStandardDeviationGSMS2"] as double?,
                    Player2TrueSkillMeanGSMS2 = reader["Player2TrueSkillMeanGSMS2"] as double?,
                    Player2TrueSkillStandardDeviationGSMS2 = reader["Player2TrueSkillStandardDeviationGSMS2"] as double?,
                    Player1TrueSkillMeanOldGSMS2 = reader["Player1TrueSkillMeanOldGSMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS2 = reader["Player1TrueSkillStandardDeviationOldGSMS2"] as double?,
                    Player2TrueSkillMeanOldGSMS2 = reader["Player2TrueSkillMeanOldGSMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS2 = reader["Player2TrueSkillStandardDeviationOldGSMS2"] as double?,
                    WinProbabilityPlayer1GSMS2 = reader["WinProbabilityPlayer1GSMS2"] as double?,
                    Player1TrueSkillMeanMS3 = reader["Player1TrueSkillMeanMS3"] as double?,
                    Player1TrueSkillStandardDeviationMS3 = reader["Player1TrueSkillStandardDeviationMS3"] as double?,
                    Player2TrueSkillMeanMS3 = reader["Player2TrueSkillMeanMS3"] as double?,
                    Player2TrueSkillStandardDeviationMS3 = reader["Player2TrueSkillStandardDeviationMS3"] as double?,
                    Player1TrueSkillMeanOldMS3 = reader["Player1TrueSkillMeanOldMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldMS3 = reader["Player1TrueSkillStandardDeviationOldMS3"] as double?,
                    Player2TrueSkillMeanOldMS3 = reader["Player2TrueSkillMeanOldMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldMS3 = reader["Player2TrueSkillStandardDeviationOldMS3"] as double?,
                    WinProbabilityPlayer1MS3 = reader["WinProbabilityPlayer1MS3"] as double?,
                    Player1TrueSkillMeanSMS3 = reader["Player1TrueSkillMeanSMS3"] as double?,
                    Player1TrueSkillStandardDeviationSMS3 = reader["Player1TrueSkillStandardDeviationSMS3"] as double?,
                    Player2TrueSkillMeanSMS3 = reader["Player2TrueSkillMeanSMS3"] as double?,
                    Player2TrueSkillStandardDeviationSMS3 = reader["Player2TrueSkillStandardDeviationSMS3"] as double?,
                    Player1TrueSkillMeanOldSMS3 = reader["Player1TrueSkillMeanOldSMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS3 = reader["Player1TrueSkillStandardDeviationOldSMS3"] as double?,
                    Player2TrueSkillMeanOldSMS3 = reader["Player2TrueSkillMeanOldSMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS3 = reader["Player2TrueSkillStandardDeviationOldSMS3"] as double?,
                    WinProbabilityPlayer1SMS3 = reader["WinProbabilityPlayer1SMS3"] as double?,
                    Player1TrueSkillMeanGSMS3 = reader["Player1TrueSkillMeanGSMS3"] as double?,
                    Player1TrueSkillStandardDeviationGSMS3 = reader["Player1TrueSkillStandardDeviationGSMS3"] as double?,
                    Player2TrueSkillMeanGSMS3 = reader["Player2TrueSkillMeanGSMS3"] as double?,
                    Player2TrueSkillStandardDeviationGSMS3 = reader["Player2TrueSkillStandardDeviationGSMS3"] as double?,
                    Player1TrueSkillMeanOldGSMS3 = reader["Player1TrueSkillMeanOldGSMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS3 = reader["Player1TrueSkillStandardDeviationOldGSMS3"] as double?,
                    Player2TrueSkillMeanOldGSMS3 = reader["Player2TrueSkillMeanOldGSMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS3 = reader["Player2TrueSkillStandardDeviationOldGSMS3"] as double?,
                    WinProbabilityPlayer1GSMS3 = reader["WinProbabilityPlayer1GSMS3"] as double?,
                    Player1TrueSkillMeanMS4 = reader["Player1TrueSkillMeanMS4"] as double?,
                    Player1TrueSkillStandardDeviationMS4 = reader["Player1TrueSkillStandardDeviationMS4"] as double?,
                    Player2TrueSkillMeanMS4 = reader["Player2TrueSkillMeanMS4"] as double?,
                    Player2TrueSkillStandardDeviationMS4 = reader["Player2TrueSkillStandardDeviationMS4"] as double?,
                    Player1TrueSkillMeanOldMS4 = reader["Player1TrueSkillMeanOldMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldMS4 = reader["Player1TrueSkillStandardDeviationOldMS4"] as double?,
                    Player2TrueSkillMeanOldMS4 = reader["Player2TrueSkillMeanOldMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldMS4 = reader["Player2TrueSkillStandardDeviationOldMS4"] as double?,
                    WinProbabilityPlayer1MS4 = reader["WinProbabilityPlayer1MS4"] as double?,
                    Player1TrueSkillMeanSMS4 = reader["Player1TrueSkillMeanSMS4"] as double?,
                    Player1TrueSkillStandardDeviationSMS4 = reader["Player1TrueSkillStandardDeviationSMS4"] as double?,
                    Player2TrueSkillMeanSMS4 = reader["Player2TrueSkillMeanSMS4"] as double?,
                    Player2TrueSkillStandardDeviationSMS4 = reader["Player2TrueSkillStandardDeviationSMS4"] as double?,
                    Player1TrueSkillMeanOldSMS4 = reader["Player1TrueSkillMeanOldSMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS4 = reader["Player1TrueSkillStandardDeviationOldSMS4"] as double?,
                    Player2TrueSkillMeanOldSMS4 = reader["Player2TrueSkillMeanOldSMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS4 = reader["Player2TrueSkillStandardDeviationOldSMS4"] as double?,
                    WinProbabilityPlayer1SMS4 = reader["WinProbabilityPlayer1SMS4"] as double?,
                    Player1TrueSkillMeanGSMS4 = reader["Player1TrueSkillMeanGSMS4"] as double?,
                    Player1TrueSkillStandardDeviationGSMS4 = reader["Player1TrueSkillStandardDeviationGSMS4"] as double?,
                    Player2TrueSkillMeanGSMS4 = reader["Player2TrueSkillMeanGSMS4"] as double?,
                    Player2TrueSkillStandardDeviationGSMS4 = reader["Player2TrueSkillStandardDeviationGSMS4"] as double?,
                    Player1TrueSkillMeanOldGSMS4 = reader["Player1TrueSkillMeanOldGSMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS4 = reader["Player1TrueSkillStandardDeviationOldGSMS4"] as double?,
                    Player2TrueSkillMeanOldGSMS4 = reader["Player2TrueSkillMeanOldGSMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS4 = reader["Player2TrueSkillStandardDeviationOldGSMS4"] as double?,
                    WinProbabilityPlayer1GSMS4 = reader["WinProbabilityPlayer1GSMS4"] as double?,
                    Player1WinsTotal = reader["Player1WinsTotal"] as int?,
                    Player1LossesTotal = reader["Player1LossesTotal"] as int?,
                    Player1WinsLastYear = reader["Player1WinsLastYear"] as int?,
                    Player1LossesLastYear = reader["Player1LossesLastYear"] as int?,
                    Player1WinsLastMonth = reader["Player1WinsLastMonth"] as int?,
                    Player1LossesLastMonth = reader["Player1LossesLastMonth"] as int?,
                    Player1WinsLastWeek = reader["Player1WinsLastWeek"] as int?,
                    Player1LossesLastWeek = reader["Player1LossesLastWeek"] as int?,
                    Player2WinsTotal = reader["Player2WinsTotal"] as int?,
                    Player2LossesTotal = reader["Player2LossesTotal"] as int?,
                    Player2WinsLastYear = reader["Player2WinsLastYear"] as int?,
                    Player2LossesLastYear = reader["Player2LossesLastYear"] as int?,
                    Player2WinsLastMonth = reader["Player2WinsLastMonth"] as int?,
                    Player2LossesLastMonth = reader["Player2LossesLastMonth"] as int?,
                    Player2WinsLastWeek = reader["Player2WinsLastWeek"] as int?,
                    Player2LossesLastWeek = reader["Player2LossesLastWeek"] as int?,
                    Player1WinsTotalS1 = reader["Player1WinsTotalS1"] as int?,
                    Player1LossesTotalS1 = reader["Player1LossesTotalS1"] as int?,
                    Player1WinsLastYearS1 = reader["Player1WinsLastYearS1"] as int?,
                    Player1LossesLastYearS1 = reader["Player1LossesLastYearS1"] as int?,
                    Player1WinsLastMonthS1 = reader["Player1WinsLastMonthS1"] as int?,
                    Player1LossesLastMonthS1 = reader["Player1LossesLastMonthS1"] as int?,
                    Player1WinsLastWeekS1 = reader["Player1WinsLastWeekS1"] as int?,
                    Player1LossesLastWeekS1 = reader["Player1LossesLastWeekS1"] as int?,
                    Player2WinsTotalS1 = reader["Player2WinsTotalS1"] as int?,
                    Player2LossesTotalS1 = reader["Player2LossesTotalS1"] as int?,
                    Player2WinsLastYearS1 = reader["Player2WinsLastYearS1"] as int?,
                    Player2LossesLastYearS1 = reader["Player2LossesLastYearS1"] as int?,
                    Player2WinsLastMonthS1 = reader["Player2WinsLastMonthS1"] as int?,
                    Player2LossesLastMonthS1 = reader["Player2LossesLastMonthS1"] as int?,
                    Player2WinsLastWeekS1 = reader["Player2WinsLastWeekS1"] as int?,
                    Player2LossesLastWeekS1 = reader["Player2LossesLastWeekS1"] as int?,
                    Player1WinsTotalS2 = reader["Player1WinsTotalS2"] as int?,
                    Player1LossesTotalS2 = reader["Player1LossesTotalS2"] as int?,
                    Player1WinsLastYearS2 = reader["Player1WinsLastYearS2"] as int?,
                    Player1LossesLastYearS2 = reader["Player1LossesLastYearS2"] as int?,
                    Player1WinsLastMonthS2 = reader["Player1WinsLastMonthS2"] as int?,
                    Player1LossesLastMonthS2 = reader["Player1LossesLastMonthS2"] as int?,
                    Player1WinsLastWeekS2 = reader["Player1WinsLastWeekS2"] as int?,
                    Player1LossesLastWeekS2 = reader["Player1LossesLastWeekS2"] as int?,
                    Player2WinsTotalS2 = reader["Player2WinsTotalS2"] as int?,
                    Player2LossesTotalS2 = reader["Player2LossesTotalS2"] as int?,
                    Player2WinsLastYearS2 = reader["Player2WinsLastYearS2"] as int?,
                    Player2LossesLastYearS2 = reader["Player2LossesLastYearS2"] as int?,
                    Player2WinsLastMonthS2 = reader["Player2WinsLastMonthS2"] as int?,
                    Player2LossesLastMonthS2 = reader["Player2LossesLastMonthS2"] as int?,
                    Player2WinsLastWeekS2 = reader["Player2WinsLastWeekS2"] as int?,
                    Player2LossesLastWeekS2 = reader["Player2LossesLastWeekS2"] as int?,
                    Player1WinsTotalS3 = reader["Player1WinsTotalS3"] as int?,
                    Player1LossesTotalS3 = reader["Player1LossesTotalS3"] as int?,
                    Player1WinsLastYearS3 = reader["Player1WinsLastYearS3"] as int?,
                    Player1LossesLastYearS3 = reader["Player1LossesLastYearS3"] as int?,
                    Player1WinsLastMonthS3 = reader["Player1WinsLastMonthS3"] as int?,
                    Player1LossesLastMonthS3 = reader["Player1LossesLastMonthS3"] as int?,
                    Player1WinsLastWeekS3 = reader["Player1WinsLastWeekS3"] as int?,
                    Player1LossesLastWeekS3 = reader["Player1LossesLastWeekS3"] as int?,
                    Player2WinsTotalS3 = reader["Player2WinsTotalS3"] as int?,
                    Player2LossesTotalS3 = reader["Player2LossesTotalS3"] as int?,
                    Player2WinsLastYearS3 = reader["Player2WinsLastYearS3"] as int?,
                    Player2LossesLastYearS3 = reader["Player2LossesLastYearS3"] as int?,
                    Player2WinsLastMonthS3 = reader["Player2WinsLastMonthS3"] as int?,
                    Player2LossesLastMonthS3 = reader["Player2LossesLastMonthS3"] as int?,
                    Player2WinsLastWeekS3 = reader["Player2WinsLastWeekS3"] as int?,
                    Player2LossesLastWeekS3 = reader["Player2LossesLastWeekS3"] as int?,
                    Player1WinsTotalS4 = reader["Player1WinsTotalS4"] as int?,
                    Player1LossesTotalS4 = reader["Player1LossesTotalS4"] as int?,
                    Player1WinsLastYearS4 = reader["Player1WinsLastYearS4"] as int?,
                    Player1LossesLastYearS4 = reader["Player1LossesLastYearS4"] as int?,
                    Player1WinsLastMonthS4 = reader["Player1WinsLastMonthS4"] as int?,
                    Player1LossesLastMonthS4 = reader["Player1LossesLastMonthS4"] as int?,
                    Player1WinsLastWeekS4 = reader["Player1WinsLastWeekS4"] as int?,
                    Player1LossesLastWeekS4 = reader["Player1LossesLastWeekS4"] as int?,
                    Player2WinsTotalS4 = reader["Player2WinsTotalS4"] as int?,
                    Player2LossesTotalS4 = reader["Player2LossesTotalS4"] as int?,
                    Player2WinsLastYearS4 = reader["Player2WinsLastYearS4"] as int?,
                    Player2LossesLastYearS4 = reader["Player2LossesLastYearS4"] as int?,
                    Player2WinsLastMonthS4 = reader["Player2WinsLastMonthS4"] as int?,
                    Player2LossesLastMonthS4 = reader["Player2LossesLastMonthS4"] as int?,
                    Player2WinsLastWeekS4 = reader["Player2WinsLastWeekS4"] as int?,
                    Player2LossesLastWeekS4 = reader["Player2LossesLastWeekS4"] as int?,
                    Player1WinsSetsTotal = reader["Player1WinsSetsTotal"] as int?,
                    Player1LossesSetsTotal = reader["Player1LossesSetsTotal"] as int?,
                    Player1WinsSetsLastYear = reader["Player1WinsSetsLastYear"] as int?,
                    Player1LossesSetsLastYear = reader["Player1LossesSetsLastYear"] as int?,
                    Player1WinsSetsLastMonth = reader["Player1WinsSetsLastMonth"] as int?,
                    Player1LossesSetsLastMonth = reader["Player1LossesSetsLastMonth"] as int?,
                    Player1WinsSetsLastWeek = reader["Player1WinsSetsLastWeek"] as int?,
                    Player1LossesSetsLastWeek = reader["Player1LossesSetsLastWeek"] as int?,
                    Player2WinsSetsTotal = reader["Player2WinsSetsTotal"] as int?,
                    Player2LossesSetsTotal = reader["Player2LossesSetsTotal"] as int?,
                    Player2WinsSetsLastYear = reader["Player2WinsSetsLastYear"] as int?,
                    Player2LossesSetsLastYear = reader["Player2LossesSetsLastYear"] as int?,
                    Player2WinsSetsLastMonth = reader["Player2WinsSetsLastMonth"] as int?,
                    Player2LossesSetsLastMonth = reader["Player2LossesSetsLastMonth"] as int?,
                    Player2WinsSetsLastWeek = reader["Player2WinsSetsLastWeek"] as int?,
                    Player2LossesSetsLastWeek = reader["Player2LossesSetsLastWeek"] as int?,
                    Player1WinsSetsTotalS1 = reader["Player1WinsSetsTotalS1"] as int?,
                    Player1LossesSetsTotalS1 = reader["Player1LossesSetsTotalS1"] as int?,
                    Player1WinsSetsLastYearS1 = reader["Player1WinsSetsLastYearS1"] as int?,
                    Player1LossesSetsLastYearS1 = reader["Player1LossesSetsLastYearS1"] as int?,
                    Player1WinsSetsLastMonthS1 = reader["Player1WinsSetsLastMonthS1"] as int?,
                    Player1LossesSetsLastMonthS1 = reader["Player1LossesSetsLastMonthS1"] as int?,
                    Player1WinsSetsLastWeekS1 = reader["Player1WinsSetsLastWeekS1"] as int?,
                    Player1LossesSetsLastWeekS1 = reader["Player1LossesSetsLastWeekS1"] as int?,
                    Player2WinsSetsTotalS1 = reader["Player2WinsSetsTotalS1"] as int?,
                    Player2LossesSetsTotalS1 = reader["Player2LossesSetsTotalS1"] as int?,
                    Player2WinsSetsLastYearS1 = reader["Player2WinsSetsLastYearS1"] as int?,
                    Player2LossesSetsLastYearS1 = reader["Player2LossesSetsLastYearS1"] as int?,
                    Player2WinsSetsLastMonthS1 = reader["Player2WinsSetsLastMonthS1"] as int?,
                    Player2LossesSetsLastMonthS1 = reader["Player2LossesSetsLastMonthS1"] as int?,
                    Player2WinsSetsLastWeekS1 = reader["Player2WinsSetsLastWeekS1"] as int?,
                    Player2LossesSetsLastWeekS1 = reader["Player2LossesSetsLastWeekS1"] as int?,
                    Player1WinsSetsTotalS2 = reader["Player1WinsSetsTotalS2"] as int?,
                    Player1LossesSetsTotalS2 = reader["Player1LossesSetsTotalS2"] as int?,
                    Player1WinsSetsLastYearS2 = reader["Player1WinsSetsLastYearS2"] as int?,
                    Player1LossesSetsLastYearS2 = reader["Player1LossesSetsLastYearS2"] as int?,
                    Player1WinsSetsLastMonthS2 = reader["Player1WinsSetsLastMonthS2"] as int?,
                    Player1LossesSetsLastMonthS2 = reader["Player1LossesSetsLastMonthS2"] as int?,
                    Player1WinsSetsLastWeekS2 = reader["Player1WinsSetsLastWeekS2"] as int?,
                    Player1LossesSetsLastWeekS2 = reader["Player1LossesSetsLastWeekS2"] as int?,
                    Player2WinsSetsTotalS2 = reader["Player2WinsSetsTotalS2"] as int?,
                    Player2LossesSetsTotalS2 = reader["Player2LossesSetsTotalS2"] as int?,
                    Player2WinsSetsLastYearS2 = reader["Player2WinsSetsLastYearS2"] as int?,
                    Player2LossesSetsLastYearS2 = reader["Player2LossesSetsLastYearS2"] as int?,
                    Player2WinsSetsLastMonthS2 = reader["Player2WinsSetsLastMonthS2"] as int?,
                    Player2LossesSetsLastMonthS2 = reader["Player2LossesSetsLastMonthS2"] as int?,
                    Player2WinsSetsLastWeekS2 = reader["Player2WinsSetsLastWeekS2"] as int?,
                    Player2LossesSetsLastWeekS2 = reader["Player2LossesSetsLastWeekS2"] as int?,
                    Player1WinsSetsTotalS3 = reader["Player1WinsSetsTotalS3"] as int?,
                    Player1LossesSetsTotalS3 = reader["Player1LossesSetsTotalS3"] as int?,
                    Player1WinsSetsLastYearS3 = reader["Player1WinsSetsLastYearS3"] as int?,
                    Player1LossesSetsLastYearS3 = reader["Player1LossesSetsLastYearS3"] as int?,
                    Player1WinsSetsLastMonthS3 = reader["Player1WinsSetsLastMonthS3"] as int?,
                    Player1LossesSetsLastMonthS3 = reader["Player1LossesSetsLastMonthS3"] as int?,
                    Player1WinsSetsLastWeekS3 = reader["Player1WinsSetsLastWeekS3"] as int?,
                    Player1LossesSetsLastWeekS3 = reader["Player1LossesSetsLastWeekS3"] as int?,
                    Player2WinsSetsTotalS3 = reader["Player2WinsSetsTotalS3"] as int?,
                    Player2LossesSetsTotalS3 = reader["Player2LossesSetsTotalS3"] as int?,
                    Player2WinsSetsLastYearS3 = reader["Player2WinsSetsLastYearS3"] as int?,
                    Player2LossesSetsLastYearS3 = reader["Player2LossesSetsLastYearS3"] as int?,
                    Player2WinsSetsLastMonthS3 = reader["Player2WinsSetsLastMonthS3"] as int?,
                    Player2LossesSetsLastMonthS3 = reader["Player2LossesSetsLastMonthS3"] as int?,
                    Player2WinsSetsLastWeekS3 = reader["Player2WinsSetsLastWeekS3"] as int?,
                    Player2LossesSetsLastWeekS3 = reader["Player2LossesSetsLastWeekS3"] as int?,
                    Player1WinsSetsTotalS4 = reader["Player1WinsSetsTotalS4"] as int?,
                    Player1LossesSetsTotalS4 = reader["Player1LossesSetsTotalS4"] as int?,
                    Player1WinsSetsLastYearS4 = reader["Player1WinsSetsLastYearS4"] as int?,
                    Player1LossesSetsLastYearS4 = reader["Player1LossesSetsLastYearS4"] as int?,
                    Player1WinsSetsLastMonthS4 = reader["Player1WinsSetsLastMonthS4"] as int?,
                    Player1LossesSetsLastMonthS4 = reader["Player1LossesSetsLastMonthS4"] as int?,
                    Player1WinsSetsLastWeekS4 = reader["Player1WinsSetsLastWeekS4"] as int?,
                    Player1LossesSetsLastWeekS4 = reader["Player1LossesSetsLastWeekS4"] as int?,
                    Player2WinsSetsTotalS4 = reader["Player2WinsSetsTotalS4"] as int?,
                    Player2LossesSetsTotalS4 = reader["Player2LossesSetsTotalS4"] as int?,
                    Player2WinsSetsLastYearS4 = reader["Player2WinsSetsLastYearS4"] as int?,
                    Player2LossesSetsLastYearS4 = reader["Player2LossesSetsLastYearS4"] as int?,
                    Player2WinsSetsLastMonthS4 = reader["Player2WinsSetsLastMonthS4"] as int?,
                    Player2LossesSetsLastMonthS4 = reader["Player2LossesSetsLastMonthS4"] as int?,
                    Player2WinsSetsLastWeekS4 = reader["Player2WinsSetsLastWeekS4"] as int?,
                    Player2LossesSetsLastWeekS4 = reader["Player2LossesSetsLastWeekS4"] as int?,
                    Player1WinsGamesTotal = reader["Player1WinsGamesTotal"] as int?,
                    Player1LossesGamesTotal = reader["Player1LossesGamesTotal"] as int?,
                    Player1WinsGamesLastYear = reader["Player1WinsGamesLastYear"] as int?,
                    Player1LossesGamesLastYear = reader["Player1LossesGamesLastYear"] as int?,
                    Player1WinsGamesLastMonth = reader["Player1WinsGamesLastMonth"] as int?,
                    Player1LossesGamesLastMonth = reader["Player1LossesGamesLastMonth"] as int?,
                    Player1WinsGamesLastWeek = reader["Player1WinsGamesLastWeek"] as int?,
                    Player1LossesGamesLastWeek = reader["Player1LossesGamesLastWeek"] as int?,
                    Player2WinsGamesTotal = reader["Player2WinsGamesTotal"] as int?,
                    Player2LossesGamesTotal = reader["Player2LossesGamesTotal"] as int?,
                    Player2WinsGamesLastYear = reader["Player2WinsGamesLastYear"] as int?,
                    Player2LossesGamesLastYear = reader["Player2LossesGamesLastYear"] as int?,
                    Player2WinsGamesLastMonth = reader["Player2WinsGamesLastMonth"] as int?,
                    Player2LossesGamesLastMonth = reader["Player2LossesGamesLastMonth"] as int?,
                    Player2WinsGamesLastWeek = reader["Player2WinsGamesLastWeek"] as int?,
                    Player2LossesGamesLastWeek = reader["Player2LossesGamesLastWeek"] as int?,
                    Player1WinsGamesTotalS1 = reader["Player1WinsGamesTotalS1"] as int?,
                    Player1LossesGamesTotalS1 = reader["Player1LossesGamesTotalS1"] as int?,
                    Player1WinsGamesLastYearS1 = reader["Player1WinsGamesLastYearS1"] as int?,
                    Player1LossesGamesLastYearS1 = reader["Player1LossesGamesLastYearS1"] as int?,
                    Player1WinsGamesLastMonthS1 = reader["Player1WinsGamesLastMonthS1"] as int?,
                    Player1LossesGamesLastMonthS1 = reader["Player1LossesGamesLastMonthS1"] as int?,
                    Player1WinsGamesLastWeekS1 = reader["Player1WinsGamesLastWeekS1"] as int?,
                    Player1LossesGamesLastWeekS1 = reader["Player1LossesGamesLastWeekS1"] as int?,
                    Player2WinsGamesTotalS1 = reader["Player2WinsGamesTotalS1"] as int?,
                    Player2LossesGamesTotalS1 = reader["Player2LossesGamesTotalS1"] as int?,
                    Player2WinsGamesLastYearS1 = reader["Player2WinsGamesLastYearS1"] as int?,
                    Player2LossesGamesLastYearS1 = reader["Player2LossesGamesLastYearS1"] as int?,
                    Player2WinsGamesLastMonthS1 = reader["Player2WinsGamesLastMonthS1"] as int?,
                    Player2LossesGamesLastMonthS1 = reader["Player2LossesGamesLastMonthS1"] as int?,
                    Player2WinsGamesLastWeekS1 = reader["Player2WinsGamesLastWeekS1"] as int?,
                    Player2LossesGamesLastWeekS1 = reader["Player2LossesGamesLastWeekS1"] as int?,
                    Player1WinsGamesTotalS2 = reader["Player1WinsGamesTotalS2"] as int?,
                    Player1LossesGamesTotalS2 = reader["Player1LossesGamesTotalS2"] as int?,
                    Player1WinsGamesLastYearS2 = reader["Player1WinsGamesLastYearS2"] as int?,
                    Player1LossesGamesLastYearS2 = reader["Player1LossesGamesLastYearS2"] as int?,
                    Player1WinsGamesLastMonthS2 = reader["Player1WinsGamesLastMonthS2"] as int?,
                    Player1LossesGamesLastMonthS2 = reader["Player1LossesGamesLastMonthS2"] as int?,
                    Player1WinsGamesLastWeekS2 = reader["Player1WinsGamesLastWeekS2"] as int?,
                    Player1LossesGamesLastWeekS2 = reader["Player1LossesGamesLastWeekS2"] as int?,
                    Player2WinsGamesTotalS2 = reader["Player2WinsGamesTotalS2"] as int?,
                    Player2LossesGamesTotalS2 = reader["Player2LossesGamesTotalS2"] as int?,
                    Player2WinsGamesLastYearS2 = reader["Player2WinsGamesLastYearS2"] as int?,
                    Player2LossesGamesLastYearS2 = reader["Player2LossesGamesLastYearS2"] as int?,
                    Player2WinsGamesLastMonthS2 = reader["Player2WinsGamesLastMonthS2"] as int?,
                    Player2LossesGamesLastMonthS2 = reader["Player2LossesGamesLastMonthS2"] as int?,
                    Player2WinsGamesLastWeekS2 = reader["Player2WinsGamesLastWeekS2"] as int?,
                    Player2LossesGamesLastWeekS2 = reader["Player2LossesGamesLastWeekS2"] as int?,
                    Player1WinsGamesTotalS3 = reader["Player1WinsGamesTotalS3"] as int?,
                    Player1LossesGamesTotalS3 = reader["Player1LossesGamesTotalS3"] as int?,
                    Player1WinsGamesLastYearS3 = reader["Player1WinsGamesLastYearS3"] as int?,
                    Player1LossesGamesLastYearS3 = reader["Player1LossesGamesLastYearS3"] as int?,
                    Player1WinsGamesLastMonthS3 = reader["Player1WinsGamesLastMonthS3"] as int?,
                    Player1LossesGamesLastMonthS3 = reader["Player1LossesGamesLastMonthS3"] as int?,
                    Player1WinsGamesLastWeekS3 = reader["Player1WinsGamesLastWeekS3"] as int?,
                    Player1LossesGamesLastWeekS3 = reader["Player1LossesGamesLastWeekS3"] as int?,
                    Player2WinsGamesTotalS3 = reader["Player2WinsGamesTotalS3"] as int?,
                    Player2LossesGamesTotalS3 = reader["Player2LossesGamesTotalS3"] as int?,
                    Player2WinsGamesLastYearS3 = reader["Player2WinsGamesLastYearS3"] as int?,
                    Player2LossesGamesLastYearS3 = reader["Player2LossesGamesLastYearS3"] as int?,
                    Player2WinsGamesLastMonthS3 = reader["Player2WinsGamesLastMonthS3"] as int?,
                    Player2LossesGamesLastMonthS3 = reader["Player2LossesGamesLastMonthS3"] as int?,
                    Player2WinsGamesLastWeekS3 = reader["Player2WinsGamesLastWeekS3"] as int?,
                    Player2LossesGamesLastWeekS3 = reader["Player2LossesGamesLastWeekS3"] as int?,
                    Player1WinsGamesTotalS4 = reader["Player1WinsGamesTotalS4"] as int?,
                    Player1LossesGamesTotalS4 = reader["Player1LossesGamesTotalS4"] as int?,
                    Player1WinsGamesLastYearS4 = reader["Player1WinsGamesLastYearS4"] as int?,
                    Player1LossesGamesLastYearS4 = reader["Player1LossesGamesLastYearS4"] as int?,
                    Player1WinsGamesLastMonthS4 = reader["Player1WinsGamesLastMonthS4"] as int?,
                    Player1LossesGamesLastMonthS4 = reader["Player1LossesGamesLastMonthS4"] as int?,
                    Player1WinsGamesLastWeekS4 = reader["Player1WinsGamesLastWeekS4"] as int?,
                    Player1LossesGamesLastWeekS4 = reader["Player1LossesGamesLastWeekS4"] as int?,
                    Player2WinsGamesTotalS4 = reader["Player2WinsGamesTotalS4"] as int?,
                    Player2LossesGamesTotalS4 = reader["Player2LossesGamesTotalS4"] as int?,
                    Player2WinsGamesLastYearS4 = reader["Player2WinsGamesLastYearS4"] as int?,
                    Player2LossesGamesLastYearS4 = reader["Player2LossesGamesLastYearS4"] as int?,
                    Player2WinsGamesLastMonthS4 = reader["Player2WinsGamesLastMonthS4"] as int?,
                    Player2LossesGamesLastMonthS4 = reader["Player2LossesGamesLastMonthS4"] as int?,
                    Player2WinsGamesLastWeekS4 = reader["Player2WinsGamesLastWeekS4"] as int?,
                    Player2LossesGamesLastWeekS4 = reader["Player2LossesGamesLastWeekS4"] as int?,
                    Player1DaysSinceLastWin = reader["Player1DaysSinceLastWin"] as int?,
                    Player2DaysSinceLastWin = reader["Player2DaysSinceLastWin"] as int?,
                    Player1DaysSinceLastWinS1 = reader["Player1DaysSinceLastWinS1"] as int?,
                    Player2DaysSinceLastWinS1 = reader["Player2DaysSinceLastWinS1"] as int?,
                    Player1DaysSinceLastWinS2 = reader["Player1DaysSinceLastWinS2"] as int?,
                    Player2DaysSinceLastWinS2 = reader["Player2DaysSinceLastWinS2"] as int?,
                    Player1DaysSinceLastWinS3 = reader["Player1DaysSinceLastWinS3"] as int?,
                    Player2DaysSinceLastWinS3 = reader["Player2DaysSinceLastWinS3"] as int?,
                    Player1DaysSinceLastWinS4 = reader["Player1DaysSinceLastWinS4"] as int?,
                    Player2DaysSinceLastWinS4 = reader["Player2DaysSinceLastWinS4"] as int?,
                    Player1DaysSinceLastLoss = reader["Player1DaysSinceLastLoss"] as int?,
                    Player2DaysSinceLastLoss = reader["Player2DaysSinceLastLoss"] as int?,
                    Player1DaysSinceLastLossS1 = reader["Player1DaysSinceLastLossS1"] as int?,
                    Player2DaysSinceLastLossS1 = reader["Player2DaysSinceLastLossS1"] as int?,
                    Player1DaysSinceLastLossS2 = reader["Player1DaysSinceLastLossS2"] as int?,
                    Player2DaysSinceLastLossS2 = reader["Player2DaysSinceLastLossS2"] as int?,
                    Player1DaysSinceLastLossS3 = reader["Player1DaysSinceLastLossS3"] as int?,
                    Player2DaysSinceLastLossS3 = reader["Player2DaysSinceLastLossS3"] as int?,
                    Player1DaysSinceLastLossS4 = reader["Player1DaysSinceLastLossS4"] as int?,
                    Player2DaysSinceLastLossS4 = reader["Player2DaysSinceLastLossS4"] as int?,
                    Player1TotalWinsAsFavourite = reader["Player1TotalWinsAsFavourite"] as int?,
                    Player2TotalWinsAsFavourite = reader["Player2TotalWinsAsFavourite"] as int?,
                    Player1TotalWinsAsUnderdog = reader["Player1TotalWinsAsUnderdog"] as int?,
                    Player2TotalWinsAsUnderdog = reader["Player2TotalWinsAsUnderdog"] as int?,
                    Player1TotalLossesAsFavourite = reader["Player1TotalLossesAsFavourite"] as int?,
                    Player2TotalLossesAsFavourite = reader["Player2TotalLossesAsFavourite"] as int?,
                    Player1TotalLossesAsUnderdog = reader["Player1TotalLossesAsUnderdog"] as int?,
                    Player2TotalLossesAsUnderdog = reader["Player2TotalLossesAsUnderdog"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavourite = reader["Player1AverageWinningProbabilityAtWonAsFavourite"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavourite = reader["Player2AverageWinningProbabilityAtWonAsFavourite"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdog = reader["Player1AverageWinningProbabilityAtWonAsUnderdog"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdog = reader["Player2AverageWinningProbabilityAtWonAsUnderdog"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavourite = reader["Player1AverageWinningProbabilityAtLossAsFavourite"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavourite = reader["Player2AverageWinningProbabilityAtLossAsFavourite"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdog = reader["Player1AverageWinningProbabilityAtLossAsUnderdog"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdog = reader["Player2AverageWinningProbabilityAtLossAsUnderdog"] as double?,
                    Player1TotalWinsAsFavouriteLastYear = reader["Player1TotalWinsAsFavouriteLastYear"] as int?,
                    Player2TotalWinsAsFavouriteLastYear = reader["Player2TotalWinsAsFavouriteLastYear"] as int?,
                    Player1TotalWinsAsUnderdogLastYear = reader["Player1TotalWinsAsUnderdogLastYear"] as int?,
                    Player2TotalWinsAsUnderdogLastYear = reader["Player2TotalWinsAsUnderdogLastYear"] as int?,
                    Player1TotalLossesAsFavouriteLastYear = reader["Player1TotalLossesAsFavouriteLastYear"] as int?,
                    Player2TotalLossesAsFavouriteLastYear = reader["Player2TotalLossesAsFavouriteLastYear"] as int?,
                    Player1TotalLossesAsUnderdogLastYear = reader["Player1TotalLossesAsUnderdogLastYear"] as int?,
                    Player2TotalLossesAsUnderdogLastYear = reader["Player2TotalLossesAsUnderdogLastYear"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastYear"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastYear"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastYear"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastYear"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastYear"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastYear"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastYear"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastYear"] as double?,
                    Player1TotalWinsAsFavouriteLastMonth = reader["Player1TotalWinsAsFavouriteLastMonth"] as int?,
                    Player2TotalWinsAsFavouriteLastMonth = reader["Player2TotalWinsAsFavouriteLastMonth"] as int?,
                    Player1TotalWinsAsUnderdogLastMonth = reader["Player1TotalWinsAsUnderdogLastMonth"] as int?,
                    Player2TotalWinsAsUnderdogLastMonth = reader["Player2TotalWinsAsUnderdogLastMonth"] as int?,
                    Player1TotalLossesAsFavouriteLastMonth = reader["Player1TotalLossesAsFavouriteLastMonth"] as int?,
                    Player2TotalLossesAsFavouriteLastMonth = reader["Player2TotalLossesAsFavouriteLastMonth"] as int?,
                    Player1TotalLossesAsUnderdogLastMonth = reader["Player1TotalLossesAsUnderdogLastMonth"] as int?,
                    Player2TotalLossesAsUnderdogLastMonth = reader["Player2TotalLossesAsUnderdogLastMonth"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth"] as double?,
                    Player1TotalWinsAsFavouriteLastWeek = reader["Player1TotalWinsAsFavouriteLastWeek"] as int?,
                    Player2TotalWinsAsFavouriteLastWeek = reader["Player2TotalWinsAsFavouriteLastWeek"] as int?,
                    Player1TotalWinsAsUnderdogLastWeek = reader["Player1TotalWinsAsUnderdogLastWeek"] as int?,
                    Player2TotalWinsAsUnderdogLastWeek = reader["Player2TotalWinsAsUnderdogLastWeek"] as int?,
                    Player1TotalLossesAsFavouriteLastWeek = reader["Player1TotalLossesAsFavouriteLastWeek"] as int?,
                    Player2TotalLossesAsFavouriteLastWeek = reader["Player2TotalLossesAsFavouriteLastWeek"] as int?,
                    Player1TotalLossesAsUnderdogLastWeek = reader["Player1TotalLossesAsUnderdogLastWeek"] as int?,
                    Player2TotalLossesAsUnderdogLastWeek = reader["Player2TotalLossesAsUnderdogLastWeek"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek"] as double?,
                    Player1H2H = reader["Player1H2H"] as int?,
                    Player2H2H = reader["Player2H2H"] as int?,
                    Player1H2HOld = reader["Player1H2HOld"] as int?,
                    Player2H2HOld = reader["Player2H2HOld"] as int?,
                    Player1H2HTrueSkillMeanM = reader["Player1H2HTrueSkillMeanM"] as double?,
                    Player1H2HTrueSkillStandardDeviationM = reader["Player1H2HTrueSkillStandardDeviationM"] as double?,
                    Player2H2HTrueSkillMeanM = reader["Player2H2HTrueSkillMeanM"] as double?,
                    Player2H2HTrueSkillStandardDeviationM = reader["Player2H2HTrueSkillStandardDeviationM"] as double?,
                    Player1H2HTrueSkillMeanOldM = reader["Player1H2HTrueSkillMeanOldM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldM = reader["Player1H2HTrueSkillStandardDeviationOldM"] as double?,
                    Player2H2HTrueSkillMeanOldM = reader["Player2H2HTrueSkillMeanOldM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldM = reader["Player2H2HTrueSkillStandardDeviationOldM"] as double?,
                    WinProbabilityPlayer1H2HM = reader["WinProbabilityPlayer1H2HM"] as double?,
                    Player1H2HTrueSkillMeanSM = reader["Player1H2HTrueSkillMeanSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationSM = reader["Player1H2HTrueSkillStandardDeviationSM"] as double?,
                    Player2H2HTrueSkillMeanSM = reader["Player2H2HTrueSkillMeanSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationSM = reader["Player2H2HTrueSkillStandardDeviationSM"] as double?,
                    Player1H2HTrueSkillMeanOldSM = reader["Player1H2HTrueSkillMeanOldSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSM = reader["Player1H2HTrueSkillStandardDeviationOldSM"] as double?,
                    Player2H2HTrueSkillMeanOldSM = reader["Player2H2HTrueSkillMeanOldSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSM = reader["Player2H2HTrueSkillStandardDeviationOldSM"] as double?,
                    WinProbabilityPlayer1H2HSM = reader["WinProbabilityPlayer1H2HSM"] as double?,
                    Player1H2HTrueSkillMeanGSM = reader["Player1H2HTrueSkillMeanGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSM = reader["Player1H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player2H2HTrueSkillMeanGSM = reader["Player2H2HTrueSkillMeanGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSM = reader["Player2H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player1H2HTrueSkillMeanOldGSM = reader["Player1H2HTrueSkillMeanOldGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSM = reader["Player1H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    Player2H2HTrueSkillMeanOldGSM = reader["Player2H2HTrueSkillMeanOldGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSM = reader["Player2H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    WinProbabilityPlayer1H2HGSM = reader["WinProbabilityPlayer1H2HGSM"] as double?,
                    Player1H2HS1 = reader["Player1H2HS1"] as int?,
                    Player2H2HS1 = reader["Player2H2HS1"] as int?,
                    Player1H2HOldS1 = reader["Player1H2HOldS1"] as int?,
                    Player2H2HOldS1 = reader["Player2H2HOldS1"] as int?,
                    Player1H2HTrueSkillMeanMS1 = reader["Player1H2HTrueSkillMeanMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS1 = reader["Player1H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player2H2HTrueSkillMeanMS1 = reader["Player2H2HTrueSkillMeanMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS1 = reader["Player2H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player1H2HTrueSkillMeanOldMS1 = reader["Player1H2HTrueSkillMeanOldMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS1 = reader["Player1H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    Player2H2HTrueSkillMeanOldMS1 = reader["Player2H2HTrueSkillMeanOldMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS1 = reader["Player2H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    WinProbabilityPlayer1H2HMS1 = reader["WinProbabilityPlayer1H2HMS1"] as double?,
                    Player1H2HTrueSkillMeanSMS1 = reader["Player1H2HTrueSkillMeanSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS1 = reader["Player1H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player2H2HTrueSkillMeanSMS1 = reader["Player2H2HTrueSkillMeanSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS1 = reader["Player2H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldSMS1 = reader["Player1H2HTrueSkillMeanOldSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldSMS1 = reader["Player2H2HTrueSkillMeanOldSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    WinProbabilityPlayer1H2HSMS1 = reader["WinProbabilityPlayer1H2HSMS1"] as double?,
                    Player1H2HTrueSkillMeanGSMS1 = reader["Player1H2HTrueSkillMeanGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS1 = reader["Player1H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player2H2HTrueSkillMeanGSMS1 = reader["Player2H2HTrueSkillMeanGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS1 = reader["Player2H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS1 = reader["Player1H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS1 = reader["Player2H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    WinProbabilityPlayer1H2HGSMS1 = reader["WinProbabilityPlayer1H2HGSMS1"] as double?,
                    Player1H2HS2 = reader["Player1H2HS2"] as int?,
                    Player2H2HS2 = reader["Player2H2HS2"] as int?,
                    Player1H2HOldS2 = reader["Player1H2HOldS2"] as int?,
                    Player2H2HOldS2 = reader["Player2H2HOldS2"] as int?,
                    Player1H2HTrueSkillMeanMS2 = reader["Player1H2HTrueSkillMeanMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS2 = reader["Player1H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player2H2HTrueSkillMeanMS2 = reader["Player2H2HTrueSkillMeanMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS2 = reader["Player2H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player1H2HTrueSkillMeanOldMS2 = reader["Player1H2HTrueSkillMeanOldMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS2 = reader["Player1H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    Player2H2HTrueSkillMeanOldMS2 = reader["Player2H2HTrueSkillMeanOldMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS2 = reader["Player2H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    WinProbabilityPlayer1H2HMS2 = reader["WinProbabilityPlayer1H2HMS2"] as double?,
                    Player1H2HTrueSkillMeanSMS2 = reader["Player1H2HTrueSkillMeanSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS2 = reader["Player1H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player2H2HTrueSkillMeanSMS2 = reader["Player2H2HTrueSkillMeanSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS2 = reader["Player2H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldSMS2 = reader["Player1H2HTrueSkillMeanOldSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldSMS2 = reader["Player2H2HTrueSkillMeanOldSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    WinProbabilityPlayer1H2HSMS2 = reader["WinProbabilityPlayer1H2HSMS2"] as double?,
                    Player1H2HTrueSkillMeanGSMS2 = reader["Player1H2HTrueSkillMeanGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS2 = reader["Player1H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player2H2HTrueSkillMeanGSMS2 = reader["Player2H2HTrueSkillMeanGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS2 = reader["Player2H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS2 = reader["Player1H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS2 = reader["Player2H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    WinProbabilityPlayer1H2HGSMS2 = reader["WinProbabilityPlayer1H2HGSMS2"] as double?,
                    Player1H2HS3 = reader["Player1H2HS3"] as int?,
                    Player2H2HS3 = reader["Player2H2HS3"] as int?,
                    Player1H2HOldS3 = reader["Player1H2HOldS3"] as int?,
                    Player2H2HOldS3 = reader["Player2H2HOldS3"] as int?,
                    Player1H2HTrueSkillMeanMS3 = reader["Player1H2HTrueSkillMeanMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS3 = reader["Player1H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player2H2HTrueSkillMeanMS3 = reader["Player2H2HTrueSkillMeanMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS3 = reader["Player2H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player1H2HTrueSkillMeanOldMS3 = reader["Player1H2HTrueSkillMeanOldMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS3 = reader["Player1H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    Player2H2HTrueSkillMeanOldMS3 = reader["Player2H2HTrueSkillMeanOldMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS3 = reader["Player2H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    WinProbabilityPlayer1H2HMS3 = reader["WinProbabilityPlayer1H2HMS3"] as double?,
                    Player1H2HTrueSkillMeanSMS3 = reader["Player1H2HTrueSkillMeanSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS3 = reader["Player1H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player2H2HTrueSkillMeanSMS3 = reader["Player2H2HTrueSkillMeanSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS3 = reader["Player2H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldSMS3 = reader["Player1H2HTrueSkillMeanOldSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldSMS3 = reader["Player2H2HTrueSkillMeanOldSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    WinProbabilityPlayer1H2HSMS3 = reader["WinProbabilityPlayer1H2HSMS3"] as double?,
                    Player1H2HTrueSkillMeanGSMS3 = reader["Player1H2HTrueSkillMeanGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS3 = reader["Player1H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player2H2HTrueSkillMeanGSMS3 = reader["Player2H2HTrueSkillMeanGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS3 = reader["Player2H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS3 = reader["Player1H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS3 = reader["Player2H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    WinProbabilityPlayer1H2HGSMS3 = reader["WinProbabilityPlayer1H2HGSMS3"] as double?,
                    Player1H2HS4 = reader["Player1H2HS4"] as int?,
                    Player2H2HS4 = reader["Player2H2HS4"] as int?,
                    Player1H2HOldS4 = reader["Player1H2HOldS4"] as int?,
                    Player2H2HOldS4 = reader["Player2H2HOldS4"] as int?,
                    Player1H2HTrueSkillMeanMS4 = reader["Player1H2HTrueSkillMeanMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS4 = reader["Player1H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player2H2HTrueSkillMeanMS4 = reader["Player2H2HTrueSkillMeanMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS4 = reader["Player2H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player1H2HTrueSkillMeanOldMS4 = reader["Player1H2HTrueSkillMeanOldMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS4 = reader["Player1H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    Player2H2HTrueSkillMeanOldMS4 = reader["Player2H2HTrueSkillMeanOldMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS4 = reader["Player2H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    WinProbabilityPlayer1H2HMS4 = reader["WinProbabilityPlayer1H2HMS4"] as double?,
                    Player1H2HTrueSkillMeanSMS4 = reader["Player1H2HTrueSkillMeanSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS4 = reader["Player1H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player2H2HTrueSkillMeanSMS4 = reader["Player2H2HTrueSkillMeanSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS4 = reader["Player2H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldSMS4 = reader["Player1H2HTrueSkillMeanOldSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldSMS4 = reader["Player2H2HTrueSkillMeanOldSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    WinProbabilityPlayer1H2HSMS4 = reader["WinProbabilityPlayer1H2HSMS4"] as double?,
                    Player1H2HTrueSkillMeanGSMS4 = reader["Player1H2HTrueSkillMeanGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS4 = reader["Player1H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player2H2HTrueSkillMeanGSMS4 = reader["Player2H2HTrueSkillMeanGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS4 = reader["Player2H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS4 = reader["Player1H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS4 = reader["Player2H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    WinProbabilityPlayer1H2HGSMS4 = reader["WinProbabilityPlayer1H2HGSMS4"] as double?,
                    Player1Streak = reader["Player1Streak"] as int?,
                    Player2Streak = reader["Player2Streak"] as int?,
                    Player1StreakS1 = reader["Player1StreakS1"] as int?,
                    Player2StreakS1 = reader["Player2StreakS1"] as int?,
                    Player1StreakS2 = reader["Player1StreakS2"] as int?,
                    Player2StreakS2 = reader["Player2StreakS2"] as int?,
                    Player1StreakS3 = reader["Player1StreakS3"] as int?,
                    Player2StreakS3 = reader["Player2StreakS3"] as int?,
                    Player1StreakS4 = reader["Player1StreakS4"] as int?,
                    Player2StreakS4 = reader["Player2StreakS4"] as int?,
                    P1SetsWon = reader["P1SetsWon"] as int?,
                    P2SetsWon = reader["P2SetsWon"] as int?,
                    P1GamesWon = reader["P1GamesWon"] as int?,
                    P2GamesWon = reader["P2GamesWon"] as int?,
                    P1SetsLoss = reader["P1SetsLoss"] as int?,
                    P2SetsLoss = reader["P2SetsLoss"] as int?,
                    P1GamesLoss = reader["P1GamesLoss"] as int?,
                    P2GamesLoss = reader["P2GamesLoss"] as int?,
                    WinProbabilityNN = reader["WinProbabilityNN"] as double?,
                    IsFinished = reader["IsFinished"]                                                 as bool?,
                };
                Matches.Add(Match);
            }

            return Matches;
        }

        public async Task<IEnumerable<Match>> GetAllMatchesForTournamentAsync(int tournamentEventTPId)
        {
            var Matches = new List<Match>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllMatchesForTournament";
            command.CommandType = CommandType.StoredProcedure;

            var p = command.CreateParameter();
            p.ParameterName = "@tournamentEventTPId";
            p.Value = tournamentEventTPId;

            command.Parameters.Add(p);

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                try
                {
                    var MatchTPId = reader["MatchTPId"];
                    var TournamentEventTPId = reader["TournamentEventTPId"];
                    var DateTime = reader["DateTime"];
                    var Player1TPId = reader["Player1TPId"];
                    var Player2TPId = reader["Player2TPId"];
                    var Player1Seed = reader["Player1Seed"];
                    var Player2Seed = reader["Player2Seed"];
                    var Result = reader["Result"];
                    var ResultDetails = reader["ResultDetails"];
                    var Player1Odds = reader["Player1Odds"];
                    var Player2Odds = reader["Player2Odds"];
                    var Player1Percentage = reader["Player1Percentage"];
                    var Player2Percentage = reader["Player2Percentage"];
                    var SurfaceId = reader["SurfaceId"];
                    var RoundId = reader["RoundId"];
                    var Player1TrueSkillMeanM = reader["Player1TrueSkillMeanM"];
                    var Player1TrueSkillStandardDeviationM = reader["Player1TrueSkillStandardDeviationM"];
                    var Player2TrueSkillMeanM = reader["Player2TrueSkillMeanM"];
                    var Player2TrueSkillStandardDeviationM = reader["Player2TrueSkillStandardDeviationM"];
                    var Player1TrueSkillMeanOldM = reader["Player1TrueSkillMeanOldM"];
                    var Player1TrueSkillStandardDeviationOldM = reader["Player1TrueSkillStandardDeviationOldM"];
                    var Player2TrueSkillMeanOldM = reader["Player2TrueSkillMeanOldM"];
                    var Player2TrueSkillStandardDeviationOldM = reader["Player2TrueSkillStandardDeviationOldM"];
                    var WinProbabilityPlayer1M = reader["WinProbabilityPlayer1M"];
                    var Player1TrueSkillMeanSM = reader["Player1TrueSkillMeanSM"];
                    var Player1TrueSkillStandardDeviationSM = reader["Player1TrueSkillStandardDeviationSM"];
                    var Player2TrueSkillMeanSM = reader["Player2TrueSkillMeanSM"];
                    var Player2TrueSkillStandardDeviationSM = reader["Player2TrueSkillStandardDeviationSM"];
                    var Player1TrueSkillMeanOldSM = reader["Player1TrueSkillMeanOldSM"];
                    var Player1TrueSkillStandardDeviationOldSM = reader["Player1TrueSkillStandardDeviationOldSM"];
                    var Player2TrueSkillMeanOldSM = reader["Player2TrueSkillMeanOldSM"];
                    var Player2TrueSkillStandardDeviationOldSM = reader["Player2TrueSkillStandardDeviationOldSM"];
                    var WinProbabilityPlayer1SM = reader["WinProbabilityPlayer1SM"];
                    var Player1TrueSkillMeanGSM = reader["Player1TrueSkillMeanGSM"];
                    var Player1TrueSkillStandardDeviationGSM = reader["Player1TrueSkillStandardDeviationGSM"];
                    var Player2TrueSkillMeanGSM = reader["Player2TrueSkillMeanGSM"];
                    var Player2TrueSkillStandardDeviationGSM = reader["Player2TrueSkillStandardDeviationGSM"];
                    var Player1TrueSkillMeanOldGSM = reader["Player1TrueSkillMeanOldGSM"];
                    var Player1TrueSkillStandardDeviationOldGSM = reader["Player1TrueSkillStandardDeviationOldGSM"];
                    var Player2TrueSkillMeanOldGSM = reader["Player2TrueSkillMeanOldGSM"];
                    var Player2TrueSkillStandardDeviationOldGSM = reader["Player2TrueSkillStandardDeviationOldGSM"];
                    var WinProbabilityPlayer1GSM = reader["WinProbabilityPlayer1GSM"];
                    var Player1TrueSkillMeanMS1 = reader["Player1TrueSkillMeanMS1"];
                    var Player1TrueSkillStandardDeviationMS1 = reader["Player1TrueSkillStandardDeviationMS1"];
                    var Player2TrueSkillMeanMS1 = reader["Player2TrueSkillMeanMS1"];
                    var Player2TrueSkillStandardDeviationMS1 = reader["Player2TrueSkillStandardDeviationMS1"];
                    var Player1TrueSkillMeanOldMS1 = reader["Player1TrueSkillMeanOldMS1"];
                    var Player1TrueSkillStandardDeviationOldMS1 = reader["Player1TrueSkillStandardDeviationOldMS1"];
                    var Player2TrueSkillMeanOldMS1 = reader["Player2TrueSkillMeanOldMS1"];
                    var Player2TrueSkillStandardDeviationOldMS1 = reader["Player2TrueSkillStandardDeviationOldMS1"];
                    var WinProbabilityPlayer1MS1 = reader["WinProbabilityPlayer1MS1"];
                    var Player1TrueSkillMeanSMS1 = reader["Player1TrueSkillMeanSMS1"];
                    var Player1TrueSkillStandardDeviationSMS1 = reader["Player1TrueSkillStandardDeviationSMS1"];
                    var Player2TrueSkillMeanSMS1 = reader["Player2TrueSkillMeanSMS1"];
                    var Player2TrueSkillStandardDeviationSMS1 = reader["Player2TrueSkillStandardDeviationSMS1"];
                    var Player1TrueSkillMeanOldSMS1 = reader["Player1TrueSkillMeanOldSMS1"];
                    var Player1TrueSkillStandardDeviationOldSMS1 = reader["Player1TrueSkillStandardDeviationOldSMS1"];
                    var Player2TrueSkillMeanOldSMS1 = reader["Player2TrueSkillMeanOldSMS1"];
                    var Player2TrueSkillStandardDeviationOldSMS1 = reader["Player2TrueSkillStandardDeviationOldSMS1"];
                    var WinProbabilityPlayer1SMS1 = reader["WinProbabilityPlayer1SMS1"];
                    var Player1TrueSkillMeanGSMS1 = reader["Player1TrueSkillMeanGSMS1"];
                    var Player1TrueSkillStandardDeviationGSMS1 = reader["Player1TrueSkillStandardDeviationGSMS1"];
                    var Player2TrueSkillMeanGSMS1 = reader["Player2TrueSkillMeanGSMS1"];
                    var Player2TrueSkillStandardDeviationGSMS1 = reader["Player2TrueSkillStandardDeviationGSMS1"];
                    var Player1TrueSkillMeanOldGSMS1 = reader["Player1TrueSkillMeanOldGSMS1"];
                    var Player1TrueSkillStandardDeviationOldGSMS1 = reader["Player1TrueSkillStandardDeviationOldGSMS1"];
                    var Player2TrueSkillMeanOldGSMS1 = reader["Player2TrueSkillMeanOldGSMS1"];
                    var Player2TrueSkillStandardDeviationOldGSMS1 = reader["Player2TrueSkillStandardDeviationOldGSMS1"];
                    var WinProbabilityPlayer1GSMS1 = reader["WinProbabilityPlayer1GSMS1"];
                    var Player1TrueSkillMeanMS2 = reader["Player1TrueSkillMeanMS2"];
                    var Player1TrueSkillStandardDeviationMS2 = reader["Player1TrueSkillStandardDeviationMS2"];
                    var Player2TrueSkillMeanMS2 = reader["Player2TrueSkillMeanMS2"];
                    var Player2TrueSkillStandardDeviationMS2 = reader["Player2TrueSkillStandardDeviationMS2"];
                    var Player1TrueSkillMeanOldMS2 = reader["Player1TrueSkillMeanOldMS2"];
                    var Player1TrueSkillStandardDeviationOldMS2 = reader["Player1TrueSkillStandardDeviationOldMS2"];
                    var Player2TrueSkillMeanOldMS2 = reader["Player2TrueSkillMeanOldMS2"];
                    var Player2TrueSkillStandardDeviationOldMS2 = reader["Player2TrueSkillStandardDeviationOldMS2"];
                    var WinProbabilityPlayer1MS2 = reader["WinProbabilityPlayer1MS2"];
                    var Player1TrueSkillMeanSMS2 = reader["Player1TrueSkillMeanSMS2"];
                    var Player1TrueSkillStandardDeviationSMS2 = reader["Player1TrueSkillStandardDeviationSMS2"];
                    var Player2TrueSkillMeanSMS2 = reader["Player2TrueSkillMeanSMS2"];
                    var Player2TrueSkillStandardDeviationSMS2 = reader["Player2TrueSkillStandardDeviationSMS2"];
                    var Player1TrueSkillMeanOldSMS2 = reader["Player1TrueSkillMeanOldSMS2"];
                    var Player1TrueSkillStandardDeviationOldSMS2 = reader["Player1TrueSkillStandardDeviationOldSMS2"];
                    var Player2TrueSkillMeanOldSMS2 = reader["Player2TrueSkillMeanOldSMS2"];
                    var Player2TrueSkillStandardDeviationOldSMS2 = reader["Player2TrueSkillStandardDeviationOldSMS2"];
                    var WinProbabilityPlayer1SMS2 = reader["WinProbabilityPlayer1SMS2"];
                    var Player1TrueSkillMeanGSMS2 = reader["Player1TrueSkillMeanGSMS2"];
                    var Player1TrueSkillStandardDeviationGSMS2 = reader["Player1TrueSkillStandardDeviationGSMS2"];
                    var Player2TrueSkillMeanGSMS2 = reader["Player2TrueSkillMeanGSMS2"];
                    var Player2TrueSkillStandardDeviationGSMS2 = reader["Player2TrueSkillStandardDeviationGSMS2"];
                    var Player1TrueSkillMeanOldGSMS2 = reader["Player1TrueSkillMeanOldGSMS2"];
                    var Player1TrueSkillStandardDeviationOldGSMS2 = reader["Player1TrueSkillStandardDeviationOldGSMS2"];
                    var Player2TrueSkillMeanOldGSMS2 = reader["Player2TrueSkillMeanOldGSMS2"];
                    var Player2TrueSkillStandardDeviationOldGSMS2 = reader["Player2TrueSkillStandardDeviationOldGSMS2"];
                    var WinProbabilityPlayer1GSMS2 = reader["WinProbabilityPlayer1GSMS2"];
                    var Player1TrueSkillMeanMS3 = reader["Player1TrueSkillMeanMS3"];
                    var Player1TrueSkillStandardDeviationMS3 = reader["Player1TrueSkillStandardDeviationMS3"];
                    var Player2TrueSkillMeanMS3 = reader["Player2TrueSkillMeanMS3"];
                    var Player2TrueSkillStandardDeviationMS3 = reader["Player2TrueSkillStandardDeviationMS3"];
                    var Player1TrueSkillMeanOldMS3 = reader["Player1TrueSkillMeanOldMS3"];
                    var Player1TrueSkillStandardDeviationOldMS3 = reader["Player1TrueSkillStandardDeviationOldMS3"];
                    var Player2TrueSkillMeanOldMS3 = reader["Player2TrueSkillMeanOldMS3"];
                    var Player2TrueSkillStandardDeviationOldMS3 = reader["Player2TrueSkillStandardDeviationOldMS3"];
                    var WinProbabilityPlayer1MS3 = reader["WinProbabilityPlayer1MS3"];
                    var Player1TrueSkillMeanSMS3 = reader["Player1TrueSkillMeanSMS3"];
                    var Player1TrueSkillStandardDeviationSMS3 = reader["Player1TrueSkillStandardDeviationSMS3"];
                    var Player2TrueSkillMeanSMS3 = reader["Player2TrueSkillMeanSMS3"];
                    var Player2TrueSkillStandardDeviationSMS3 = reader["Player2TrueSkillStandardDeviationSMS3"];
                    var Player1TrueSkillMeanOldSMS3 = reader["Player1TrueSkillMeanOldSMS3"];
                    var Player1TrueSkillStandardDeviationOldSMS3 = reader["Player1TrueSkillStandardDeviationOldSMS3"];
                    var Player2TrueSkillMeanOldSMS3 = reader["Player2TrueSkillMeanOldSMS3"];
                    var Player2TrueSkillStandardDeviationOldSMS3 = reader["Player2TrueSkillStandardDeviationOldSMS3"];
                    var WinProbabilityPlayer1SMS3 = reader["WinProbabilityPlayer1SMS3"];
                    var Player1TrueSkillMeanGSMS3 = reader["Player1TrueSkillMeanGSMS3"];
                    var Player1TrueSkillStandardDeviationGSMS3 = reader["Player1TrueSkillStandardDeviationGSMS3"];
                    var Player2TrueSkillMeanGSMS3 = reader["Player2TrueSkillMeanGSMS3"];
                    var Player2TrueSkillStandardDeviationGSMS3 = reader["Player2TrueSkillStandardDeviationGSMS3"];
                    var Player1TrueSkillMeanOldGSMS3 = reader["Player1TrueSkillMeanOldGSMS3"];
                    var Player1TrueSkillStandardDeviationOldGSMS3 = reader["Player1TrueSkillStandardDeviationOldGSMS3"];
                    var Player2TrueSkillMeanOldGSMS3 = reader["Player2TrueSkillMeanOldGSMS3"];
                    var Player2TrueSkillStandardDeviationOldGSMS3 = reader["Player2TrueSkillStandardDeviationOldGSMS3"];
                    var WinProbabilityPlayer1GSMS3 = reader["WinProbabilityPlayer1GSMS3"];
                    var Player1TrueSkillMeanMS4 = reader["Player1TrueSkillMeanMS4"];
                    var Player1TrueSkillStandardDeviationMS4 = reader["Player1TrueSkillStandardDeviationMS4"];
                    var Player2TrueSkillMeanMS4 = reader["Player2TrueSkillMeanMS4"];
                    var Player2TrueSkillStandardDeviationMS4 = reader["Player2TrueSkillStandardDeviationMS4"];
                    var Player1TrueSkillMeanOldMS4 = reader["Player1TrueSkillMeanOldMS4"];
                    var Player1TrueSkillStandardDeviationOldMS4 = reader["Player1TrueSkillStandardDeviationOldMS4"];
                    var Player2TrueSkillMeanOldMS4 = reader["Player2TrueSkillMeanOldMS4"];
                    var Player2TrueSkillStandardDeviationOldMS4 = reader["Player2TrueSkillStandardDeviationOldMS4"];
                    var WinProbabilityPlayer1MS4 = reader["WinProbabilityPlayer1MS4"];
                    var Player1TrueSkillMeanSMS4 = reader["Player1TrueSkillMeanSMS4"];
                    var Player1TrueSkillStandardDeviationSMS4 = reader["Player1TrueSkillStandardDeviationSMS4"];
                    var Player2TrueSkillMeanSMS4 = reader["Player2TrueSkillMeanSMS4"];
                    var Player2TrueSkillStandardDeviationSMS4 = reader["Player2TrueSkillStandardDeviationSMS4"];
                    var Player1TrueSkillMeanOldSMS4 = reader["Player1TrueSkillMeanOldSMS4"];
                    var Player1TrueSkillStandardDeviationOldSMS4 = reader["Player1TrueSkillStandardDeviationOldSMS4"];
                    var Player2TrueSkillMeanOldSMS4 = reader["Player2TrueSkillMeanOldSMS4"];
                    var Player2TrueSkillStandardDeviationOldSMS4 = reader["Player2TrueSkillStandardDeviationOldSMS4"];
                    var WinProbabilityPlayer1SMS4 = reader["WinProbabilityPlayer1SMS4"];
                    var Player1TrueSkillMeanGSMS4 = reader["Player1TrueSkillMeanGSMS4"];
                    var Player1TrueSkillStandardDeviationGSMS4 = reader["Player1TrueSkillStandardDeviationGSMS4"];
                    var Player2TrueSkillMeanGSMS4 = reader["Player2TrueSkillMeanGSMS4"];
                    var Player2TrueSkillStandardDeviationGSMS4 = reader["Player2TrueSkillStandardDeviationGSMS4"];
                    var Player1TrueSkillMeanOldGSMS4 = reader["Player1TrueSkillMeanOldGSMS4"];
                    var Player1TrueSkillStandardDeviationOldGSMS4 = reader["Player1TrueSkillStandardDeviationOldGSMS4"];
                    var Player2TrueSkillMeanOldGSMS4 = reader["Player2TrueSkillMeanOldGSMS4"];
                    var Player2TrueSkillStandardDeviationOldGSMS4 = reader["Player2TrueSkillStandardDeviationOldGSMS4"];
                    var WinProbabilityPlayer1GSMS4 = reader["WinProbabilityPlayer1GSMS4"];
                    var Player1WinsTotal = reader["Player1WinsTotal"];
                    var Player1LossesTotal = reader["Player1LossesTotal"];
                    var Player1WinsLastYear = reader["Player1WinsLastYear"];
                    var Player1LossesLastYear = reader["Player1LossesLastYear"];
                    var Player1WinsLastMonth = reader["Player1WinsLastMonth"];
                    var Player1LossesLastMonth = reader["Player1LossesLastMonth"];
                    var Player1WinsLastWeek = reader["Player1WinsLastWeek"];
                    var Player1LossesLastWeek = reader["Player1LossesLastWeek"];
                    var Player2WinsTotal = reader["Player2WinsTotal"];
                    var Player2LossesTotal = reader["Player2LossesTotal"];
                    var Player2WinsLastYear = reader["Player2WinsLastYear"];
                    var Player2LossesLastYear = reader["Player2LossesLastYear"];
                    var Player2WinsLastMonth = reader["Player2WinsLastMonth"];
                    var Player2LossesLastMonth = reader["Player2LossesLastMonth"];
                    var Player2WinsLastWeek = reader["Player2WinsLastWeek"];
                    var Player2LossesLastWeek = reader["Player2LossesLastWeek"];
                    var Player1WinsTotalS1 = reader["Player1WinsTotalS1"];
                    var Player1LossesTotalS1 = reader["Player1LossesTotalS1"];
                    var Player1WinsLastYearS1 = reader["Player1WinsLastYearS1"];
                    var Player1LossesLastYearS1 = reader["Player1LossesLastYearS1"];
                    var Player1WinsLastMonthS1 = reader["Player1WinsLastMonthS1"];
                    var Player1LossesLastMonthS1 = reader["Player1LossesLastMonthS1"];
                    var Player1WinsLastWeekS1 = reader["Player1WinsLastWeekS1"];
                    var Player1LossesLastWeekS1 = reader["Player1LossesLastWeekS1"];
                    var Player2WinsTotalS1 = reader["Player2WinsTotalS1"];
                    var Player2LossesTotalS1 = reader["Player2LossesTotalS1"];
                    var Player2WinsLastYearS1 = reader["Player2WinsLastYearS1"];
                    var Player2LossesLastYearS1 = reader["Player2LossesLastYearS1"];
                    var Player2WinsLastMonthS1 = reader["Player2WinsLastMonthS1"];
                    var Player2LossesLastMonthS1 = reader["Player2LossesLastMonthS1"];
                    var Player2WinsLastWeekS1 = reader["Player2WinsLastWeekS1"];
                    var Player2LossesLastWeekS1 = reader["Player2LossesLastWeekS1"];
                    var Player1WinsTotalS2 = reader["Player1WinsTotalS2"];
                    var Player1LossesTotalS2 = reader["Player1LossesTotalS2"];
                    var Player1WinsLastYearS2 = reader["Player1WinsLastYearS2"];
                    var Player1LossesLastYearS2 = reader["Player1LossesLastYearS2"];
                    var Player1WinsLastMonthS2 = reader["Player1WinsLastMonthS2"];
                    var Player1LossesLastMonthS2 = reader["Player1LossesLastMonthS2"];
                    var Player1WinsLastWeekS2 = reader["Player1WinsLastWeekS2"];
                    var Player1LossesLastWeekS2 = reader["Player1LossesLastWeekS2"];
                    var Player2WinsTotalS2 = reader["Player2WinsTotalS2"];
                    var Player2LossesTotalS2 = reader["Player2LossesTotalS2"];
                    var Player2WinsLastYearS2 = reader["Player2WinsLastYearS2"];
                    var Player2LossesLastYearS2 = reader["Player2LossesLastYearS2"];
                    var Player2WinsLastMonthS2 = reader["Player2WinsLastMonthS2"];
                    var Player2LossesLastMonthS2 = reader["Player2LossesLastMonthS2"];
                    var Player2WinsLastWeekS2 = reader["Player2WinsLastWeekS2"];
                    var Player2LossesLastWeekS2 = reader["Player2LossesLastWeekS2"];
                    var Player1WinsTotalS3 = reader["Player1WinsTotalS3"];
                    var Player1LossesTotalS3 = reader["Player1LossesTotalS3"];
                    var Player1WinsLastYearS3 = reader["Player1WinsLastYearS3"];
                    var Player1LossesLastYearS3 = reader["Player1LossesLastYearS3"];
                    var Player1WinsLastMonthS3 = reader["Player1WinsLastMonthS3"];
                    var Player1LossesLastMonthS3 = reader["Player1LossesLastMonthS3"];
                    var Player1WinsLastWeekS3 = reader["Player1WinsLastWeekS3"];
                    var Player1LossesLastWeekS3 = reader["Player1LossesLastWeekS3"];
                    var Player2WinsTotalS3 = reader["Player2WinsTotalS3"];
                    var Player2LossesTotalS3 = reader["Player2LossesTotalS3"];
                    var Player2WinsLastYearS3 = reader["Player2WinsLastYearS3"];
                    var Player2LossesLastYearS3 = reader["Player2LossesLastYearS3"];
                    var Player2WinsLastMonthS3 = reader["Player2WinsLastMonthS3"];
                    var Player2LossesLastMonthS3 = reader["Player2LossesLastMonthS3"];
                    var Player2WinsLastWeekS3 = reader["Player2WinsLastWeekS3"];
                    var Player2LossesLastWeekS3 = reader["Player2LossesLastWeekS3"];
                    var Player1WinsTotalS4 = reader["Player1WinsTotalS4"];
                    var Player1LossesTotalS4 = reader["Player1LossesTotalS4"];
                    var Player1WinsLastYearS4 = reader["Player1WinsLastYearS4"];
                    var Player1LossesLastYearS4 = reader["Player1LossesLastYearS4"];
                    var Player1WinsLastMonthS4 = reader["Player1WinsLastMonthS4"];
                    var Player1LossesLastMonthS4 = reader["Player1LossesLastMonthS4"];
                    var Player1WinsLastWeekS4 = reader["Player1WinsLastWeekS4"];
                    var Player1LossesLastWeekS4 = reader["Player1LossesLastWeekS4"];
                    var Player2WinsTotalS4 = reader["Player2WinsTotalS4"];
                    var Player2LossesTotalS4 = reader["Player2LossesTotalS4"];
                    var Player2WinsLastYearS4 = reader["Player2WinsLastYearS4"];
                    var Player2LossesLastYearS4 = reader["Player2LossesLastYearS4"];
                    var Player2WinsLastMonthS4 = reader["Player2WinsLastMonthS4"];
                    var Player2LossesLastMonthS4 = reader["Player2LossesLastMonthS4"];
                    var Player2WinsLastWeekS4 = reader["Player2WinsLastWeekS4"];
                    var Player2LossesLastWeekS4 = reader["Player2LossesLastWeekS4"];
                    var Player1WinsSetsTotal = reader["Player1WinsSetsTotal"];
                    var Player1LossesSetsTotal = reader["Player1LossesSetsTotal"];
                    var Player1WinsSetsLastYear = reader["Player1WinsSetsLastYear"];
                    var Player1LossesSetsLastYear = reader["Player1LossesSetsLastYear"];
                    var Player1WinsSetsLastMonth = reader["Player1WinsSetsLastMonth"];
                    var Player1LossesSetsLastMonth = reader["Player1LossesSetsLastMonth"];
                    var Player1WinsSetsLastWeek = reader["Player1WinsSetsLastWeek"];
                    var Player1LossesSetsLastWeek = reader["Player1LossesSetsLastWeek"];
                    var Player2WinsSetsTotal = reader["Player2WinsSetsTotal"];
                    var Player2LossesSetsTotal = reader["Player2LossesSetsTotal"];
                    var Player2WinsSetsLastYear = reader["Player2WinsSetsLastYear"];
                    var Player2LossesSetsLastYear = reader["Player2LossesSetsLastYear"];
                    var Player2WinsSetsLastMonth = reader["Player2WinsSetsLastMonth"];
                    var Player2LossesSetsLastMonth = reader["Player2LossesSetsLastMonth"];
                    var Player2WinsSetsLastWeek = reader["Player2WinsSetsLastWeek"];
                    var Player2LossesSetsLastWeek = reader["Player2LossesSetsLastWeek"];
                    var Player1WinsSetsTotalS1 = reader["Player1WinsSetsTotalS1"];
                    var Player1LossesSetsTotalS1 = reader["Player1LossesSetsTotalS1"];
                    var Player1WinsSetsLastYearS1 = reader["Player1WinsSetsLastYearS1"];
                    var Player1LossesSetsLastYearS1 = reader["Player1LossesSetsLastYearS1"];
                    var Player1WinsSetsLastMonthS1 = reader["Player1WinsSetsLastMonthS1"];
                    var Player1LossesSetsLastMonthS1 = reader["Player1LossesSetsLastMonthS1"];
                    var Player1WinsSetsLastWeekS1 = reader["Player1WinsSetsLastWeekS1"];
                    var Player1LossesSetsLastWeekS1 = reader["Player1LossesSetsLastWeekS1"];
                    var Player2WinsSetsTotalS1 = reader["Player2WinsSetsTotalS1"];
                    var Player2LossesSetsTotalS1 = reader["Player2LossesSetsTotalS1"];
                    var Player2WinsSetsLastYearS1 = reader["Player2WinsSetsLastYearS1"];
                    var Player2LossesSetsLastYearS1 = reader["Player2LossesSetsLastYearS1"];
                    var Player2WinsSetsLastMonthS1 = reader["Player2WinsSetsLastMonthS1"];
                    var Player2LossesSetsLastMonthS1 = reader["Player2LossesSetsLastMonthS1"];
                    var Player2WinsSetsLastWeekS1 = reader["Player2WinsSetsLastWeekS1"];
                    var Player2LossesSetsLastWeekS1 = reader["Player2LossesSetsLastWeekS1"];
                    var Player1WinsSetsTotalS2 = reader["Player1WinsSetsTotalS2"];
                    var Player1LossesSetsTotalS2 = reader["Player1LossesSetsTotalS2"];
                    var Player1WinsSetsLastYearS2 = reader["Player1WinsSetsLastYearS2"];
                    var Player1LossesSetsLastYearS2 = reader["Player1LossesSetsLastYearS2"];
                    var Player1WinsSetsLastMonthS2 = reader["Player1WinsSetsLastMonthS2"];
                    var Player1LossesSetsLastMonthS2 = reader["Player1LossesSetsLastMonthS2"];
                    var Player1WinsSetsLastWeekS2 = reader["Player1WinsSetsLastWeekS2"];
                    var Player1LossesSetsLastWeekS2 = reader["Player1LossesSetsLastWeekS2"];
                    var Player2WinsSetsTotalS2 = reader["Player2WinsSetsTotalS2"];
                    var Player2LossesSetsTotalS2 = reader["Player2LossesSetsTotalS2"];
                    var Player2WinsSetsLastYearS2 = reader["Player2WinsSetsLastYearS2"];
                    var Player2LossesSetsLastYearS2 = reader["Player2LossesSetsLastYearS2"];
                    var Player2WinsSetsLastMonthS2 = reader["Player2WinsSetsLastMonthS2"];
                    var Player2LossesSetsLastMonthS2 = reader["Player2LossesSetsLastMonthS2"];
                    var Player2WinsSetsLastWeekS2 = reader["Player2WinsSetsLastWeekS2"];
                    var Player2LossesSetsLastWeekS2 = reader["Player2LossesSetsLastWeekS2"];
                    var Player1WinsSetsTotalS3 = reader["Player1WinsSetsTotalS3"];
                    var Player1LossesSetsTotalS3 = reader["Player1LossesSetsTotalS3"];
                    var Player1WinsSetsLastYearS3 = reader["Player1WinsSetsLastYearS3"];
                    var Player1LossesSetsLastYearS3 = reader["Player1LossesSetsLastYearS3"];
                    var Player1WinsSetsLastMonthS3 = reader["Player1WinsSetsLastMonthS3"];
                    var Player1LossesSetsLastMonthS3 = reader["Player1LossesSetsLastMonthS3"];
                    var Player1WinsSetsLastWeekS3 = reader["Player1WinsSetsLastWeekS3"];
                    var Player1LossesSetsLastWeekS3 = reader["Player1LossesSetsLastWeekS3"];
                    var Player2WinsSetsTotalS3 = reader["Player2WinsSetsTotalS3"];
                    var Player2LossesSetsTotalS3 = reader["Player2LossesSetsTotalS3"];
                    var Player2WinsSetsLastYearS3 = reader["Player2WinsSetsLastYearS3"];
                    var Player2LossesSetsLastYearS3 = reader["Player2LossesSetsLastYearS3"];
                    var Player2WinsSetsLastMonthS3 = reader["Player2WinsSetsLastMonthS3"];
                    var Player2LossesSetsLastMonthS3 = reader["Player2LossesSetsLastMonthS3"];
                    var Player2WinsSetsLastWeekS3 = reader["Player2WinsSetsLastWeekS3"];
                    var Player2LossesSetsLastWeekS3 = reader["Player2LossesSetsLastWeekS3"];
                    var Player1WinsSetsTotalS4 = reader["Player1WinsSetsTotalS4"];
                    var Player1LossesSetsTotalS4 = reader["Player1LossesSetsTotalS4"];
                    var Player1WinsSetsLastYearS4 = reader["Player1WinsSetsLastYearS4"];
                    var Player1LossesSetsLastYearS4 = reader["Player1LossesSetsLastYearS4"];
                    var Player1WinsSetsLastMonthS4 = reader["Player1WinsSetsLastMonthS4"];
                    var Player1LossesSetsLastMonthS4 = reader["Player1LossesSetsLastMonthS4"];
                    var Player1WinsSetsLastWeekS4 = reader["Player1WinsSetsLastWeekS4"];
                    var Player1LossesSetsLastWeekS4 = reader["Player1LossesSetsLastWeekS4"];
                    var Player2WinsSetsTotalS4 = reader["Player2WinsSetsTotalS4"];
                    var Player2LossesSetsTotalS4 = reader["Player2LossesSetsTotalS4"];
                    var Player2WinsSetsLastYearS4 = reader["Player2WinsSetsLastYearS4"];
                    var Player2LossesSetsLastYearS4 = reader["Player2LossesSetsLastYearS4"];
                    var Player2WinsSetsLastMonthS4 = reader["Player2WinsSetsLastMonthS4"];
                    var Player2LossesSetsLastMonthS4 = reader["Player2LossesSetsLastMonthS4"];
                    var Player2WinsSetsLastWeekS4 = reader["Player2WinsSetsLastWeekS4"];
                    var Player2LossesSetsLastWeekS4 = reader["Player2LossesSetsLastWeekS4"];
                    var Player1WinsGamesTotal = reader["Player1WinsGamesTotal"];
                    var Player1LossesGamesTotal = reader["Player1LossesGamesTotal"];
                    var Player1WinsGamesLastYear = reader["Player1WinsGamesLastYear"];
                    var Player1LossesGamesLastYear = reader["Player1LossesGamesLastYear"];
                    var Player1WinsGamesLastMonth = reader["Player1WinsGamesLastMonth"];
                    var Player1LossesGamesLastMonth = reader["Player1LossesGamesLastMonth"];
                    var Player1WinsGamesLastWeek = reader["Player1WinsGamesLastWeek"];
                    var Player1LossesGamesLastWeek = reader["Player1LossesGamesLastWeek"];
                    var Player2WinsGamesTotal = reader["Player2WinsGamesTotal"];
                    var Player2LossesGamesTotal = reader["Player2LossesGamesTotal"];
                    var Player2WinsGamesLastYear = reader["Player2WinsGamesLastYear"];
                    var Player2LossesGamesLastYear = reader["Player2LossesGamesLastYear"];
                    var Player2WinsGamesLastMonth = reader["Player2WinsGamesLastMonth"];
                    var Player2LossesGamesLastMonth = reader["Player2LossesGamesLastMonth"];
                    var Player2WinsGamesLastWeek = reader["Player2WinsGamesLastWeek"];
                    var Player2LossesGamesLastWeek = reader["Player2LossesGamesLastWeek"];
                    var Player1WinsGamesTotalS1 = reader["Player1WinsGamesTotalS1"];
                    var Player1LossesGamesTotalS1 = reader["Player1LossesGamesTotalS1"];
                    var Player1WinsGamesLastYearS1 = reader["Player1WinsGamesLastYearS1"];
                    var Player1LossesGamesLastYearS1 = reader["Player1LossesGamesLastYearS1"];
                    var Player1WinsGamesLastMonthS1 = reader["Player1WinsGamesLastMonthS1"];
                    var Player1LossesGamesLastMonthS1 = reader["Player1LossesGamesLastMonthS1"];
                    var Player1WinsGamesLastWeekS1 = reader["Player1WinsGamesLastWeekS1"];
                    var Player1LossesGamesLastWeekS1 = reader["Player1LossesGamesLastWeekS1"];
                    var Player2WinsGamesTotalS1 = reader["Player2WinsGamesTotalS1"];
                    var Player2LossesGamesTotalS1 = reader["Player2LossesGamesTotalS1"];
                    var Player2WinsGamesLastYearS1 = reader["Player2WinsGamesLastYearS1"];
                    var Player2LossesGamesLastYearS1 = reader["Player2LossesGamesLastYearS1"];
                    var Player2WinsGamesLastMonthS1 = reader["Player2WinsGamesLastMonthS1"];
                    var Player2LossesGamesLastMonthS1 = reader["Player2LossesGamesLastMonthS1"];
                    var Player2WinsGamesLastWeekS1 = reader["Player2WinsGamesLastWeekS1"];
                    var Player2LossesGamesLastWeekS1 = reader["Player2LossesGamesLastWeekS1"];
                    var Player1WinsGamesTotalS2 = reader["Player1WinsGamesTotalS2"];
                    var Player1LossesGamesTotalS2 = reader["Player1LossesGamesTotalS2"];
                    var Player1WinsGamesLastYearS2 = reader["Player1WinsGamesLastYearS2"];
                    var Player1LossesGamesLastYearS2 = reader["Player1LossesGamesLastYearS2"];
                    var Player1WinsGamesLastMonthS2 = reader["Player1WinsGamesLastMonthS2"];
                    var Player1LossesGamesLastMonthS2 = reader["Player1LossesGamesLastMonthS2"];
                    var Player1WinsGamesLastWeekS2 = reader["Player1WinsGamesLastWeekS2"];
                    var Player1LossesGamesLastWeekS2 = reader["Player1LossesGamesLastWeekS2"];
                    var Player2WinsGamesTotalS2 = reader["Player2WinsGamesTotalS2"];
                    var Player2LossesGamesTotalS2 = reader["Player2LossesGamesTotalS2"];
                    var Player2WinsGamesLastYearS2 = reader["Player2WinsGamesLastYearS2"];
                    var Player2LossesGamesLastYearS2 = reader["Player2LossesGamesLastYearS2"];
                    var Player2WinsGamesLastMonthS2 = reader["Player2WinsGamesLastMonthS2"];
                    var Player2LossesGamesLastMonthS2 = reader["Player2LossesGamesLastMonthS2"];
                    var Player2WinsGamesLastWeekS2 = reader["Player2WinsGamesLastWeekS2"];
                    var Player2LossesGamesLastWeekS2 = reader["Player2LossesGamesLastWeekS2"];
                    var Player1WinsGamesTotalS3 = reader["Player1WinsGamesTotalS3"];
                    var Player1LossesGamesTotalS3 = reader["Player1LossesGamesTotalS3"];
                    var Player1WinsGamesLastYearS3 = reader["Player1WinsGamesLastYearS3"];
                    var Player1LossesGamesLastYearS3 = reader["Player1LossesGamesLastYearS3"];
                    var Player1WinsGamesLastMonthS3 = reader["Player1WinsGamesLastMonthS3"];
                    var Player1LossesGamesLastMonthS3 = reader["Player1LossesGamesLastMonthS3"];
                    var Player1WinsGamesLastWeekS3 = reader["Player1WinsGamesLastWeekS3"];
                    var Player1LossesGamesLastWeekS3 = reader["Player1LossesGamesLastWeekS3"];
                    var Player2WinsGamesTotalS3 = reader["Player2WinsGamesTotalS3"];
                    var Player2LossesGamesTotalS3 = reader["Player2LossesGamesTotalS3"];
                    var Player2WinsGamesLastYearS3 = reader["Player2WinsGamesLastYearS3"];
                    var Player2LossesGamesLastYearS3 = reader["Player2LossesGamesLastYearS3"];
                    var Player2WinsGamesLastMonthS3 = reader["Player2WinsGamesLastMonthS3"];
                    var Player2LossesGamesLastMonthS3 = reader["Player2LossesGamesLastMonthS3"];
                    var Player2WinsGamesLastWeekS3 = reader["Player2WinsGamesLastWeekS3"];
                    var Player2LossesGamesLastWeekS3 = reader["Player2LossesGamesLastWeekS3"];
                    var Player1WinsGamesTotalS4 = reader["Player1WinsGamesTotalS4"];
                    var Player1LossesGamesTotalS4 = reader["Player1LossesGamesTotalS4"];
                    var Player1WinsGamesLastYearS4 = reader["Player1WinsGamesLastYearS4"];
                    var Player1LossesGamesLastYearS4 = reader["Player1LossesGamesLastYearS4"];
                    var Player1WinsGamesLastMonthS4 = reader["Player1WinsGamesLastMonthS4"];
                    var Player1LossesGamesLastMonthS4 = reader["Player1LossesGamesLastMonthS4"];
                    var Player1WinsGamesLastWeekS4 = reader["Player1WinsGamesLastWeekS4"];
                    var Player1LossesGamesLastWeekS4 = reader["Player1LossesGamesLastWeekS4"];
                    var Player2WinsGamesTotalS4 = reader["Player2WinsGamesTotalS4"];
                    var Player2LossesGamesTotalS4 = reader["Player2LossesGamesTotalS4"];
                    var Player2WinsGamesLastYearS4 = reader["Player2WinsGamesLastYearS4"];
                    var Player2LossesGamesLastYearS4 = reader["Player2LossesGamesLastYearS4"];
                    var Player2WinsGamesLastMonthS4 = reader["Player2WinsGamesLastMonthS4"];
                    var Player2LossesGamesLastMonthS4 = reader["Player2LossesGamesLastMonthS4"];
                    var Player2WinsGamesLastWeekS4 = reader["Player2WinsGamesLastWeekS4"];
                    var Player2LossesGamesLastWeekS4 = reader["Player2LossesGamesLastWeekS4"];
                    var Player1DaysSinceLastWin = reader["Player1DaysSinceLastWin"];
                    var Player2DaysSinceLastWin = reader["Player2DaysSinceLastWin"];
                    var Player1DaysSinceLastWinS1 = reader["Player1DaysSinceLastWinS1"];
                    var Player2DaysSinceLastWinS1 = reader["Player2DaysSinceLastWinS1"];
                    var Player1DaysSinceLastWinS2 = reader["Player1DaysSinceLastWinS2"];
                    var Player2DaysSinceLastWinS2 = reader["Player2DaysSinceLastWinS2"];
                    var Player1DaysSinceLastWinS3 = reader["Player1DaysSinceLastWinS3"];
                    var Player2DaysSinceLastWinS3 = reader["Player2DaysSinceLastWinS3"];
                    var Player1DaysSinceLastWinS4 = reader["Player1DaysSinceLastWinS4"];
                    var Player2DaysSinceLastWinS4 = reader["Player2DaysSinceLastWinS4"];
                    var Player1DaysSinceLastLoss = reader["Player1DaysSinceLastLoss"];
                    var Player2DaysSinceLastLoss = reader["Player2DaysSinceLastLoss"];
                    var Player1DaysSinceLastLossS1 = reader["Player1DaysSinceLastLossS1"];
                    var Player2DaysSinceLastLossS1 = reader["Player2DaysSinceLastLossS1"];
                    var Player1DaysSinceLastLossS2 = reader["Player1DaysSinceLastLossS2"];
                    var Player2DaysSinceLastLossS2 = reader["Player2DaysSinceLastLossS2"];
                    var Player1DaysSinceLastLossS3 = reader["Player1DaysSinceLastLossS3"];
                    var Player2DaysSinceLastLossS3 = reader["Player2DaysSinceLastLossS3"];
                    var Player1DaysSinceLastLossS4 = reader["Player1DaysSinceLastLossS4"];
                    var Player2DaysSinceLastLossS4 = reader["Player2DaysSinceLastLossS4"];
                    var Player1TotalWinsAsFavourite = reader["Player1TotalWinsAsFavourite"];
                    var Player2TotalWinsAsFavourite = reader["Player2TotalWinsAsFavourite"];
                    var Player1TotalWinsAsUnderdog = reader["Player1TotalWinsAsUnderdog"];
                    var Player2TotalWinsAsUnderdog = reader["Player2TotalWinsAsUnderdog"];
                    var Player1TotalLossesAsFavourite = reader["Player1TotalLossesAsFavourite"];
                    var Player2TotalLossesAsFavourite = reader["Player2TotalLossesAsFavourite"];
                    var Player1TotalLossesAsUnderdog = reader["Player1TotalLossesAsUnderdog"];
                    var Player2TotalLossesAsUnderdog = reader["Player2TotalLossesAsUnderdog"];
                    var Player1AverageWinningProbabilityAtWonAsFavourite = reader["Player1AverageWinningProbabilityAtWonAsFavourite"];
                    var Player2AverageWinningProbabilityAtWonAsFavourite = reader["Player2AverageWinningProbabilityAtWonAsFavourite"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdog = reader["Player1AverageWinningProbabilityAtWonAsUnderdog"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdog = reader["Player2AverageWinningProbabilityAtWonAsUnderdog"];
                    var Player1AverageWinningProbabilityAtLossAsFavourite = reader["Player1AverageWinningProbabilityAtLossAsFavourite"];
                    var Player2AverageWinningProbabilityAtLossAsFavourite = reader["Player2AverageWinningProbabilityAtLossAsFavourite"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdog = reader["Player1AverageWinningProbabilityAtLossAsUnderdog"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdog = reader["Player2AverageWinningProbabilityAtLossAsUnderdog"];
                    var Player1TotalWinsAsFavouriteLastYear = reader["Player1TotalWinsAsFavouriteLastYear"];
                    var Player2TotalWinsAsFavouriteLastYear = reader["Player2TotalWinsAsFavouriteLastYear"];
                    var Player1TotalWinsAsUnderdogLastYear = reader["Player1TotalWinsAsUnderdogLastYear"];
                    var Player2TotalWinsAsUnderdogLastYear = reader["Player2TotalWinsAsUnderdogLastYear"];
                    var Player1TotalLossesAsFavouriteLastYear = reader["Player1TotalLossesAsFavouriteLastYear"];
                    var Player2TotalLossesAsFavouriteLastYear = reader["Player2TotalLossesAsFavouriteLastYear"];
                    var Player1TotalLossesAsUnderdogLastYear = reader["Player1TotalLossesAsUnderdogLastYear"];
                    var Player2TotalLossesAsUnderdogLastYear = reader["Player2TotalLossesAsUnderdogLastYear"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastYear"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastYear"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastYear"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastYear"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastYear"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastYear"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastYear"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastYear"];
                    var Player1TotalWinsAsFavouriteLastMonth = reader["Player1TotalWinsAsFavouriteLastMonth"];
                    var Player2TotalWinsAsFavouriteLastMonth = reader["Player2TotalWinsAsFavouriteLastMonth"];
                    var Player1TotalWinsAsUnderdogLastMonth = reader["Player1TotalWinsAsUnderdogLastMonth"];
                    var Player2TotalWinsAsUnderdogLastMonth = reader["Player2TotalWinsAsUnderdogLastMonth"];
                    var Player1TotalLossesAsFavouriteLastMonth = reader["Player1TotalLossesAsFavouriteLastMonth"];
                    var Player2TotalLossesAsFavouriteLastMonth = reader["Player2TotalLossesAsFavouriteLastMonth"];
                    var Player1TotalLossesAsUnderdogLastMonth = reader["Player1TotalLossesAsUnderdogLastMonth"];
                    var Player2TotalLossesAsUnderdogLastMonth = reader["Player2TotalLossesAsUnderdogLastMonth"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth"];
                    var Player1TotalWinsAsFavouriteLastWeek = reader["Player1TotalWinsAsFavouriteLastWeek"];
                    var Player2TotalWinsAsFavouriteLastWeek = reader["Player2TotalWinsAsFavouriteLastWeek"];
                    var Player1TotalWinsAsUnderdogLastWeek = reader["Player1TotalWinsAsUnderdogLastWeek"];
                    var Player2TotalWinsAsUnderdogLastWeek = reader["Player2TotalWinsAsUnderdogLastWeek"];
                    var Player1TotalLossesAsFavouriteLastWeek = reader["Player1TotalLossesAsFavouriteLastWeek"];
                    var Player2TotalLossesAsFavouriteLastWeek = reader["Player2TotalLossesAsFavouriteLastWeek"];
                    var Player1TotalLossesAsUnderdogLastWeek = reader["Player1TotalLossesAsUnderdogLastWeek"];
                    var Player2TotalLossesAsUnderdogLastWeek = reader["Player2TotalLossesAsUnderdogLastWeek"];
                    var Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek"];
                    var Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek"];
                    var Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek"];
                    var Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek"];
                    var Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek"];
                    var Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek"];
                    var Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek"];
                    var Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek"];
                    var Player1H2H = reader["Player1H2H"];
                    var Player2H2H = reader["Player2H2H"];
                    var Player1H2HOld = reader["Player1H2HOld"];
                    var Player2H2HOld = reader["Player2H2HOld"];
                    var Player1H2HTrueSkillMeanM = reader["Player1H2HTrueSkillMeanM"];
                    var Player1H2HTrueSkillStandardDeviationM = reader["Player1H2HTrueSkillStandardDeviationM"];
                    var Player2H2HTrueSkillMeanM = reader["Player2H2HTrueSkillMeanM"];
                    var Player2H2HTrueSkillStandardDeviationM = reader["Player2H2HTrueSkillStandardDeviationM"];
                    var Player1H2HTrueSkillMeanOldM = reader["Player1H2HTrueSkillMeanOldM"];
                    var Player1H2HTrueSkillStandardDeviationOldM = reader["Player1H2HTrueSkillStandardDeviationOldM"];
                    var Player2H2HTrueSkillMeanOldM = reader["Player2H2HTrueSkillMeanOldM"];
                    var Player2H2HTrueSkillStandardDeviationOldM = reader["Player2H2HTrueSkillStandardDeviationOldM"];
                    var WinProbabilityPlayer1H2HM = reader["WinProbabilityPlayer1H2HM"];
                    var Player1H2HTrueSkillMeanSM = reader["Player1H2HTrueSkillMeanSM"];
                    var Player1H2HTrueSkillStandardDeviationSM = reader["Player1H2HTrueSkillStandardDeviationSM"];
                    var Player2H2HTrueSkillMeanSM = reader["Player2H2HTrueSkillMeanSM"];
                    var Player2H2HTrueSkillStandardDeviationSM = reader["Player2H2HTrueSkillStandardDeviationSM"];
                    var Player1H2HTrueSkillMeanOldSM = reader["Player1H2HTrueSkillMeanOldSM"];
                    var Player1H2HTrueSkillStandardDeviationOldSM = reader["Player1H2HTrueSkillStandardDeviationOldSM"];
                    var Player2H2HTrueSkillMeanOldSM = reader["Player2H2HTrueSkillMeanOldSM"];
                    var Player2H2HTrueSkillStandardDeviationOldSM = reader["Player2H2HTrueSkillStandardDeviationOldSM"];
                    var WinProbabilityPlayer1H2HSM = reader["WinProbabilityPlayer1H2HSM"];
                    var Player1H2HTrueSkillMeanGSM = reader["Player1H2HTrueSkillMeanGSM"];
                    var Player1H2HTrueSkillStandardDeviationGSM = reader["Player1H2HTrueSkillStandardDeviationGSM"];
                    var Player2H2HTrueSkillMeanGSM = reader["Player2H2HTrueSkillMeanGSM"];
                    var Player2H2HTrueSkillStandardDeviationGSM = reader["Player2H2HTrueSkillStandardDeviationGSM"];
                    var Player1H2HTrueSkillMeanOldGSM = reader["Player1H2HTrueSkillMeanOldGSM"];
                    var Player1H2HTrueSkillStandardDeviationOldGSM = reader["Player1H2HTrueSkillStandardDeviationOldGSM"];
                    var Player2H2HTrueSkillMeanOldGSM = reader["Player2H2HTrueSkillMeanOldGSM"];
                    var Player2H2HTrueSkillStandardDeviationOldGSM = reader["Player2H2HTrueSkillStandardDeviationOldGSM"];
                    var WinProbabilityPlayer1H2HGSM = reader["WinProbabilityPlayer1H2HGSM"];
                    var Player1H2HS1 = reader["Player1H2HS1"];
                    var Player2H2HS1 = reader["Player2H2HS1"];
                    var Player1H2HOldS1 = reader["Player1H2HOldS1"];
                    var Player2H2HOldS1 = reader["Player2H2HOldS1"];
                    var Player1H2HTrueSkillMeanMS1 = reader["Player1H2HTrueSkillMeanMS1"];
                    var Player1H2HTrueSkillStandardDeviationMS1 = reader["Player1H2HTrueSkillStandardDeviationMS1"];
                    var Player2H2HTrueSkillMeanMS1 = reader["Player2H2HTrueSkillMeanMS1"];
                    var Player2H2HTrueSkillStandardDeviationMS1 = reader["Player2H2HTrueSkillStandardDeviationMS1"];
                    var Player1H2HTrueSkillMeanOldMS1 = reader["Player1H2HTrueSkillMeanOldMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldMS1 = reader["Player1H2HTrueSkillStandardDeviationOldMS1"];
                    var Player2H2HTrueSkillMeanOldMS1 = reader["Player2H2HTrueSkillMeanOldMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldMS1 = reader["Player2H2HTrueSkillStandardDeviationOldMS1"];
                    var WinProbabilityPlayer1H2HMS1 = reader["WinProbabilityPlayer1H2HMS1"];
                    var Player1H2HTrueSkillMeanSMS1 = reader["Player1H2HTrueSkillMeanSMS1"];
                    var Player1H2HTrueSkillStandardDeviationSMS1 = reader["Player1H2HTrueSkillStandardDeviationSMS1"];
                    var Player2H2HTrueSkillMeanSMS1 = reader["Player2H2HTrueSkillMeanSMS1"];
                    var Player2H2HTrueSkillStandardDeviationSMS1 = reader["Player2H2HTrueSkillStandardDeviationSMS1"];
                    var Player1H2HTrueSkillMeanOldSMS1 = reader["Player1H2HTrueSkillMeanOldSMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldSMS1"];
                    var Player2H2HTrueSkillMeanOldSMS1 = reader["Player2H2HTrueSkillMeanOldSMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldSMS1"];
                    var WinProbabilityPlayer1H2HSMS1 = reader["WinProbabilityPlayer1H2HSMS1"];
                    var Player1H2HTrueSkillMeanGSMS1 = reader["Player1H2HTrueSkillMeanGSMS1"];
                    var Player1H2HTrueSkillStandardDeviationGSMS1 = reader["Player1H2HTrueSkillStandardDeviationGSMS1"];
                    var Player2H2HTrueSkillMeanGSMS1 = reader["Player2H2HTrueSkillMeanGSMS1"];
                    var Player2H2HTrueSkillStandardDeviationGSMS1 = reader["Player2H2HTrueSkillStandardDeviationGSMS1"];
                    var Player1H2HTrueSkillMeanOldGSMS1 = reader["Player1H2HTrueSkillMeanOldGSMS1"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS1"];
                    var Player2H2HTrueSkillMeanOldGSMS1 = reader["Player2H2HTrueSkillMeanOldGSMS1"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS1"];
                    var WinProbabilityPlayer1H2HGSMS1 = reader["WinProbabilityPlayer1H2HGSMS1"];
                    var Player1H2HS2 = reader["Player1H2HS2"];
                    var Player2H2HS2 = reader["Player2H2HS2"];
                    var Player1H2HOldS2 = reader["Player1H2HOldS2"];
                    var Player2H2HOldS2 = reader["Player2H2HOldS2"];
                    var Player1H2HTrueSkillMeanMS2 = reader["Player1H2HTrueSkillMeanMS2"];
                    var Player1H2HTrueSkillStandardDeviationMS2 = reader["Player1H2HTrueSkillStandardDeviationMS2"];
                    var Player2H2HTrueSkillMeanMS2 = reader["Player2H2HTrueSkillMeanMS2"];
                    var Player2H2HTrueSkillStandardDeviationMS2 = reader["Player2H2HTrueSkillStandardDeviationMS2"];
                    var Player1H2HTrueSkillMeanOldMS2 = reader["Player1H2HTrueSkillMeanOldMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldMS2 = reader["Player1H2HTrueSkillStandardDeviationOldMS2"];
                    var Player2H2HTrueSkillMeanOldMS2 = reader["Player2H2HTrueSkillMeanOldMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldMS2 = reader["Player2H2HTrueSkillStandardDeviationOldMS2"];
                    var WinProbabilityPlayer1H2HMS2 = reader["WinProbabilityPlayer1H2HMS2"];
                    var Player1H2HTrueSkillMeanSMS2 = reader["Player1H2HTrueSkillMeanSMS2"];
                    var Player1H2HTrueSkillStandardDeviationSMS2 = reader["Player1H2HTrueSkillStandardDeviationSMS2"];
                    var Player2H2HTrueSkillMeanSMS2 = reader["Player2H2HTrueSkillMeanSMS2"];
                    var Player2H2HTrueSkillStandardDeviationSMS2 = reader["Player2H2HTrueSkillStandardDeviationSMS2"];
                    var Player1H2HTrueSkillMeanOldSMS2 = reader["Player1H2HTrueSkillMeanOldSMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldSMS2"];
                    var Player2H2HTrueSkillMeanOldSMS2 = reader["Player2H2HTrueSkillMeanOldSMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldSMS2"];
                    var WinProbabilityPlayer1H2HSMS2 = reader["WinProbabilityPlayer1H2HSMS2"];
                    var Player1H2HTrueSkillMeanGSMS2 = reader["Player1H2HTrueSkillMeanGSMS2"];
                    var Player1H2HTrueSkillStandardDeviationGSMS2 = reader["Player1H2HTrueSkillStandardDeviationGSMS2"];
                    var Player2H2HTrueSkillMeanGSMS2 = reader["Player2H2HTrueSkillMeanGSMS2"];
                    var Player2H2HTrueSkillStandardDeviationGSMS2 = reader["Player2H2HTrueSkillStandardDeviationGSMS2"];
                    var Player1H2HTrueSkillMeanOldGSMS2 = reader["Player1H2HTrueSkillMeanOldGSMS2"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS2"];
                    var Player2H2HTrueSkillMeanOldGSMS2 = reader["Player2H2HTrueSkillMeanOldGSMS2"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS2"];
                    var WinProbabilityPlayer1H2HGSMS2 = reader["WinProbabilityPlayer1H2HGSMS2"];
                    var Player1H2HS3 = reader["Player1H2HS3"];
                    var Player2H2HS3 = reader["Player2H2HS3"];
                    var Player1H2HOldS3 = reader["Player1H2HOldS3"];
                    var Player2H2HOldS3 = reader["Player2H2HOldS3"];
                    var Player1H2HTrueSkillMeanMS3 = reader["Player1H2HTrueSkillMeanMS3"];
                    var Player1H2HTrueSkillStandardDeviationMS3 = reader["Player1H2HTrueSkillStandardDeviationMS3"];
                    var Player2H2HTrueSkillMeanMS3 = reader["Player2H2HTrueSkillMeanMS3"];
                    var Player2H2HTrueSkillStandardDeviationMS3 = reader["Player2H2HTrueSkillStandardDeviationMS3"];
                    var Player1H2HTrueSkillMeanOldMS3 = reader["Player1H2HTrueSkillMeanOldMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldMS3 = reader["Player1H2HTrueSkillStandardDeviationOldMS3"];
                    var Player2H2HTrueSkillMeanOldMS3 = reader["Player2H2HTrueSkillMeanOldMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldMS3 = reader["Player2H2HTrueSkillStandardDeviationOldMS3"];
                    var WinProbabilityPlayer1H2HMS3 = reader["WinProbabilityPlayer1H2HMS3"];
                    var Player1H2HTrueSkillMeanSMS3 = reader["Player1H2HTrueSkillMeanSMS3"];
                    var Player1H2HTrueSkillStandardDeviationSMS3 = reader["Player1H2HTrueSkillStandardDeviationSMS3"];
                    var Player2H2HTrueSkillMeanSMS3 = reader["Player2H2HTrueSkillMeanSMS3"];
                    var Player2H2HTrueSkillStandardDeviationSMS3 = reader["Player2H2HTrueSkillStandardDeviationSMS3"];
                    var Player1H2HTrueSkillMeanOldSMS3 = reader["Player1H2HTrueSkillMeanOldSMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldSMS3"];
                    var Player2H2HTrueSkillMeanOldSMS3 = reader["Player2H2HTrueSkillMeanOldSMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldSMS3"];
                    var WinProbabilityPlayer1H2HSMS3 = reader["WinProbabilityPlayer1H2HSMS3"];
                    var Player1H2HTrueSkillMeanGSMS3 = reader["Player1H2HTrueSkillMeanGSMS3"];
                    var Player1H2HTrueSkillStandardDeviationGSMS3 = reader["Player1H2HTrueSkillStandardDeviationGSMS3"];
                    var Player2H2HTrueSkillMeanGSMS3 = reader["Player2H2HTrueSkillMeanGSMS3"];
                    var Player2H2HTrueSkillStandardDeviationGSMS3 = reader["Player2H2HTrueSkillStandardDeviationGSMS3"];
                    var Player1H2HTrueSkillMeanOldGSMS3 = reader["Player1H2HTrueSkillMeanOldGSMS3"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS3"];
                    var Player2H2HTrueSkillMeanOldGSMS3 = reader["Player2H2HTrueSkillMeanOldGSMS3"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS3"];
                    var WinProbabilityPlayer1H2HGSMS3 = reader["WinProbabilityPlayer1H2HGSMS3"];
                    var Player1H2HS4 = reader["Player1H2HS4"];
                    var Player2H2HS4 = reader["Player2H2HS4"];
                    var Player1H2HOldS4 = reader["Player1H2HOldS4"];
                    var Player2H2HOldS4 = reader["Player2H2HOldS4"];
                    var Player1H2HTrueSkillMeanMS4 = reader["Player1H2HTrueSkillMeanMS4"];
                    var Player1H2HTrueSkillStandardDeviationMS4 = reader["Player1H2HTrueSkillStandardDeviationMS4"];
                    var Player2H2HTrueSkillMeanMS4 = reader["Player2H2HTrueSkillMeanMS4"];
                    var Player2H2HTrueSkillStandardDeviationMS4 = reader["Player2H2HTrueSkillStandardDeviationMS4"];
                    var Player1H2HTrueSkillMeanOldMS4 = reader["Player1H2HTrueSkillMeanOldMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldMS4 = reader["Player1H2HTrueSkillStandardDeviationOldMS4"];
                    var Player2H2HTrueSkillMeanOldMS4 = reader["Player2H2HTrueSkillMeanOldMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldMS4 = reader["Player2H2HTrueSkillStandardDeviationOldMS4"];
                    var WinProbabilityPlayer1H2HMS4 = reader["WinProbabilityPlayer1H2HMS4"];
                    var Player1H2HTrueSkillMeanSMS4 = reader["Player1H2HTrueSkillMeanSMS4"];
                    var Player1H2HTrueSkillStandardDeviationSMS4 = reader["Player1H2HTrueSkillStandardDeviationSMS4"];
                    var Player2H2HTrueSkillMeanSMS4 = reader["Player2H2HTrueSkillMeanSMS4"];
                    var Player2H2HTrueSkillStandardDeviationSMS4 = reader["Player2H2HTrueSkillStandardDeviationSMS4"];
                    var Player1H2HTrueSkillMeanOldSMS4 = reader["Player1H2HTrueSkillMeanOldSMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldSMS4"];
                    var Player2H2HTrueSkillMeanOldSMS4 = reader["Player2H2HTrueSkillMeanOldSMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldSMS4"];
                    var WinProbabilityPlayer1H2HSMS4 = reader["WinProbabilityPlayer1H2HSMS4"];
                    var Player1H2HTrueSkillMeanGSMS4 = reader["Player1H2HTrueSkillMeanGSMS4"];
                    var Player1H2HTrueSkillStandardDeviationGSMS4 = reader["Player1H2HTrueSkillStandardDeviationGSMS4"];
                    var Player2H2HTrueSkillMeanGSMS4 = reader["Player2H2HTrueSkillMeanGSMS4"];
                    var Player2H2HTrueSkillStandardDeviationGSMS4 = reader["Player2H2HTrueSkillStandardDeviationGSMS4"];
                    var Player1H2HTrueSkillMeanOldGSMS4 = reader["Player1H2HTrueSkillMeanOldGSMS4"];
                    var Player1H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS4"];
                    var Player2H2HTrueSkillMeanOldGSMS4 = reader["Player2H2HTrueSkillMeanOldGSMS4"];
                    var Player2H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS4"];
                    var WinProbabilityPlayer1H2HGSMS4 = reader["WinProbabilityPlayer1H2HGSMS4"];
                    var Player1Streak = reader["Player1Streak"];
                    var Player2Streak = reader["Player2Streak"];
                    var Player1StreakS1 = reader["Player1StreakS1"];
                    var Player2StreakS1 = reader["Player2StreakS1"];
                    var Player1StreakS2 = reader["Player1StreakS2"];
                    var Player2StreakS2 = reader["Player2StreakS2"];
                    var Player1StreakS3 = reader["Player1StreakS3"];
                    var Player2StreakS3 = reader["Player2StreakS3"];
                    var Player1StreakS4 = reader["Player1StreakS4"];
                    var Player2StreakS4 = reader["Player2StreakS4"];
                    var P1SetsWon = reader["P1SetsWon"];
                    var P2SetsWon = reader["P2SetsWon"];
                    var P1GamesWon = reader["P1GamesWon"];
                    var P2GamesWon = reader["P2GamesWon"];
                    var P1SetsLoss = reader["P1SetsLoss"];
                    var P2SetsLoss = reader["P2SetsLoss"];
                    var P1GamesLoss = reader["P1GamesLoss"];
                    var P2GamesLoss = reader["P2GamesLoss"];
                    var WinProbabilityNN = reader["WinProbabilityNN"];
                    var IsFinished = 1; // dodati nakon dodavanja atrbuta u bazu reader["IsFinished"];
                }
                catch (Exception ex)
                {
                    string aaa;
                    aaa = "";
                }

                var Match = new Match
                {
                    MatchTPId = reader["MatchTPId"] as int?,
                    TournamentEventTPId = reader["TournamentEventTPId"] as int?,
                    DateTime = reader["DateTime"] as DateTime?,
                    Player1TPId = reader["Player1TPId"] as int?,
                    Player2TPId = reader["Player2TPId"] as int?,
                    Player1Seed = reader["Player1Seed"] as string,
                    Player2Seed = reader["Player2Seed"] as string,
                    Result = reader["Result"] as string,
                    ResultDetails = reader["ResultDetails"] as string,
                    Player1Odds = reader["Player1Odds"] as double?,
                    Player2Odds = reader["Player2Odds"] as double?,
                    Player1Percentage = reader["Player1Percentage"] as double?,
                    Player2Percentage = reader["Player2Percentage"] as double?,
                    SurfaceId = reader["SurfaceId"] as int?,
                    RoundId = reader["RoundId"] as int?,
                    Player1TrueSkillMeanM = reader["Player1TrueSkillMeanM"] as double?,
                    Player1TrueSkillStandardDeviationM = reader["Player1TrueSkillStandardDeviationM"] as double?,
                    Player2TrueSkillMeanM = reader["Player2TrueSkillMeanM"] as double?,
                    Player2TrueSkillStandardDeviationM = reader["Player2TrueSkillStandardDeviationM"] as double?,
                    Player1TrueSkillMeanOldM = reader["Player1TrueSkillMeanOldM"] as double?,
                    Player1TrueSkillStandardDeviationOldM = reader["Player1TrueSkillStandardDeviationOldM"] as double?,
                    Player2TrueSkillMeanOldM = reader["Player2TrueSkillMeanOldM"] as double?,
                    Player2TrueSkillStandardDeviationOldM = reader["Player2TrueSkillStandardDeviationOldM"] as double?,
                    WinProbabilityPlayer1M = reader["WinProbabilityPlayer1M"] as double?,
                    Player1TrueSkillMeanSM = reader["Player1TrueSkillMeanSM"] as double?,
                    Player1TrueSkillStandardDeviationSM = reader["Player1TrueSkillStandardDeviationSM"] as double?,
                    Player2TrueSkillMeanSM = reader["Player2TrueSkillMeanSM"] as double?,
                    Player2TrueSkillStandardDeviationSM = reader["Player2TrueSkillStandardDeviationSM"] as double?,
                    Player1TrueSkillMeanOldSM = reader["Player1TrueSkillMeanOldSM"] as double?,
                    Player1TrueSkillStandardDeviationOldSM = reader["Player1TrueSkillStandardDeviationOldSM"] as double?,
                    Player2TrueSkillMeanOldSM = reader["Player2TrueSkillMeanOldSM"] as double?,
                    Player2TrueSkillStandardDeviationOldSM = reader["Player2TrueSkillStandardDeviationOldSM"] as double?,
                    WinProbabilityPlayer1SM = reader["WinProbabilityPlayer1SM"] as double?,
                    Player1TrueSkillMeanGSM = reader["Player1TrueSkillMeanGSM"] as double?,
                    Player1TrueSkillStandardDeviationGSM = reader["Player1TrueSkillStandardDeviationGSM"] as double?,
                    Player2TrueSkillMeanGSM = reader["Player2TrueSkillMeanGSM"] as double?,
                    Player2TrueSkillStandardDeviationGSM = reader["Player2TrueSkillStandardDeviationGSM"] as double?,
                    Player1TrueSkillMeanOldGSM = reader["Player1TrueSkillMeanOldGSM"] as double?,
                    Player1TrueSkillStandardDeviationOldGSM = reader["Player1TrueSkillStandardDeviationOldGSM"] as double?,
                    Player2TrueSkillMeanOldGSM = reader["Player2TrueSkillMeanOldGSM"] as double?,
                    Player2TrueSkillStandardDeviationOldGSM = reader["Player2TrueSkillStandardDeviationOldGSM"] as double?,
                    WinProbabilityPlayer1GSM = reader["WinProbabilityPlayer1GSM"] as double?,
                    Player1TrueSkillMeanMS1 = reader["Player1TrueSkillMeanMS1"] as double?,
                    Player1TrueSkillStandardDeviationMS1 = reader["Player1TrueSkillStandardDeviationMS1"] as double?,
                    Player2TrueSkillMeanMS1 = reader["Player2TrueSkillMeanMS1"] as double?,
                    Player2TrueSkillStandardDeviationMS1 = reader["Player2TrueSkillStandardDeviationMS1"] as double?,
                    Player1TrueSkillMeanOldMS1 = reader["Player1TrueSkillMeanOldMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldMS1 = reader["Player1TrueSkillStandardDeviationOldMS1"] as double?,
                    Player2TrueSkillMeanOldMS1 = reader["Player2TrueSkillMeanOldMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldMS1 = reader["Player2TrueSkillStandardDeviationOldMS1"] as double?,
                    WinProbabilityPlayer1MS1 = reader["WinProbabilityPlayer1MS1"] as double?,
                    Player1TrueSkillMeanSMS1 = reader["Player1TrueSkillMeanSMS1"] as double?,
                    Player1TrueSkillStandardDeviationSMS1 = reader["Player1TrueSkillStandardDeviationSMS1"] as double?,
                    Player2TrueSkillMeanSMS1 = reader["Player2TrueSkillMeanSMS1"] as double?,
                    Player2TrueSkillStandardDeviationSMS1 = reader["Player2TrueSkillStandardDeviationSMS1"] as double?,
                    Player1TrueSkillMeanOldSMS1 = reader["Player1TrueSkillMeanOldSMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS1 = reader["Player1TrueSkillStandardDeviationOldSMS1"] as double?,
                    Player2TrueSkillMeanOldSMS1 = reader["Player2TrueSkillMeanOldSMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS1 = reader["Player2TrueSkillStandardDeviationOldSMS1"] as double?,
                    WinProbabilityPlayer1SMS1 = reader["WinProbabilityPlayer1SMS1"] as double?,
                    Player1TrueSkillMeanGSMS1 = reader["Player1TrueSkillMeanGSMS1"] as double?,
                    Player1TrueSkillStandardDeviationGSMS1 = reader["Player1TrueSkillStandardDeviationGSMS1"] as double?,
                    Player2TrueSkillMeanGSMS1 = reader["Player2TrueSkillMeanGSMS1"] as double?,
                    Player2TrueSkillStandardDeviationGSMS1 = reader["Player2TrueSkillStandardDeviationGSMS1"] as double?,
                    Player1TrueSkillMeanOldGSMS1 = reader["Player1TrueSkillMeanOldGSMS1"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS1 = reader["Player1TrueSkillStandardDeviationOldGSMS1"] as double?,
                    Player2TrueSkillMeanOldGSMS1 = reader["Player2TrueSkillMeanOldGSMS1"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS1 = reader["Player2TrueSkillStandardDeviationOldGSMS1"] as double?,
                    WinProbabilityPlayer1GSMS1 = reader["WinProbabilityPlayer1GSMS1"] as double?,
                    Player1TrueSkillMeanMS2 = reader["Player1TrueSkillMeanMS2"] as double?,
                    Player1TrueSkillStandardDeviationMS2 = reader["Player1TrueSkillStandardDeviationMS2"] as double?,
                    Player2TrueSkillMeanMS2 = reader["Player2TrueSkillMeanMS2"] as double?,
                    Player2TrueSkillStandardDeviationMS2 = reader["Player2TrueSkillStandardDeviationMS2"] as double?,
                    Player1TrueSkillMeanOldMS2 = reader["Player1TrueSkillMeanOldMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldMS2 = reader["Player1TrueSkillStandardDeviationOldMS2"] as double?,
                    Player2TrueSkillMeanOldMS2 = reader["Player2TrueSkillMeanOldMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldMS2 = reader["Player2TrueSkillStandardDeviationOldMS2"] as double?,
                    WinProbabilityPlayer1MS2 = reader["WinProbabilityPlayer1MS2"] as double?,
                    Player1TrueSkillMeanSMS2 = reader["Player1TrueSkillMeanSMS2"] as double?,
                    Player1TrueSkillStandardDeviationSMS2 = reader["Player1TrueSkillStandardDeviationSMS2"] as double?,
                    Player2TrueSkillMeanSMS2 = reader["Player2TrueSkillMeanSMS2"] as double?,
                    Player2TrueSkillStandardDeviationSMS2 = reader["Player2TrueSkillStandardDeviationSMS2"] as double?,
                    Player1TrueSkillMeanOldSMS2 = reader["Player1TrueSkillMeanOldSMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS2 = reader["Player1TrueSkillStandardDeviationOldSMS2"] as double?,
                    Player2TrueSkillMeanOldSMS2 = reader["Player2TrueSkillMeanOldSMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS2 = reader["Player2TrueSkillStandardDeviationOldSMS2"] as double?,
                    WinProbabilityPlayer1SMS2 = reader["WinProbabilityPlayer1SMS2"] as double?,
                    Player1TrueSkillMeanGSMS2 = reader["Player1TrueSkillMeanGSMS2"] as double?,
                    Player1TrueSkillStandardDeviationGSMS2 = reader["Player1TrueSkillStandardDeviationGSMS2"] as double?,
                    Player2TrueSkillMeanGSMS2 = reader["Player2TrueSkillMeanGSMS2"] as double?,
                    Player2TrueSkillStandardDeviationGSMS2 = reader["Player2TrueSkillStandardDeviationGSMS2"] as double?,
                    Player1TrueSkillMeanOldGSMS2 = reader["Player1TrueSkillMeanOldGSMS2"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS2 = reader["Player1TrueSkillStandardDeviationOldGSMS2"] as double?,
                    Player2TrueSkillMeanOldGSMS2 = reader["Player2TrueSkillMeanOldGSMS2"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS2 = reader["Player2TrueSkillStandardDeviationOldGSMS2"] as double?,
                    WinProbabilityPlayer1GSMS2 = reader["WinProbabilityPlayer1GSMS2"] as double?,
                    Player1TrueSkillMeanMS3 = reader["Player1TrueSkillMeanMS3"] as double?,
                    Player1TrueSkillStandardDeviationMS3 = reader["Player1TrueSkillStandardDeviationMS3"] as double?,
                    Player2TrueSkillMeanMS3 = reader["Player2TrueSkillMeanMS3"] as double?,
                    Player2TrueSkillStandardDeviationMS3 = reader["Player2TrueSkillStandardDeviationMS3"] as double?,
                    Player1TrueSkillMeanOldMS3 = reader["Player1TrueSkillMeanOldMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldMS3 = reader["Player1TrueSkillStandardDeviationOldMS3"] as double?,
                    Player2TrueSkillMeanOldMS3 = reader["Player2TrueSkillMeanOldMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldMS3 = reader["Player2TrueSkillStandardDeviationOldMS3"] as double?,
                    WinProbabilityPlayer1MS3 = reader["WinProbabilityPlayer1MS3"] as double?,
                    Player1TrueSkillMeanSMS3 = reader["Player1TrueSkillMeanSMS3"] as double?,
                    Player1TrueSkillStandardDeviationSMS3 = reader["Player1TrueSkillStandardDeviationSMS3"] as double?,
                    Player2TrueSkillMeanSMS3 = reader["Player2TrueSkillMeanSMS3"] as double?,
                    Player2TrueSkillStandardDeviationSMS3 = reader["Player2TrueSkillStandardDeviationSMS3"] as double?,
                    Player1TrueSkillMeanOldSMS3 = reader["Player1TrueSkillMeanOldSMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS3 = reader["Player1TrueSkillStandardDeviationOldSMS3"] as double?,
                    Player2TrueSkillMeanOldSMS3 = reader["Player2TrueSkillMeanOldSMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS3 = reader["Player2TrueSkillStandardDeviationOldSMS3"] as double?,
                    WinProbabilityPlayer1SMS3 = reader["WinProbabilityPlayer1SMS3"] as double?,
                    Player1TrueSkillMeanGSMS3 = reader["Player1TrueSkillMeanGSMS3"] as double?,
                    Player1TrueSkillStandardDeviationGSMS3 = reader["Player1TrueSkillStandardDeviationGSMS3"] as double?,
                    Player2TrueSkillMeanGSMS3 = reader["Player2TrueSkillMeanGSMS3"] as double?,
                    Player2TrueSkillStandardDeviationGSMS3 = reader["Player2TrueSkillStandardDeviationGSMS3"] as double?,
                    Player1TrueSkillMeanOldGSMS3 = reader["Player1TrueSkillMeanOldGSMS3"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS3 = reader["Player1TrueSkillStandardDeviationOldGSMS3"] as double?,
                    Player2TrueSkillMeanOldGSMS3 = reader["Player2TrueSkillMeanOldGSMS3"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS3 = reader["Player2TrueSkillStandardDeviationOldGSMS3"] as double?,
                    WinProbabilityPlayer1GSMS3 = reader["WinProbabilityPlayer1GSMS3"] as double?,
                    Player1TrueSkillMeanMS4 = reader["Player1TrueSkillMeanMS4"] as double?,
                    Player1TrueSkillStandardDeviationMS4 = reader["Player1TrueSkillStandardDeviationMS4"] as double?,
                    Player2TrueSkillMeanMS4 = reader["Player2TrueSkillMeanMS4"] as double?,
                    Player2TrueSkillStandardDeviationMS4 = reader["Player2TrueSkillStandardDeviationMS4"] as double?,
                    Player1TrueSkillMeanOldMS4 = reader["Player1TrueSkillMeanOldMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldMS4 = reader["Player1TrueSkillStandardDeviationOldMS4"] as double?,
                    Player2TrueSkillMeanOldMS4 = reader["Player2TrueSkillMeanOldMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldMS4 = reader["Player2TrueSkillStandardDeviationOldMS4"] as double?,
                    WinProbabilityPlayer1MS4 = reader["WinProbabilityPlayer1MS4"] as double?,
                    Player1TrueSkillMeanSMS4 = reader["Player1TrueSkillMeanSMS4"] as double?,
                    Player1TrueSkillStandardDeviationSMS4 = reader["Player1TrueSkillStandardDeviationSMS4"] as double?,
                    Player2TrueSkillMeanSMS4 = reader["Player2TrueSkillMeanSMS4"] as double?,
                    Player2TrueSkillStandardDeviationSMS4 = reader["Player2TrueSkillStandardDeviationSMS4"] as double?,
                    Player1TrueSkillMeanOldSMS4 = reader["Player1TrueSkillMeanOldSMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldSMS4 = reader["Player1TrueSkillStandardDeviationOldSMS4"] as double?,
                    Player2TrueSkillMeanOldSMS4 = reader["Player2TrueSkillMeanOldSMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldSMS4 = reader["Player2TrueSkillStandardDeviationOldSMS4"] as double?,
                    WinProbabilityPlayer1SMS4 = reader["WinProbabilityPlayer1SMS4"] as double?,
                    Player1TrueSkillMeanGSMS4 = reader["Player1TrueSkillMeanGSMS4"] as double?,
                    Player1TrueSkillStandardDeviationGSMS4 = reader["Player1TrueSkillStandardDeviationGSMS4"] as double?,
                    Player2TrueSkillMeanGSMS4 = reader["Player2TrueSkillMeanGSMS4"] as double?,
                    Player2TrueSkillStandardDeviationGSMS4 = reader["Player2TrueSkillStandardDeviationGSMS4"] as double?,
                    Player1TrueSkillMeanOldGSMS4 = reader["Player1TrueSkillMeanOldGSMS4"] as double?,
                    Player1TrueSkillStandardDeviationOldGSMS4 = reader["Player1TrueSkillStandardDeviationOldGSMS4"] as double?,
                    Player2TrueSkillMeanOldGSMS4 = reader["Player2TrueSkillMeanOldGSMS4"] as double?,
                    Player2TrueSkillStandardDeviationOldGSMS4 = reader["Player2TrueSkillStandardDeviationOldGSMS4"] as double?,
                    WinProbabilityPlayer1GSMS4 = reader["WinProbabilityPlayer1GSMS4"] as double?,
                    Player1WinsTotal = reader["Player1WinsTotal"] as int?,
                    Player1LossesTotal = reader["Player1LossesTotal"] as int?,
                    Player1WinsLastYear = reader["Player1WinsLastYear"] as int?,
                    Player1LossesLastYear = reader["Player1LossesLastYear"] as int?,
                    Player1WinsLastMonth = reader["Player1WinsLastMonth"] as int?,
                    Player1LossesLastMonth = reader["Player1LossesLastMonth"] as int?,
                    Player1WinsLastWeek = reader["Player1WinsLastWeek"] as int?,
                    Player1LossesLastWeek = reader["Player1LossesLastWeek"] as int?,
                    Player2WinsTotal = reader["Player2WinsTotal"] as int?,
                    Player2LossesTotal = reader["Player2LossesTotal"] as int?,
                    Player2WinsLastYear = reader["Player2WinsLastYear"] as int?,
                    Player2LossesLastYear = reader["Player2LossesLastYear"] as int?,
                    Player2WinsLastMonth = reader["Player2WinsLastMonth"] as int?,
                    Player2LossesLastMonth = reader["Player2LossesLastMonth"] as int?,
                    Player2WinsLastWeek = reader["Player2WinsLastWeek"] as int?,
                    Player2LossesLastWeek = reader["Player2LossesLastWeek"] as int?,
                    Player1WinsTotalS1 = reader["Player1WinsTotalS1"] as int?,
                    Player1LossesTotalS1 = reader["Player1LossesTotalS1"] as int?,
                    Player1WinsLastYearS1 = reader["Player1WinsLastYearS1"] as int?,
                    Player1LossesLastYearS1 = reader["Player1LossesLastYearS1"] as int?,
                    Player1WinsLastMonthS1 = reader["Player1WinsLastMonthS1"] as int?,
                    Player1LossesLastMonthS1 = reader["Player1LossesLastMonthS1"] as int?,
                    Player1WinsLastWeekS1 = reader["Player1WinsLastWeekS1"] as int?,
                    Player1LossesLastWeekS1 = reader["Player1LossesLastWeekS1"] as int?,
                    Player2WinsTotalS1 = reader["Player2WinsTotalS1"] as int?,
                    Player2LossesTotalS1 = reader["Player2LossesTotalS1"] as int?,
                    Player2WinsLastYearS1 = reader["Player2WinsLastYearS1"] as int?,
                    Player2LossesLastYearS1 = reader["Player2LossesLastYearS1"] as int?,
                    Player2WinsLastMonthS1 = reader["Player2WinsLastMonthS1"] as int?,
                    Player2LossesLastMonthS1 = reader["Player2LossesLastMonthS1"] as int?,
                    Player2WinsLastWeekS1 = reader["Player2WinsLastWeekS1"] as int?,
                    Player2LossesLastWeekS1 = reader["Player2LossesLastWeekS1"] as int?,
                    Player1WinsTotalS2 = reader["Player1WinsTotalS2"] as int?,
                    Player1LossesTotalS2 = reader["Player1LossesTotalS2"] as int?,
                    Player1WinsLastYearS2 = reader["Player1WinsLastYearS2"] as int?,
                    Player1LossesLastYearS2 = reader["Player1LossesLastYearS2"] as int?,
                    Player1WinsLastMonthS2 = reader["Player1WinsLastMonthS2"] as int?,
                    Player1LossesLastMonthS2 = reader["Player1LossesLastMonthS2"] as int?,
                    Player1WinsLastWeekS2 = reader["Player1WinsLastWeekS2"] as int?,
                    Player1LossesLastWeekS2 = reader["Player1LossesLastWeekS2"] as int?,
                    Player2WinsTotalS2 = reader["Player2WinsTotalS2"] as int?,
                    Player2LossesTotalS2 = reader["Player2LossesTotalS2"] as int?,
                    Player2WinsLastYearS2 = reader["Player2WinsLastYearS2"] as int?,
                    Player2LossesLastYearS2 = reader["Player2LossesLastYearS2"] as int?,
                    Player2WinsLastMonthS2 = reader["Player2WinsLastMonthS2"] as int?,
                    Player2LossesLastMonthS2 = reader["Player2LossesLastMonthS2"] as int?,
                    Player2WinsLastWeekS2 = reader["Player2WinsLastWeekS2"] as int?,
                    Player2LossesLastWeekS2 = reader["Player2LossesLastWeekS2"] as int?,
                    Player1WinsTotalS3 = reader["Player1WinsTotalS3"] as int?,
                    Player1LossesTotalS3 = reader["Player1LossesTotalS3"] as int?,
                    Player1WinsLastYearS3 = reader["Player1WinsLastYearS3"] as int?,
                    Player1LossesLastYearS3 = reader["Player1LossesLastYearS3"] as int?,
                    Player1WinsLastMonthS3 = reader["Player1WinsLastMonthS3"] as int?,
                    Player1LossesLastMonthS3 = reader["Player1LossesLastMonthS3"] as int?,
                    Player1WinsLastWeekS3 = reader["Player1WinsLastWeekS3"] as int?,
                    Player1LossesLastWeekS3 = reader["Player1LossesLastWeekS3"] as int?,
                    Player2WinsTotalS3 = reader["Player2WinsTotalS3"] as int?,
                    Player2LossesTotalS3 = reader["Player2LossesTotalS3"] as int?,
                    Player2WinsLastYearS3 = reader["Player2WinsLastYearS3"] as int?,
                    Player2LossesLastYearS3 = reader["Player2LossesLastYearS3"] as int?,
                    Player2WinsLastMonthS3 = reader["Player2WinsLastMonthS3"] as int?,
                    Player2LossesLastMonthS3 = reader["Player2LossesLastMonthS3"] as int?,
                    Player2WinsLastWeekS3 = reader["Player2WinsLastWeekS3"] as int?,
                    Player2LossesLastWeekS3 = reader["Player2LossesLastWeekS3"] as int?,
                    Player1WinsTotalS4 = reader["Player1WinsTotalS4"] as int?,
                    Player1LossesTotalS4 = reader["Player1LossesTotalS4"] as int?,
                    Player1WinsLastYearS4 = reader["Player1WinsLastYearS4"] as int?,
                    Player1LossesLastYearS4 = reader["Player1LossesLastYearS4"] as int?,
                    Player1WinsLastMonthS4 = reader["Player1WinsLastMonthS4"] as int?,
                    Player1LossesLastMonthS4 = reader["Player1LossesLastMonthS4"] as int?,
                    Player1WinsLastWeekS4 = reader["Player1WinsLastWeekS4"] as int?,
                    Player1LossesLastWeekS4 = reader["Player1LossesLastWeekS4"] as int?,
                    Player2WinsTotalS4 = reader["Player2WinsTotalS4"] as int?,
                    Player2LossesTotalS4 = reader["Player2LossesTotalS4"] as int?,
                    Player2WinsLastYearS4 = reader["Player2WinsLastYearS4"] as int?,
                    Player2LossesLastYearS4 = reader["Player2LossesLastYearS4"] as int?,
                    Player2WinsLastMonthS4 = reader["Player2WinsLastMonthS4"] as int?,
                    Player2LossesLastMonthS4 = reader["Player2LossesLastMonthS4"] as int?,
                    Player2WinsLastWeekS4 = reader["Player2WinsLastWeekS4"] as int?,
                    Player2LossesLastWeekS4 = reader["Player2LossesLastWeekS4"] as int?,
                    Player1WinsSetsTotal = reader["Player1WinsSetsTotal"] as int?,
                    Player1LossesSetsTotal = reader["Player1LossesSetsTotal"] as int?,
                    Player1WinsSetsLastYear = reader["Player1WinsSetsLastYear"] as int?,
                    Player1LossesSetsLastYear = reader["Player1LossesSetsLastYear"] as int?,
                    Player1WinsSetsLastMonth = reader["Player1WinsSetsLastMonth"] as int?,
                    Player1LossesSetsLastMonth = reader["Player1LossesSetsLastMonth"] as int?,
                    Player1WinsSetsLastWeek = reader["Player1WinsSetsLastWeek"] as int?,
                    Player1LossesSetsLastWeek = reader["Player1LossesSetsLastWeek"] as int?,
                    Player2WinsSetsTotal = reader["Player2WinsSetsTotal"] as int?,
                    Player2LossesSetsTotal = reader["Player2LossesSetsTotal"] as int?,
                    Player2WinsSetsLastYear = reader["Player2WinsSetsLastYear"] as int?,
                    Player2LossesSetsLastYear = reader["Player2LossesSetsLastYear"] as int?,
                    Player2WinsSetsLastMonth = reader["Player2WinsSetsLastMonth"] as int?,
                    Player2LossesSetsLastMonth = reader["Player2LossesSetsLastMonth"] as int?,
                    Player2WinsSetsLastWeek = reader["Player2WinsSetsLastWeek"] as int?,
                    Player2LossesSetsLastWeek = reader["Player2LossesSetsLastWeek"] as int?,
                    Player1WinsSetsTotalS1 = reader["Player1WinsSetsTotalS1"] as int?,
                    Player1LossesSetsTotalS1 = reader["Player1LossesSetsTotalS1"] as int?,
                    Player1WinsSetsLastYearS1 = reader["Player1WinsSetsLastYearS1"] as int?,
                    Player1LossesSetsLastYearS1 = reader["Player1LossesSetsLastYearS1"] as int?,
                    Player1WinsSetsLastMonthS1 = reader["Player1WinsSetsLastMonthS1"] as int?,
                    Player1LossesSetsLastMonthS1 = reader["Player1LossesSetsLastMonthS1"] as int?,
                    Player1WinsSetsLastWeekS1 = reader["Player1WinsSetsLastWeekS1"] as int?,
                    Player1LossesSetsLastWeekS1 = reader["Player1LossesSetsLastWeekS1"] as int?,
                    Player2WinsSetsTotalS1 = reader["Player2WinsSetsTotalS1"] as int?,
                    Player2LossesSetsTotalS1 = reader["Player2LossesSetsTotalS1"] as int?,
                    Player2WinsSetsLastYearS1 = reader["Player2WinsSetsLastYearS1"] as int?,
                    Player2LossesSetsLastYearS1 = reader["Player2LossesSetsLastYearS1"] as int?,
                    Player2WinsSetsLastMonthS1 = reader["Player2WinsSetsLastMonthS1"] as int?,
                    Player2LossesSetsLastMonthS1 = reader["Player2LossesSetsLastMonthS1"] as int?,
                    Player2WinsSetsLastWeekS1 = reader["Player2WinsSetsLastWeekS1"] as int?,
                    Player2LossesSetsLastWeekS1 = reader["Player2LossesSetsLastWeekS1"] as int?,
                    Player1WinsSetsTotalS2 = reader["Player1WinsSetsTotalS2"] as int?,
                    Player1LossesSetsTotalS2 = reader["Player1LossesSetsTotalS2"] as int?,
                    Player1WinsSetsLastYearS2 = reader["Player1WinsSetsLastYearS2"] as int?,
                    Player1LossesSetsLastYearS2 = reader["Player1LossesSetsLastYearS2"] as int?,
                    Player1WinsSetsLastMonthS2 = reader["Player1WinsSetsLastMonthS2"] as int?,
                    Player1LossesSetsLastMonthS2 = reader["Player1LossesSetsLastMonthS2"] as int?,
                    Player1WinsSetsLastWeekS2 = reader["Player1WinsSetsLastWeekS2"] as int?,
                    Player1LossesSetsLastWeekS2 = reader["Player1LossesSetsLastWeekS2"] as int?,
                    Player2WinsSetsTotalS2 = reader["Player2WinsSetsTotalS2"] as int?,
                    Player2LossesSetsTotalS2 = reader["Player2LossesSetsTotalS2"] as int?,
                    Player2WinsSetsLastYearS2 = reader["Player2WinsSetsLastYearS2"] as int?,
                    Player2LossesSetsLastYearS2 = reader["Player2LossesSetsLastYearS2"] as int?,
                    Player2WinsSetsLastMonthS2 = reader["Player2WinsSetsLastMonthS2"] as int?,
                    Player2LossesSetsLastMonthS2 = reader["Player2LossesSetsLastMonthS2"] as int?,
                    Player2WinsSetsLastWeekS2 = reader["Player2WinsSetsLastWeekS2"] as int?,
                    Player2LossesSetsLastWeekS2 = reader["Player2LossesSetsLastWeekS2"] as int?,
                    Player1WinsSetsTotalS3 = reader["Player1WinsSetsTotalS3"] as int?,
                    Player1LossesSetsTotalS3 = reader["Player1LossesSetsTotalS3"] as int?,
                    Player1WinsSetsLastYearS3 = reader["Player1WinsSetsLastYearS3"] as int?,
                    Player1LossesSetsLastYearS3 = reader["Player1LossesSetsLastYearS3"] as int?,
                    Player1WinsSetsLastMonthS3 = reader["Player1WinsSetsLastMonthS3"] as int?,
                    Player1LossesSetsLastMonthS3 = reader["Player1LossesSetsLastMonthS3"] as int?,
                    Player1WinsSetsLastWeekS3 = reader["Player1WinsSetsLastWeekS3"] as int?,
                    Player1LossesSetsLastWeekS3 = reader["Player1LossesSetsLastWeekS3"] as int?,
                    Player2WinsSetsTotalS3 = reader["Player2WinsSetsTotalS3"] as int?,
                    Player2LossesSetsTotalS3 = reader["Player2LossesSetsTotalS3"] as int?,
                    Player2WinsSetsLastYearS3 = reader["Player2WinsSetsLastYearS3"] as int?,
                    Player2LossesSetsLastYearS3 = reader["Player2LossesSetsLastYearS3"] as int?,
                    Player2WinsSetsLastMonthS3 = reader["Player2WinsSetsLastMonthS3"] as int?,
                    Player2LossesSetsLastMonthS3 = reader["Player2LossesSetsLastMonthS3"] as int?,
                    Player2WinsSetsLastWeekS3 = reader["Player2WinsSetsLastWeekS3"] as int?,
                    Player2LossesSetsLastWeekS3 = reader["Player2LossesSetsLastWeekS3"] as int?,
                    Player1WinsSetsTotalS4 = reader["Player1WinsSetsTotalS4"] as int?,
                    Player1LossesSetsTotalS4 = reader["Player1LossesSetsTotalS4"] as int?,
                    Player1WinsSetsLastYearS4 = reader["Player1WinsSetsLastYearS4"] as int?,
                    Player1LossesSetsLastYearS4 = reader["Player1LossesSetsLastYearS4"] as int?,
                    Player1WinsSetsLastMonthS4 = reader["Player1WinsSetsLastMonthS4"] as int?,
                    Player1LossesSetsLastMonthS4 = reader["Player1LossesSetsLastMonthS4"] as int?,
                    Player1WinsSetsLastWeekS4 = reader["Player1WinsSetsLastWeekS4"] as int?,
                    Player1LossesSetsLastWeekS4 = reader["Player1LossesSetsLastWeekS4"] as int?,
                    Player2WinsSetsTotalS4 = reader["Player2WinsSetsTotalS4"] as int?,
                    Player2LossesSetsTotalS4 = reader["Player2LossesSetsTotalS4"] as int?,
                    Player2WinsSetsLastYearS4 = reader["Player2WinsSetsLastYearS4"] as int?,
                    Player2LossesSetsLastYearS4 = reader["Player2LossesSetsLastYearS4"] as int?,
                    Player2WinsSetsLastMonthS4 = reader["Player2WinsSetsLastMonthS4"] as int?,
                    Player2LossesSetsLastMonthS4 = reader["Player2LossesSetsLastMonthS4"] as int?,
                    Player2WinsSetsLastWeekS4 = reader["Player2WinsSetsLastWeekS4"] as int?,
                    Player2LossesSetsLastWeekS4 = reader["Player2LossesSetsLastWeekS4"] as int?,
                    Player1WinsGamesTotal = reader["Player1WinsGamesTotal"] as int?,
                    Player1LossesGamesTotal = reader["Player1LossesGamesTotal"] as int?,
                    Player1WinsGamesLastYear = reader["Player1WinsGamesLastYear"] as int?,
                    Player1LossesGamesLastYear = reader["Player1LossesGamesLastYear"] as int?,
                    Player1WinsGamesLastMonth = reader["Player1WinsGamesLastMonth"] as int?,
                    Player1LossesGamesLastMonth = reader["Player1LossesGamesLastMonth"] as int?,
                    Player1WinsGamesLastWeek = reader["Player1WinsGamesLastWeek"] as int?,
                    Player1LossesGamesLastWeek = reader["Player1LossesGamesLastWeek"] as int?,
                    Player2WinsGamesTotal = reader["Player2WinsGamesTotal"] as int?,
                    Player2LossesGamesTotal = reader["Player2LossesGamesTotal"] as int?,
                    Player2WinsGamesLastYear = reader["Player2WinsGamesLastYear"] as int?,
                    Player2LossesGamesLastYear = reader["Player2LossesGamesLastYear"] as int?,
                    Player2WinsGamesLastMonth = reader["Player2WinsGamesLastMonth"] as int?,
                    Player2LossesGamesLastMonth = reader["Player2LossesGamesLastMonth"] as int?,
                    Player2WinsGamesLastWeek = reader["Player2WinsGamesLastWeek"] as int?,
                    Player2LossesGamesLastWeek = reader["Player2LossesGamesLastWeek"] as int?,
                    Player1WinsGamesTotalS1 = reader["Player1WinsGamesTotalS1"] as int?,
                    Player1LossesGamesTotalS1 = reader["Player1LossesGamesTotalS1"] as int?,
                    Player1WinsGamesLastYearS1 = reader["Player1WinsGamesLastYearS1"] as int?,
                    Player1LossesGamesLastYearS1 = reader["Player1LossesGamesLastYearS1"] as int?,
                    Player1WinsGamesLastMonthS1 = reader["Player1WinsGamesLastMonthS1"] as int?,
                    Player1LossesGamesLastMonthS1 = reader["Player1LossesGamesLastMonthS1"] as int?,
                    Player1WinsGamesLastWeekS1 = reader["Player1WinsGamesLastWeekS1"] as int?,
                    Player1LossesGamesLastWeekS1 = reader["Player1LossesGamesLastWeekS1"] as int?,
                    Player2WinsGamesTotalS1 = reader["Player2WinsGamesTotalS1"] as int?,
                    Player2LossesGamesTotalS1 = reader["Player2LossesGamesTotalS1"] as int?,
                    Player2WinsGamesLastYearS1 = reader["Player2WinsGamesLastYearS1"] as int?,
                    Player2LossesGamesLastYearS1 = reader["Player2LossesGamesLastYearS1"] as int?,
                    Player2WinsGamesLastMonthS1 = reader["Player2WinsGamesLastMonthS1"] as int?,
                    Player2LossesGamesLastMonthS1 = reader["Player2LossesGamesLastMonthS1"] as int?,
                    Player2WinsGamesLastWeekS1 = reader["Player2WinsGamesLastWeekS1"] as int?,
                    Player2LossesGamesLastWeekS1 = reader["Player2LossesGamesLastWeekS1"] as int?,
                    Player1WinsGamesTotalS2 = reader["Player1WinsGamesTotalS2"] as int?,
                    Player1LossesGamesTotalS2 = reader["Player1LossesGamesTotalS2"] as int?,
                    Player1WinsGamesLastYearS2 = reader["Player1WinsGamesLastYearS2"] as int?,
                    Player1LossesGamesLastYearS2 = reader["Player1LossesGamesLastYearS2"] as int?,
                    Player1WinsGamesLastMonthS2 = reader["Player1WinsGamesLastMonthS2"] as int?,
                    Player1LossesGamesLastMonthS2 = reader["Player1LossesGamesLastMonthS2"] as int?,
                    Player1WinsGamesLastWeekS2 = reader["Player1WinsGamesLastWeekS2"] as int?,
                    Player1LossesGamesLastWeekS2 = reader["Player1LossesGamesLastWeekS2"] as int?,
                    Player2WinsGamesTotalS2 = reader["Player2WinsGamesTotalS2"] as int?,
                    Player2LossesGamesTotalS2 = reader["Player2LossesGamesTotalS2"] as int?,
                    Player2WinsGamesLastYearS2 = reader["Player2WinsGamesLastYearS2"] as int?,
                    Player2LossesGamesLastYearS2 = reader["Player2LossesGamesLastYearS2"] as int?,
                    Player2WinsGamesLastMonthS2 = reader["Player2WinsGamesLastMonthS2"] as int?,
                    Player2LossesGamesLastMonthS2 = reader["Player2LossesGamesLastMonthS2"] as int?,
                    Player2WinsGamesLastWeekS2 = reader["Player2WinsGamesLastWeekS2"] as int?,
                    Player2LossesGamesLastWeekS2 = reader["Player2LossesGamesLastWeekS2"] as int?,
                    Player1WinsGamesTotalS3 = reader["Player1WinsGamesTotalS3"] as int?,
                    Player1LossesGamesTotalS3 = reader["Player1LossesGamesTotalS3"] as int?,
                    Player1WinsGamesLastYearS3 = reader["Player1WinsGamesLastYearS3"] as int?,
                    Player1LossesGamesLastYearS3 = reader["Player1LossesGamesLastYearS3"] as int?,
                    Player1WinsGamesLastMonthS3 = reader["Player1WinsGamesLastMonthS3"] as int?,
                    Player1LossesGamesLastMonthS3 = reader["Player1LossesGamesLastMonthS3"] as int?,
                    Player1WinsGamesLastWeekS3 = reader["Player1WinsGamesLastWeekS3"] as int?,
                    Player1LossesGamesLastWeekS3 = reader["Player1LossesGamesLastWeekS3"] as int?,
                    Player2WinsGamesTotalS3 = reader["Player2WinsGamesTotalS3"] as int?,
                    Player2LossesGamesTotalS3 = reader["Player2LossesGamesTotalS3"] as int?,
                    Player2WinsGamesLastYearS3 = reader["Player2WinsGamesLastYearS3"] as int?,
                    Player2LossesGamesLastYearS3 = reader["Player2LossesGamesLastYearS3"] as int?,
                    Player2WinsGamesLastMonthS3 = reader["Player2WinsGamesLastMonthS3"] as int?,
                    Player2LossesGamesLastMonthS3 = reader["Player2LossesGamesLastMonthS3"] as int?,
                    Player2WinsGamesLastWeekS3 = reader["Player2WinsGamesLastWeekS3"] as int?,
                    Player2LossesGamesLastWeekS3 = reader["Player2LossesGamesLastWeekS3"] as int?,
                    Player1WinsGamesTotalS4 = reader["Player1WinsGamesTotalS4"] as int?,
                    Player1LossesGamesTotalS4 = reader["Player1LossesGamesTotalS4"] as int?,
                    Player1WinsGamesLastYearS4 = reader["Player1WinsGamesLastYearS4"] as int?,
                    Player1LossesGamesLastYearS4 = reader["Player1LossesGamesLastYearS4"] as int?,
                    Player1WinsGamesLastMonthS4 = reader["Player1WinsGamesLastMonthS4"] as int?,
                    Player1LossesGamesLastMonthS4 = reader["Player1LossesGamesLastMonthS4"] as int?,
                    Player1WinsGamesLastWeekS4 = reader["Player1WinsGamesLastWeekS4"] as int?,
                    Player1LossesGamesLastWeekS4 = reader["Player1LossesGamesLastWeekS4"] as int?,
                    Player2WinsGamesTotalS4 = reader["Player2WinsGamesTotalS4"] as int?,
                    Player2LossesGamesTotalS4 = reader["Player2LossesGamesTotalS4"] as int?,
                    Player2WinsGamesLastYearS4 = reader["Player2WinsGamesLastYearS4"] as int?,
                    Player2LossesGamesLastYearS4 = reader["Player2LossesGamesLastYearS4"] as int?,
                    Player2WinsGamesLastMonthS4 = reader["Player2WinsGamesLastMonthS4"] as int?,
                    Player2LossesGamesLastMonthS4 = reader["Player2LossesGamesLastMonthS4"] as int?,
                    Player2WinsGamesLastWeekS4 = reader["Player2WinsGamesLastWeekS4"] as int?,
                    Player2LossesGamesLastWeekS4 = reader["Player2LossesGamesLastWeekS4"] as int?,
                    Player1DaysSinceLastWin = reader["Player1DaysSinceLastWin"] as int?,
                    Player2DaysSinceLastWin = reader["Player2DaysSinceLastWin"] as int?,
                    Player1DaysSinceLastWinS1 = reader["Player1DaysSinceLastWinS1"] as int?,
                    Player2DaysSinceLastWinS1 = reader["Player2DaysSinceLastWinS1"] as int?,
                    Player1DaysSinceLastWinS2 = reader["Player1DaysSinceLastWinS2"] as int?,
                    Player2DaysSinceLastWinS2 = reader["Player2DaysSinceLastWinS2"] as int?,
                    Player1DaysSinceLastWinS3 = reader["Player1DaysSinceLastWinS3"] as int?,
                    Player2DaysSinceLastWinS3 = reader["Player2DaysSinceLastWinS3"] as int?,
                    Player1DaysSinceLastWinS4 = reader["Player1DaysSinceLastWinS4"] as int?,
                    Player2DaysSinceLastWinS4 = reader["Player2DaysSinceLastWinS4"] as int?,
                    Player1DaysSinceLastLoss = reader["Player1DaysSinceLastLoss"] as int?,
                    Player2DaysSinceLastLoss = reader["Player2DaysSinceLastLoss"] as int?,
                    Player1DaysSinceLastLossS1 = reader["Player1DaysSinceLastLossS1"] as int?,
                    Player2DaysSinceLastLossS1 = reader["Player2DaysSinceLastLossS1"] as int?,
                    Player1DaysSinceLastLossS2 = reader["Player1DaysSinceLastLossS2"] as int?,
                    Player2DaysSinceLastLossS2 = reader["Player2DaysSinceLastLossS2"] as int?,
                    Player1DaysSinceLastLossS3 = reader["Player1DaysSinceLastLossS3"] as int?,
                    Player2DaysSinceLastLossS3 = reader["Player2DaysSinceLastLossS3"] as int?,
                    Player1DaysSinceLastLossS4 = reader["Player1DaysSinceLastLossS4"] as int?,
                    Player2DaysSinceLastLossS4 = reader["Player2DaysSinceLastLossS4"] as int?,
                    Player1TotalWinsAsFavourite = reader["Player1TotalWinsAsFavourite"] as int?,
                    Player2TotalWinsAsFavourite = reader["Player2TotalWinsAsFavourite"] as int?,
                    Player1TotalWinsAsUnderdog = reader["Player1TotalWinsAsUnderdog"] as int?,
                    Player2TotalWinsAsUnderdog = reader["Player2TotalWinsAsUnderdog"] as int?,
                    Player1TotalLossesAsFavourite = reader["Player1TotalLossesAsFavourite"] as int?,
                    Player2TotalLossesAsFavourite = reader["Player2TotalLossesAsFavourite"] as int?,
                    Player1TotalLossesAsUnderdog = reader["Player1TotalLossesAsUnderdog"] as int?,
                    Player2TotalLossesAsUnderdog = reader["Player2TotalLossesAsUnderdog"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavourite = reader["Player1AverageWinningProbabilityAtWonAsFavourite"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavourite = reader["Player2AverageWinningProbabilityAtWonAsFavourite"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdog = reader["Player1AverageWinningProbabilityAtWonAsUnderdog"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdog = reader["Player2AverageWinningProbabilityAtWonAsUnderdog"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavourite = reader["Player1AverageWinningProbabilityAtLossAsFavourite"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavourite = reader["Player2AverageWinningProbabilityAtLossAsFavourite"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdog = reader["Player1AverageWinningProbabilityAtLossAsUnderdog"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdog = reader["Player2AverageWinningProbabilityAtLossAsUnderdog"] as double?,
                    Player1TotalWinsAsFavouriteLastYear = reader["Player1TotalWinsAsFavouriteLastYear"] as int?,
                    Player2TotalWinsAsFavouriteLastYear = reader["Player2TotalWinsAsFavouriteLastYear"] as int?,
                    Player1TotalWinsAsUnderdogLastYear = reader["Player1TotalWinsAsUnderdogLastYear"] as int?,
                    Player2TotalWinsAsUnderdogLastYear = reader["Player2TotalWinsAsUnderdogLastYear"] as int?,
                    Player1TotalLossesAsFavouriteLastYear = reader["Player1TotalLossesAsFavouriteLastYear"] as int?,
                    Player2TotalLossesAsFavouriteLastYear = reader["Player2TotalLossesAsFavouriteLastYear"] as int?,
                    Player1TotalLossesAsUnderdogLastYear = reader["Player1TotalLossesAsUnderdogLastYear"] as int?,
                    Player2TotalLossesAsUnderdogLastYear = reader["Player2TotalLossesAsUnderdogLastYear"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastYear"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastYear"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastYear"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastYear"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastYear"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastYear = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastYear"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastYear"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastYear = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastYear"] as double?,
                    Player1TotalWinsAsFavouriteLastMonth = reader["Player1TotalWinsAsFavouriteLastMonth"] as int?,
                    Player2TotalWinsAsFavouriteLastMonth = reader["Player2TotalWinsAsFavouriteLastMonth"] as int?,
                    Player1TotalWinsAsUnderdogLastMonth = reader["Player1TotalWinsAsUnderdogLastMonth"] as int?,
                    Player2TotalWinsAsUnderdogLastMonth = reader["Player2TotalWinsAsUnderdogLastMonth"] as int?,
                    Player1TotalLossesAsFavouriteLastMonth = reader["Player1TotalLossesAsFavouriteLastMonth"] as int?,
                    Player2TotalLossesAsFavouriteLastMonth = reader["Player2TotalLossesAsFavouriteLastMonth"] as int?,
                    Player1TotalLossesAsUnderdogLastMonth = reader["Player1TotalLossesAsUnderdogLastMonth"] as int?,
                    Player2TotalLossesAsUnderdogLastMonth = reader["Player2TotalLossesAsUnderdogLastMonth"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth"] as double?,
                    Player1TotalWinsAsFavouriteLastWeek = reader["Player1TotalWinsAsFavouriteLastWeek"] as int?,
                    Player2TotalWinsAsFavouriteLastWeek = reader["Player2TotalWinsAsFavouriteLastWeek"] as int?,
                    Player1TotalWinsAsUnderdogLastWeek = reader["Player1TotalWinsAsUnderdogLastWeek"] as int?,
                    Player2TotalWinsAsUnderdogLastWeek = reader["Player2TotalWinsAsUnderdogLastWeek"] as int?,
                    Player1TotalLossesAsFavouriteLastWeek = reader["Player1TotalLossesAsFavouriteLastWeek"] as int?,
                    Player2TotalLossesAsFavouriteLastWeek = reader["Player2TotalLossesAsFavouriteLastWeek"] as int?,
                    Player1TotalLossesAsUnderdogLastWeek = reader["Player1TotalLossesAsUnderdogLastWeek"] as int?,
                    Player2TotalLossesAsUnderdogLastWeek = reader["Player2TotalLossesAsUnderdogLastWeek"] as int?,
                    Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek = reader["Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek"] as double?,
                    Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek"] as double?,
                    Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek = reader["Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek"] as double?,
                    Player1H2H = reader["Player1H2H"] as int?,
                    Player2H2H = reader["Player2H2H"] as int?,
                    Player1H2HOld = reader["Player1H2HOld"] as int?,
                    Player2H2HOld = reader["Player2H2HOld"] as int?,
                    Player1H2HTrueSkillMeanM = reader["Player1H2HTrueSkillMeanM"] as double?,
                    Player1H2HTrueSkillStandardDeviationM = reader["Player1H2HTrueSkillStandardDeviationM"] as double?,
                    Player2H2HTrueSkillMeanM = reader["Player2H2HTrueSkillMeanM"] as double?,
                    Player2H2HTrueSkillStandardDeviationM = reader["Player2H2HTrueSkillStandardDeviationM"] as double?,
                    Player1H2HTrueSkillMeanOldM = reader["Player1H2HTrueSkillMeanOldM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldM = reader["Player1H2HTrueSkillStandardDeviationOldM"] as double?,
                    Player2H2HTrueSkillMeanOldM = reader["Player2H2HTrueSkillMeanOldM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldM = reader["Player2H2HTrueSkillStandardDeviationOldM"] as double?,
                    WinProbabilityPlayer1H2HM = reader["WinProbabilityPlayer1H2HM"] as double?,
                    Player1H2HTrueSkillMeanSM = reader["Player1H2HTrueSkillMeanSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationSM = reader["Player1H2HTrueSkillStandardDeviationSM"] as double?,
                    Player2H2HTrueSkillMeanSM = reader["Player2H2HTrueSkillMeanSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationSM = reader["Player2H2HTrueSkillStandardDeviationSM"] as double?,
                    Player1H2HTrueSkillMeanOldSM = reader["Player1H2HTrueSkillMeanOldSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSM = reader["Player1H2HTrueSkillStandardDeviationOldSM"] as double?,
                    Player2H2HTrueSkillMeanOldSM = reader["Player2H2HTrueSkillMeanOldSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSM = reader["Player2H2HTrueSkillStandardDeviationOldSM"] as double?,
                    WinProbabilityPlayer1H2HSM = reader["WinProbabilityPlayer1H2HSM"] as double?,
                    Player1H2HTrueSkillMeanGSM = reader["Player1H2HTrueSkillMeanGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSM = reader["Player1H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player2H2HTrueSkillMeanGSM = reader["Player2H2HTrueSkillMeanGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSM = reader["Player2H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player1H2HTrueSkillMeanOldGSM = reader["Player1H2HTrueSkillMeanOldGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSM = reader["Player1H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    Player2H2HTrueSkillMeanOldGSM = reader["Player2H2HTrueSkillMeanOldGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSM = reader["Player2H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    WinProbabilityPlayer1H2HGSM = reader["WinProbabilityPlayer1H2HGSM"] as double?,
                    Player1H2HS1 = reader["Player1H2HS1"] as int?,
                    Player2H2HS1 = reader["Player2H2HS1"] as int?,
                    Player1H2HOldS1 = reader["Player1H2HOldS1"] as int?,
                    Player2H2HOldS1 = reader["Player2H2HOldS1"] as int?,
                    Player1H2HTrueSkillMeanMS1 = reader["Player1H2HTrueSkillMeanMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS1 = reader["Player1H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player2H2HTrueSkillMeanMS1 = reader["Player2H2HTrueSkillMeanMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS1 = reader["Player2H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player1H2HTrueSkillMeanOldMS1 = reader["Player1H2HTrueSkillMeanOldMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS1 = reader["Player1H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    Player2H2HTrueSkillMeanOldMS1 = reader["Player2H2HTrueSkillMeanOldMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS1 = reader["Player2H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    WinProbabilityPlayer1H2HMS1 = reader["WinProbabilityPlayer1H2HMS1"] as double?,
                    Player1H2HTrueSkillMeanSMS1 = reader["Player1H2HTrueSkillMeanSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS1 = reader["Player1H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player2H2HTrueSkillMeanSMS1 = reader["Player2H2HTrueSkillMeanSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS1 = reader["Player2H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldSMS1 = reader["Player1H2HTrueSkillMeanOldSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldSMS1 = reader["Player2H2HTrueSkillMeanOldSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    WinProbabilityPlayer1H2HSMS1 = reader["WinProbabilityPlayer1H2HSMS1"] as double?,
                    Player1H2HTrueSkillMeanGSMS1 = reader["Player1H2HTrueSkillMeanGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS1 = reader["Player1H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player2H2HTrueSkillMeanGSMS1 = reader["Player2H2HTrueSkillMeanGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS1 = reader["Player2H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS1 = reader["Player1H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS1 = reader["Player2H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    WinProbabilityPlayer1H2HGSMS1 = reader["WinProbabilityPlayer1H2HGSMS1"] as double?,
                    Player1H2HS2 = reader["Player1H2HS2"] as int?,
                    Player2H2HS2 = reader["Player2H2HS2"] as int?,
                    Player1H2HOldS2 = reader["Player1H2HOldS2"] as int?,
                    Player2H2HOldS2 = reader["Player2H2HOldS2"] as int?,
                    Player1H2HTrueSkillMeanMS2 = reader["Player1H2HTrueSkillMeanMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS2 = reader["Player1H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player2H2HTrueSkillMeanMS2 = reader["Player2H2HTrueSkillMeanMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS2 = reader["Player2H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player1H2HTrueSkillMeanOldMS2 = reader["Player1H2HTrueSkillMeanOldMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS2 = reader["Player1H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    Player2H2HTrueSkillMeanOldMS2 = reader["Player2H2HTrueSkillMeanOldMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS2 = reader["Player2H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    WinProbabilityPlayer1H2HMS2 = reader["WinProbabilityPlayer1H2HMS2"] as double?,
                    Player1H2HTrueSkillMeanSMS2 = reader["Player1H2HTrueSkillMeanSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS2 = reader["Player1H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player2H2HTrueSkillMeanSMS2 = reader["Player2H2HTrueSkillMeanSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS2 = reader["Player2H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldSMS2 = reader["Player1H2HTrueSkillMeanOldSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldSMS2 = reader["Player2H2HTrueSkillMeanOldSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    WinProbabilityPlayer1H2HSMS2 = reader["WinProbabilityPlayer1H2HSMS2"] as double?,
                    Player1H2HTrueSkillMeanGSMS2 = reader["Player1H2HTrueSkillMeanGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS2 = reader["Player1H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player2H2HTrueSkillMeanGSMS2 = reader["Player2H2HTrueSkillMeanGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS2 = reader["Player2H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS2 = reader["Player1H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS2 = reader["Player2H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    WinProbabilityPlayer1H2HGSMS2 = reader["WinProbabilityPlayer1H2HGSMS2"] as double?,
                    Player1H2HS3 = reader["Player1H2HS3"] as int?,
                    Player2H2HS3 = reader["Player2H2HS3"] as int?,
                    Player1H2HOldS3 = reader["Player1H2HOldS3"] as int?,
                    Player2H2HOldS3 = reader["Player2H2HOldS3"] as int?,
                    Player1H2HTrueSkillMeanMS3 = reader["Player1H2HTrueSkillMeanMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS3 = reader["Player1H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player2H2HTrueSkillMeanMS3 = reader["Player2H2HTrueSkillMeanMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS3 = reader["Player2H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player1H2HTrueSkillMeanOldMS3 = reader["Player1H2HTrueSkillMeanOldMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS3 = reader["Player1H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    Player2H2HTrueSkillMeanOldMS3 = reader["Player2H2HTrueSkillMeanOldMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS3 = reader["Player2H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    WinProbabilityPlayer1H2HMS3 = reader["WinProbabilityPlayer1H2HMS3"] as double?,
                    Player1H2HTrueSkillMeanSMS3 = reader["Player1H2HTrueSkillMeanSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS3 = reader["Player1H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player2H2HTrueSkillMeanSMS3 = reader["Player2H2HTrueSkillMeanSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS3 = reader["Player2H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldSMS3 = reader["Player1H2HTrueSkillMeanOldSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldSMS3 = reader["Player2H2HTrueSkillMeanOldSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    WinProbabilityPlayer1H2HSMS3 = reader["WinProbabilityPlayer1H2HSMS3"] as double?,
                    Player1H2HTrueSkillMeanGSMS3 = reader["Player1H2HTrueSkillMeanGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS3 = reader["Player1H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player2H2HTrueSkillMeanGSMS3 = reader["Player2H2HTrueSkillMeanGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS3 = reader["Player2H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS3 = reader["Player1H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS3 = reader["Player2H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    WinProbabilityPlayer1H2HGSMS3 = reader["WinProbabilityPlayer1H2HGSMS3"] as double?,
                    Player1H2HS4 = reader["Player1H2HS4"] as int?,
                    Player2H2HS4 = reader["Player2H2HS4"] as int?,
                    Player1H2HOldS4 = reader["Player1H2HOldS4"] as int?,
                    Player2H2HOldS4 = reader["Player2H2HOldS4"] as int?,
                    Player1H2HTrueSkillMeanMS4 = reader["Player1H2HTrueSkillMeanMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS4 = reader["Player1H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player2H2HTrueSkillMeanMS4 = reader["Player2H2HTrueSkillMeanMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS4 = reader["Player2H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player1H2HTrueSkillMeanOldMS4 = reader["Player1H2HTrueSkillMeanOldMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS4 = reader["Player1H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    Player2H2HTrueSkillMeanOldMS4 = reader["Player2H2HTrueSkillMeanOldMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS4 = reader["Player2H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    WinProbabilityPlayer1H2HMS4 = reader["WinProbabilityPlayer1H2HMS4"] as double?,
                    Player1H2HTrueSkillMeanSMS4 = reader["Player1H2HTrueSkillMeanSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS4 = reader["Player1H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player2H2HTrueSkillMeanSMS4 = reader["Player2H2HTrueSkillMeanSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS4 = reader["Player2H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldSMS4 = reader["Player1H2HTrueSkillMeanOldSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldSMS4 = reader["Player2H2HTrueSkillMeanOldSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    WinProbabilityPlayer1H2HSMS4 = reader["WinProbabilityPlayer1H2HSMS4"] as double?,
                    Player1H2HTrueSkillMeanGSMS4 = reader["Player1H2HTrueSkillMeanGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS4 = reader["Player1H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player2H2HTrueSkillMeanGSMS4 = reader["Player2H2HTrueSkillMeanGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS4 = reader["Player2H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS4 = reader["Player1H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS4 = reader["Player2H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    WinProbabilityPlayer1H2HGSMS4 = reader["WinProbabilityPlayer1H2HGSMS4"] as double?,
                    Player1Streak = reader["Player1Streak"] as int?,
                    Player2Streak = reader["Player2Streak"] as int?,
                    Player1StreakS1 = reader["Player1StreakS1"] as int?,
                    Player2StreakS1 = reader["Player2StreakS1"] as int?,
                    Player1StreakS2 = reader["Player1StreakS2"] as int?,
                    Player2StreakS2 = reader["Player2StreakS2"] as int?,
                    Player1StreakS3 = reader["Player1StreakS3"] as int?,
                    Player2StreakS3 = reader["Player2StreakS3"] as int?,
                    Player1StreakS4 = reader["Player1StreakS4"] as int?,
                    Player2StreakS4 = reader["Player2StreakS4"] as int?,
                    P1SetsWon = reader["P1SetsWon"] as int?,
                    P2SetsWon = reader["P2SetsWon"] as int?,
                    P1GamesWon = reader["P1GamesWon"] as int?,
                    P2GamesWon = reader["P2GamesWon"] as int?,
                    P1SetsLoss = reader["P1SetsLoss"] as int?,
                    P2SetsLoss = reader["P2SetsLoss"] as int?,
                    P1GamesLoss = reader["P1GamesLoss"] as int?,
                    P2GamesLoss = reader["P2GamesLoss"] as int?,
                    WinProbabilityNN = reader["WinProbabilityNN"] as double?,
                    IsFinished = reader["IsFinished"]                                                 as bool?,
                };
                Matches.Add(Match);
            }

            return Matches;
        }

        public async Task InsertMatchAsync(Match match)
        {
 
            try
            {
                using var command = _connection.CreateCommand();
                command.CommandText = "InsertMatch";
                command.CommandType = CommandType.StoredProcedure;

                var parameters = new (string, object?)[]
                {
                ("@MatchTPId"                                                 , match.MatchTPId                                                 ),
                ("@TournamentEventTPId"                                       , match.TournamentEventTPId                                       ),
                ("@DateTime"                                                  , match.DateTime                                                  ),
                ("@Player1TPId"                                               , match.Player1TPId                                               ),
                ("@Player2TPId"                                               , match.Player2TPId                                               ),
                ("@Player1Seed"                                               , match.Player1Seed                                               ),
                ("@Player2Seed"                                               , match.Player2Seed                                               ),
                ("@Result"                                                    , match.Result                                                    ),
                ("@ResultDetails"                                             , match.ResultDetails                                             ),
                ("@Player1Odds"                                               , match.Player1Odds                                               ),
                ("@Player2Odds"                                               , match.Player2Odds                                               ),
                ("@Player1Percentage"                                         , match.Player1Percentage                                         ),
                ("@Player2Percentage"                                         , match.Player2Percentage                                         ),
                ("@SurfaceId"                                                 , match.SurfaceId                                                 ),
                ("@RoundId"                                                   , match.RoundId                                                   ),
                ("@Player1TrueSkillMeanM"                                     , match.Player1TrueSkillMeanM                                     ),
                ("@Player1TrueSkillStandardDeviationM"                        , match.Player1TrueSkillStandardDeviationM                        ),
                ("@Player2TrueSkillMeanM"                                     , match.Player2TrueSkillMeanM                                     ),
                ("@Player2TrueSkillStandardDeviationM"                        , match.Player2TrueSkillStandardDeviationM                        ),
                ("@Player1TrueSkillMeanOldM"                                  , match.Player1TrueSkillMeanOldM                                  ),
                ("@Player1TrueSkillStandardDeviationOldM"                     , match.Player1TrueSkillStandardDeviationOldM                     ),
                ("@Player2TrueSkillMeanOldM"                                  , match.Player2TrueSkillMeanOldM                                  ),
                ("@Player2TrueSkillStandardDeviationOldM"                     , match.Player2TrueSkillStandardDeviationOldM                     ),
                ("@WinProbabilityPlayer1M"                                    , match.WinProbabilityPlayer1M                                    ),
                ("@Player1TrueSkillMeanSM"                                    , match.Player1TrueSkillMeanSM                                    ),
                ("@Player1TrueSkillStandardDeviationSM"                       , match.Player1TrueSkillStandardDeviationSM                       ),
                ("@Player2TrueSkillMeanSM"                                    , match.Player2TrueSkillMeanSM                                    ),
                ("@Player2TrueSkillStandardDeviationSM"                       , match.Player2TrueSkillStandardDeviationSM                       ),
                ("@Player1TrueSkillMeanOldSM"                                 , match.Player1TrueSkillMeanOldSM                                 ),
                ("@Player1TrueSkillStandardDeviationOldSM"                    , match.Player1TrueSkillStandardDeviationOldSM                    ),
                ("@Player2TrueSkillMeanOldSM"                                 , match.Player2TrueSkillMeanOldSM                                 ),
                ("@Player2TrueSkillStandardDeviationOldSM"                    , match.Player2TrueSkillStandardDeviationOldSM                    ),
                ("@WinProbabilityPlayer1SM"                                   , match.WinProbabilityPlayer1SM                                   ),
                ("@Player1TrueSkillMeanGSM"                                   , match.Player1TrueSkillMeanGSM                                   ),
                ("@Player1TrueSkillStandardDeviationGSM"                      , match.Player1TrueSkillStandardDeviationGSM                      ),
                ("@Player2TrueSkillMeanGSM"                                   , match.Player2TrueSkillMeanGSM                                   ),
                ("@Player2TrueSkillStandardDeviationGSM"                      , match.Player2TrueSkillStandardDeviationGSM                      ),
                ("@Player1TrueSkillMeanOldGSM"                                , match.Player1TrueSkillMeanOldGSM                                ),
                ("@Player1TrueSkillStandardDeviationOldGSM"                   , match.Player1TrueSkillStandardDeviationOldGSM                   ),
                ("@Player2TrueSkillMeanOldGSM"                                , match.Player2TrueSkillMeanOldGSM                                ),
                ("@Player2TrueSkillStandardDeviationOldGSM"                   , match.Player2TrueSkillStandardDeviationOldGSM                   ),
                ("@WinProbabilityPlayer1GSM"                                  , match.WinProbabilityPlayer1GSM                                  ),
                ("@Player1TrueSkillMeanMS1"                                   , match.Player1TrueSkillMeanMS1                                   ),
                ("@Player1TrueSkillStandardDeviationMS1"                      , match.Player1TrueSkillStandardDeviationMS1                      ),
                ("@Player2TrueSkillMeanMS1"                                   , match.Player2TrueSkillMeanMS1                                   ),
                ("@Player2TrueSkillStandardDeviationMS1"                      , match.Player2TrueSkillStandardDeviationMS1                      ),
                ("@Player1TrueSkillMeanOldMS1"                                , match.Player1TrueSkillMeanOldMS1                                ),
                ("@Player1TrueSkillStandardDeviationOldMS1"                   , match.Player1TrueSkillStandardDeviationOldMS1                   ),
                ("@Player2TrueSkillMeanOldMS1"                                , match.Player2TrueSkillMeanOldMS1                                ),
                ("@Player2TrueSkillStandardDeviationOldMS1"                   , match.Player2TrueSkillStandardDeviationOldMS1                   ),
                ("@WinProbabilityPlayer1MS1"                                  , match.WinProbabilityPlayer1MS1                                  ),
                ("@Player1TrueSkillMeanSMS1"                                  , match.Player1TrueSkillMeanSMS1                                  ),
                ("@Player1TrueSkillStandardDeviationSMS1"                     , match.Player1TrueSkillStandardDeviationSMS1                     ),
                ("@Player2TrueSkillMeanSMS1"                                  , match.Player2TrueSkillMeanSMS1                                  ),
                ("@Player2TrueSkillStandardDeviationSMS1"                     , match.Player2TrueSkillStandardDeviationSMS1                     ),
                ("@Player1TrueSkillMeanOldSMS1"                               , match.Player1TrueSkillMeanOldSMS1                               ),
                ("@Player1TrueSkillStandardDeviationOldSMS1"                  , match.Player1TrueSkillStandardDeviationOldSMS1                  ),
                ("@Player2TrueSkillMeanOldSMS1"                               , match.Player2TrueSkillMeanOldSMS1                               ),
                ("@Player2TrueSkillStandardDeviationOldSMS1"                  , match.Player2TrueSkillStandardDeviationOldSMS1                  ),
                ("@WinProbabilityPlayer1SMS1"                                 , match.WinProbabilityPlayer1SMS1                                 ),
                ("@Player1TrueSkillMeanGSMS1"                                 , match.Player1TrueSkillMeanGSMS1                                 ),
                ("@Player1TrueSkillStandardDeviationGSMS1"                    , match.Player1TrueSkillStandardDeviationGSMS1                    ),
                ("@Player2TrueSkillMeanGSMS1"                                 , match.Player2TrueSkillMeanGSMS1                                 ),
                ("@Player2TrueSkillStandardDeviationGSMS1"                    , match.Player2TrueSkillStandardDeviationGSMS1                    ),
                ("@Player1TrueSkillMeanOldGSMS1"                              , match.Player1TrueSkillMeanOldGSMS1                              ),
                ("@Player1TrueSkillStandardDeviationOldGSMS1"                 , match.Player1TrueSkillStandardDeviationOldGSMS1                 ),
                ("@Player2TrueSkillMeanOldGSMS1"                              , match.Player2TrueSkillMeanOldGSMS1                              ),
                ("@Player2TrueSkillStandardDeviationOldGSMS1"                 , match.Player2TrueSkillStandardDeviationOldGSMS1                 ),
                ("@WinProbabilityPlayer1GSMS1"                                , match.WinProbabilityPlayer1GSMS1                                ),
                ("@Player1TrueSkillMeanMS2"                                   , match.Player1TrueSkillMeanMS2                                   ),
                ("@Player1TrueSkillStandardDeviationMS2"                      , match.Player1TrueSkillStandardDeviationMS2                      ),
                ("@Player2TrueSkillMeanMS2"                                   , match.Player2TrueSkillMeanMS2                                   ),
                ("@Player2TrueSkillStandardDeviationMS2"                      , match.Player2TrueSkillStandardDeviationMS2                      ),
                ("@Player1TrueSkillMeanOldMS2"                                , match.Player1TrueSkillMeanOldMS2                                ),
                ("@Player1TrueSkillStandardDeviationOldMS2"                   , match.Player1TrueSkillStandardDeviationOldMS2                   ),
                ("@Player2TrueSkillMeanOldMS2"                                , match.Player2TrueSkillMeanOldMS2                                ),
                ("@Player2TrueSkillStandardDeviationOldMS2"                   , match.Player2TrueSkillStandardDeviationOldMS2                   ),
                ("@WinProbabilityPlayer1MS2"                                  , match.WinProbabilityPlayer1MS2                                  ),
                ("@Player1TrueSkillMeanSMS2"                                  , match.Player1TrueSkillMeanSMS2                                  ),
                ("@Player1TrueSkillStandardDeviationSMS2"                     , match.Player1TrueSkillStandardDeviationSMS2                     ),
                ("@Player2TrueSkillMeanSMS2"                                  , match.Player2TrueSkillMeanSMS2                                  ),
                ("@Player2TrueSkillStandardDeviationSMS2"                     , match.Player2TrueSkillStandardDeviationSMS2                     ),
                ("@Player1TrueSkillMeanOldSMS2"                               , match.Player1TrueSkillMeanOldSMS2                               ),
                ("@Player1TrueSkillStandardDeviationOldSMS2"                  , match.Player1TrueSkillStandardDeviationOldSMS2                  ),
                ("@Player2TrueSkillMeanOldSMS2"                               , match.Player2TrueSkillMeanOldSMS2                               ),
                ("@Player2TrueSkillStandardDeviationOldSMS2"                  , match.Player2TrueSkillStandardDeviationOldSMS2                  ),
                ("@WinProbabilityPlayer1SMS2"                                 , match.WinProbabilityPlayer1SMS2                                 ),
                ("@Player1TrueSkillMeanGSMS2"                                 , match.Player1TrueSkillMeanGSMS2                                 ),
                ("@Player1TrueSkillStandardDeviationGSMS2"                    , match.Player1TrueSkillStandardDeviationGSMS2                    ),
                ("@Player2TrueSkillMeanGSMS2"                                 , match.Player2TrueSkillMeanGSMS2                                 ),
                ("@Player2TrueSkillStandardDeviationGSMS2"                    , match.Player2TrueSkillStandardDeviationGSMS2                    ),
                ("@Player1TrueSkillMeanOldGSMS2"                              , match.Player1TrueSkillMeanOldGSMS2                              ),
                ("@Player1TrueSkillStandardDeviationOldGSMS2"                 , match.Player1TrueSkillStandardDeviationOldGSMS2                 ),
                ("@Player2TrueSkillMeanOldGSMS2"                              , match.Player2TrueSkillMeanOldGSMS2                              ),
                ("@Player2TrueSkillStandardDeviationOldGSMS2"                 , match.Player2TrueSkillStandardDeviationOldGSMS2                 ),
                ("@WinProbabilityPlayer1GSMS2"                                , match.WinProbabilityPlayer1GSMS2                                ),
                ("@Player1TrueSkillMeanMS3"                                   , match.Player1TrueSkillMeanMS3                                   ),
                ("@Player1TrueSkillStandardDeviationMS3"                      , match.Player1TrueSkillStandardDeviationMS3                      ),
                ("@Player2TrueSkillMeanMS3"                                   , match.Player2TrueSkillMeanMS3                                   ),
                ("@Player2TrueSkillStandardDeviationMS3"                      , match.Player2TrueSkillStandardDeviationMS3                      ),
                ("@Player1TrueSkillMeanOldMS3"                                , match.Player1TrueSkillMeanOldMS3                                ),
                ("@Player1TrueSkillStandardDeviationOldMS3"                   , match.Player1TrueSkillStandardDeviationOldMS3                   ),
                ("@Player2TrueSkillMeanOldMS3"                                , match.Player2TrueSkillMeanOldMS3                                ),
                ("@Player2TrueSkillStandardDeviationOldMS3"                   , match.Player2TrueSkillStandardDeviationOldMS3                   ),
                ("@WinProbabilityPlayer1MS3"                                  , match.WinProbabilityPlayer1MS3                                  ),
                ("@Player1TrueSkillMeanSMS3"                                  , match.Player1TrueSkillMeanSMS3                                  ),
                ("@Player1TrueSkillStandardDeviationSMS3"                     , match.Player1TrueSkillStandardDeviationSMS3                     ),
                ("@Player2TrueSkillMeanSMS3"                                  , match.Player2TrueSkillMeanSMS3                                  ),
                ("@Player2TrueSkillStandardDeviationSMS3"                     , match.Player2TrueSkillStandardDeviationSMS3                     ),
                ("@Player1TrueSkillMeanOldSMS3"                               , match.Player1TrueSkillMeanOldSMS3                               ),
                ("@Player1TrueSkillStandardDeviationOldSMS3"                  , match.Player1TrueSkillStandardDeviationOldSMS3                  ),
                ("@Player2TrueSkillMeanOldSMS3"                               , match.Player2TrueSkillMeanOldSMS3                               ),
                ("@Player2TrueSkillStandardDeviationOldSMS3"                  , match.Player2TrueSkillStandardDeviationOldSMS3                  ),
                ("@WinProbabilityPlayer1SMS3"                                 , match.WinProbabilityPlayer1SMS3                                 ),
                ("@Player1TrueSkillMeanGSMS3"                                 , match.Player1TrueSkillMeanGSMS3                                 ),
                ("@Player1TrueSkillStandardDeviationGSMS3"                    , match.Player1TrueSkillStandardDeviationGSMS3                    ),
                ("@Player2TrueSkillMeanGSMS3"                                 , match.Player2TrueSkillMeanGSMS3                                 ),
                ("@Player2TrueSkillStandardDeviationGSMS3"                    , match.Player2TrueSkillStandardDeviationGSMS3                    ),
                ("@Player1TrueSkillMeanOldGSMS3"                              , match.Player1TrueSkillMeanOldGSMS3                              ),
                ("@Player1TrueSkillStandardDeviationOldGSMS3"                 , match.Player1TrueSkillStandardDeviationOldGSMS3                 ),
                ("@Player2TrueSkillMeanOldGSMS3"                              , match.Player2TrueSkillMeanOldGSMS3                              ),
                ("@Player2TrueSkillStandardDeviationOldGSMS3"                 , match.Player2TrueSkillStandardDeviationOldGSMS3                 ),
                ("@WinProbabilityPlayer1GSMS3"                                , match.WinProbabilityPlayer1GSMS3                                ),
                ("@Player1TrueSkillMeanMS4"                                   , match.Player1TrueSkillMeanMS4                                   ),
                ("@Player1TrueSkillStandardDeviationMS4"                      , match.Player1TrueSkillStandardDeviationMS4                      ),
                ("@Player2TrueSkillMeanMS4"                                   , match.Player2TrueSkillMeanMS4                                   ),
                ("@Player2TrueSkillStandardDeviationMS4"                      , match.Player2TrueSkillStandardDeviationMS4                      ),
                ("@Player1TrueSkillMeanOldMS4"                                , match.Player1TrueSkillMeanOldMS4                                ),
                ("@Player1TrueSkillStandardDeviationOldMS4"                   , match.Player1TrueSkillStandardDeviationOldMS4                   ),
                ("@Player2TrueSkillMeanOldMS4"                                , match.Player2TrueSkillMeanOldMS4                                ),
                ("@Player2TrueSkillStandardDeviationOldMS4"                   , match.Player2TrueSkillStandardDeviationOldMS4                   ),
                ("@WinProbabilityPlayer1MS4"                                  , match.WinProbabilityPlayer1MS4                                  ),
                ("@Player1TrueSkillMeanSMS4"                                  , match.Player1TrueSkillMeanSMS4                                  ),
                ("@Player1TrueSkillStandardDeviationSMS4"                     , match.Player1TrueSkillStandardDeviationSMS4                     ),
                ("@Player2TrueSkillMeanSMS4"                                  , match.Player2TrueSkillMeanSMS4                                  ),
                ("@Player2TrueSkillStandardDeviationSMS4"                     , match.Player2TrueSkillStandardDeviationSMS4                     ),
                ("@Player1TrueSkillMeanOldSMS4"                               , match.Player1TrueSkillMeanOldSMS4                               ),
                ("@Player1TrueSkillStandardDeviationOldSMS4"                  , match.Player1TrueSkillStandardDeviationOldSMS4                  ),
                ("@Player2TrueSkillMeanOldSMS4"                               , match.Player2TrueSkillMeanOldSMS4                               ),
                ("@Player2TrueSkillStandardDeviationOldSMS4"                  , match.Player2TrueSkillStandardDeviationOldSMS4                  ),
                ("@WinProbabilityPlayer1SMS4"                                 , match.WinProbabilityPlayer1SMS4                                 ),
                ("@Player1TrueSkillMeanGSMS4"                                 , match.Player1TrueSkillMeanGSMS4                                 ),
                ("@Player1TrueSkillStandardDeviationGSMS4"                    , match.Player1TrueSkillStandardDeviationGSMS4                    ),
                ("@Player2TrueSkillMeanGSMS4"                                 , match.Player2TrueSkillMeanGSMS4                                 ),
                ("@Player2TrueSkillStandardDeviationGSMS4"                    , match.Player2TrueSkillStandardDeviationGSMS4                    ),
                ("@Player1TrueSkillMeanOldGSMS4"                              , match.Player1TrueSkillMeanOldGSMS4                              ),
                ("@Player1TrueSkillStandardDeviationOldGSMS4"                 , match.Player1TrueSkillStandardDeviationOldGSMS4                 ),
                ("@Player2TrueSkillMeanOldGSMS4"                              , match.Player2TrueSkillMeanOldGSMS4                              ),
                ("@Player2TrueSkillStandardDeviationOldGSMS4"                 , match.Player2TrueSkillStandardDeviationOldGSMS4                 ),
                ("@WinProbabilityPlayer1GSMS4"                                , match.WinProbabilityPlayer1GSMS4                                ),
                ("@Player1WinsTotal"                                          , match.Player1WinsTotal                                          ),
                ("@Player1LossesTotal"                                        , match.Player1LossesTotal                                        ),
                ("@Player1WinsLastYear"                                       , match.Player1WinsLastYear                                       ),
                ("@Player1LossesLastYear"                                     , match.Player1LossesLastYear                                     ),
                ("@Player1WinsLastMonth"                                      , match.Player1WinsLastMonth                                      ),
                ("@Player1LossesLastMonth"                                    , match.Player1LossesLastMonth                                    ),
                ("@Player1WinsLastWeek"                                       , match.Player1WinsLastWeek                                       ),
                ("@Player1LossesLastWeek"                                     , match.Player1LossesLastWeek                                     ),
                ("@Player2WinsTotal"                                          , match.Player2WinsTotal                                          ),
                ("@Player2LossesTotal"                                        , match.Player2LossesTotal                                        ),
                ("@Player2WinsLastYear"                                       , match.Player2WinsLastYear                                       ),
                ("@Player2LossesLastYear"                                     , match.Player2LossesLastYear                                     ),
                ("@Player2WinsLastMonth"                                      , match.Player2WinsLastMonth                                      ),
                ("@Player2LossesLastMonth"                                    , match.Player2LossesLastMonth                                    ),
                ("@Player2WinsLastWeek"                                       , match.Player2WinsLastWeek                                       ),
                ("@Player2LossesLastWeek"                                     , match.Player2LossesLastWeek                                     ),
                ("@Player1WinsTotalS1"                                        , match.Player1WinsTotalS1                                        ),
                ("@Player1LossesTotalS1"                                      , match.Player1LossesTotalS1                                      ),
                ("@Player1WinsLastYearS1"                                     , match.Player1WinsLastYearS1                                     ),
                ("@Player1LossesLastYearS1"                                   , match.Player1LossesLastYearS1                                   ),
                ("@Player1WinsLastMonthS1"                                    , match.Player1WinsLastMonthS1                                    ),
                ("@Player1LossesLastMonthS1"                                  , match.Player1LossesLastMonthS1                                  ),
                ("@Player1WinsLastWeekS1"                                     , match.Player1WinsLastWeekS1                                     ),
                ("@Player1LossesLastWeekS1"                                   , match.Player1LossesLastWeekS1                                   ),
                ("@Player2WinsTotalS1"                                        , match.Player2WinsTotalS1                                        ),
                ("@Player2LossesTotalS1"                                      , match.Player2LossesTotalS1                                      ),
                ("@Player2WinsLastYearS1"                                     , match.Player2WinsLastYearS1                                     ),
                ("@Player2LossesLastYearS1"                                   , match.Player2LossesLastYearS1                                   ),
                ("@Player2WinsLastMonthS1"                                    , match.Player2WinsLastMonthS1                                    ),
                ("@Player2LossesLastMonthS1"                                  , match.Player2LossesLastMonthS1                                  ),
                ("@Player2WinsLastWeekS1"                                     , match.Player2WinsLastWeekS1                                     ),
                ("@Player2LossesLastWeekS1"                                   , match.Player2LossesLastWeekS1                                   ),
                ("@Player1WinsTotalS2"                                        , match.Player1WinsTotalS2                                        ),
                ("@Player1LossesTotalS2"                                      , match.Player1LossesTotalS2                                      ),
                ("@Player1WinsLastYearS2"                                     , match.Player1WinsLastYearS2                                     ),
                ("@Player1LossesLastYearS2"                                   , match.Player1LossesLastYearS2                                   ),
                ("@Player1WinsLastMonthS2"                                    , match.Player1WinsLastMonthS2                                    ),
                ("@Player1LossesLastMonthS2"                                  , match.Player1LossesLastMonthS2                                  ),
                ("@Player1WinsLastWeekS2"                                     , match.Player1WinsLastWeekS2                                     ),
                ("@Player1LossesLastWeekS2"                                   , match.Player1LossesLastWeekS2                                   ),
                ("@Player2WinsTotalS2"                                        , match.Player2WinsTotalS2                                        ),
                ("@Player2LossesTotalS2"                                      , match.Player2LossesTotalS2                                      ),
                ("@Player2WinsLastYearS2"                                     , match.Player2WinsLastYearS2                                     ),
                ("@Player2LossesLastYearS2"                                   , match.Player2LossesLastYearS2                                   ),
                ("@Player2WinsLastMonthS2"                                    , match.Player2WinsLastMonthS2                                    ),
                ("@Player2LossesLastMonthS2"                                  , match.Player2LossesLastMonthS2                                  ),
                ("@Player2WinsLastWeekS2"                                     , match.Player2WinsLastWeekS2                                     ),
                ("@Player2LossesLastWeekS2"                                   , match.Player2LossesLastWeekS2                                   ),
                ("@Player1WinsTotalS3"                                        , match.Player1WinsTotalS3                                        ),
                ("@Player1LossesTotalS3"                                      , match.Player1LossesTotalS3                                      ),
                ("@Player1WinsLastYearS3"                                     , match.Player1WinsLastYearS3                                     ),
                ("@Player1LossesLastYearS3"                                   , match.Player1LossesLastYearS3                                   ),
                ("@Player1WinsLastMonthS3"                                    , match.Player1WinsLastMonthS3                                    ),
                ("@Player1LossesLastMonthS3"                                  , match.Player1LossesLastMonthS3                                  ),
                ("@Player1WinsLastWeekS3"                                     , match.Player1WinsLastWeekS3                                     ),
                ("@Player1LossesLastWeekS3"                                   , match.Player1LossesLastWeekS3                                   ),
                ("@Player2WinsTotalS3"                                        , match.Player2WinsTotalS3                                        ),
                ("@Player2LossesTotalS3"                                      , match.Player2LossesTotalS3                                      ),
                ("@Player2WinsLastYearS3"                                     , match.Player2WinsLastYearS3                                     ),
                ("@Player2LossesLastYearS3"                                   , match.Player2LossesLastYearS3                                   ),
                ("@Player2WinsLastMonthS3"                                    , match.Player2WinsLastMonthS3                                    ),
                ("@Player2LossesLastMonthS3"                                  , match.Player2LossesLastMonthS3                                  ),
                ("@Player2WinsLastWeekS3"                                     , match.Player2WinsLastWeekS3                                     ),
                ("@Player2LossesLastWeekS3"                                   , match.Player2LossesLastWeekS3                                   ),
                ("@Player1WinsTotalS4"                                        , match.Player1WinsTotalS4                                        ),
                ("@Player1LossesTotalS4"                                      , match.Player1LossesTotalS4                                      ),
                ("@Player1WinsLastYearS4"                                     , match.Player1WinsLastYearS4                                     ),
                ("@Player1LossesLastYearS4"                                   , match.Player1LossesLastYearS4                                   ),
                ("@Player1WinsLastMonthS4"                                    , match.Player1WinsLastMonthS4                                    ),
                ("@Player1LossesLastMonthS4"                                  , match.Player1LossesLastMonthS4                                  ),
                ("@Player1WinsLastWeekS4"                                     , match.Player1WinsLastWeekS4                                     ),
                ("@Player1LossesLastWeekS4"                                   , match.Player1LossesLastWeekS4                                   ),
                ("@Player2WinsTotalS4"                                        , match.Player2WinsTotalS4                                        ),
                ("@Player2LossesTotalS4"                                      , match.Player2LossesTotalS4                                      ),
                ("@Player2WinsLastYearS4"                                     , match.Player2WinsLastYearS4                                     ),
                ("@Player2LossesLastYearS4"                                   , match.Player2LossesLastYearS4                                   ),
                ("@Player2WinsLastMonthS4"                                    , match.Player2WinsLastMonthS4                                    ),
                ("@Player2LossesLastMonthS4"                                  , match.Player2LossesLastMonthS4                                  ),
                ("@Player2WinsLastWeekS4"                                     , match.Player2WinsLastWeekS4                                     ),
                ("@Player2LossesLastWeekS4"                                   , match.Player2LossesLastWeekS4                                   ),
                ("@Player1WinsSetsTotal"                                      , match.Player1WinsSetsTotal                                      ),
                ("@Player1LossesSetsTotal"                                    , match.Player1LossesSetsTotal                                    ),
                ("@Player1WinsSetsLastYear"                                   , match.Player1WinsSetsLastYear                                   ),
                ("@Player1LossesSetsLastYear"                                 , match.Player1LossesSetsLastYear                                 ),
                ("@Player1WinsSetsLastMonth"                                  , match.Player1WinsSetsLastMonth                                  ),
                ("@Player1LossesSetsLastMonth"                                , match.Player1LossesSetsLastMonth                                ),
                ("@Player1WinsSetsLastWeek"                                   , match.Player1WinsSetsLastWeek                                   ),
                ("@Player1LossesSetsLastWeek"                                 , match.Player1LossesSetsLastWeek                                 ),
                ("@Player2WinsSetsTotal"                                      , match.Player2WinsSetsTotal                                      ),
                ("@Player2LossesSetsTotal"                                    , match.Player2LossesSetsTotal                                    ),
                ("@Player2WinsSetsLastYear"                                   , match.Player2WinsSetsLastYear                                   ),
                ("@Player2LossesSetsLastYear"                                 , match.Player2LossesSetsLastYear                                 ),
                ("@Player2WinsSetsLastMonth"                                  , match.Player2WinsSetsLastMonth                                  ),
                ("@Player2LossesSetsLastMonth"                                , match.Player2LossesSetsLastMonth                                ),
                ("@Player2WinsSetsLastWeek"                                   , match.Player2WinsSetsLastWeek                                   ),
                ("@Player2LossesSetsLastWeek"                                 , match.Player2LossesSetsLastWeek                                 ),
                ("@Player1WinsSetsTotalS1"                                    , match.Player1WinsSetsTotalS1                                    ),
                ("@Player1LossesSetsTotalS1"                                  , match.Player1LossesSetsTotalS1                                  ),
                ("@Player1WinsSetsLastYearS1"                                 , match.Player1WinsSetsLastYearS1                                 ),
                ("@Player1LossesSetsLastYearS1"                               , match.Player1LossesSetsLastYearS1                               ),
                ("@Player1WinsSetsLastMonthS1"                                , match.Player1WinsSetsLastMonthS1                                ),
                ("@Player1LossesSetsLastMonthS1"                              , match.Player1LossesSetsLastMonthS1                              ),
                ("@Player1WinsSetsLastWeekS1"                                 , match.Player1WinsSetsLastWeekS1                                 ),
                ("@Player1LossesSetsLastWeekS1"                               , match.Player1LossesSetsLastWeekS1                               ),
                ("@Player2WinsSetsTotalS1"                                    , match.Player2WinsSetsTotalS1                                    ),
                ("@Player2LossesSetsTotalS1"                                  , match.Player2LossesSetsTotalS1                                  ),
                ("@Player2WinsSetsLastYearS1"                                 , match.Player2WinsSetsLastYearS1                                 ),
                ("@Player2LossesSetsLastYearS1"                               , match.Player2LossesSetsLastYearS1                               ),
                ("@Player2WinsSetsLastMonthS1"                                , match.Player2WinsSetsLastMonthS1                                ),
                ("@Player2LossesSetsLastMonthS1"                              , match.Player2LossesSetsLastMonthS1                              ),
                ("@Player2WinsSetsLastWeekS1"                                 , match.Player2WinsSetsLastWeekS1                                 ),
                ("@Player2LossesSetsLastWeekS1"                               , match.Player2LossesSetsLastWeekS1                               ),
                ("@Player1WinsSetsTotalS2"                                    , match.Player1WinsSetsTotalS2                                    ),
                ("@Player1LossesSetsTotalS2"                                  , match.Player1LossesSetsTotalS2                                  ),
                ("@Player1WinsSetsLastYearS2"                                 , match.Player1WinsSetsLastYearS2                                 ),
                ("@Player1LossesSetsLastYearS2"                               , match.Player1LossesSetsLastYearS2                               ),
                ("@Player1WinsSetsLastMonthS2"                                , match.Player1WinsSetsLastMonthS2                                ),
                ("@Player1LossesSetsLastMonthS2"                              , match.Player1LossesSetsLastMonthS2                              ),
                ("@Player1WinsSetsLastWeekS2"                                 , match.Player1WinsSetsLastWeekS2                                 ),
                ("@Player1LossesSetsLastWeekS2"                               , match.Player1LossesSetsLastWeekS2                               ),
                ("@Player2WinsSetsTotalS2"                                    , match.Player2WinsSetsTotalS2                                    ),
                ("@Player2LossesSetsTotalS2"                                  , match.Player2LossesSetsTotalS2                                  ),
                ("@Player2WinsSetsLastYearS2"                                 , match.Player2WinsSetsLastYearS2                                 ),
                ("@Player2LossesSetsLastYearS2"                               , match.Player2LossesSetsLastYearS2                               ),
                ("@Player2WinsSetsLastMonthS2"                                , match.Player2WinsSetsLastMonthS2                                ),
                ("@Player2LossesSetsLastMonthS2"                              , match.Player2LossesSetsLastMonthS2                              ),
                ("@Player2WinsSetsLastWeekS2"                                 , match.Player2WinsSetsLastWeekS2                                 ),
                ("@Player2LossesSetsLastWeekS2"                               , match.Player2LossesSetsLastWeekS2                               ),
                ("@Player1WinsSetsTotalS3"                                    , match.Player1WinsSetsTotalS3                                    ),
                ("@Player1LossesSetsTotalS3"                                  , match.Player1LossesSetsTotalS3                                  ),
                ("@Player1WinsSetsLastYearS3"                                 , match.Player1WinsSetsLastYearS3                                 ),
                ("@Player1LossesSetsLastYearS3"                               , match.Player1LossesSetsLastYearS3                               ),
                ("@Player1WinsSetsLastMonthS3"                                , match.Player1WinsSetsLastMonthS3                                ),
                ("@Player1LossesSetsLastMonthS3"                              , match.Player1LossesSetsLastMonthS3                              ),
                ("@Player1WinsSetsLastWeekS3"                                 , match.Player1WinsSetsLastWeekS3                                 ),
                ("@Player1LossesSetsLastWeekS3"                               , match.Player1LossesSetsLastWeekS3                               ),
                ("@Player2WinsSetsTotalS3"                                    , match.Player2WinsSetsTotalS3                                    ),
                ("@Player2LossesSetsTotalS3"                                  , match.Player2LossesSetsTotalS3                                  ),
                ("@Player2WinsSetsLastYearS3"                                 , match.Player2WinsSetsLastYearS3                                 ),
                ("@Player2LossesSetsLastYearS3"                               , match.Player2LossesSetsLastYearS3                               ),
                ("@Player2WinsSetsLastMonthS3"                                , match.Player2WinsSetsLastMonthS3                                ),
                ("@Player2LossesSetsLastMonthS3"                              , match.Player2LossesSetsLastMonthS3                              ),
                ("@Player2WinsSetsLastWeekS3"                                 , match.Player2WinsSetsLastWeekS3                                 ),
                ("@Player2LossesSetsLastWeekS3"                               , match.Player2LossesSetsLastWeekS3                               ),
                ("@Player1WinsSetsTotalS4"                                    , match.Player1WinsSetsTotalS4                                    ),
                ("@Player1LossesSetsTotalS4"                                  , match.Player1LossesSetsTotalS4                                  ),
                ("@Player1WinsSetsLastYearS4"                                 , match.Player1WinsSetsLastYearS4                                 ),
                ("@Player1LossesSetsLastYearS4"                               , match.Player1LossesSetsLastYearS4                               ),
                ("@Player1WinsSetsLastMonthS4"                                , match.Player1WinsSetsLastMonthS4                                ),
                ("@Player1LossesSetsLastMonthS4"                              , match.Player1LossesSetsLastMonthS4                              ),
                ("@Player1WinsSetsLastWeekS4"                                 , match.Player1WinsSetsLastWeekS4                                 ),
                ("@Player1LossesSetsLastWeekS4"                               , match.Player1LossesSetsLastWeekS4                               ),
                ("@Player2WinsSetsTotalS4"                                    , match.Player2WinsSetsTotalS4                                    ),
                ("@Player2LossesSetsTotalS4"                                  , match.Player2LossesSetsTotalS4                                  ),
                ("@Player2WinsSetsLastYearS4"                                 , match.Player2WinsSetsLastYearS4                                 ),
                ("@Player2LossesSetsLastYearS4"                               , match.Player2LossesSetsLastYearS4                               ),
                ("@Player2WinsSetsLastMonthS4"                                , match.Player2WinsSetsLastMonthS4                                ),
                ("@Player2LossesSetsLastMonthS4"                              , match.Player2LossesSetsLastMonthS4                              ),
                ("@Player2WinsSetsLastWeekS4"                                 , match.Player2WinsSetsLastWeekS4                                 ),
                ("@Player2LossesSetsLastWeekS4"                               , match.Player2LossesSetsLastWeekS4                               ),
                ("@Player1WinsGamesTotal"                                     , match.Player1WinsGamesTotal                                     ),
                ("@Player1LossesGamesTotal"                                   , match.Player1LossesGamesTotal                                   ),
                ("@Player1WinsGamesLastYear"                                  , match.Player1WinsGamesLastYear                                  ),
                ("@Player1LossesGamesLastYear"                                , match.Player1LossesGamesLastYear                                ),
                ("@Player1WinsGamesLastMonth"                                 , match.Player1WinsGamesLastMonth                                 ),
                ("@Player1LossesGamesLastMonth"                               , match.Player1LossesGamesLastMonth                               ),
                ("@Player1WinsGamesLastWeek"                                  , match.Player1WinsGamesLastWeek                                  ),
                ("@Player1LossesGamesLastWeek"                                , match.Player1LossesGamesLastWeek                                ),
                ("@Player2WinsGamesTotal"                                     , match.Player2WinsGamesTotal                                     ),
                ("@Player2LossesGamesTotal"                                   , match.Player2LossesGamesTotal                                   ),
                ("@Player2WinsGamesLastYear"                                  , match.Player2WinsGamesLastYear                                  ),
                ("@Player2LossesGamesLastYear"                                , match.Player2LossesGamesLastYear                                ),
                ("@Player2WinsGamesLastMonth"                                 , match.Player2WinsGamesLastMonth                                 ),
                ("@Player2LossesGamesLastMonth"                               , match.Player2LossesGamesLastMonth                               ),
                ("@Player2WinsGamesLastWeek"                                  , match.Player2WinsGamesLastWeek                                  ),
                ("@Player2LossesGamesLastWeek"                                , match.Player2LossesGamesLastWeek                                ),
                ("@Player1WinsGamesTotalS1"                                   , match.Player1WinsGamesTotalS1                                   ),
                ("@Player1LossesGamesTotalS1"                                 , match.Player1LossesGamesTotalS1                                 ),
                ("@Player1WinsGamesLastYearS1"                                , match.Player1WinsGamesLastYearS1                                ),
                ("@Player1LossesGamesLastYearS1"                              , match.Player1LossesGamesLastYearS1                              ),
                ("@Player1WinsGamesLastMonthS1"                               , match.Player1WinsGamesLastMonthS1                               ),
                ("@Player1LossesGamesLastMonthS1"                             , match.Player1LossesGamesLastMonthS1                             ),
                ("@Player1WinsGamesLastWeekS1"                                , match.Player1WinsGamesLastWeekS1                                ),
                ("@Player1LossesGamesLastWeekS1"                              , match.Player1LossesGamesLastWeekS1                              ),
                ("@Player2WinsGamesTotalS1"                                   , match.Player2WinsGamesTotalS1                                   ),
                ("@Player2LossesGamesTotalS1"                                 , match.Player2LossesGamesTotalS1                                 ),
                ("@Player2WinsGamesLastYearS1"                                , match.Player2WinsGamesLastYearS1                                ),
                ("@Player2LossesGamesLastYearS1"                              , match.Player2LossesGamesLastYearS1                              ),
                ("@Player2WinsGamesLastMonthS1"                               , match.Player2WinsGamesLastMonthS1                               ),
                ("@Player2LossesGamesLastMonthS1"                             , match.Player2LossesGamesLastMonthS1                             ),
                ("@Player2WinsGamesLastWeekS1"                                , match.Player2WinsGamesLastWeekS1                                ),
                ("@Player2LossesGamesLastWeekS1"                              , match.Player2LossesGamesLastWeekS1                              ),
                ("@Player1WinsGamesTotalS2"                                   , match.Player1WinsGamesTotalS2                                   ),
                ("@Player1LossesGamesTotalS2"                                 , match.Player1LossesGamesTotalS2                                 ),
                ("@Player1WinsGamesLastYearS2"                                , match.Player1WinsGamesLastYearS2                                ),
                ("@Player1LossesGamesLastYearS2"                              , match.Player1LossesGamesLastYearS2                              ),
                ("@Player1WinsGamesLastMonthS2"                               , match.Player1WinsGamesLastMonthS2                               ),
                ("@Player1LossesGamesLastMonthS2"                             , match.Player1LossesGamesLastMonthS2                             ),
                ("@Player1WinsGamesLastWeekS2"                                , match.Player1WinsGamesLastWeekS2                                ),
                ("@Player1LossesGamesLastWeekS2"                              , match.Player1LossesGamesLastWeekS2                              ),
                ("@Player2WinsGamesTotalS2"                                   , match.Player2WinsGamesTotalS2                                   ),
                ("@Player2LossesGamesTotalS2"                                 , match.Player2LossesGamesTotalS2                                 ),
                ("@Player2WinsGamesLastYearS2"                                , match.Player2WinsGamesLastYearS2                                ),
                ("@Player2LossesGamesLastYearS2"                              , match.Player2LossesGamesLastYearS2                              ),
                ("@Player2WinsGamesLastMonthS2"                               , match.Player2WinsGamesLastMonthS2                               ),
                ("@Player2LossesGamesLastMonthS2"                             , match.Player2LossesGamesLastMonthS2                             ),
                ("@Player2WinsGamesLastWeekS2"                                , match.Player2WinsGamesLastWeekS2                                ),
                ("@Player2LossesGamesLastWeekS2"                              , match.Player2LossesGamesLastWeekS2                              ),
                ("@Player1WinsGamesTotalS3"                                   , match.Player1WinsGamesTotalS3                                   ),
                ("@Player1LossesGamesTotalS3"                                 , match.Player1LossesGamesTotalS3                                 ),
                ("@Player1WinsGamesLastYearS3"                                , match.Player1WinsGamesLastYearS3                                ),
                ("@Player1LossesGamesLastYearS3"                              , match.Player1LossesGamesLastYearS3                              ),
                ("@Player1WinsGamesLastMonthS3"                               , match.Player1WinsGamesLastMonthS3                               ),
                ("@Player1LossesGamesLastMonthS3"                             , match.Player1LossesGamesLastMonthS3                             ),
                ("@Player1WinsGamesLastWeekS3"                                , match.Player1WinsGamesLastWeekS3                                ),
                ("@Player1LossesGamesLastWeekS3"                              , match.Player1LossesGamesLastWeekS3                              ),
                ("@Player2WinsGamesTotalS3"                                   , match.Player2WinsGamesTotalS3                                   ),
                ("@Player2LossesGamesTotalS3"                                 , match.Player2LossesGamesTotalS3                                 ),
                ("@Player2WinsGamesLastYearS3"                                , match.Player2WinsGamesLastYearS3                                ),
                ("@Player2LossesGamesLastYearS3"                              , match.Player2LossesGamesLastYearS3                              ),
                ("@Player2WinsGamesLastMonthS3"                               , match.Player2WinsGamesLastMonthS3                               ),
                ("@Player2LossesGamesLastMonthS3"                             , match.Player2LossesGamesLastMonthS3                             ),
                ("@Player2WinsGamesLastWeekS3"                                , match.Player2WinsGamesLastWeekS3                                ),
                ("@Player2LossesGamesLastWeekS3"                              , match.Player2LossesGamesLastWeekS3                              ),
                ("@Player1WinsGamesTotalS4"                                   , match.Player1WinsGamesTotalS4                                   ),
                ("@Player1LossesGamesTotalS4"                                 , match.Player1LossesGamesTotalS4                                 ),
                ("@Player1WinsGamesLastYearS4"                                , match.Player1WinsGamesLastYearS4                                ),
                ("@Player1LossesGamesLastYearS4"                              , match.Player1LossesGamesLastYearS4                              ),
                ("@Player1WinsGamesLastMonthS4"                               , match.Player1WinsGamesLastMonthS4                               ),
                ("@Player1LossesGamesLastMonthS4"                             , match.Player1LossesGamesLastMonthS4                             ),
                ("@Player1WinsGamesLastWeekS4"                                , match.Player1WinsGamesLastWeekS4                                ),
                ("@Player1LossesGamesLastWeekS4"                              , match.Player1LossesGamesLastWeekS4                              ),
                ("@Player2WinsGamesTotalS4"                                   , match.Player2WinsGamesTotalS4                                   ),
                ("@Player2LossesGamesTotalS4"                                 , match.Player2LossesGamesTotalS4                                 ),
                ("@Player2WinsGamesLastYearS4"                                , match.Player2WinsGamesLastYearS4                                ),
                ("@Player2LossesGamesLastYearS4"                              , match.Player2LossesGamesLastYearS4                              ),
                ("@Player2WinsGamesLastMonthS4"                               , match.Player2WinsGamesLastMonthS4                               ),
                ("@Player2LossesGamesLastMonthS4"                             , match.Player2LossesGamesLastMonthS4                             ),
                ("@Player2WinsGamesLastWeekS4"                                , match.Player2WinsGamesLastWeekS4                                ),
                ("@Player2LossesGamesLastWeekS4"                              , match.Player2LossesGamesLastWeekS4                              ),
                ("@Player1DaysSinceLastWin"                                   , match.Player1DaysSinceLastWin                                   ),
                ("@Player2DaysSinceLastWin"                                   , match.Player2DaysSinceLastWin                                   ),
                ("@Player1DaysSinceLastWinS1"                                 , match.Player1DaysSinceLastWinS1                                 ),
                ("@Player2DaysSinceLastWinS1"                                 , match.Player2DaysSinceLastWinS1                                 ),
                ("@Player1DaysSinceLastWinS2"                                 , match.Player1DaysSinceLastWinS2                                 ),
                ("@Player2DaysSinceLastWinS2"                                 , match.Player2DaysSinceLastWinS2                                 ),
                ("@Player1DaysSinceLastWinS3"                                 , match.Player1DaysSinceLastWinS3                                 ),
                ("@Player2DaysSinceLastWinS3"                                 , match.Player2DaysSinceLastWinS3                                 ),
                ("@Player1DaysSinceLastWinS4"                                 , match.Player1DaysSinceLastWinS4                                 ),
                ("@Player2DaysSinceLastWinS4"                                 , match.Player2DaysSinceLastWinS4                                 ),
                ("@Player1DaysSinceLastLoss"                                  , match.Player1DaysSinceLastLoss                                  ),
                ("@Player2DaysSinceLastLoss"                                  , match.Player2DaysSinceLastLoss                                  ),
                ("@Player1DaysSinceLastLossS1"                                , match.Player1DaysSinceLastLossS1                                ),
                ("@Player2DaysSinceLastLossS1"                                , match.Player2DaysSinceLastLossS1                                ),
                ("@Player1DaysSinceLastLossS2"                                , match.Player1DaysSinceLastLossS2                                ),
                ("@Player2DaysSinceLastLossS2"                                , match.Player2DaysSinceLastLossS2                                ),
                ("@Player1DaysSinceLastLossS3"                                , match.Player1DaysSinceLastLossS3                                ),
                ("@Player2DaysSinceLastLossS3"                                , match.Player2DaysSinceLastLossS3                                ),
                ("@Player1DaysSinceLastLossS4"                                , match.Player1DaysSinceLastLossS4                                ),
                ("@Player2DaysSinceLastLossS4"                                , match.Player2DaysSinceLastLossS4                                ),
                ("@Player1TotalWinsAsFavourite"                               , match.Player1TotalWinsAsFavourite                               ),
                ("@Player2TotalWinsAsFavourite"                               , match.Player2TotalWinsAsFavourite                               ),
                ("@Player1TotalWinsAsUnderdog"                                , match.Player1TotalWinsAsUnderdog                                ),
                ("@Player2TotalWinsAsUnderdog"                                , match.Player2TotalWinsAsUnderdog                                ),
                ("@Player1TotalLossesAsFavourite"                             , match.Player1TotalLossesAsFavourite                             ),
                ("@Player2TotalLossesAsFavourite"                             , match.Player2TotalLossesAsFavourite                             ),
                ("@Player1TotalLossesAsUnderdog"                              , match.Player1TotalLossesAsUnderdog                              ),
                ("@Player2TotalLossesAsUnderdog"                              , match.Player2TotalLossesAsUnderdog                              ),
                ("@Player1AverageWinningProbabilityAtWonAsFavourite"          , match.Player1AverageWinningProbabilityAtWonAsFavourite          ),
                ("@Player2AverageWinningProbabilityAtWonAsFavourite"          , match.Player2AverageWinningProbabilityAtWonAsFavourite          ),
                ("@Player1AverageWinningProbabilityAtWonAsUnderdog"           , match.Player1AverageWinningProbabilityAtWonAsUnderdog           ),
                ("@Player2AverageWinningProbabilityAtWonAsUnderdog"           , match.Player2AverageWinningProbabilityAtWonAsUnderdog           ),
                ("@Player1AverageWinningProbabilityAtLossAsFavourite"         , match.Player1AverageWinningProbabilityAtLossAsFavourite         ),
                ("@Player2AverageWinningProbabilityAtLossAsFavourite"         , match.Player2AverageWinningProbabilityAtLossAsFavourite         ),
                ("@Player1AverageWinningProbabilityAtLossAsUnderdog"          , match.Player1AverageWinningProbabilityAtLossAsUnderdog          ),
                ("@Player2AverageWinningProbabilityAtLossAsUnderdog"          , match.Player2AverageWinningProbabilityAtLossAsUnderdog          ),
                ("@Player1TotalWinsAsFavouriteLastYear"                       , match.Player1TotalWinsAsFavouriteLastYear                       ),
                ("@Player2TotalWinsAsFavouriteLastYear"                       , match.Player2TotalWinsAsFavouriteLastYear                       ),
                ("@Player1TotalWinsAsUnderdogLastYear"                        , match.Player1TotalWinsAsUnderdogLastYear                        ),
                ("@Player2TotalWinsAsUnderdogLastYear"                        , match.Player2TotalWinsAsUnderdogLastYear                        ),
                ("@Player1TotalLossesAsFavouriteLastYear"                     , match.Player1TotalLossesAsFavouriteLastYear                     ),
                ("@Player2TotalLossesAsFavouriteLastYear"                     , match.Player2TotalLossesAsFavouriteLastYear                     ),
                ("@Player1TotalLossesAsUnderdogLastYear"                      , match.Player1TotalLossesAsUnderdogLastYear                      ),
                ("@Player2TotalLossesAsUnderdogLastYear"                      , match.Player2TotalLossesAsUnderdogLastYear                      ),
                ("@Player1AverageWinningProbabilityAtWonAsFavouriteLastYear"  , match.Player1AverageWinningProbabilityAtWonAsFavouriteLastYear  ),
                ("@Player2AverageWinningProbabilityAtWonAsFavouriteLastYear"  , match.Player2AverageWinningProbabilityAtWonAsFavouriteLastYear  ),
                ("@Player1AverageWinningProbabilityAtWonAsUnderdogLastYear"   , match.Player1AverageWinningProbabilityAtWonAsUnderdogLastYear   ),
                ("@Player2AverageWinningProbabilityAtWonAsUnderdogLastYear"   , match.Player2AverageWinningProbabilityAtWonAsUnderdogLastYear   ),
                ("@Player1AverageWinningProbabilityAtLossAsFavouriteLastYear" , match.Player1AverageWinningProbabilityAtLossAsFavouriteLastYear ),
                ("@Player2AverageWinningProbabilityAtLossAsFavouriteLastYear" , match.Player2AverageWinningProbabilityAtLossAsFavouriteLastYear ),
                ("@Player1AverageWinningProbabilityAtLossAsUnderdogLastYear"  , match.Player1AverageWinningProbabilityAtLossAsUnderdogLastYear  ),
                ("@Player2AverageWinningProbabilityAtLossAsUnderdogLastYear"  , match.Player2AverageWinningProbabilityAtLossAsUnderdogLastYear  ),
                ("@Player1TotalWinsAsFavouriteLastMonth"                      , match.Player1TotalWinsAsFavouriteLastMonth                      ),
                ("@Player2TotalWinsAsFavouriteLastMonth"                      , match.Player2TotalWinsAsFavouriteLastMonth                      ),
                ("@Player1TotalWinsAsUnderdogLastMonth"                       , match.Player1TotalWinsAsUnderdogLastMonth                       ),
                ("@Player2TotalWinsAsUnderdogLastMonth"                       , match.Player2TotalWinsAsUnderdogLastMonth                       ),
                ("@Player1TotalLossesAsFavouriteLastMonth"                    , match.Player1TotalLossesAsFavouriteLastMonth                    ),
                ("@Player2TotalLossesAsFavouriteLastMonth"                    , match.Player2TotalLossesAsFavouriteLastMonth                    ),
                ("@Player1TotalLossesAsUnderdogLastMonth"                     , match.Player1TotalLossesAsUnderdogLastMonth                     ),
                ("@Player2TotalLossesAsUnderdogLastMonth"                     , match.Player2TotalLossesAsUnderdogLastMonth                     ),
                ("@Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth" , match.Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth ),
                ("@Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth" , match.Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth ),
                ("@Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth"  , match.Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth  ),
                ("@Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth"  , match.Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth  ),
                ("@Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth", match.Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth),
                ("@Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth", match.Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth),
                ("@Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth" , match.Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth ),
                ("@Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth" , match.Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth ),
                ("@Player1TotalWinsAsFavouriteLastWeek"                       , match.Player1TotalWinsAsFavouriteLastWeek                       ),
                ("@Player2TotalWinsAsFavouriteLastWeek"                       , match.Player2TotalWinsAsFavouriteLastWeek                       ),
                ("@Player1TotalWinsAsUnderdogLastWeek"                        , match.Player1TotalWinsAsUnderdogLastWeek                        ),
                ("@Player2TotalWinsAsUnderdogLastWeek"                        , match.Player2TotalWinsAsUnderdogLastWeek                        ),
                ("@Player1TotalLossesAsFavouriteLastWeek"                     , match.Player1TotalLossesAsFavouriteLastWeek                     ),
                ("@Player2TotalLossesAsFavouriteLastWeek"                     , match.Player2TotalLossesAsFavouriteLastWeek                     ),
                ("@Player1TotalLossesAsUnderdogLastWeek"                      , match.Player1TotalLossesAsUnderdogLastWeek                      ),
                ("@Player2TotalLossesAsUnderdogLastWeek"                      , match.Player2TotalLossesAsUnderdogLastWeek                      ),
                ("@Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek"  , match.Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek  ),
                ("@Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek"  , match.Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek  ),
                ("@Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek"   , match.Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek   ),
                ("@Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek"   , match.Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek   ),
                ("@Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek" , match.Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek ),
                ("@Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek" , match.Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek ),
                ("@Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek"  , match.Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek  ),
                ("@Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek"  , match.Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek  ),
                ("@Player1H2H"                                                , match.Player1H2H                                                ),
                ("@Player2H2H"                                                , match.Player2H2H                                                ),
                ("@Player1H2HS1"                                              , match.Player1H2HS1                                              ),
                ("@Player2H2HS1"                                              , match.Player2H2HS1                                              ),
                ("@Player1H2HS2"                                              , match.Player1H2HS2                                              ),
                ("@Player2H2HS2"                                              , match.Player2H2HS2                                              ),
                ("@Player1H2HS3"                                              , match.Player1H2HS3                                              ),
                ("@Player2H2HS3"                                              , match.Player2H2HS3                                              ),
                ("@Player1H2HS4"                                              , match.Player1H2HS4                                              ),
                ("@Player2H2HS4"                                              , match.Player2H2HS4                                              ),
                ("@Player1H2HOld"                                             , match.Player1H2HOld                                             ),
                ("@Player2H2HOld"                                             , match.Player2H2HOld                                             ),
                ("@Player1H2HOldS1"                                           , match.Player1H2HOldS1                                           ),
                ("@Player2H2HOldS1"                                           , match.Player2H2HOldS1                                           ),
                ("@Player1H2HOldS2"                                           , match.Player1H2HOldS2                                           ),
                ("@Player2H2HOldS2"                                           , match.Player2H2HOldS2                                           ),
                ("@Player1H2HOldS3"                                           , match.Player1H2HOldS3                                           ),
                ("@Player2H2HOldS3"                                           , match.Player2H2HOldS3                                           ),
                ("@Player1H2HOldS4"                                           , match.Player1H2HOldS4                                           ),
                ("@Player2H2HOldS4"                                           , match.Player2H2HOldS4                                           ),
                ("@Player1H2HTrueSkillMeanM"                                  , match.Player1H2HTrueSkillMeanM                                  ),
                ("@Player1H2HTrueSkillStandardDeviationM"                     , match.Player1H2HTrueSkillStandardDeviationM                     ),
                ("@Player2H2HTrueSkillMeanM"                                  , match.Player2H2HTrueSkillMeanM                                  ),
                ("@Player2H2HTrueSkillStandardDeviationM"                     , match.Player2H2HTrueSkillStandardDeviationM                     ),
                ("@Player1H2HTrueSkillMeanOldM"                               , match.Player1H2HTrueSkillMeanOldM                               ),
                ("@Player1H2HTrueSkillStandardDeviationOldM"                  , match.Player1H2HTrueSkillStandardDeviationOldM                  ),
                ("@Player2H2HTrueSkillMeanOldM"                               , match.Player2H2HTrueSkillMeanOldM                               ),
                ("@Player2H2HTrueSkillStandardDeviationOldM"                  , match.Player2H2HTrueSkillStandardDeviationOldM                  ),
                ("@WinProbabilityPlayer1H2HM"                                 , match.WinProbabilityPlayer1H2HM                                 ),
                ("@Player1H2HTrueSkillMeanSM"                                 , match.Player1H2HTrueSkillMeanSM                                 ),
                ("@Player1H2HTrueSkillStandardDeviationSM"                    , match.Player1H2HTrueSkillStandardDeviationSM                    ),
                ("@Player2H2HTrueSkillMeanSM"                                 , match.Player2H2HTrueSkillMeanSM                                 ),
                ("@Player2H2HTrueSkillStandardDeviationSM"                    , match.Player2H2HTrueSkillStandardDeviationSM                    ),
                ("@Player1H2HTrueSkillMeanOldSM"                              , match.Player1H2HTrueSkillMeanOldSM                              ),
                ("@Player1H2HTrueSkillStandardDeviationOldSM"                 , match.Player1H2HTrueSkillStandardDeviationOldSM                 ),
                ("@Player2H2HTrueSkillMeanOldSM"                              , match.Player2H2HTrueSkillMeanOldSM                              ),
                ("@Player2H2HTrueSkillStandardDeviationOldSM"                 , match.Player2H2HTrueSkillStandardDeviationOldSM                 ),
                ("@WinProbabilityPlayer1H2HSM"                                , match.WinProbabilityPlayer1H2HSM                                ),
                ("@Player1H2HTrueSkillMeanGSM"                                , match.Player1H2HTrueSkillMeanGSM                                ),
                ("@Player1H2HTrueSkillStandardDeviationGSM"                   , match.Player1H2HTrueSkillStandardDeviationGSM                   ),
                ("@Player2H2HTrueSkillMeanGSM"                                , match.Player2H2HTrueSkillMeanGSM                                ),
                ("@Player2H2HTrueSkillStandardDeviationGSM"                   , match.Player2H2HTrueSkillStandardDeviationGSM                   ),
                ("@Player1H2HTrueSkillMeanOldGSM"                             , match.Player1H2HTrueSkillMeanOldGSM                             ),
                ("@Player1H2HTrueSkillStandardDeviationOldGSM"                , match.Player1H2HTrueSkillStandardDeviationOldGSM                ),
                ("@Player2H2HTrueSkillMeanOldGSM"                             , match.Player2H2HTrueSkillMeanOldGSM                             ),
                ("@Player2H2HTrueSkillStandardDeviationOldGSM"                , match.Player2H2HTrueSkillStandardDeviationOldGSM                ),
                ("@WinProbabilityPlayer1H2HGSM"                               , match.WinProbabilityPlayer1H2HGSM                               ),
                ("@Player1H2HTrueSkillMeanMS1"                                , match.Player1H2HTrueSkillMeanMS1                                ),
                ("@Player1H2HTrueSkillStandardDeviationMS1"                   , match.Player1H2HTrueSkillStandardDeviationMS1                   ),
                ("@Player2H2HTrueSkillMeanMS1"                                , match.Player2H2HTrueSkillMeanMS1                                ),
                ("@Player2H2HTrueSkillStandardDeviationMS1"                   , match.Player2H2HTrueSkillStandardDeviationMS1                   ),
                ("@Player1H2HTrueSkillMeanOldMS1"                             , match.Player1H2HTrueSkillMeanOldMS1                             ),
                ("@Player1H2HTrueSkillStandardDeviationOldMS1"                , match.Player1H2HTrueSkillStandardDeviationOldMS1                ),
                ("@Player2H2HTrueSkillMeanOldMS1"                             , match.Player2H2HTrueSkillMeanOldMS1                             ),
                ("@Player2H2HTrueSkillStandardDeviationOldMS1"                , match.Player2H2HTrueSkillStandardDeviationOldMS1                ),
                ("@WinProbabilityPlayer1H2HMS1"                               , match.WinProbabilityPlayer1H2HMS1                               ),
                ("@Player1H2HTrueSkillMeanSMS1"                               , match.Player1H2HTrueSkillMeanSMS1                               ),
                ("@Player1H2HTrueSkillStandardDeviationSMS1"                  , match.Player1H2HTrueSkillStandardDeviationSMS1                  ),
                ("@Player2H2HTrueSkillMeanSMS1"                               , match.Player2H2HTrueSkillMeanSMS1                               ),
                ("@Player2H2HTrueSkillStandardDeviationSMS1"                  , match.Player2H2HTrueSkillStandardDeviationSMS1                  ),
                ("@Player1H2HTrueSkillMeanOldSMS1"                            , match.Player1H2HTrueSkillMeanOldSMS1                            ),
                ("@Player1H2HTrueSkillStandardDeviationOldSMS1"               , match.Player1H2HTrueSkillStandardDeviationOldSMS1               ),
                ("@Player2H2HTrueSkillMeanOldSMS1"                            , match.Player2H2HTrueSkillMeanOldSMS1                            ),
                ("@Player2H2HTrueSkillStandardDeviationOldSMS1"               , match.Player2H2HTrueSkillStandardDeviationOldSMS1               ),
                ("@WinProbabilityPlayer1H2HSMS1"                              , match.WinProbabilityPlayer1H2HSMS1                              ),
                ("@Player1H2HTrueSkillMeanGSMS1"                              , match.Player1H2HTrueSkillMeanGSMS1                              ),
                ("@Player1H2HTrueSkillStandardDeviationGSMS1"                 , match.Player1H2HTrueSkillStandardDeviationGSMS1                 ),
                ("@Player2H2HTrueSkillMeanGSMS1"                              , match.Player2H2HTrueSkillMeanGSMS1                              ),
                ("@Player2H2HTrueSkillStandardDeviationGSMS1"                 , match.Player2H2HTrueSkillStandardDeviationGSMS1                 ),
                ("@Player1H2HTrueSkillMeanOldGSMS1"                           , match.Player1H2HTrueSkillMeanOldGSMS1                           ),
                ("@Player1H2HTrueSkillStandardDeviationOldGSMS1"              , match.Player1H2HTrueSkillStandardDeviationOldGSMS1              ),
                ("@Player2H2HTrueSkillMeanOldGSMS1"                           , match.Player2H2HTrueSkillMeanOldGSMS1                           ),
                ("@Player2H2HTrueSkillStandardDeviationOldGSMS1"              , match.Player2H2HTrueSkillStandardDeviationOldGSMS1              ),
                ("@WinProbabilityPlayer1H2HGSMS1"                             , match.WinProbabilityPlayer1H2HGSMS1                             ),
                ("@Player1H2HTrueSkillMeanMS2"                                , match.Player1H2HTrueSkillMeanMS2                                ),
                ("@Player1H2HTrueSkillStandardDeviationMS2"                   , match.Player1H2HTrueSkillStandardDeviationMS2                   ),
                ("@Player2H2HTrueSkillMeanMS2"                                , match.Player2H2HTrueSkillMeanMS2                                ),
                ("@Player2H2HTrueSkillStandardDeviationMS2"                   , match.Player2H2HTrueSkillStandardDeviationMS2                   ),
                ("@Player1H2HTrueSkillMeanOldMS2"                             , match.Player1H2HTrueSkillMeanOldMS2                             ),
                ("@Player1H2HTrueSkillStandardDeviationOldMS2"                , match.Player1H2HTrueSkillStandardDeviationOldMS2                ),
                ("@Player2H2HTrueSkillMeanOldMS2"                             , match.Player2H2HTrueSkillMeanOldMS2                             ),
                ("@Player2H2HTrueSkillStandardDeviationOldMS2"                , match.Player2H2HTrueSkillStandardDeviationOldMS2                ),
                ("@WinProbabilityPlayer1H2HMS2"                               , match.WinProbabilityPlayer1H2HMS2                               ),
                ("@Player1H2HTrueSkillMeanSMS2"                               , match.Player1H2HTrueSkillMeanSMS2                               ),
                ("@Player1H2HTrueSkillStandardDeviationSMS2"                  , match.Player1H2HTrueSkillStandardDeviationSMS2                  ),
                ("@Player2H2HTrueSkillMeanSMS2"                               , match.Player2H2HTrueSkillMeanSMS2                               ),
                ("@Player2H2HTrueSkillStandardDeviationSMS2"                  , match.Player2H2HTrueSkillStandardDeviationSMS2                  ),
                ("@Player1H2HTrueSkillMeanOldSMS2"                            , match.Player1H2HTrueSkillMeanOldSMS2                            ),
                ("@Player1H2HTrueSkillStandardDeviationOldSMS2"               , match.Player1H2HTrueSkillStandardDeviationOldSMS2               ),
                ("@Player2H2HTrueSkillMeanOldSMS2"                            , match.Player2H2HTrueSkillMeanOldSMS2                            ),
                ("@Player2H2HTrueSkillStandardDeviationOldSMS2"               , match.Player2H2HTrueSkillStandardDeviationOldSMS2               ),
                ("@WinProbabilityPlayer1H2HSMS2"                              , match.WinProbabilityPlayer1H2HSMS2                              ),
                ("@Player1H2HTrueSkillMeanGSMS2"                              , match.Player1H2HTrueSkillMeanGSMS2                              ),
                ("@Player1H2HTrueSkillStandardDeviationGSMS2"                 , match.Player1H2HTrueSkillStandardDeviationGSMS2                 ),
                ("@Player2H2HTrueSkillMeanGSMS2"                              , match.Player2H2HTrueSkillMeanGSMS2                              ),
                ("@Player2H2HTrueSkillStandardDeviationGSMS2"                 , match.Player2H2HTrueSkillStandardDeviationGSMS2                 ),
                ("@Player1H2HTrueSkillMeanOldGSMS2"                           , match.Player1H2HTrueSkillMeanOldGSMS2                           ),
                ("@Player1H2HTrueSkillStandardDeviationOldGSMS2"              , match.Player1H2HTrueSkillStandardDeviationOldGSMS2              ),
                ("@Player2H2HTrueSkillMeanOldGSMS2"                           , match.Player2H2HTrueSkillMeanOldGSMS2                           ),
                ("@Player2H2HTrueSkillStandardDeviationOldGSMS2"              , match.Player2H2HTrueSkillStandardDeviationOldGSMS2              ),
                ("@WinProbabilityPlayer1H2HGSMS2"                             , match.WinProbabilityPlayer1H2HGSMS2                             ),
                ("@Player1H2HTrueSkillMeanMS3"                                , match.Player1H2HTrueSkillMeanMS3                                ),
                ("@Player1H2HTrueSkillStandardDeviationMS3"                   , match.Player1H2HTrueSkillStandardDeviationMS3                   ),
                ("@Player2H2HTrueSkillMeanMS3"                                , match.Player2H2HTrueSkillMeanMS3                                ),
                ("@Player2H2HTrueSkillStandardDeviationMS3"                   , match.Player2H2HTrueSkillStandardDeviationMS3                   ),
                ("@Player1H2HTrueSkillMeanOldMS3"                             , match.Player1H2HTrueSkillMeanOldMS3                             ),
                ("@Player1H2HTrueSkillStandardDeviationOldMS3"                , match.Player1H2HTrueSkillStandardDeviationOldMS3                ),
                ("@Player2H2HTrueSkillMeanOldMS3"                             , match.Player2H2HTrueSkillMeanOldMS3                             ),
                ("@Player2H2HTrueSkillStandardDeviationOldMS3"                , match.Player2H2HTrueSkillStandardDeviationOldMS3                ),
                ("@WinProbabilityPlayer1H2HMS3"                               , match.WinProbabilityPlayer1H2HMS3                               ),
                ("@Player1H2HTrueSkillMeanSMS3"                               , match.Player1H2HTrueSkillMeanSMS3                               ),
                ("@Player1H2HTrueSkillStandardDeviationSMS3"                  , match.Player1H2HTrueSkillStandardDeviationSMS3                  ),
                ("@Player2H2HTrueSkillMeanSMS3"                               , match.Player2H2HTrueSkillMeanSMS3                               ),
                ("@Player2H2HTrueSkillStandardDeviationSMS3"                  , match.Player2H2HTrueSkillStandardDeviationSMS3                  ),
                ("@Player1H2HTrueSkillMeanOldSMS3"                            , match.Player1H2HTrueSkillMeanOldSMS3                            ),
                ("@Player1H2HTrueSkillStandardDeviationOldSMS3"               , match.Player1H2HTrueSkillStandardDeviationOldSMS3               ),
                ("@Player2H2HTrueSkillMeanOldSMS3"                            , match.Player2H2HTrueSkillMeanOldSMS3                            ),
                ("@Player2H2HTrueSkillStandardDeviationOldSMS3"               , match.Player2H2HTrueSkillStandardDeviationOldSMS3               ),
                ("@WinProbabilityPlayer1H2HSMS3"                              , match.WinProbabilityPlayer1H2HSMS3                              ),
                ("@Player1H2HTrueSkillMeanGSMS3"                              , match.Player1H2HTrueSkillMeanGSMS3                              ),
                ("@Player1H2HTrueSkillStandardDeviationGSMS3"                 , match.Player1H2HTrueSkillStandardDeviationGSMS3                 ),
                ("@Player2H2HTrueSkillMeanGSMS3"                              , match.Player2H2HTrueSkillMeanGSMS3                              ),
                ("@Player2H2HTrueSkillStandardDeviationGSMS3"                 , match.Player2H2HTrueSkillStandardDeviationGSMS3                 ),
                ("@Player1H2HTrueSkillMeanOldGSMS3"                           , match.Player1H2HTrueSkillMeanOldGSMS3                           ),
                ("@Player1H2HTrueSkillStandardDeviationOldGSMS3"              , match.Player1H2HTrueSkillStandardDeviationOldGSMS3              ),
                ("@Player2H2HTrueSkillMeanOldGSMS3"                           , match.Player2H2HTrueSkillMeanOldGSMS3                           ),
                ("@Player2H2HTrueSkillStandardDeviationOldGSMS3"              , match.Player2H2HTrueSkillStandardDeviationOldGSMS3              ),
                ("@WinProbabilityPlayer1H2HGSMS3"                             , match.WinProbabilityPlayer1H2HGSMS3                             ),
                ("@Player1H2HTrueSkillMeanMS4"                                , match.Player1H2HTrueSkillMeanMS4                                ),
                ("@Player1H2HTrueSkillStandardDeviationMS4"                   , match.Player1H2HTrueSkillStandardDeviationMS4                   ),
                ("@Player2H2HTrueSkillMeanMS4"                                , match.Player2H2HTrueSkillMeanMS4                                ),
                ("@Player2H2HTrueSkillStandardDeviationMS4"                   , match.Player2H2HTrueSkillStandardDeviationMS4                   ),
                ("@Player1H2HTrueSkillMeanOldMS4"                             , match.Player1H2HTrueSkillMeanOldMS4                             ),
                ("@Player1H2HTrueSkillStandardDeviationOldMS4"                , match.Player1H2HTrueSkillStandardDeviationOldMS4                ),
                ("@Player2H2HTrueSkillMeanOldMS4"                             , match.Player2H2HTrueSkillMeanOldMS4                             ),
                ("@Player2H2HTrueSkillStandardDeviationOldMS4"                , match.Player2H2HTrueSkillStandardDeviationOldMS4                ),
                ("@WinProbabilityPlayer1H2HMS4"                               , match.WinProbabilityPlayer1H2HMS4                               ),
                ("@Player1H2HTrueSkillMeanSMS4"                               , match.Player1H2HTrueSkillMeanSMS4                               ),
                ("@Player1H2HTrueSkillStandardDeviationSMS4"                  , match.Player1H2HTrueSkillStandardDeviationSMS4                  ),
                ("@Player2H2HTrueSkillMeanSMS4"                               , match.Player2H2HTrueSkillMeanSMS4                               ),
                ("@Player2H2HTrueSkillStandardDeviationSMS4"                  , match.Player2H2HTrueSkillStandardDeviationSMS4                  ),
                ("@Player1H2HTrueSkillMeanOldSMS4"                            , match.Player1H2HTrueSkillMeanOldSMS4                            ),
                ("@Player1H2HTrueSkillStandardDeviationOldSMS4"               , match.Player1H2HTrueSkillStandardDeviationOldSMS4               ),
                ("@Player2H2HTrueSkillMeanOldSMS4"                            , match.Player2H2HTrueSkillMeanOldSMS4                            ),
                ("@Player2H2HTrueSkillStandardDeviationOldSMS4"               , match.Player2H2HTrueSkillStandardDeviationOldSMS4               ),
                ("@WinProbabilityPlayer1H2HSMS4"                              , match.WinProbabilityPlayer1H2HSMS4                              ),
                ("@Player1H2HTrueSkillMeanGSMS4"                              , match.Player1H2HTrueSkillMeanGSMS4                              ),
                ("@Player1H2HTrueSkillStandardDeviationGSMS4"                 , match.Player1H2HTrueSkillStandardDeviationGSMS4                 ),
                ("@Player2H2HTrueSkillMeanGSMS4"                              , match.Player2H2HTrueSkillMeanGSMS4                              ),
                ("@Player2H2HTrueSkillStandardDeviationGSMS4"                 , match.Player2H2HTrueSkillStandardDeviationGSMS4                 ),
                ("@Player1H2HTrueSkillMeanOldGSMS4"                           , match.Player1H2HTrueSkillMeanOldGSMS4                           ),
                ("@Player1H2HTrueSkillStandardDeviationOldGSMS4"              , match.Player1H2HTrueSkillStandardDeviationOldGSMS4              ),
                ("@Player2H2HTrueSkillMeanOldGSMS4"                           , match.Player2H2HTrueSkillMeanOldGSMS4                           ),
                ("@Player2H2HTrueSkillStandardDeviationOldGSMS4"              , match.Player2H2HTrueSkillStandardDeviationOldGSMS4              ),
                ("@WinProbabilityPlayer1H2HGSMS4"                             , match.WinProbabilityPlayer1H2HGSMS4                             ),
                ("@Player1Streak"                                             , match.Player1Streak                                             ),
                ("@Player2Streak"                                             , match.Player2Streak                                             ),
                ("@Player1StreakS1"                                           , match.Player1StreakS1                                           ),
                ("@Player2StreakS1"                                           , match.Player2StreakS1                                           ),
                ("@Player1StreakS2"                                           , match.Player1StreakS2                                           ),
                ("@Player2StreakS2"                                           , match.Player2StreakS2                                           ),
                ("@Player1StreakS3"                                           , match.Player1StreakS3                                           ),
                ("@Player2StreakS3"                                           , match.Player2StreakS3                                           ),
                ("@Player1StreakS4"                                           , match.Player1StreakS4                                           ),
                ("@Player2StreakS4"                                           , match.Player2StreakS4                                           ),
                ("@P1SetsWon"                                                 , match.P1SetsWon                                                 ),
                ("@P2SetsWon"                                                 , match.P2SetsWon                                                 ),
                ("@P1GamesWon"                                                , match.P1GamesWon                                                ),
                ("@P2GamesWon"                                                , match.P2GamesWon                                                ),
                ("@P1SetsLoss"                                                , match.P1SetsLoss                                                ),
                ("@P2SetsLoss"                                                , match.P2SetsLoss                                                ),
                ("@P1GamesLoss"                                               , match.P1GamesLoss                                               ),
                ("@P2GamesLoss"                                               , match.P2GamesLoss                                               ),
                ("@WinProbabilityNN"                                          , match.WinProbabilityNN                                          ),
                ("@IsFinished"                                                , match.IsFinished                                                )
                };

                foreach (var (name, value) in parameters)
                {
                    var p = command.CreateParameter();
                    p.ParameterName = name;
                    p.Value = value ?? DBNull.Value;
                    command.Parameters.Add(p);
                }

                if (_connection.State != ConnectionState.Open) _connection.Open();
                await Task.Run(() => command.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                string aaa;
                aaa = "";
            }
        }

        public Models.Match GetLatestH2HMatchAsync(int p1, int p2)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using var command = _connection.CreateCommand();
                command.CommandText = "dbo.GetLatestH2HMatch";
                command.CommandType = CommandType.StoredProcedure;

                var p1Param = command.CreateParameter();
                p1Param.ParameterName = "@p1Id";
                p1Param.Value = p1;                 
                                                     
                command.Parameters.Add(p1Param);     

                var p2Param = command.CreateParameter();
                p2Param.ParameterName = "@p2Id";
                p2Param.Value = p2;                  
                                                     
                command.Parameters.Add(p2Param);     

                using var reader = command.ExecuteReader(
                    CommandBehavior.SingleRow | CommandBehavior.CloseConnection);

                if (!reader.Read())
                    return null;

                var res = new Models.Match
                {
                    MatchTPId = reader["MatchTPId"] as int?,
                    TournamentEventTPId = reader["TournamentEventTPId"] as int?,
                    DateTime = reader["DateTime"] as DateTime?,
                    Player1TPId = reader["Player1TPId"] as int?,
                    Player2TPId = reader["Player2TPId"] as int?,
                    Player1H2H = reader["Player1H2H"] as int?,
                    Player2H2H = reader["Player2H2H"] as int?,
                    Player1H2HOld = reader["Player1H2HOld"] as int?,
                    Player2H2HOld = reader["Player2H2HOld"] as int?,
                    Player1H2HTrueSkillMeanM = reader["Player1H2HTrueSkillMeanM"] as double?,
                    Player1H2HTrueSkillStandardDeviationM = reader["Player1H2HTrueSkillStandardDeviationM"] as double?,
                    Player2H2HTrueSkillMeanM = reader["Player2H2HTrueSkillMeanM"] as double?,
                    Player2H2HTrueSkillStandardDeviationM = reader["Player2H2HTrueSkillStandardDeviationM"] as double?,
                    Player1H2HTrueSkillMeanOldM = reader["Player1H2HTrueSkillMeanOldM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldM = reader["Player1H2HTrueSkillStandardDeviationOldM"] as double?,
                    Player2H2HTrueSkillMeanOldM = reader["Player2H2HTrueSkillMeanOldM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldM = reader["Player2H2HTrueSkillStandardDeviationOldM"] as double?,
                    WinProbabilityPlayer1H2HM = reader["WinProbabilityPlayer1H2HM"] as double?,
                    Player1H2HTrueSkillMeanSM = reader["Player1H2HTrueSkillMeanSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationSM = reader["Player1H2HTrueSkillStandardDeviationSM"] as double?,
                    Player2H2HTrueSkillMeanSM = reader["Player2H2HTrueSkillMeanSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationSM = reader["Player2H2HTrueSkillStandardDeviationSM"] as double?,
                    Player1H2HTrueSkillMeanOldSM = reader["Player1H2HTrueSkillMeanOldSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSM = reader["Player1H2HTrueSkillStandardDeviationOldSM"] as double?,
                    Player2H2HTrueSkillMeanOldSM = reader["Player2H2HTrueSkillMeanOldSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSM = reader["Player2H2HTrueSkillStandardDeviationOldSM"] as double?,
                    WinProbabilityPlayer1H2HSM = reader["WinProbabilityPlayer1H2HSM"] as double?,
                    Player1H2HTrueSkillMeanGSM = reader["Player1H2HTrueSkillMeanGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSM = reader["Player1H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player2H2HTrueSkillMeanGSM = reader["Player2H2HTrueSkillMeanGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSM = reader["Player2H2HTrueSkillStandardDeviationGSM"] as double?,
                    Player1H2HTrueSkillMeanOldGSM = reader["Player1H2HTrueSkillMeanOldGSM"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSM = reader["Player1H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    Player2H2HTrueSkillMeanOldGSM = reader["Player2H2HTrueSkillMeanOldGSM"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSM = reader["Player2H2HTrueSkillStandardDeviationOldGSM"] as double?,
                    WinProbabilityPlayer1H2HGSM = reader["WinProbabilityPlayer1H2HGSM"] as double?,
                    Player1H2HS1 = reader["Player1H2HS1"] as int?,
                    Player2H2HS1 = reader["Player2H2HS1"] as int?,
                    Player1H2HOldS1 = reader["Player1H2HOldS1"] as int?,
                    Player2H2HOldS1 = reader["Player2H2HOldS1"] as int?,
                    Player1H2HTrueSkillMeanMS1 = reader["Player1H2HTrueSkillMeanMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS1 = reader["Player1H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player2H2HTrueSkillMeanMS1 = reader["Player2H2HTrueSkillMeanMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS1 = reader["Player2H2HTrueSkillStandardDeviationMS1"] as double?,
                    Player1H2HTrueSkillMeanOldMS1 = reader["Player1H2HTrueSkillMeanOldMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS1 = reader["Player1H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    Player2H2HTrueSkillMeanOldMS1 = reader["Player2H2HTrueSkillMeanOldMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS1 = reader["Player2H2HTrueSkillStandardDeviationOldMS1"] as double?,
                    WinProbabilityPlayer1H2HMS1 = reader["WinProbabilityPlayer1H2HMS1"] as double?,
                    Player1H2HTrueSkillMeanSMS1 = reader["Player1H2HTrueSkillMeanSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS1 = reader["Player1H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player2H2HTrueSkillMeanSMS1 = reader["Player2H2HTrueSkillMeanSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS1 = reader["Player2H2HTrueSkillStandardDeviationSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldSMS1 = reader["Player1H2HTrueSkillMeanOldSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldSMS1 = reader["Player2H2HTrueSkillMeanOldSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldSMS1"] as double?,
                    WinProbabilityPlayer1H2HSMS1 = reader["WinProbabilityPlayer1H2HSMS1"] as double?,
                    Player1H2HTrueSkillMeanGSMS1 = reader["Player1H2HTrueSkillMeanGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS1 = reader["Player1H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player2H2HTrueSkillMeanGSMS1 = reader["Player2H2HTrueSkillMeanGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS1 = reader["Player2H2HTrueSkillStandardDeviationGSMS1"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS1 = reader["Player1H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS1 = reader["Player2H2HTrueSkillMeanOldGSMS1"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS1 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS1"] as double?,
                    WinProbabilityPlayer1H2HGSMS1 = reader["WinProbabilityPlayer1H2HGSMS1"] as double?,
                    Player1H2HS2 = reader["Player1H2HS2"] as int?,
                    Player2H2HS2 = reader["Player2H2HS2"] as int?,
                    Player1H2HOldS2 = reader["Player1H2HOldS2"] as int?,
                    Player2H2HOldS2 = reader["Player2H2HOldS2"] as int?,
                    Player1H2HTrueSkillMeanMS2 = reader["Player1H2HTrueSkillMeanMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS2 = reader["Player1H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player2H2HTrueSkillMeanMS2 = reader["Player2H2HTrueSkillMeanMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS2 = reader["Player2H2HTrueSkillStandardDeviationMS2"] as double?,
                    Player1H2HTrueSkillMeanOldMS2 = reader["Player1H2HTrueSkillMeanOldMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS2 = reader["Player1H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    Player2H2HTrueSkillMeanOldMS2 = reader["Player2H2HTrueSkillMeanOldMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS2 = reader["Player2H2HTrueSkillStandardDeviationOldMS2"] as double?,
                    WinProbabilityPlayer1H2HMS2 = reader["WinProbabilityPlayer1H2HMS2"] as double?,
                    Player1H2HTrueSkillMeanSMS2 = reader["Player1H2HTrueSkillMeanSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS2 = reader["Player1H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player2H2HTrueSkillMeanSMS2 = reader["Player2H2HTrueSkillMeanSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS2 = reader["Player2H2HTrueSkillStandardDeviationSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldSMS2 = reader["Player1H2HTrueSkillMeanOldSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldSMS2 = reader["Player2H2HTrueSkillMeanOldSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldSMS2"] as double?,
                    WinProbabilityPlayer1H2HSMS2 = reader["WinProbabilityPlayer1H2HSMS2"] as double?,
                    Player1H2HTrueSkillMeanGSMS2 = reader["Player1H2HTrueSkillMeanGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS2 = reader["Player1H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player2H2HTrueSkillMeanGSMS2 = reader["Player2H2HTrueSkillMeanGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS2 = reader["Player2H2HTrueSkillStandardDeviationGSMS2"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS2 = reader["Player1H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS2 = reader["Player2H2HTrueSkillMeanOldGSMS2"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS2 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS2"] as double?,
                    WinProbabilityPlayer1H2HGSMS2 = reader["WinProbabilityPlayer1H2HGSMS2"] as double?,
                    Player1H2HS3 = reader["Player1H2HS3"] as int?,
                    Player2H2HS3 = reader["Player2H2HS3"] as int?,
                    Player1H2HOldS3 = reader["Player1H2HOldS3"] as int?,
                    Player2H2HOldS3 = reader["Player2H2HOldS3"] as int?,
                    Player1H2HTrueSkillMeanMS3 = reader["Player1H2HTrueSkillMeanMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS3 = reader["Player1H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player2H2HTrueSkillMeanMS3 = reader["Player2H2HTrueSkillMeanMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS3 = reader["Player2H2HTrueSkillStandardDeviationMS3"] as double?,
                    Player1H2HTrueSkillMeanOldMS3 = reader["Player1H2HTrueSkillMeanOldMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS3 = reader["Player1H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    Player2H2HTrueSkillMeanOldMS3 = reader["Player2H2HTrueSkillMeanOldMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS3 = reader["Player2H2HTrueSkillStandardDeviationOldMS3"] as double?,
                    WinProbabilityPlayer1H2HMS3 = reader["WinProbabilityPlayer1H2HMS3"] as double?,
                    Player1H2HTrueSkillMeanSMS3 = reader["Player1H2HTrueSkillMeanSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS3 = reader["Player1H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player2H2HTrueSkillMeanSMS3 = reader["Player2H2HTrueSkillMeanSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS3 = reader["Player2H2HTrueSkillStandardDeviationSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldSMS3 = reader["Player1H2HTrueSkillMeanOldSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldSMS3 = reader["Player2H2HTrueSkillMeanOldSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldSMS3"] as double?,
                    WinProbabilityPlayer1H2HSMS3 = reader["WinProbabilityPlayer1H2HSMS3"] as double?,
                    Player1H2HTrueSkillMeanGSMS3 = reader["Player1H2HTrueSkillMeanGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS3 = reader["Player1H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player2H2HTrueSkillMeanGSMS3 = reader["Player2H2HTrueSkillMeanGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS3 = reader["Player2H2HTrueSkillStandardDeviationGSMS3"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS3 = reader["Player1H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS3 = reader["Player2H2HTrueSkillMeanOldGSMS3"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS3 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS3"] as double?,
                    WinProbabilityPlayer1H2HGSMS3 = reader["WinProbabilityPlayer1H2HGSMS3"] as double?,
                    Player1H2HS4 = reader["Player1H2HS4"] as int?,
                    Player2H2HS4 = reader["Player2H2HS4"] as int?,
                    Player1H2HOldS4 = reader["Player1H2HOldS4"] as int?,
                    Player2H2HOldS4 = reader["Player2H2HOldS4"] as int?,
                    Player1H2HTrueSkillMeanMS4 = reader["Player1H2HTrueSkillMeanMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationMS4 = reader["Player1H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player2H2HTrueSkillMeanMS4 = reader["Player2H2HTrueSkillMeanMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationMS4 = reader["Player2H2HTrueSkillStandardDeviationMS4"] as double?,
                    Player1H2HTrueSkillMeanOldMS4 = reader["Player1H2HTrueSkillMeanOldMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldMS4 = reader["Player1H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    Player2H2HTrueSkillMeanOldMS4 = reader["Player2H2HTrueSkillMeanOldMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldMS4 = reader["Player2H2HTrueSkillStandardDeviationOldMS4"] as double?,
                    WinProbabilityPlayer1H2HMS4 = reader["WinProbabilityPlayer1H2HMS4"] as double?,
                    Player1H2HTrueSkillMeanSMS4 = reader["Player1H2HTrueSkillMeanSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationSMS4 = reader["Player1H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player2H2HTrueSkillMeanSMS4 = reader["Player2H2HTrueSkillMeanSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationSMS4 = reader["Player2H2HTrueSkillStandardDeviationSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldSMS4 = reader["Player1H2HTrueSkillMeanOldSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldSMS4 = reader["Player2H2HTrueSkillMeanOldSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldSMS4"] as double?,
                    WinProbabilityPlayer1H2HSMS4 = reader["WinProbabilityPlayer1H2HSMS4"] as double?,
                    Player1H2HTrueSkillMeanGSMS4 = reader["Player1H2HTrueSkillMeanGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationGSMS4 = reader["Player1H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player2H2HTrueSkillMeanGSMS4 = reader["Player2H2HTrueSkillMeanGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationGSMS4 = reader["Player2H2HTrueSkillStandardDeviationGSMS4"] as double?,
                    Player1H2HTrueSkillMeanOldGSMS4 = reader["Player1H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player1H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player1H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    Player2H2HTrueSkillMeanOldGSMS4 = reader["Player2H2HTrueSkillMeanOldGSMS4"] as double?,
                    Player2H2HTrueSkillStandardDeviationOldGSMS4 = reader["Player2H2HTrueSkillStandardDeviationOldGSMS4"] as double?,
                    WinProbabilityPlayer1H2HGSMS4 = reader["WinProbabilityPlayer1H2HGSMS4"] as double?,
                };
                return res;
            }
            catch (Exception ex)
            {
                // log ex, nemoj ga gutati u produkciji
                return null;
            }
        }

        public async Task<bool> ExistsAsync(int te, int p1, int p2)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = @"SELECT 1 FROM dbo.Match WHERE TournamentEventTPId=@te AND Player1TPId=@p1 AND Player2TPId=@p2;";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@te", SqlDbType.Int) { Value = te });
            cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = p1 });
            cmd.Parameters.Add(new SqlParameter("@p2", SqlDbType.Int) { Value = p2 });
            if (_connection.State != ConnectionState.Open) _connection.Open();
            var obj = cmd.ExecuteScalar();
            return obj != null && obj != DBNull.Value;
        }

        public async Task<int> DeleteActiveByIdAsync(int matchTPId, CancellationToken ct = default)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "dbo.Match_DeleteActiveById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@MatchTPId", SqlDbType.Int) { Value = matchTPId });

            var rowsParam = new SqlParameter("@RowsDeleted", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(rowsParam);

            if (_connection.State != ConnectionState.Open) _connection.Open();
            await Task.Run(() => command.ExecuteNonQuery());

            return rowsParam.Value is int n ? n : 0;
        }

        public async Task<IReadOnlyList<OddsQuoteDTO>> GetCleanOddsByMatchAsync(int matchTPId)
        {
            var list = new List<OddsQuoteDTO>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "dbo.GetMatchOddsByMatch";
            cmd.CommandType = CommandType.StoredProcedure;

            var p = cmd.CreateParameter();
            p.ParameterName = "@MatchTPId";
            p.Value = matchTPId;
            cmd.Parameters.Add(p);

            if (_connection.State != ConnectionState.Open) _connection.Open();
            using var reader = await ((SqlCommand)cmd).ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new OddsQuoteDTO
                {
                    OddsId = reader.GetInt32(reader.GetOrdinal("OddsId")),
                    MatchTPId = reader.GetInt32(reader.GetOrdinal("MatchTPId")),
                    BookieId = reader.GetInt32(reader.GetOrdinal("BookieId")),
                    BookieName = reader.GetString(reader.GetOrdinal("BookieName")),
                    DateTime = reader["DateTime"] is DBNull ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateTime")),
                    SourceFileTime = reader.GetDateTime(reader.GetOrdinal("SourceFileTime")),
                    CoalescedTime = reader.GetDateTime(reader.GetOrdinal("CoalescedTime")),
                    SeriesOrdinal = reader["SeriesOrdinal"] is DBNull ? (int?)null : reader.GetInt32(reader.GetOrdinal("SeriesOrdinal")),
                    Player1Odds = reader["Player1Odds"] is DBNull ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Player1Odds")),
                    Player2Odds = reader["Player2Odds"] is DBNull ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Player2Odds")),
                });
            }
            return list;
        }
    }
}