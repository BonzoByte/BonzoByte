using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class MatchActiveOddsRepository : IMatchActiveOddsRepository
    {
        private readonly IDbConnection _connection;

        public MatchActiveOddsRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<MatchActiveOdds>> GetAllMatchActiveOddsAsync()
        {
            var matchActiveOdds = new List<MatchActiveOdds>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllMatchActiveOdds";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var matchActiveOddsEntry = new MatchActiveOdds
                {
                    MatchTPId = reader["MatchTPId"] != DBNull.Value ? Convert.ToInt32(reader["MatchTPId"]) : (int?)null,
                    BookieId = reader["BookieId"] != DBNull.Value ? Convert.ToInt32(reader["BookieId"]) : (int?)null,
                    DateTime = reader["DateTime"] != DBNull.Value ? Convert.ToDateTime(reader["DateTime"]) : (DateTime?)null,
                    Player1Odds = reader["Player1Odds"] as double?,
                    Player2Odds = reader["Player2Odds"] as double?
                };
                matchActiveOdds.Add(matchActiveOddsEntry);
            }

            return matchActiveOdds;
        }
    }
}