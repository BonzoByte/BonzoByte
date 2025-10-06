using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BonzoByte.Core.Helpers
{
    public sealed class FrontendShapeConfig
    {
        public int PercentDecimals { get; init; } = 2;
        public int StatDecimals { get; init; } = 2;
        public int OddsDecimals { get; init; } = 2;
        public int DefaultDecimals { get; init; } = 2;

        // Heuristike po nazivu JSON polja (case-insensitive)
        public Func<string, bool> IsPercent { get; init; } = name =>
        {
            name = name.ToLowerInvariant();
            return name.Contains("probability") || name.EndsWith("ratio") || name.EndsWith("rate") || name.EndsWith("perc");
        };

        public Func<string, bool> IsOdds { get; init; } = name =>
        {
            name = name.ToLowerInvariant();
            return name == "player1odds" || name == "player2odds";
        };

        public Func<string, bool> IsStat { get; init; } = name =>
        {
            name = name.ToLowerInvariant();
            return name.Contains("mean") || name.Contains("standarddeviation") || name.EndsWith("sd");
        };
    }

    public static class FrontendNumberShaper
    {
        public static void Shape(object? obj, FrontendShapeConfig? cfg = null)
        {
            if (obj is null) return;
            cfg ??= new FrontendShapeConfig();
            ShapeInternal(obj, cfg, new HashSet<object>(ReferenceEqualityComparer.Instance));
        }

        private static void ShapeInternal(object obj, FrontendShapeConfig cfg, HashSet<object> visited)
        {
            if (obj is null || visited.Contains(obj)) return;
            visited.Add(obj);

            // Kolekcije
            if (obj is IEnumerable en && obj is not string)
            {
                foreach (var item in en) if (item != null) ShapeInternal(item, cfg, visited);
                return;
            }

            var t = obj.GetType();
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead || !p.CanWrite) continue;

                var propType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                var value = p.GetValue(obj);

                if (value is null) continue;

                if (propType == typeof(double) || propType == typeof(float) || propType == typeof(decimal))
                {
                    var name = p.Name; // naziv .NET propertyja; JSON će biti camelCase, ali heuristike su neovisne
                    double v = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                    if (cfg.IsPercent(name))
                    {
                        const double EPS = 1e-9;
                        if (v <= 1.0 + EPS) v = Math.Round(v * 100.0, cfg.PercentDecimals);
                        else v = Math.Round(v, cfg.PercentDecimals);

                        // safety clamp
                        if (!double.IsNaN(v) && !double.IsInfinity(v))
                            v = Math.Max(0.0, Math.Min(100.0, v));
                    }
                    else if (cfg.IsOdds(name))
                    {
                        v = Math.Round(v, cfg.OddsDecimals);
                    }
                    else if (cfg.IsStat(name))
                    {
                        v = Math.Round(v, cfg.StatDecimals);
                    }
                    else
                    {
                        v = Math.Round(v, cfg.DefaultDecimals);
                    }

                    if (propType == typeof(decimal)) p.SetValue(obj, (decimal)v);
                    else if (propType == typeof(float)) p.SetValue(obj, (float)v);
                    else p.SetValue(obj, v);
                }
                else if (!propType.IsPrimitive && propType != typeof(string) && !propType.IsEnum)
                {
                    ShapeInternal(value, cfg, visited); // rekurzija za ugniježđene objekte (npr. Odds bundle)
                }
            }
        }

        // Reference equality comparer da izbjegnemo beskonačne cikluse
        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new();
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}