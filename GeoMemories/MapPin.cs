using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoMemories
{
    public class MapPin
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(nameof(Trip.ID))]
        public int TripID { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}