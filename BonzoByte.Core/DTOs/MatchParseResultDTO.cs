using BonzoByte.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonzoByte.Core.DTOs
{
    public class MatchParseResultDTO
    {
        public List<TournamentEvent> TournamentEvents { get; set; } = new();
        public List<Match> Matches { get; set; } = new();
        public List<Match> MatchesLastWeek { get; set; } = new();
        public List<Match> MatchesLastMonth { get; set; } = new();
        public List<Match> MatchesLastYear { get; set; } = new();
        public List<Player> Players { get; set; } = new();
    }
}
