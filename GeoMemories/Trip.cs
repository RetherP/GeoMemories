using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    public partial class Trip:ObservableObject
    {
        [ObservableProperty]
        [property: PrimaryKey]
        [property: AutoIncrement]
        int iD;
        [ObservableProperty]
        string name;
        [ObservableProperty]
        string description;
        [ObservableProperty]
        DateTime startDate;
        [ObservableProperty]
        DateTime endDate;

        public Trip GetCopy()
        {
            return (Trip)this.MemberwiseClone();
        }
    }
}