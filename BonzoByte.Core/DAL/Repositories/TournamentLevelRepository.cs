using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class TournamentLevelRepository : ITournamentLevelRepository
    {
        private readonly IDbConnection _connection;

        public TournamentLevelRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<TournamentLevel>> GetAllTournamentLevelsAsync()
        {
            var TournamentLevels = new List<TournamentLevel>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllTournamentLevels";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var TournamentLevel = new TournamentLevel
                {
                    TournamentLevelId = reader["TournamentLevelId"] != DBNull.Value ? Convert.ToByte(reader["TournamentLevelId"]) : (byte?)null,
                    TournamentLevelName = reader["TournamentLevelName"].ToString()!.Trim() as string
                };
                TournamentLevels.Add(TournamentLevel);
            }

            return TournamentLevels;
        }
    }
}