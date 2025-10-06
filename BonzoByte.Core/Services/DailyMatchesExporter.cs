using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonzoByte.Core.Services
{
    public sealed class DailyMatchesExporter
    {
        private readonly string _connectionString;
        private readonly string _storedProcName;

        public DailyMatchesExporter(string connectionString, string storedProcName = "dbo.GetDailyMatchesForDate")
        {
            _connectionString = connectionString;
            _storedProcName = storedProcName;
        }

        public async Task RunAsync(DateOnly from, DateOnly to, Func<DateOnly, SqlDataReader, Task> handleDayAsync, CancellationToken ct = default)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(ct);

            for (var d = from; d <= to; d = d.AddDays(1))
            {
                ct.ThrowIfCancellationRequested();

                using var cmd = new SqlCommand(_storedProcName, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0 // dugotrajno punjenje: bez limita; po želji postavi npr. 300s
                };

                // @Date kao SQL 'date'; DateOnly -> DateTime (00:00) je ok
                var p = new SqlParameter("@Day", SqlDbType.Date) { Value = d.ToDateTime(TimeOnly.MinValue) };
                cmd.Parameters.Add(p);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                // Ako SP vraća više result setova, handleDayAsync neka pročita sve;
                // ili ovdje odradi petlju NextResultAsync po potrebi.
                await handleDayAsync(d, reader);

                // Osiguraj da je reader do kraja pročitan prije idućeg dana:
                while (await reader.NextResultAsync(ct)) { /* no-op */ }

                Console.WriteLine($"[DailyExport] {d:yyyy-MM-dd} processed.");
            }
        }

        public static DateOnly GetLocalTodayEuropeZagreb()
        {
            var tz = TryGetZagrebTimeZone();
            var todayLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;
            return DateOnly.FromDateTime(todayLocal);
        }

        private static TimeZoneInfo TryGetZagrebTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb"); }
            catch { return TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"); } // Windows fallback
        }
    }
}
