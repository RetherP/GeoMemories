using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Repositories
{
    public class TripRepository : ITripRepository
    {
        DbContext _dbContext;
        public TripRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateTripAsync(Trip trip)
        {
            await _dbContext.db.InsertAsync(trip);
        }

        public async Task DeleteTripAsync(int id)
        {
            var toRemove = await _dbContext.db.Table<Trip>().Where(x => x.ID == id).FirstOrDefaultAsync();
            await _dbContext.db.DeleteAsync(toRemove);
        }

        public async Task<List<Trip>> GetAllTripAsync()
        {
            return await _dbContext.db.Table<Trip>().ToListAsync();
        }

        public async Task<Trip> GetTripByIdAsync(int id)
        {
            return await _dbContext.db.Table<Trip>().Where(x=> x.ID == id).FirstOrDefaultAsync();
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            await _dbContext.db.UpdateAsync(trip);
        }
    }
}
