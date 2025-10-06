using BonzoByte.Core.Models;

namespace BonzoByte.Core.DAL.Interfaces
{
    public interface ISurfaceRepository
    {
        Task<IEnumerable<Surface>> GetAllSurfacesAsync();
    }
}