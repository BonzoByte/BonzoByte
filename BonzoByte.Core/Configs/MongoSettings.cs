namespace BonzoByte.Core.Configs
{
    public class MongoSettings
    {
        public required string ConnectionString { get; set; }
        public required string Database         { get; set; }
    }
}