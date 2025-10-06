using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BonzoByte.Core.Models;

namespace BonzoByte.Core.SQLGen
{
    public static class PlayerInsertScriptGenerator
    {
        // Poredak 100% prati tvoju CREATE TABLE [dbo].[Player]
        private static readonly string[] Columns = new[]
        {
            "PlayerTPId","PlayerName","CountryTPId","PlayerBirthDate","PlayerHeight","PlayerWeight","PlayerTurnedPro","PlaysId","TournamentTypeId",
            "TrueSkillMeanM","TrueSkillStandardDeviationM","TrueSkillMeanSM","TrueSkillStandardDeviationSM","TrueSkillMeanGSM","TrueSkillStandardDeviationGSM",
            "TrueSkillMeanMS1","TrueSkillStandardDeviationMS1","TrueSkillMeanSMS1","TrueSkillStandardDeviationSMS1","TrueSkillMeanGSMS1","TrueSkillStandardDeviationGSMS1",
            "TrueSkillMeanMS2","TrueSkillStandardDeviationMS2","TrueSkillMeanSMS2","TrueSkillStandardDeviationSMS2","TrueSkillMeanGSMS2","TrueSkillStandardDeviationGSMS2",
            "TrueSkillMeanMS3","TrueSkillStandardDeviationMS3","TrueSkillMeanSMS3","TrueSkillStandardDeviationSMS3","TrueSkillMeanGSMS3","TrueSkillStandardDeviationGSMS3",
            "TrueSkillMeanMS4","TrueSkillStandardDeviationMS4","TrueSkillMeanSMS4","TrueSkillStandardDeviationSMS4","TrueSkillMeanGSMS4","TrueSkillStandardDeviationGSMS4",
            "WinsTotal","LossesTotal","WinsLastYear","LossesLastYear","WinsLastMonth","LossesLastMonth","WinsLastWeek","LossesLastWeek",
            "WinsTotalS1","LossesTotalS1","WinsLastYearS1","LossesLastYearS1","WinsLastMonthS1","LossesLastMonthS1","WinsLastWeekS1","LossesLastWeekS1",
            "WinsTotalS2","LossesTotalS2","WinsLastYearS2","LossesLastYearS2","WinsLastMonthS2","LossesLastMonthS2","WinsLastWeekS2","LossesLastWeekS2",
            "WinsTotalS3","LossesTotalS3","WinsLastYearS3","LossesLastYearS3","WinsLastMonthS3","LossesLastMonthS3","WinsLastWeekS3","LossesLastWeekS3",
            "WinsTotalS4","LossesTotalS4","WinsLastYearS4","LossesLastYearS4","WinsLastMonthS4","LossesLastMonthS4","WinsLastWeekS4","LossesLastWeekS4",
            "WinsSetsTotal","LossesSetsTotal","WinsSetsLastYear","LossesSetsLastYear","WinsSetsLastMonth","LossesSetsLastMonth","WinsSetsLastWeek","LossesSetsLastWeek",
            "WinsSetsTotalS1","LossesSetsTotalS1","WinsSetsLastYearS1","LossesSetsLastYearS1","WinsSetsLastMonthS1","LossesSetsLastMonthS1","WinsSetsLastWeekS1","LossesSetsLastWeekS1",
            "WinsSetsTotalS2","LossesSetsTotalS2","WinsSetsLastYearS2","LossesSetsLastYearS2","WinsSetsLastMonthS2","LossesSetsLastMonthS2","WinsSetsLastWeekS2","LossesSetsLastWeekS2",
            "WinsSetsTotalS3","LossesSetsTotalS3","WinsSetsLastYearS3","LossesSetsLastYearS3","WinsSetsLastMonthS3","LossesSetsLastMonthS3","WinsSetsLastWeekS3","LossesSetsLastWeekS3",
            "WinsSetsTotalS4","LossesSetsTotalS4","WinsSetsLastYearS4","LossesSetsLastYearS4","WinsSetsLastMonthS4","LossesSetsLastMonthS4","WinsSetsLastWeekS4","LossesSetsLastWeekS4",
            "WinsGamesTotal","LossesGamesTotal","WinsGamesLastYear","LossesGamesLastYear","WinsGamesLastMonth","LossesGamesLastMonth","WinsGamesLastWeek","LossesGamesLastWeek",
            "WinsGamesTotalS1","LossesGamesTotalS1","WinsGamesLastYearS1","LossesGamesLastYearS1","WinsGamesLastMonthS1","LossesGamesLastMonthS1","WinsGamesLastWeekS1","LossesGamesLastWeekS1",
            "WinsGamesTotalS2","LossesGamesTotalS2","WinsGamesLastYearS2","LossesGamesLastYearS2","WinsGamesLastMonthS2","LossesGamesLastMonthS2","WinsGamesLastWeekS2","LossesGamesLastWeekS2",
            "WinsGamesTotalS3","LossesGamesTotalS3","WinsGamesLastYearS3","LossesGamesLastYearS3","WinsGamesLastMonthS3","LossesGamesLastMonthS3","WinsGamesLastWeekS3","LossesGamesLastWeekS3",
            "WinsGamesTotalS4","LossesGamesTotalS4","WinsGamesLastYearS4","LossesGamesLastYearS4","WinsGamesLastMonthS4","LossesGamesLastMonthS4","WinsGamesLastWeekS4","LossesGamesLastWeekS4",
            "DateSinceLastWin","DateSinceLastWinS1","DateSinceLastWinS2","DateSinceLastWinS3","DateSinceLastWinS4",
            "DateSinceLastLoss","DateSinceLastLossS1","DateSinceLastLossS2","DateSinceLastLossS3","DateSinceLastLossS4",
            "TotalWinsAsFavourite","TotalWinsAsUnderdog","TotalLossesAsFavourite","TotalLossesAsUnderdog",
            "WinsAsFavouriteRatio","LossesAsFavouriteRatio","WinsAsUnderdogRatio","LossesAsUnderdogRatio",
            "AverageWinningProbabilityAtWonAsFavourite","AverageWinningProbabilityAtWonAsUnderdog","AverageWinningProbabilityAtLossAsFavourite","AverageWinningProbabilityAtLossAsUnderdog",
            "TotalWinsAsFavouriteLastYear","TotalWinsAsUnderdogLastYear","TotalLossesAsFavouriteLastYear","TotalLossesAsUnderdogLastYear",
            "WinsAsFavouriteLastYearRatio","LossesAsFavouriteLastYearRatio","WinsAsUnderdogLastYearRatio","LossesAsUnderdogLastYearRatio",
            "AverageWinningProbabilityAtWonAsFavouriteLastYear","AverageWinningProbabilityAtWonAsUnderdogLastYear","AverageWinningProbabilityAtLossAsFavouriteLastYear","AverageWinningProbabilityAtLossAsUnderdogLastYear",
            "TotalWinsAsFavouriteLastMonth","TotalWinsAsUnderdogLastMonth","TotalLossesAsFavouriteLastMonth","TotalLossesAsUnderdogLastMonth",
            "WinsAsFavouriteLastMonthRatio","LossesAsFavouriteLastMonthRatio","WinsAsUnderdogLastMonthRatio","LossesAsUnderdogLastMonthRatio",
            "AverageWinningProbabilityAtWonAsFavouriteLastMonth","AverageWinningProbabilityAtWonAsUnderdogLastMonth","AverageWinningProbabilityAtLossAsFavouriteLastMonth","AverageWinningProbabilityAtLossAsUnderdogLastMonth",
            "TotalWinsAsFavouriteLastWeek","TotalWinsAsUnderdogLastWeek","TotalLossesAsFavouriteLastWeek","TotalLossesAsUnderdogLastWeek",
            "WinsAsFavouriteLastWeekRatio","LossesAsFavouriteLastWeekRatio","WinsAsUnderdogLastWeekRatio","LossesAsUnderdogLastWeekRatio",
            "AverageWinningProbabilityAtWonAsFavouriteLastWeek","AverageWinningProbabilityAtWonAsUnderdogLastWeek","AverageWinningProbabilityAtLossAsFavouriteLastWeek","AverageWinningProbabilityAtLossAsUnderdogLastWeek",
            "Streak","StreakS1","StreakS2","StreakS3","StreakS4"
        };

