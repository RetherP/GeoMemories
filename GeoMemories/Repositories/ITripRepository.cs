using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Repositories
{
    public interface ITripRepository
    {
        Task CreateTripAsync(Trip trip);
        Task DeleteTripAsync(int id);
        Task<Trip> GetTripByIdAsync(int id);
        Task<List<Trip>> GetAllTripAsync();
        Task UpdateTripAsync(Trip trip);
    }
}
