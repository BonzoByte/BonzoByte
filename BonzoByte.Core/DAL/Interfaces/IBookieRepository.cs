using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface IBookieRepository
    {
        Task<IEnumerable<Bookie>> GetAllBookiesAsync();
        Task InsertBookieAsync(Bookie bookie);
        Task InsertMatchOddsAsync(MatchOdds odds);
    }
}