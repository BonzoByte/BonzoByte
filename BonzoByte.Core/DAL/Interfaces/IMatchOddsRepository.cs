using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface IMatchOddsRepository
    {
        Task<IEnumerable<MatchOdds>> GetAllMatchOddsAsync();
        Task<int> InsertMatchOddsAsync(MatchOdds odds);
        Task<IEnumerable<MatchOdds>> GetOddsByMatchAsync(int matchTPId);
        Task UpdateFlagsAsync(int oddsId, bool isSuspicious, bool isLikelySwitched, short suspiciousMask);
    }
}