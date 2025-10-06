namespace BonzoByte.Core.Models
{
    public class User
    {
        public int   ? UserId        { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserLastName  { get; set; }
        public string? UserMail      { get; set; }
        public string? UserPassword  { get; set; }
        public int   ? UserTypeId    { get; set; }
        public bool  ? Registered    { get; set; }
    }
}