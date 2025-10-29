using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace GeoMemories
{
    [QueryProperty(nameof(EditedTrip), "EditedTip")]
    [QueryProperty(nameof(addedPics), "addedpics")]
    [QueryProperty(nameof(addedPins), "addedpins")]
    public partial class MainPageViewModel : ObservableObject
    {
        private IMemoryDB db;

        public ObservableCollection<Trip> Trips { get; set; }
        public ObservableCollection<MapPin> MapPins { get; set; }
        public ObservableCollection<Picture> Pictures { get; set; }

        public ObservableCollection<MapPin> addedPins { get; set; } = new ObservableCollection<MapPin>();
        public ObservableCollection<Picture> addedPics { get; set; } = new ObservableCollection<Picture>();

        [ObservableProperty]
        Trip selectedTrip;

        [ObservableProperty]
        Trip editedTrip;
        async partial void OnEditedTripChanged(Trip value)
        {
            if (value != null)
            {
                if (SelectedTrip != null)
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
        [RelayCommand]
        public async Task DeleteTrip()
        {
            if (SelectedTrip != null)
            {
                //Azért kell a ToList mert azt nem járhatom be foreach-el amit menet közben módosítok
                foreach (var item in MapPins.ToList())
                {
                    if (item.TripID == SelectedTrip.ID)
                    {
                        MapPins.Remove(item);
                        await db.DeleteMapPinAsync(item.ID);
                    }
                }
                foreach (var item in Pictures.ToList())
                {
                    if (item.ID == SelectedTrip.ID)
                    {
                        Pictures.Remove(item);
                        await db.DeletePictureByIdAsync(item.ID);
                    }
                }
                await db.DeleteTripAsync(SelectedTrip.ID);
                Trips.Remove(SelectedTrip);
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Please select a trip to delete");
            }
        }
        [RelayCommand]
        public async Task EditTrip()
        {
            if (SelectedTrip != null)
            {
                var param = new ShellNavigationQueryParameters
                {
                    {"EditedTrip",SelectedTrip },
                    {"MapPins", MapPins},
                    {"Pictures",Pictures }
                };
                await Shell.Current.GoToAsync("edittrip", param);
                SelectedTrip = null;
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Please select a trip to edit");
            }
        }
        [RelayCommand]
        public async Task NewTrip()
        {
            int id = -1;
            var list = await db.GetAllTripAsync();
            if(list.Count != 0)
                id = list.LastOrDefault().ID;
            var param = new ShellNavigationQueryParameters
            {
                {"NewTrip",new Trip() {ID = id+1 } },
                {"MapPins",new ObservableCollection<MapPin>()},
                { "Pictures", new ObservableCollection<Picture>()},
            };
            await Shell.Current.GoToAsync("newtrip", param);
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
        [RelayCommand]
        public async Task DetailTrip()
        {
            throw new NotImplementedException();
        }
    }
}
