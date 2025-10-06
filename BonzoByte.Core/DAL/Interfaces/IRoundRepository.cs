using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface IRoundRepository
    {
        Task<IEnumerable<Round>> GetAllRoundsAsync();
    }
}