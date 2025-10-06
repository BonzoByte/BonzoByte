using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    public sealed class BookieDTO
    {
        [BsonElement("bookieId")] public int BookieId { get; set; }
        [BsonElement("bookieName")] public string BookieName { get; set; } = "";
    }
}