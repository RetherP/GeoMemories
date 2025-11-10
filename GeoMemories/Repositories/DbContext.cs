using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Repositories
{
    public class DbContext
    {
        SQLiteOpenFlags Flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create;
        string dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "memory.db3");
        public SQLiteAsyncConnection db;

        public DbContext()
        {
            db = new SQLiteAsyncConnection(dbPath, Flags);
            db.CreateTableAsync<Trip>().Wait();
            db.CreateTableAsync<MapPin>().Wait();
            db.CreateTableAsync<Picture>().Wait();
        }
    }
}