        // Kolone koje su NULLable prema tvojoj shemi.
        private static readonly HashSet<string> NullableColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CountryTPId","PlayerBirthDate","PlayerHeight","PlayerWeight","PlayerTurnedPro","PlaysId","TournamentTypeId",
            // sve DateSince* su NULL
            "DateSinceLastWin","DateSinceLastWinS1","DateSinceLastWinS2","DateSinceLastWinS3","DateSinceLastWinS4",
            "DateSinceLastLoss","DateSinceLastLossS1","DateSinceLastLossS2","DateSinceLastLossS3","DateSinceLastLossS4",
            // sve Favourite/Underdog agregacije su NULLable
            "TotalWinsAsFavourite","TotalWinsAsUnderdog","TotalLossesAsFavourite","TotalLossesAsUnderdog",
            "WinsAsFavouriteRatio","LossesAsFavouriteRatio","WinsAsUnderdogRatio","LossesAsUnderdogRatio",
            "AverageWinningProbabilityAtWonAsFavourite","AverageWinningProbabilityAtWonAsUnderdog","AverageWinningProbabilityAtLossAsFavourite","AverageWinningProbabilityAtLossAsUnderdog",
            "TotalWinsAsFavouriteLastYear","TotalWinsAsUnderdogLastYear","TotalLossesAsFavouriteLastYear","TotalLossesAsUnderdogLastYear",
            "WinsAsFavouriteLastYearRatio","LossesAsFavouriteLastYearRatio","WinsAsUnderdogLastYearRatio","LossesAsUnderdogLastYearRatio",
            "AverageWinningProbabilityAtWonAsFavouriteLastYear","AverageWinningProbabilityAtWonAsUnderdogLastYear","AverageWinningProbabilityAtLossAsFavouriteLastYear","AverageWinningProbabilityAtLossAsUnderdogLastYear",
            "TotalWinsAsFavouriteLastMonth","TotalWinsAsUnderdogLastMonth","TotalLossesAsFavouriteLastMonth","TotalLossesAsUnderdogLastMonth",
            "WinsAsFavouriteLastMonthRatio","LossesAsFavouriteLastMonthRatio","WinsAsUnderdogLastMonthRatio","LossesAsUnderdogLastMonthRatio",
            "AverageWinningProbabilityAtWonAsFavouriteLastMonth","AverageWinningProbabilityAtWonAsUnderdogLastMonth","AverageWinningProbabilityAtLossAsFavouriteLastMonth","AverageWinningProbabilityAtLossAsUnderdogLastMonth",
            "TotalWinsAsFavouriteLastWeek","TotalWinsAsUnderdogLastWeek","TotalLossesAsFavouriteLastWeek","TotalLossesAsUnderdogLastWeek",
            "WinsAsFavouriteLastWeekRatio","LossesAsFavouriteLastWeekRatio","WinsAsUnderdogLastWeekRatio","LossesAsUnderdogLastWeekRatio",
            "AverageWinningProbabilityAtWonAsFavouriteLastWeek","AverageWinningProbabilityAtWonAsUnderdogLastWeek","AverageWinningProbabilityAtLossAsFavouriteLastWeek","AverageWinningProbabilityAtLossAsUnderdogLastWeek",
            "Streak","StreakS1","StreakS2","StreakS3","StreakS4"
        };

