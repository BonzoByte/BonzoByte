namespace BonzoByte.Core.Models
{
    public class MatchOdds
    {
        public int? MatchTPId { get; set; }
        public int? BookieId { get; set; }
        public DateTime? DateTime { get; set; }   // UTC or null
        public DateTime? SourceFileTime { get; set; }   // UTC, not null in DB (ima default)
        public DateTime? CoalescedTime { get; set; }   // computed kolona u DB; u modelu opcionalno
        public int? SeriesOrdinal { get; set; }   // 0 = header (najnovije u bloku), 1,2,...
        public decimal? Player1Odds { get; set; }   // decimal(9,3)
        public decimal? Player2Odds { get; set; }   // decimal(9,3)
        public DateTime? IngestedAt { get; set; }   // UTC
        public bool? IsSuspicious { get; set; }   // bit
        public bool? IsLikelySwitched { get; set; }   // bit
        public short? SuspiciousMask { get; set; }   // smallint
        public int? OddsId { get; set; }   // identity PK (ako ga želiš čitati)
    }
}