namespace BonzoByte.Core.Models
{
    public class TournamentEvent
    {
        public int        ? TournamentEventTPId { get; set; }
        public string     ? TournamentEventName { get; set; }
        public int        ? CountryTPId         { get; set; }
        public DateTime   ? TournamentEventDate { get; set; }
        public int        ? TournamentLevelId   { get; set; }
        public int        ? TournamentTypeId    { get; set; }
        public int        ? Prize               { get; set; }
        public int        ? SurfaceId           { get; set; }
    }
}