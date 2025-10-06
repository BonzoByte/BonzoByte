using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class RoundRepository: IRoundRepository
    {
        private readonly IDbConnection _connection;

        public RoundRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Round>> GetAllRoundsAsync()
        {
            var rounds = new List<Round>();

            using var command   = _connection.CreateCommand();
            command.CommandText = "GetAllRounds";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var round = new Round
                {
                    RoundId   = reader["RoundId"] != DBNull.Value ? Convert.ToByte(reader["RoundId"]) : (byte?)null,
                    RoundName = reader["RoundName"] as string
                };
                rounds.Add(round);
            }

            return rounds;
        }
    }
}