namespace BonzoByte.Core.Models.TrueSkill
{
    public class SetWonBy
    {
        public string? Set { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public string? ResultDetails { get; set; }
        public int WhoWon { get; set; }
        public int By { get; set; }
        public int Games { get; set; }
        public int GamesP1 { get; set; }
        public int GamesP2 { get; set; }
        public double DistributionBySetsP1 { get; set; }
        public double DistributionByGamesP1 { get; set; }
        public double DistributionBySetsP2 { get; set; }
        public double DistributionByGamesP2 { get; set; }
    }
}