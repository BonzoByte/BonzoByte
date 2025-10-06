using BonzoByte.Core.Models;
using System.Data;
using System.Drawing;

namespace BonzoByte.Core.DAL.Repositories
{
    public class TournamentEventRepository : ITournamentEventRepository
    {
        private readonly IDbConnection _connection;

        public TournamentEventRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<TournamentEvent>> GetAllTournamentEventsAsync()
        {
            var TournamentEvents = new List<TournamentEvent>();

            using var command   = _connection.CreateCommand();
            command.CommandText = "GetAllTournamentEvents";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                try
                {
                    var TournamentEventTPId = reader["TournamentEventTPId"] as int?;
                    var TournamentEventName = reader["TournamentEventName"] as string;
                    var CountryTPId = reader["CountryTPId"];
                    var TournamentEventDate = reader["TournamentEventDate"] as DateTime?;
                    var TournamentLevelId = reader["TournamentLevelId"];
                    var TournamentTypeId = reader["TournamentTypeId"];
                    var Prize = reader["Prize"] as int?;
                    var SurfaceId = reader["SurfaceId"];
                    var TournamentEvent = new TournamentEvent
                    {
                        TournamentEventTPId = reader["TournamentEventTPId"] as int?,
                        TournamentEventName = reader["TournamentEventName"] as string,
                        CountryTPId = CountryTPId as int?,
                        TournamentEventDate = reader["TournamentEventDate"] as DateTime?,
                        TournamentLevelId = TournamentLevelId as int?,
                        TournamentTypeId = TournamentTypeId as int?,
                        Prize = reader["Prize"] as int?,
                        SurfaceId = (int)SurfaceId
                    };
                    TournamentEvents.Add(TournamentEvent);

                }
                catch (Exception ex)
                {
                    string aaa;
                    aaa = ex.Message;
                }


            }

            return TournamentEvents;
        }

        public async Task InsertTournamentEvent(TournamentEvent tournamentEvent, Models.Match match)
        {
            try
            {
                using var command = _connection.CreateCommand();
                command.CommandText = "InsertTournamentEvent";
                command.CommandType = CommandType.StoredProcedure;

                AddParam(command, "@TournamentEventTPId", tournamentEvent.TournamentEventTPId);
                AddParam(command, "@TournamentEventName", tournamentEvent.TournamentEventName);
                AddParam(command, "@CountryTPId", tournamentEvent.CountryTPId);
                AddParam(command, "@TournamentEventDate", tournamentEvent.TournamentEventDate);
                AddParam(command, "@TournamentLevelId", tournamentEvent.TournamentLevelId);
                AddParam(command, "@TournamentTypeId", tournamentEvent.TournamentTypeId);
                AddParam(command, "@Prize", tournamentEvent.Prize);
                AddParam(command, "@SurfaceId", tournamentEvent.SurfaceId);

                if (_connection.State != ConnectionState.Open) _connection.Open();
                await Task.Run(() => command.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                string aaa;
                aaa = "";
            }

        }

        private void AddParam(IDbCommand cmd, string name, object? value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }
    }
}