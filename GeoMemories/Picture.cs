using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    public class Picture
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        [ForeignKey(nameof(Trip.ID))]
        public int TripID { get; set; }
        public string FilePath { get; set; } = string.Empty;
        
    }
}
