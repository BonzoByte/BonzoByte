using System.Text;

namespace BonzoByte.Core.Logging
{
    public static class InvalidMatchLogger
    {
        private static readonly string path = "logs/invalid_matches.csv";
        private static bool headerWritten = false;

        public static void Log(int matchId, List<string> p1, List<string> p2, string reason)
        {
            try
            {
                Directory.CreateDirectory("logs");
                using var sw = new StreamWriter(path, append: true, Encoding.UTF8);

                if (!headerWritten && new FileInfo(path).Length == 0)
                {
                    sw.WriteLine("MatchTPId,P1SetScores,P2SetScores,Reason");
                    headerWritten = true;
                }

                var p1Str = string.Join(" ", p1).Replace("\"", "'");
                var p2Str = string.Join(" ", p2).Replace("\"", "'");
                var reasonClean = reason.Replace("\"", "'");

                sw.WriteLine($"{matchId},\"{p1Str}\",\"{p2Str}\",\"{reasonClean}\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Greška pri logiranju invalid meča: {ex.Message}");
            }
        }
    }
}