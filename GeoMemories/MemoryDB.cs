using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    public class MemoryDB : IMemoryDB
    {
        SQLite.SQLiteOpenFlags Flags = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create;
        string dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "memories.db3");
        SQLiteAsyncConnection db;
        public MemoryDB()
        {
            db = new SQLiteAsyncConnection(dbPath,Flags);
            db.CreateTableAsync<Trip>().Wait();
            db.CreateTableAsync<MapPin>().Wait();
            db.CreateTableAsync<Picture>().Wait();
        }
        public async Task CreateMapPinAsync(MapPin map)
        {
            await db.InsertAsync(map);
        }

        public async Task CreatePictureAsync(Picture picture)
        {
            await db.InsertAsync(picture);
        }

        public async Task CreateTripAsync(Trip trip)
        {
            await db.InsertAsync(trip);
        }

        public async Task DeleteMapPinAsync(int id)
        {
            var ToRemove = db.Table<MapPin>().Where(x => x.ID == id);
            await db.DeleteAsync(ToRemove);
        }

        public async Task DeletePictureByIdAsync(int id)
        {
            var ToRemove = db.Table<Picture>().Where(x => x.ID == id);
            await db.DeleteAsync(ToRemove);
        }

        public async Task DeleteTripAsync(int id)
        {
            var ToRemove = db.Table<Trip>().Where(x => x.ID == id);
            await db.DeleteAsync(ToRemove);
        }

        public async Task<List<MapPin>> GetAllMapPinsAsync()
        {
            return await db.Table<MapPin>().ToListAsync();
        }

        public async Task<List<Picture>> GetAllPictures()
        {
            return await db.Table<Picture>().ToListAsync();
        }

        public async Task<List<Trip>> GetAllTripAsync()
        {
            return await db.Table<Trip>().ToListAsync();
        }

        public async Task<MapPin> GetMapPinAsync(int id)
        {
            return await db.Table<MapPin>().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<Picture> GetPictureByIdAsync(int id)
        {
            return await db.Table<Picture>().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<Trip> GetTripByIdAsync(int id)
        {
            return await db.Table<Trip>().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task UpdateMapPinAsync(MapPin map)
        {
            await db.UpdateAsync(map);
        }

        public async Task UpdatePictureAsync(Picture picture)
        {
            await db.UpdateAsync(picture);
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            await db.UpdateAsync(trip);
        }
    }
}
