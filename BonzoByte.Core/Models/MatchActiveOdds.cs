namespace BonzoByte.Core.Models
{
    public class MatchActiveOdds
    {
        public int     ? MatchTPId   { get; set; }
        public int     ? BookieId    { get; set; }
        public DateTime? DateTime    { get; set; }
        public double  ? Player1Odds { get; set; }
        public double  ? Player2Odds { get; set; }
    }
}