using System.Text;

namespace BonzoByte.Core.Helpers
{
    public static class InvalidResultLogger
    {
        private static readonly object _lock = new();
        private static string _filePath = @"d:\exported\invalid-results.log"; // prilagodi po želji

        public static void ConfigurePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            _filePath = path;
        }

        public static void Log(string context, string raw)
        {
            try
            {
                lock (_lock)
                {
                    var line = $"{DateTime.Now:O}\t{context}\t{raw.Replace("\r", " ").Replace("\n", " ")}{Environment.NewLine}";
                    File.AppendAllText(_filePath, line, Encoding.UTF8);
                }
            }
            catch { /* best-effort */ }
        }
    }
}