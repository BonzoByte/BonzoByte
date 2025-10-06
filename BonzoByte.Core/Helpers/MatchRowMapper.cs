using static BonzoByte.Core.Services.MatchTournamentParser;

namespace BonzoByte.Core.Helpers
{
    public sealed class MatchRowMapper
    {
        private readonly IReadOnlyList<ColumnSpec> _cols;
        private readonly IReadOnlyDictionary<string, Func<Models.Match, object?>> _getters;

        public MatchRowMapper(IReadOnlyList<ColumnSpec> cols,
                              IReadOnlyDictionary<string, Func<Models.Match, object?>> getters)
        {
            _cols = cols;
            _getters = getters;
        }

        public string BuildValuesTuple(Models.Match m)
        {
            var parts = new string[_cols.Count];
            for (int i = 0; i < _cols.Count; i++)
            {
                var col = _cols[i];
                var val = _getters.TryGetValue(col.Name, out var fn) ? fn(m) : null;
                parts[i] = SqlValueFormatter.Format(val, col);
            }
            return "(" + string.Join(", ", parts) + ")";
        }

        public string BuildColumnList() => string.Join(", ", _cols.Select(c => $"[{c.Name}]"));
    }
}