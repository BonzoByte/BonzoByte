using System.Data;
using System.Globalization;
using static BonzoByte.Core.Services.MatchTournamentParser;

namespace BonzoByte.Core.Helpers
{
    public static class SqlValueFormatter
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        public static string Format(object? value, ColumnSpec spec)
        {
            if (value is null) return "NULL";

            switch (spec.Type)
            {
                case SqlDbType.Int:
                case SqlDbType.TinyInt:
                    return Convert.ToInt32(value).ToString(Inv);

                case SqlDbType.Bit:
                    return value is bool b ? (b ? "1" : "0") : "NULL";

                case SqlDbType.DateTime:
                    if (value is DateTime dt)
                    {
                        // ako koristiš UTC: dt = dt.ToUniversalTime();
                        return $"'{dt:yyyy-MM-dd HH:mm:ss.fff}'";
                    }
                    return "NULL";

                case SqlDbType.Decimal:
                    // očekujemo scale=2 za (5,2) – za formatiranje je dovoljno round(2)
                    if (value is IConvertible)
                    {
                        var d = Convert.ToDecimal(value, Inv);
                        var rounded = Math.Round(d, spec.Scale ?? 2, MidpointRounding.AwayFromZero);
                        // opcijski range guard za (5,2): -999.99..999.99
                        if ((spec.Scale ?? 2) == 2 && (rounded < -999.99m || rounded > 999.99m))
                            return "NULL";
                        return rounded.ToString(Inv);
                    }
                    return "NULL";

                case SqlDbType.Float:
                    if (value is IConvertible)
                    {
                        var dbl = Convert.ToDouble(value, Inv);
                        if (double.IsNaN(dbl) || double.IsInfinity(dbl)) return "NULL";
                        return dbl.ToString("G17", Inv);
                    }
                    return "NULL";

                case SqlDbType.Char:
                case SqlDbType.VarChar:
                    return QuoteVarchar(Convert.ToString(value, Inv)!, spec.Length);

                case SqlDbType.NVarChar:
                    return QuoteNVarChar(Convert.ToString(value, Inv)!, spec.Length);

                default:
                    // Add more types if needed
                    return "NULL";
            }
        }

        private static string QuoteVarchar(string s, int? maxLen)
        {
            if (maxLen is int L && s.Length > L) s = s.Substring(0, L);
            s = s.Replace("'", "''");
            return $"'{s}'";
        }

        private static string QuoteNVarChar(string s, int? maxLen)
        {
            if (maxLen is int L && s.Length > L) s = s.Substring(0, L);
            s = s.Replace("'", "''");
            return $"N'{s}'";
        }
    }
}