using BonzoByte.Core.Models;
using BonzoByte.Core.Services.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

public sealed class Core40FeatureExtractor : ICore40FeatureExtractor
{
    private readonly string _connStr;
    private readonly IReadOnlyList<string> _featureNames;

    public Core40FeatureExtractor(string connStr, IReadOnlyList<string> featureNames)
    {
        _connStr = connStr;
        _featureNames = featureNames;
    }

    public async Task<FeatureVector?> BuildAsync(int matchTPId, CancellationToken ct = default)
    {
        // 1) Vektor 1..40 iz TVF-a (kanonski poredak po _featureNames)
        var x = await BuildVectorFromTvfAsync(matchTPId, _featureNames, ct);

        // 2) Router metrika i meta iz dbo.Match
        var row = await LoadMatchRawAsync(matchTPId, ct);
        if (row == null) return null;

        int p1Matches = GetInt(row, "Player1WinsTotal") + GetInt(row, "Player1LossesTotal");
        int p2Matches = GetInt(row, "Player2WinsTotal") + GetInt(row, "Player2LossesTotal");

        // NEW: SurfaceId i H2HMatches
        int surfaceId = GetInt(row, "SurfaceId");                   // 1=unknown,2=clay,3=grass,4=hard
        int p1H2H = GetInt(row, "Player1H2HOld");                   // ili izmijeni na svoj naziv
        int p2H2H = GetInt(row, "Player2H2HOld");                   // "
        int h2hMatches = Math.Max(0, Math.Max(p1H2H, p2H2H));       // sigurni minimum

        return new FeatureVector
        {
            MatchTPId = matchTPId,
            Names = _featureNames,
            Values = x,
            P1Matches = p1Matches,
            P2Matches = p2Matches,
            SurfaceId = surfaceId,
            H2HMatches = h2hMatches
        };
    }

    private async Task<double[]> BuildVectorFromTvfAsync(int matchTPId, IReadOnlyList<string> featureNames, CancellationToken ct)
    {
        var values = new double[featureNames.Count];

        await using var con = new SqlConnection(_connStr);
        await con.OpenAsync(ct);

        const string sql = "SELECT * FROM dbo.fn_NN_Features_v1(@mid);";
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@mid", SqlDbType.Int).Value = matchTPId;
        cmd.CommandTimeout = 0;

        await using var rd = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
        if (!await rd.ReadAsync(ct))
            throw new InvalidOperationException($"fn_NN_Features_v1({matchTPId}) returned no rows.");

        if (rd.FieldCount < featureNames.Count)
            throw new InvalidOperationException($"TVF returned {rd.FieldCount} cols, need {featureNames.Count}.");

        for (int i = 0; i < featureNames.Count; i++)
        {
            var colName = featureNames[i];
            int ord = rd.GetOrdinal(colName); // fail-fast ako kolona ne postoji
            object o = rd.GetValue(ord);
            double v = (o is DBNull) ? 0.0 : Convert.ToDouble(o, CultureInfo.InvariantCulture);
            if (double.IsNaN(v) || double.IsInfinity(v)) v = 0.0;
            values[i] = v;
        }

        return values;
    }

    private async Task<Dictionary<string, object>?> LoadMatchRawAsync(int matchTPId, CancellationToken ct)
    {
        await using var con = new SqlConnection(_connStr);
        await con.OpenAsync(ct);

        const string sql = @"SELECT * FROM dbo.Match WHERE MatchTPId = @mid;";
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@mid", SqlDbType.Int).Value = matchTPId;

        await using var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
        if (!await rdr.ReadAsync(ct)) return null;

        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < rdr.FieldCount; i++)
            dict[rdr.GetName(i)] = rdr.GetValue(i);
        return dict;
    }

    private static int GetInt(IDictionary<string, object> row, string col)
        => row.TryGetValue(col, out var v) && v is not DBNull
           ? Convert.ToInt32(v, CultureInfo.InvariantCulture)
           : 0;
}