        // Property cache radi brzine
        private static readonly Dictionary<string, PropertyInfo> PropCache =
            typeof(Player).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                          .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        public static void GenerateScript(IEnumerable<Player> items, string outputSqlPath, string databaseName = "BonzoByte", int batchSize = 1000)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputSqlPath) ?? ".");
            var sb = new StringBuilder(1 << 20);

            sb.AppendLine("USE [" + databaseName + "];");
            sb.AppendLine("SET NOCOUNT ON;");
            sb.AppendLine("SET XACT_ABORT ON;");
            sb.AppendLine();
            sb.AppendLine("BEGIN TRAN;");
            sb.AppendLine();

            int inBatch = 0;

            void FlushBatch()
            {
                if (inBatch > 0)
                {
                    sb.AppendLine(";");
                    sb.AppendLine();
                    inBatch = 0;
                }
            }

            void BeginInsertHeader()
            {
                sb.AppendLine("INSERT INTO [dbo].[Player]");
                sb.AppendLine("    ([" + string.Join("],[", Columns) + "])");
                sb.AppendLine("VALUES");
            }

            foreach (var pl in items)
            {
                if (inBatch == 0) BeginInsertHeader();

                string row = BuildValuesRow(pl);
                if (inBatch > 0) sb.AppendLine("," + row);
                else sb.AppendLine(row);

                inBatch++;
                if (inBatch >= batchSize)
                    FlushBatch();
            }

            FlushBatch();
            sb.AppendLine("COMMIT;");

            File.WriteAllText(outputSqlPath, sb.ToString(), new UTF8Encoding(false));
        }

        private static string BuildValuesRow(Player p)
        {
            var vals = new string[Columns.Length];
            for (int i = 0; i < Columns.Length; i++)
            {
                string col = Columns[i];
                vals[i] = ToSqlLiteral(col, GetValue(p, col));
            }
            return "(" + string.Join(",", vals) + ")";
        }

        private static object GetValue(Player p, string col)
        {
            if (!PropCache.TryGetValue(col, out var prop))
            {
                // nema property-ja u modelu → default
                return DefaultFor(col);
            }

            var val = prop.GetValue(p, null);
            if (val == null)
            {
                return DefaultFor(col);
            }

            // posebni slučajevi po tipu kolone
            if (col.Equals("PlayerName", StringComparison.OrdinalIgnoreCase))
            {
                // NVARCHAR(50)
                return val;
            }

            if (col.Equals("PlayerBirthDate", StringComparison.OrdinalIgnoreCase))
            {
                return (DateTime?)val;
            }

            if (col.StartsWith("DateSince", StringComparison.OrdinalIgnoreCase))
            {
                return (DateTime?)val;
            }

            if (col.Equals("CountryTPId", StringComparison.OrdinalIgnoreCase) ||
                col.Equals("PlaysId", StringComparison.OrdinalIgnoreCase))
            {
                // tinyint
                var n = Convert.ToInt32(val, CultureInfo.InvariantCulture);
                if (n < 0) n = 0; else if (n > 255) n = 255;
                return n;
            }

            // sve brojčane NOT NULL ili NULLable
            return val;
        }

        private static object DefaultFor(string col)
        {
            // NOT NULL polja moraju dobiti 0/0.0; NULLable smiju NULL
            if (NullableColumns.Contains(col))
                return DBNull.Value;

            // PlayerName je NOT NULL NVARCHAR(50) → ako nedostaje, stavi 'Unknown'
            if (col.Equals("PlayerName", StringComparison.OrdinalIgnoreCase))
                return "Unknown";

            // sve ostalo tretiramo kao numerike s default 0
            return 0;
        }

        private static string ToSqlLiteral(string col, object val)
        {
            if (val == null || val is DBNull) return "NULL";

            if (col.Equals("PlayerName", StringComparison.OrdinalIgnoreCase))
            {
                var s = Convert.ToString(val) ?? "";
                var esc = s.Replace("'", "''");
                return $"N'{esc}'";
            }

            if (col.Equals("PlayerBirthDate", StringComparison.OrdinalIgnoreCase))
            {
                var dt = (DateTime?)val;
                return dt.HasValue ? $"'{dt.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}'" : "NULL";
            }

            if (col.StartsWith("DateSince", StringComparison.OrdinalIgnoreCase))
            {
                var dt = (DateTime?)val;
                return dt.HasValue ? $"'{dt.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}'" : "NULL";
            }

            // numerike
            switch (Type.GetTypeCode(val.GetType()))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return Convert.ToInt64(val, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return Convert.ToDouble(val, CultureInfo.InvariantCulture).ToString("R", CultureInfo.InvariantCulture); // “R” točan literal

                case TypeCode.String:
                    // should be only PlayerName, ali fallback:
                    var esc = Convert.ToString(val)?.Replace("'", "''") ?? "";
                    return $"N'{esc}'";

                default:
                    // fallback
                    var s = Convert.ToString(val, CultureInfo.InvariantCulture) ?? "";
                    var e = s.Replace("'", "''");
                    return $"N'{e}'";
            }
        }
    }
}