using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Repositories
{
    public interface IPlaysRepository
    {
        Task<IEnumerable<Plays>> GetAllPlaysAsync();
    }
}