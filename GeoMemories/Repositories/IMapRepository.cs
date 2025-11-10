using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Repositories
{
    public interface IMapRepository
    {
        Task CreateMapPinAsync(MapPin map);
        Task DeleteMapPinAsync(int id);
        Task UpdateMapPinAsync(MapPin map);
        Task<MapPin> GetMapPinAsync(int id);
        Task<List<MapPin>> GetAllMapPinsAsync();
    }
}
