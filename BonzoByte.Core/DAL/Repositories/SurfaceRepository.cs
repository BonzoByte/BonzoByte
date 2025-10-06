using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class SurfaceRepository : ISurfaceRepository
    {
        private readonly IDbConnection _connection;

        public SurfaceRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Surface>> GetAllSurfacesAsync()
        {
            var surfaces = new List<Surface>();

            using var command   = _connection.CreateCommand();
            command.CommandText = "GetAllSurfaces";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var surface = new Surface
                {
                    SurfaceId   = reader["SurfaceId"] != DBNull.Value ? Convert.ToByte(reader["SurfaceId"]) : (byte?)null,
                    SurfaceName = reader["SurfaceName"] as string
                };
                surfaces.Add(surface);
            }

            return surfaces;
        }
    }
}