using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class PlaysRepository : IPlaysRepository
    {
        private readonly IDbConnection _connection;

        public PlaysRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Plays>> GetAllPlaysAsync()
        {
            var plays = new List<Plays>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllPlays";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var play = new Plays
                {
                    PlaysId = reader["PlaysId"] != DBNull.Value ? Convert.ToByte(reader["PlaysId"]) : (byte?)null,
                    PlaysName = reader["PlaysName"] as string
                };
                plays.Add(play);
            }

            return plays;
        }
    }
}