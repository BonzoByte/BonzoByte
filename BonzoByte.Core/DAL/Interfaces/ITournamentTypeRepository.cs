using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface ITournamentTypeRepository
    {
        Task<IEnumerable<TournamentType>> GetAllTournamentTypesAsync();
    }
}