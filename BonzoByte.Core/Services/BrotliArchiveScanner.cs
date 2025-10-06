using Microsoft.Extensions.Configuration;
using System.Diagnostics.Metrics;

namespace BonzoByte.Core.Services
{
    public class BrotliArchiveScanner
    {
        private readonly string[] _searchDirectories;

        public BrotliArchiveScanner(IConfiguration configuration)
        {
            _searchDirectories = new[]
            {
                "d:\\brArchives\\Results\\Finished\\",
                "d:\\brArchives\\Results\\Working\\"
            };
        }

        /// <summary>
        /// Returns a dictionary of all .br archive keys (yyyy_MM_dd_tp) and their full paths.
        /// </summary>
        public Dictionary<string, string> GetAllArchiveKeys()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var allDates = new List<DateTime>();

            foreach (var dir in _searchDirectories)
            {
                if (!Directory.Exists(dir))
                    continue;

                foreach (var filePath in Directory.EnumerateFiles(dir, "*.br", SearchOption.TopDirectoryOnly))
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var parts = fileName.Split('_');
                    if (parts.Length < 4) continue;

                    if (DateTime.TryParseExact($"{parts[0]}_{parts[1]}_{parts[2]}", "yyyy_MM_dd", null,
                        System.Globalization.DateTimeStyles.None, out var date))
                    {
                        allDates.Add(date);
                    }

                    // Izbjegni duplikate
                    if (!dict.ContainsKey(fileName))
                    {
                        dict[fileName] = filePath;
                    }
                }
            }

            // ➕ Dodaj 3 dana unazad od najkasnijeg datuma
            if (allDates.Any())
            {
                var latestDate = allDates.Max();

                for (int i = 1; i <= 8; i++)
                {
                    var retroDate = latestDate.AddDays(-i);

                    if (retroDate.Year >= 1990)
                    {
                        for (int t = 1; t <= 4; t++)
                        {
                            var key = $"{retroDate:yyyy_MM_dd}_t{t}";
                            if (!dict.ContainsKey(key))
                            {
                                dict[key] = "[synthetic]"; // nije pravi file path, ali tretira se kao da postoji
                            }
                        }
                    }
                }
            }

            return dict;
        }

    }
}