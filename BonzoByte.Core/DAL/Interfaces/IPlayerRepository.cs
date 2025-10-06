using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Repositories
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task InsertPlayerAsync(Player player);
        Task UpdatePlayerAsync(Player player, Models.Match match);
        Task<bool> ExistsAsync(int playerTPId);
    }
}