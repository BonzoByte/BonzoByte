using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class MatchOddsRepository : IMatchOddsRepository
    {
        private readonly IDbConnection _connection;

        public MatchOddsRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<MatchOdds>> GetAllMatchOddsAsync()
        {
            var list = new List<MatchOdds>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "GetAllMatchOdds";
            cmd.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();
            using var reader = await ((SqlCommand)cmd).ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(Map(reader));

            return list;
        }

        public async Task<IEnumerable<MatchOdds>> GetOddsByMatchAsync(int matchTPId)
        {
            var list = new List<MatchOdds>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                SELECT OddsId, MatchTPId, BookieId, DateTime, SourceFileTime, CoalescedTime,
                       SeriesOrdinal, Player1Odds, Player2Odds, IngestedAt,
                       IsSuspicious, IsLikelySwitched, SuspiciousMask
                FROM dbo.MatchOdds
                WHERE MatchTPId = @mid;";
            cmd.CommandType = CommandType.Text;

            var p = cmd.CreateParameter();
            p.ParameterName = "@mid";
            p.Value = matchTPId;
            cmd.Parameters.Add(p);

            if (_connection.State != ConnectionState.Open) _connection.Open();
            using var reader = await ((SqlCommand)cmd).ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(Map(reader));

            return list;
        }

        public async Task<int> InsertMatchOddsAsync(MatchOdds odds)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO dbo.MatchOdds
                    (MatchTPId, BookieId, DateTime, SourceFileTime, SeriesOrdinal,
                     Player1Odds, Player2Odds, IngestedAt, IsSuspicious, IsLikelySwitched, SuspiciousMask)
                VALUES
                    (@MatchTPId, @BookieId, @DateTime, @SourceFileTime, @SeriesOrdinal,
                     @Player1Odds, @Player2Odds, @IngestedAt, @IsSuspicious, @IsLikelySwitched, @SuspiciousMask);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            cmd.CommandType = CommandType.Text;

            AddParam(cmd, "@MatchTPId", SqlDbType.Int, odds.MatchTPId);
            AddParam(cmd, "@BookieId", SqlDbType.Int, odds.BookieId);
            AddParam(cmd, "@DateTime", SqlDbType.DateTime2, odds.DateTime);
            AddParam(cmd, "@SourceFileTime", SqlDbType.DateTime2, odds.SourceFileTime ?? DateTime.UtcNow);
            AddParam(cmd, "@SeriesOrdinal", SqlDbType.Int, odds.SeriesOrdinal);

            var p1 = new SqlParameter("@Player1Odds", SqlDbType.Decimal) { Precision = 9, Scale = 3, Value = (object?)odds.Player1Odds ?? DBNull.Value };
            var p2 = new SqlParameter("@Player2Odds", SqlDbType.Decimal) { Precision = 9, Scale = 3, Value = (object?)odds.Player2Odds ?? DBNull.Value };
            ((SqlCommand)cmd).Parameters.Add(p1);
            ((SqlCommand)cmd).Parameters.Add(p2);

            AddParam(cmd, "@IngestedAt", SqlDbType.DateTime2, odds.IngestedAt ?? DateTime.UtcNow);
            AddParam(cmd, "@IsSuspicious", SqlDbType.Bit, odds.IsSuspicious ?? false);
            AddParam(cmd, "@IsLikelySwitched", SqlDbType.Bit, odds.IsLikelySwitched ?? false);
            AddParam(cmd, "@SuspiciousMask", SqlDbType.SmallInt, odds.SuspiciousMask ?? (short)0);

            if (_connection.State != ConnectionState.Open) _connection.Open();
            await ((SqlCommand)cmd).ExecuteNonQueryAsync();
            var newId = (int)await ((SqlCommand)cmd).ExecuteScalarAsync();
            return newId;
        }

        public async Task UpdateFlagsAsync(int oddsId, bool isSuspicious, bool isLikelySwitched, short suspiciousMask)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                UPDATE dbo.MatchOdds
                SET IsSuspicious = @isS, IsLikelySwitched = @isLS, SuspiciousMask = @mask
                WHERE OddsId = @id;";
            cmd.CommandType = CommandType.Text;

            AddParam(cmd, "@id", SqlDbType.Int, oddsId);
            AddParam(cmd, "@isS", SqlDbType.Bit, isSuspicious);
            AddParam(cmd, "@isLS", SqlDbType.Bit, isLikelySwitched);
            AddParam(cmd, "@mask", SqlDbType.SmallInt, suspiciousMask);

            if (_connection.State != ConnectionState.Open) _connection.Open();
            await ((SqlCommand)cmd).ExecuteNonQueryAsync();
        }

        private static void AddParam(IDbCommand cmd, string name, SqlDbType type, object? value)
        {
            var p = new SqlParameter(name, type) { Value = value ?? DBNull.Value };
            ((SqlCommand)cmd).Parameters.Add(p);
        }

        private static MatchOdds Map(IDataRecord r)
        {
            return new MatchOdds
            {
                OddsId = r["OddsId"] != DBNull.Value ? Convert.ToInt32(r["OddsId"]) : (int?)null,
                MatchTPId = r["MatchTPId"] != DBNull.Value ? Convert.ToInt32(r["MatchTPId"]) : (int?)null,
                BookieId = r["BookieId"] != DBNull.Value ? Convert.ToInt32(r["BookieId"]) : (int?)null,
                DateTime = r["DateTime"] != DBNull.Value ? Convert.ToDateTime(r["DateTime"]) : (DateTime?)null,
                SourceFileTime = r["SourceFileTime"] != DBNull.Value ? Convert.ToDateTime(r["SourceFileTime"]) : (DateTime?)null,
                CoalescedTime = r["CoalescedTime"] != DBNull.Value ? Convert.ToDateTime(r["CoalescedTime"]) : (DateTime?)null,
                SeriesOrdinal = r["SeriesOrdinal"] != DBNull.Value ? Convert.ToInt32(r["SeriesOrdinal"]) : (int?)null,
                Player1Odds = r["Player1Odds"] != DBNull.Value ? Convert.ToDecimal(r["Player1Odds"]) : (decimal?)null,
                Player2Odds = r["Player2Odds"] != DBNull.Value ? Convert.ToDecimal(r["Player2Odds"]) : (decimal?)null,
                IngestedAt = r["IngestedAt"] != DBNull.Value ? Convert.ToDateTime(r["IngestedAt"]) : (DateTime?)null,
                IsSuspicious = r["IsSuspicious"] != DBNull.Value ? Convert.ToBoolean(r["IsSuspicious"]) : (bool?)null,
                IsLikelySwitched = r["IsLikelySwitched"] != DBNull.Value ? Convert.ToBoolean(r["IsLikelySwitched"]) : (bool?)null,
                SuspiciousMask = r["SuspiciousMask"] != DBNull.Value ? Convert.ToInt16(r["SuspiciousMask"]) : (short?)null
            };
        }
    }
}