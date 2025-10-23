using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    public interface IMemoryDB
    {
        //Trip CRUD methods
        Task CreateTripAsync(Trip trip);
        Task DeleteTripAsync(int id);
        Task<Trip> GetTripByIdAsync(int id);
        Task<List<Trip>> GetAllTripAsync();
        Task UpdateTripAsync(Trip trip);

        //Picture CRUD methods
        Task<Picture> GetPictureByIdAsync(int id);
        Task DeletePictureByIdAsync(int id);
        Task<List<Picture>> GetAllPictures();
        Task UpdatePictureAsync(Picture picture);
        Task CreatePictureAsync(Picture picture);

        //Mapping CRUD methods
        Task CreateMapPinAsync(MapPin map);
        Task DeleteMapPinAsync(int id);
        Task UpdateMapPinAsync(MapPin map);
        Task<MapPin> GetMapPinAsync(int id);
        Task<List<MapPin>> GetAllMapPinsAsync();
    }
}
