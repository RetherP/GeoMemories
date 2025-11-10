using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Repositories
{
    public class MapRepository : IMapRepository
    {
        DbContext _dbContext;
        public MapRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateMapPinAsync(MapPin map)
        {
            await _dbContext.db.InsertAsync(map);
        }

        public async Task DeleteMapPinAsync(int id)
        {
            var toRemove = await _dbContext.db.Table<MapPin>().Where(x => x.ID==id).FirstOrDefaultAsync();
            await _dbContext.db.DeleteAsync(toRemove);
        }

        public async Task<List<MapPin>> GetAllMapPinsAsync()
        {
            return await _dbContext.db.Table<MapPin>().ToListAsync();
        }

        public async Task<MapPin> GetMapPinAsync(int id)
        {
            return await _dbContext.db.Table<MapPin>().Where(x=>x.ID == id).FirstOrDefaultAsync();
        }

        public async Task UpdateMapPinAsync(MapPin map)
        {
            await _dbContext.db.UpdateAsync(map);
        }
    }
}
