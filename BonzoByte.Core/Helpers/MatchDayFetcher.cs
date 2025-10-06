using Microsoft.Data.SqlClient;
using System.Data;

namespace BonzoByte.Core.Helpers
{
    public static class MatchDayFetcher
    {
        /// <summary>
        /// Iterira od 1990-01-01 (Europe/Zagreb) do današnjeg (Europe/Zagreb),
        /// i za svaki dan poziva SP dbo.GetAllMatchesByDate s UTC granicama.
        /// </summary>
        /// <param name="connection">Otvorit će se po potrebi; možeš proslijediti DI-jem.</param>
        /// <param name="consumeDayAsync">
        /// Handler koji prima datum (local, Europe/Zagreb) i SqlDataReader sa svim mečevima tog dana.
        /// U handleru pročitaj reader do kraja. Reader se automatski disposa nakon handlera.
        /// </param>
        public static async Task FetchDayByDayAsync(
            IDbConnection connection,
            Func<DateOnly, SqlDataReader, Task> consumeDayAsync,
            CancellationToken ct = default)
        {
            // 1) Lokalni “danas” u Zagrebu (DST safe)
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb");
            var startLocal = new DateOnly(1990, 1, 1);
            var todayLocal = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz));

            for (var d = startLocal; d <= todayLocal; d = d.AddDays(1))
            {
                ct.ThrowIfCancellationRequested();

                // 2) Granice dana u lokalnom vremenu
                var fromLocal = d.ToDateTime(TimeOnly.MinValue);      // 00:00:00 local
                var toLocal = fromLocal.AddDays(1);                 // sljedeći dan 00:00:00 local

                // 3) Konverzija u UTC (DST-safe)
                var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromLocal, tz);
                var toUtc = TimeZoneInfo.ConvertTimeToUtc(toLocal, tz);

                using var cmd = (SqlCommand)connection.CreateCommand();
                cmd.CommandText = "dbo.GetAllMatchesByDate";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@FromUtc", System.Data.SqlDbType.DateTime2) { Value = fromUtc });
                cmd.Parameters.Add(new SqlParameter("@ToUtc", System.Data.SqlDbType.DateTime2) { Value = toUtc });

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using var reader = await cmd.ExecuteReaderAsync(ct);
                await consumeDayAsync(d, reader);
            }
        }
    }
}
