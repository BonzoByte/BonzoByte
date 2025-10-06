using BonzoByte.Core.DAL.Interfaces;
using BonzoByte.Core.Models;
using System.Data;

namespace BonzoByte.Core.DAL.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IDbConnection _connection;

        public CountryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            var countries = new List<Country>();

            using var command = _connection.CreateCommand();
            command.CommandText = "GetAllCountries";
            command.CommandType = CommandType.StoredProcedure;

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using var reader = await Task.Run(() => command.ExecuteReader());
            while (await Task.Run(() => reader.Read()))
            {
                var country = new Country
                {
                    CountryTPId = reader["CountryTPId"] != DBNull.Value ? Convert.ToInt32(reader["CountryTPId"]) : (int?)null,
                    CountryISO2 = reader["CountryISO2"] as string,
                    CountryISO3 = reader["CountryISO3"] as string,
                    CountryFull = reader["CountryFull"] as string
                };
                countries.Add(country);
            }

            return countries;
        }
    }
}