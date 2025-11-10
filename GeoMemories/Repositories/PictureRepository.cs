using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Repositories
{
    public class PictureRepository : IPictureRepository
    {
        DbContext _dbContext;

        public PictureRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreatePictureAsync(Picture picture)
        {
            await _dbContext.db.InsertAsync(picture);
        }

        public async Task DeletePictureByIdAsync(int id)
        {
            var toRemove = await _dbContext.db.Table<Picture>().Where(x => x.ID == id).FirstOrDefaultAsync();
            await _dbContext.db.DeleteAsync(toRemove);
        }

        public async Task<List<Picture>> GetAllPicturesAsync()
        {
            return await _dbContext.db.Table<Picture>().ToListAsync();
        }

        public async Task<Picture> GetPictureByIdAsync(int id)
        {
            return await _dbContext.db.Table<Picture>().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task UpdatePictureAsync(Picture picture)
        {
            await _dbContext.db.UpdateAsync(picture);
        }
    }
}
