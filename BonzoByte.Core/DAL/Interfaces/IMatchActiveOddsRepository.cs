using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface IMatchActiveOddsRepository
    {
        Task<IEnumerable<MatchActiveOdds>> GetAllMatchActiveOddsAsync();
    }
}