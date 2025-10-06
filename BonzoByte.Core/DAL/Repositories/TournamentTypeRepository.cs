using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class TournamentTypeRepository : ITournamentTypeRepository
    {
        private readonly IDbConnection _connection;

        public TournamentTypeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<TournamentType>> GetAllTournamentTypesAsync()
        {
            var TournamentTypes = new List<TournamentType>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllTournamentTypes";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var TournamentType = new TournamentType
                {
                    TournamentTypeId = reader["TournamentTypeId"] != DBNull.Value ? Convert.ToByte(reader["TournamentTypeId"]) : (byte?)null,
                    TournamentTypeName = reader["TournamentTypeName"] as string
                };
                TournamentTypes.Add(TournamentType);
            }

            return TournamentTypes;
        }
    }
}