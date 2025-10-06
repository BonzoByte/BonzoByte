using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    // “Merged market” točka u vremenu (preko svih bookija)
    public sealed class MergedOddsPointDTO
    {
        [BsonElement("coalescedTime")] public DateTime CoalescedTime { get; set; }
        [BsonIgnoreIfNull][BsonElement("avgP1")] public decimal? AvgP1 { get; set; }
        [BsonIgnoreIfNull][BsonElement("avgP2")] public decimal? AvgP2 { get; set; }
        [BsonIgnoreIfNull][BsonElement("medianP1")] public decimal? MedianP1 { get; set; }
        [BsonIgnoreIfNull][BsonElement("medianP2")] public decimal? MedianP2 { get; set; }
        [BsonElement("samples")] public int Samples { get; set; }
    }
}