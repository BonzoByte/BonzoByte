using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class BookieRepository : IBookieRepository
    {
        private readonly IDbConnection _connection;

        public BookieRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Bookie>> GetAllBookiesAsync()
        {
            var bookies = new List<Bookie>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllBookies";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var bookie = new Bookie
                {
                    BookieId   = reader["BookieId"] != DBNull.Value ? Convert.ToInt32(reader["BookieId"]) : (int?)null,
                    BookieName = reader["BookieName"] as string,
                };
                bookies.Add(bookie);
            }

            return bookies;
        }

        public async Task InsertBookieAsync(Bookie bookie)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "InsertBookie";
            command.CommandType = CommandType.StoredProcedure;

            var paramId = command.CreateParameter();
            paramId.ParameterName = "@BookieId";
            paramId.Value = bookie.BookieId!;
            command.Parameters.Add(paramId);

            var paramName = command.CreateParameter();
            paramName.ParameterName = "@BookieName";
            paramName.Value = bookie.BookieName!;
            command.Parameters.Add(paramName);

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            await Task.Run(() => command.ExecuteNonQuery());
        }

        public async Task InsertMatchOddsAsync(MatchOdds odds)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "InsertMatchOdds";
            command.CommandType = CommandType.StoredProcedure;

            var matchIdParam = command.CreateParameter();
            matchIdParam.ParameterName = "@MatchTPId";
            matchIdParam.Value = odds.MatchTPId!;
            command.Parameters.Add(matchIdParam);

            var bookieIdParam = command.CreateParameter();
            bookieIdParam.ParameterName = "@BookieId";
            bookieIdParam.Value = odds.BookieId!;
            command.Parameters.Add(bookieIdParam);

            var dateParam = command.CreateParameter();
            dateParam.ParameterName = "@DateTime";
            dateParam.Value = (object?)odds.DateTime ?? DBNull.Value;
            command.Parameters.Add(dateParam);

            var p1Param = command.CreateParameter();
            p1Param.ParameterName = "@Player1Odds";
            p1Param.Value = (object?)odds.Player1Odds ?? DBNull.Value;
            command.Parameters.Add(p1Param);

            var p2Param = command.CreateParameter();
            p2Param.ParameterName = "@Player2Odds";
            p2Param.Value = (object?)odds.Player2Odds ?? DBNull.Value;
            command.Parameters.Add(p2Param);

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            await Task.Run(() => command.ExecuteNonQuery());
        }
    }
}