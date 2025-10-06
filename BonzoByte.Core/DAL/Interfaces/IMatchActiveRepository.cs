using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface IMatchActiveRepository
    {
        Task<IEnumerable<MatchActive>> GetAllMatchesActiveAsync();
    }
}