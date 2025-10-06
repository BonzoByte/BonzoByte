using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    public class PlayerMongoDTO
    {
        public int     ? playerTPId          { get; set; }
        public string  ? playerName          { get; set; }
        public int     ? countryTPId         { get; set; }
        public string  ? countryISO2         { get; set; }
        public string  ? countryISO3         { get; set; }
        public string  ? countryFull         { get; set; }
        public DateTime? playerBirthDate     { get; set; }
        public int     ? playerHeight        { get; set; }
        public int     ? playerWeight        { get; set; }
        public int     ? playerTurnedPro     { get; set; }
        public int     ? playsId             { get; set; }
        public string  ? playsName           { get; set; }
        public int     ? tournamentTypeId    { get; set; }
        public string  ? tournamentTypeName  { get; set; }
        public double  ? winRatio            { get; set; }
        public int     ? matches             { get; set; }
        public double  ? trueSkillMean       { get; set; }
        public double  ? careerTrueSkillMean { get; set; }
    }
}