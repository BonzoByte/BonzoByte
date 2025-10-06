using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    public class TournamentEventMongoDTO
    {
        public int                  tournamentEventTPId { get; set; }
        public string  ?            tournamentEventName { get; set; }
        public DateTime?            tournamentEventDate { get; set; }
        public short   ?            countryTPId         { get; set; }
        public string  ?            countryISO2         { get; set; }
        public string  ?            countryISO3         { get; set; }
        public string  ?            countryFull         { get; set; }
        public byte    ?            tournamentLevelId   { get; set; }
        public string  ?            tournamentLevelName { get; set; }
        public byte    ?            tournamentTypeId    { get; set; }
        public string  ?            tournamentTypeName  { get; set; }
        public int     ?            surfaceId           { get; set; }
        public string  ?            surfaceName         { get; set; }
        public string  ?            prize               { get; set; }
        public int     ?            matches             { get; set; }
        public List<MatchDetailsDTO>? matchMongoDTOs      { get; set; }
    }
}