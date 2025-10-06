using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    // Ukupni agregati (preko svih bookija i svih citata)
    public sealed class MarketAggregateDTO
    {
        [BsonIgnoreIfNull][BsonElement("opening")] public OddsQuoteDTO? Opening { get; set; }
        [BsonIgnoreIfNull][BsonElement("closing")] public OddsQuoteDTO? Closing { get; set; }

        [BsonIgnoreIfNull][BsonElement("p1Min")] public decimal? P1Min { get; set; }
        [BsonIgnoreIfNull][BsonElement("p1Max")] public decimal? P1Max { get; set; }
        [BsonIgnoreIfNull][BsonElement("p1Avg")] public decimal? P1Avg { get; set; }
        [BsonIgnoreIfNull][BsonElement("p1Median")] public decimal? P1Median { get; set; }

        [BsonIgnoreIfNull][BsonElement("p2Min")] public decimal? P2Min { get; set; }
        [BsonIgnoreIfNull][BsonElement("p2Max")] public decimal? P2Max { get; set; }
        [BsonIgnoreIfNull][BsonElement("p2Avg")] public decimal? P2Avg { get; set; }
        [BsonIgnoreIfNull][BsonElement("p2Median")] public decimal? P2Median { get; set; }

        [BsonElement("samples")] public int Samples { get; set; }
    }
}