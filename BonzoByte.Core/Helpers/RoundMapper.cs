namespace BonzoByte.Core.Helpers
{
    public static class RoundMapper
    {
        private static readonly Dictionary<string, int> _map = new()
        {
            ["kvalifikacije"] = 1,
            ["1. kolo"] = 2,
            ["2. kolo"] = 3,
            ["3. kolo"] = 4,
            ["4. kolo"] = 5,
            ["5. kolo"] = 6,
            ["-5. kolo"] = 6,
            ["osmina-finala"] = 7,
            ["četvrt-finala"] = 8,
            ["cetvrt-finala"] = 8,
            ["polu-finale"] = 9,
            ["finale"] = 10
        };

        public static int? Map(string roundName)
        {
            if (string.IsNullOrWhiteSpace(roundName)) return null;
            roundName = roundName.Trim().ToLowerInvariant();
            // prvo probaj precizne mape
            return roundName switch
            {
                "kvalifikacije" => 1,
                "1. kolo" => 2,
                "2. kolo" => 3,
                "3. kolo" => 4,
                "4. kolo" => 5,
                "5. kolo" => 6,
                "-5. kolo" => 6,
                "osmina-finala" => 7,
                "cetvrt-finala" => 8,
                "četvrt-finala" => 8,
                "polu-finale" => 9,
                "finale" => 10,
                _ => null
            };
        }

    }
}