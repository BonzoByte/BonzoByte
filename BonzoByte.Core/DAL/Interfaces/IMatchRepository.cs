using BonzoByte.Core.DTOs;
using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllMatchesAsync();
        Task<IEnumerable<Match>> GetAllMatchesForPlayerAsync(int playerTPId);
        Task<IEnumerable<Match>> GetAllMatchesForTournamentAsync(int tournamentEventTPId);
        Match GetLatestH2HMatchAsync(int p1, int p2);
        Task InsertMatchAsync(Match match);
        Task<bool> ExistsAsync(int te, int p1, int p2);
        Task<int> DeleteActiveByIdAsync(int matchTpId, CancellationToken ct = default);
        Task<IReadOnlyList<OddsQuoteDTO>> GetCleanOddsByMatchAsync(int matchTPId);
    }
}