using BonzoByte.Core.Models;
using System.Globalization;
using System.Text;

namespace BonzoByte.Core.SQLGen
{
    public static class TournamentEventInsertScriptGenerator
    {
        public static void GenerateScript(
            IEnumerable<TournamentEvent> items,
            string outputSqlPath,
            string databaseName = "BonzoByte",
            int batchSize = 1000)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputSqlPath) ?? ".");

            var sb = new StringBuilder(1 << 20); // 1MB start buffer
            sb.AppendLine("USE [" + databaseName + "];");
            sb.AppendLine("SET NOCOUNT ON;");
            sb.AppendLine("SET XACT_ABORT ON;");
            sb.AppendLine();
            sb.AppendLine("BEGIN TRAN;");
            sb.AppendLine();

            int i = 0;
            int inBatch = 0;

            void FlushBatchTerminator()
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
                sb.AppendLine("INSERT INTO [dbo].[TournamentEvent]");
                sb.AppendLine("    ([TournamentEventTPId],[TournamentEventName],[CountryTPId],[TournamentEventDate],[TournamentLevelId],[TournamentTypeId],[Prize],[SurfaceId])");
                sb.AppendLine("VALUES");
            }

            foreach (var te in items)
            {
                if (inBatch == 0) BeginInsertHeader();

                string row = BuildValuesRow(te);
                if (inBatch > 0) sb.AppendLine("," + row);
                else sb.AppendLine(row);

                inBatch++;
                i++;

                if (inBatch >= batchSize)
                {
                    FlushBatchTerminator();
                }
            }

            // zadnji batch ako je ostao otvoren
            FlushBatchTerminator();

            sb.AppendLine("COMMIT;");

            File.WriteAllText(outputSqlPath, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        private static string BuildValuesRow(TournamentEvent te)
        {
            // mapiranje polja -> SQL literali
            string v_Id = SqlInt(te.TournamentEventTPId);
            string v_Name = SqlVarchar(te.TournamentEventName);
            string v_Country = SqlTinyInt(te.CountryTPId);
            string v_Date = SqlDate(te.TournamentEventDate);
            string v_Level = SqlTinyInt(te.TournamentLevelId);
            string v_Type = SqlTinyInt(te.TournamentTypeId);
            string v_Prize = SqlIntNull(te.Prize ?? 0);
            string v_Surface = SqlTinyInt(te.SurfaceId);

            return $"({v_Id},{v_Name},{v_Country},{v_Date},{v_Level},{v_Type},{v_Prize},{v_Surface})";
        }

        // === SQL literal helpers (varchar verzija) ===

        private static string SqlVarchar(string s)
        {
            if (string.IsNullOrEmpty(s)) return "NULL";
            var escaped = s.Replace("'", "''");
            return $"'{escaped}'";
        }

        private static string SqlDate(DateTime? dt)
        {
            if (!dt.HasValue) return "NULL";
            return $"'{dt.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}'";
        }

        private static string SqlInt(int? v)
        {
            if (!v.HasValue) return "NULL";
            return v.Value.ToString(CultureInfo.InvariantCulture);
        }

        private static string SqlInt(int v) => v.ToString(CultureInfo.InvariantCulture);

        private static string SqlIntNull(int v) => v == 0 ? "0" : v.ToString(CultureInfo.InvariantCulture);

        private static string SqlTinyInt(int? v)
        {
            if (!v.HasValue) return "NULL";
            int x = v.Value;
            if (x < 0) x = 0; else if (x > 255) x = 255;
            return x.ToString(CultureInfo.InvariantCulture);
        }
    }
}