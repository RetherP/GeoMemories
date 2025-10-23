using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    [QueryProperty(nameof(EditedTrip),"EditedTrip")]
    [QueryProperty(nameof(addedPics),"addedpics")]
    [QueryProperty(nameof(addedPins),"addedpins")]
    public partial class MainPageViewModel:ObservableObject
    {
        private IMemoryDB db;

        public ObservableCollection<Trip> Trips { get; set; }
        public ObservableCollection<MapPin> MapPins { get; set; }
        public ObservableCollection<Picture> Pictures { get; set; }

        private List<MapPin> addedPins = new List<MapPin>();
        private List<Picture> addedPics = new List<Picture>();

        [ObservableProperty]
        private Trip selectedTrip;

        [ObservableProperty]
        private Trip editedTrip;
        async partial void OnEditedTripChanged(Trip value)
        {
            if(value != null && addedPins.Count != 0 && addedPics.Count != 0)
            {
                if(SelectedTrip != null)
                {
                    Trips.Remove(SelectedTrip);
                    var MapRemove = MapPins.Where(x => x.TripID == value.ID);
                    foreach (var item in MapRemove)
                    {
                        MapPins.Remove(item);
                        await db.DeleteMapPinAsync(item.ID);
                    }
                    var PictureToRemove = Pictures.Where(x => x.TripID == value.ID);
                    foreach (var item in PictureToRemove)
                    {
                        Pictures.Remove(item);
                        await db.DeletePictureByIdAsync(item.ID);
                    }
                    await db.UpdateTripAsync(value);
                    SelectedTrip = null;
                }
                else
                {
                    await db.CreateTripAsync(value);

                }
                Trips.Add(value);
                foreach (var item in addedPins)
                {
                    MapPins.Add(item);
                    await db.CreateMapPinAsync(item);
                }
                foreach (var item in addedPics)
                {
                    Pictures.Add(item);
                    await db.CreatePictureAsync(item);
                }
            }
        }
        public MainPageViewModel(IMemoryDB db)
        {
            this.db = db;
            Trips = new ObservableCollection<Trip>();
            MapPins = new ObservableCollection<MapPin>();
            Pictures = new ObservableCollection<Picture>();
        }
        public async Task InitAsync()
        {
            var tripList = await db.GetAllTripAsync();
            var mapList = await db.GetAllMapPinsAsync();
            var picList = await db.GetAllPicturesAsync();
            Trips.Clear();
            tripList.ForEach(x => Trips.Add(x));
            MapPins.Clear();
            mapList.ForEach(x => MapPins.Add(x));
            Pictures.Clear();
            picList.ForEach(x => Pictures.Add(x));
        }
    }
}
