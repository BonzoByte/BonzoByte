using BonzoByte.Core.Models.TrueSkill;
using Microsoft.Extensions.Logging;

namespace BonzoByte.Core.Comparers
{
    /// <summary>
    /// Komparator za sortiranje gemova po poziciji.
    /// </summary>
    public class GameWonRankComparer : IComparer<GameWonRank>
    {
        private readonly ILogger _logger;

        public GameWonRankComparer(ILogger logger)
        {
            _logger = logger;
        }

        public int Compare(GameWonRank? x, GameWonRank? y)
        {
            if (x == null || y == null)
            {
                _logger.LogWarning("Null encountered in GameWonRank comparison.");
                return 0;
            }

            return x.GamePos.CompareTo(y.GamePos);
        }
    }
}