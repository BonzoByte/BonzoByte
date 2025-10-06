using Microsoft.Data.SqlClient;
using System.Data;

namespace BonzoByte.Core.Services
{
    public sealed class TournamentMatchesExporter
    {
        private readonly string _connectionString;
        private readonly string _storedProcName;

        public TournamentMatchesExporter(
            string connectionString,
            string storedProcName = "dbo.GetTournamentMatches")
        {
            _connectionString = connectionString;
            _storedProcName = storedProcName;
        }

        /// <summary>
        /// Pokreće export za popis TournamentEventTPId-ova (redoslijed koji predaš).
        /// </summary>
        /// <param name="tournamentEventIds">Lista TE ID-ova za procesirati.</param>
        /// <param name="handleEventAsync">Callback koji čita SqlDataReader i serijalizira (.json/.br).</param>
        /// <param name="sortDirection">"ASC" ili "DESC" (default "ASC").</param>
        public async Task RunAsync(
            IEnumerable<int> tournamentEventIds,
            Func<int, SqlDataReader, Task> handleEventAsync,
            string sortDirection = "ASC",
            CancellationToken ct = default)
        {
            if (tournamentEventIds is null) throw new ArgumentNullException(nameof(tournamentEventIds));
            if (handleEventAsync is null) throw new ArgumentNullException(nameof(handleEventAsync));

            // normaliziraj smjer
            var dir = string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(ct);

            foreach (var teId in tournamentEventIds)
            {
                ct.ThrowIfCancellationRequested();

                using var cmd = new SqlCommand(_storedProcName, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0 // dugo punjenje: bez limita (po želji postavi npr. 300s)
                };

                cmd.Parameters.Add(new SqlParameter("@TournamentEventTPId", SqlDbType.Int) { Value = teId });
                // Ako tvoja SP ima opcionalni parametar @SortDirection
                cmd.Parameters.Add(new SqlParameter("@SortDirection", SqlDbType.VarChar, 4) { Value = dir });

                using var reader = await cmd.ExecuteReaderAsync(ct);

                await handleEventAsync(teId, reader);

                // Ako SP ikad vrati dodatne setove, isperi ih ovdje
                while (await reader.NextResultAsync(ct)) { /* no-op */ }

                Console.WriteLine($"[TournamentExport] TE={teId} processed ({dir}).");
            }
        }

        /// <summary>
        /// Pogodnost za jedan turnir.
        /// </summary>
        public Task RunOneAsync(
            int tournamentEventId,
            Func<int, SqlDataReader, Task> handleEventAsync,
            string sortDirection = "ASC",
            CancellationToken ct = default)
            => RunAsync(new[] { tournamentEventId }, handleEventAsync, sortDirection, ct);
    }
}