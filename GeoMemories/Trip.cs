using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    public class Trip
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
    }
}