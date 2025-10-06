using BonzoByte.Core.Models;
using BonzoByte.ML;

namespace BonzoByte.Core.Services.Interfaces
{
    public interface ICore40FeatureExtractor
    {
        Task<FeatureVector?> BuildAsync(int matchTPId, CancellationToken ct = default);
    }
}