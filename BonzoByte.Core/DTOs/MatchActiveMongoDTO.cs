using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BonzoByte.Core.DTOs
{
    public class MatchActiveMongoDTO
    {
        [BsonId]
        public long _id                             { get; set; }
        public DateTime? dateTime                   { get; set; }
        public int     ? tournamentEventTPId        { get; set; }
        public int     ? tournamentEventCountryTPId { get; set; }
        public string  ? tournamentEventCountryISO2 { get; set; }
        public string  ? tournamentEventCountryISO3 { get; set; }
        public string  ? tournamentEventCountryFull { get; set; }
        public string  ? tournamentEventName        { get; set; }
        public int     ? surfaceId                  { get; set; }
        public string  ? surfaceName                { get; set; }
        public int     ? roundId                    { get; set; }
        public string  ? roundName                  { get; set; }
        public int     ? tournamentLevelId          { get; set; }
        public string  ? tournamentLevelName        { get; set; }
        public int     ? tournamentTypeId           { get; set; }
        public string  ? tournamentTypeName         { get; set; }
        public int     ? player1TPId                { get; set; }
        public int     ? player1CountryTPId         { get; set; }
        public string  ? player1CountryISO2         { get; set; }
        public string  ? player1CountryISO3         { get; set; }
        public string  ? player1CountryFull         { get; set; }
        public string  ? player1Name                { get; set; }
        public int     ? player2TPId                { get; set; }
        public int     ? player2CountryTPId         { get; set; }
        public string  ? player2CountryISO2         { get; set; }
        public string  ? player2CountryISO3         { get; set; }
        public string  ? player2CountryFull         { get; set; }
        public string  ? player2Name                { get; set; }
        public double  ? player1Odds                { get; set; }
        public double  ? player2Odds                { get; set; }
        public string  ? prize                      { get; set; }
        public double  ? winProbabilityPlayer1NN    { get; set; }
        public double  ? winProbabilityPlayer2NN    { get; set; }
        public double  ? valueMarginPlayer1         { get; set; }
        public double  ? valueMarginPlayer2         { get; set; }
        public string  ? who2Bet                    { get; set; }
        [BsonElement("__v")]
        public int __v                              { get; set; } = 0;
    }
}