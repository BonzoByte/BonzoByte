using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonzoByte.Core.DTOs
{
    public class MatchDTO
    {
        [BsonId]
        public int       matchTPId                  { get; set; }
        public DateTime? dateTime                   { get; set; }
        public int     ? tournamentEventTPId        { get; set; }
        public int     ? tournamentEventCountryTPId { get; set; }
        public string  ? tournamentEventCountryISO2 { get; set; }
        public string  ? tournamentEventCountryISO3 { get; set; }
        public string  ? tournamentEventCountryFull { get; set; }
        public int     ? prize                      { get; set; }
        public int     ? tournamentEventSurfaceId   { get; set; }
        public string  ? tournamentEventSurfaceName { get; set; }
        public DateTime? tournamentEventDate        { get; set; }
        public string  ? tournamentEventName        { get; set; }
        public int     ? tournamentLevelId          { get; set; }
        public string  ? tournamentLevelName        { get; set; }
        public int     ? tournamentTypeId           { get; set; }
        public string  ? tournamentTypeName         { get; set; }
        public int     ? player1TPId                { get; set; }
        public string  ? player1Seed                { get; set; }
        public string  ? player1Name                { get; set; }
        public int     ? player1CountryTPId         { get; set; }
        public string  ? player1CountryISO2         { get; set; }
        public string  ? player1CountryISO3         { get; set; }
        public string  ? player1CountryFull         { get; set; }
        public int     ? player1PlaysId             { get; set; }
        public string  ? player1PlaysName           { get; set; }
        public int     ? player2TPId                { get; set; }
        public string  ? player2Seed                { get; set; }
        public string  ? player2Name                { get; set; }
        public int     ? player2CountryTPId         { get; set; }
        public string  ? player2CountryISO2         { get; set; }
        public string  ? player2CountryISO3         { get; set; }
        public string  ? player2CountryFull         { get; set; }
        public int     ? player2PlaysId             { get; set; }
        public string  ? player2PlaysName           { get; set; }
        public string  ? result                     { get; set; }
        public string  ? resultDetails              { get; set; }
        public double  ? player1Odds                { get; set; }
        public double  ? player2Odds                { get; set; }
        public double  ? player1Percentage          { get; set; }
        public double  ? player2Percentage          { get; set; }
        public int     ? matchSurfaceId             { get; set; }
        public string  ? matchSurfaceName           { get; set; }
        public int     ? roundId                    { get; set; }
        public string  ? roundName                  { get; set; }
        public double  ? winProbabilityPlayer1NN    { get; set; }
        public double  ? winProbabilityPlayer2NN    { get; set; }
        public double  ? bestP1                     { get; set; }
        public double  ? bestP2                     { get; set; }
        public double  ? valueMarginPlayer1         { get; set; }
        public double  ? valueMarginPlayer2         { get; set; }
        public int     ? who2Bet                    { get; set; }
        public bool    ? isFinished                 { get; set; }
    }
}
