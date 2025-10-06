using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Repositories
{
    public interface ITournamentEventRepository
    {
        Task<IEnumerable<TournamentEvent>> GetAllTournamentEventsAsync();
        Task InsertTournamentEvent(TournamentEvent tournamentEvent, Models.Match match);
    }
}