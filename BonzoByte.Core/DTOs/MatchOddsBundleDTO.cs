using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    // Paket koji ide u MatchDetailsDTO kao sekcija “odds”
    public sealed class MatchOddsBundleDTO
    {
        [BsonElement("matchTPId")] public int MatchTPId { get; set; }
        [BsonElement("bookies")] public List<BookieOddsDTO> Bookies { get; set; } = new();
        [BsonIgnoreIfNull][BsonElement("merged")] public List<MergedOddsPointDTO>? Merged { get; set; }
        [BsonElement("overall")] public MarketAggregateDTO Overall { get; set; } = new();
    }
}