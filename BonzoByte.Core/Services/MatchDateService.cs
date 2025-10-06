using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BonzoByte.Core.Services
{
    public class MatchDateService
    {
        private readonly string _connectionString;

        public MatchDateService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<(DateTime fromDate, DateTime toDate)> GetMatchDateRangeAsync()
        {
            DateTime? latestMatchDate = null;

            using var connection = new SqlConnection(_connectionString);
            using var command    = new SqlCommand   ("dbo.GetLatestMatchDate", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            if (result != DBNull.Value) latestMatchDate = (DateTime?)result;

            var baseDate = latestMatchDate ?? DateTime.Now.Date;

            return (
                fromDate: baseDate.AddDays(-3),
                toDate:   baseDate.AddDays(3)
            );
        }
    }
}