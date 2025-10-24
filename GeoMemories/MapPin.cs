using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoMemories
{
    public partial class MapPin : ObservableObject
    {
        [ObservableProperty]
        [property: PrimaryKey]
        [property: AutoIncrement]
        int iD;
        [ObservableProperty]
        [property: ForeignKey(nameof(Trip.ID))]
        int tripID;
        [ObservableProperty]
        double latitude;
        [ObservableProperty]
        double longitude;
        [ObservableProperty]
        string title;
        [ObservableProperty]
        string address;

        public MapPin GetCopy()
        {
            return (MapPin)this.MemberwiseClone();
        }
    }
}