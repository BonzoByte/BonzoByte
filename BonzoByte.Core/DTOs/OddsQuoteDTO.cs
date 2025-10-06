using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    public sealed class OddsQuoteDTO
    {
        [BsonElement("oddsId")] public int OddsId { get; set; }
        [BsonElement("matchTPId")] public int MatchTPId { get; set; }
        [BsonElement("bookieId")] public int BookieId { get; set; }
        [BsonElement("bookieName")] public string BookieName { get; set; } = "";
        [BsonIgnoreIfNull]
        [BsonElement("dateTime")]
        public DateTime? DateTime { get; set; }

        [BsonElement("sourceFileTime")] public DateTime? SourceFileTime { get; set; }
        [BsonElement("coalescedTime")] public DateTime CoalescedTime { get; set; }
        [BsonIgnoreIfNull]
        [BsonElement("seriesOrdinal")]
        public int? SeriesOrdinal { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("player1Odds")]
        public decimal? Player1Odds { get; set; }
        [BsonIgnoreIfNull]
        [BsonElement("player2Odds")]
        public decimal? Player2Odds { get; set; }
    }
}