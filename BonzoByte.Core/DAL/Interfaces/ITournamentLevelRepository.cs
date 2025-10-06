using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface ITournamentLevelRepository
    {
        Task<IEnumerable<TournamentLevel>> GetAllTournamentLevelsAsync();
    }
}