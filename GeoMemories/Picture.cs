using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    public partial class Picture: ObservableObject
    {
        [ObservableProperty]
        [property: PrimaryKey]
        [property: AutoIncrement]
        int iD;
        [ObservableProperty]
        [property: ForeignKey(nameof(Trip.ID))]
        int tripID;
        [ObservableProperty]
        string filePath;
        
        public Picture GetCopy()
        {
            return (Picture)this.MemberwiseClone();
        }
    }
}
