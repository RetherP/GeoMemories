using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Repositories
{
    public interface IPictureRepository
    {
        Task<Picture> GetPictureByIdAsync(int id);
        Task DeletePictureByIdAsync(int id);
        Task<List<Picture>> GetAllPicturesAsync();
        Task UpdatePictureAsync(Picture picture);
        Task CreatePictureAsync(Picture picture);

    }
}
