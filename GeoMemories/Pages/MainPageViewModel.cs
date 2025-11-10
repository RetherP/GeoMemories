using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GeoMemories.Repositories;
using System.Collections.ObjectModel;

namespace GeoMemories
{
    [QueryProperty(nameof(EditedTrip), "EditedTip")]
    [QueryProperty(nameof(addedPics), "addedpics")]
    [QueryProperty(nameof(addedPins), "addedpins")]
    public partial class MainPageViewModel : ObservableObject
    {
        //private IMemoryDB db;
        private ITripRepository tripRepository;
        private IPictureRepository pictureRepository;
        private IMapRepository mapRepository;

        public ObservableCollection<Trip> Trips { get; set; }
        public ObservableCollection<MapPin> MapPins { get; set; }
        public ObservableCollection<Picture> Pictures { get; set; }

        public ObservableCollection<MapPin> addedPins { get; set; } = new ObservableCollection<MapPin>();
        public ObservableCollection<Picture> addedPics { get; set; } = new ObservableCollection<Picture>();

        [ObservableProperty]
        Trip selectedTrip;

        [ObservableProperty]
        Trip editedTrip;

        [ObservableProperty]
        string search;

        async partial void OnEditedTripChanged(Trip value)
        {
            if (value == null) return;
            if (SelectedTrip != null)
            {
                Trips.Remove(SelectedTrip);
                var MapRemove = MapPins.Where(x => x.TripID == value.ID);
                foreach (var item in MapRemove.ToList())
                {
                    MapPins.Remove(item);
                    await mapRepository.DeleteMapPinAsync(item.ID);
                }
                var PictureToRemove = Pictures.Where(x => x.TripID == value.ID);
                foreach (var item in PictureToRemove.ToList())
                {
                    Pictures.Remove(item);
                    await pictureRepository.DeletePictureByIdAsync(item.ID);
                }
                await tripRepository.UpdateTripAsync(value);
                SelectedTrip = null;
            }
            else
            {
                await tripRepository.CreateTripAsync(value);
            }
            Trips.Add(value);
            foreach (var item in addedPins)
            {
                MapPins.Add(item);
                await mapRepository.CreateMapPinAsync(item);
            }
            foreach (var item in addedPics)
            {
                Pictures.Add(item);
                await pictureRepository.CreatePictureAsync(item);
            }
            EditedTrip = null;
            await InitAsync();
            WeakReferenceMessenger.Default.Send("The Save was succesful");
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
                        await mapRepository.DeleteMapPinAsync(item.ID);
                    }
                }
                foreach (var item in Pictures.ToList())
                {
                    if (item.ID == SelectedTrip.ID)
                    {
                        Pictures.Remove(item);
                        await pictureRepository.DeletePictureByIdAsync(item.ID);
                    }
                }
                await tripRepository.DeleteTripAsync(SelectedTrip.ID);
                Trips.Remove(SelectedTrip);
                SelectedTrip = null;
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
                    {"EditedTrip",SelectedTrip},
                    {"MapPins", MapPins},
                    {"Pictures",Pictures }
                };
                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    WeakReferenceMessenger.Default.Send("Please connect to the internet to use the app.");
                    return;
                }
                await Shell.Current.GoToAsync("edittrip", param);
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Please select a trip to edit");
            }
        }
        [RelayCommand]
        public async Task NewTrip()
        {
            SelectedTrip = null;
            int id = -1;
            var list = await tripRepository.GetAllTripAsync();
            if (list.Count != 0)
                id = list.Max(x=> x.ID);
            var param = new ShellNavigationQueryParameters
            {
                {"NewTrip",new Trip() {ID = id+1, StartDate = DateTime.Now, EndDate = DateTime.Now } },
                {"MapPins",new ObservableCollection<MapPin>()},
                { "Pictures", new ObservableCollection<Picture>()},
            };
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                WeakReferenceMessenger.Default.Send("Please connect to the internet to use the app.");
                return;
            }
            await Shell.Current.GoToAsync("newtrip", param);
        }
        public MainPageViewModel(ITripRepository tripRepository, IPictureRepository pictureRepository, IMapRepository mapRepository)
        {

            Trips = new ObservableCollection<Trip>();
            MapPins = new ObservableCollection<MapPin>();
            Pictures = new ObservableCollection<Picture>();
            this.tripRepository = tripRepository;
            this.pictureRepository = pictureRepository;
            this.mapRepository = mapRepository;
        }
        public async Task InitAsync()
        {
            var tripList = await tripRepository.GetAllTripAsync();
            var mapList = await mapRepository.GetAllMapPinsAsync();
            var picList = await pictureRepository.GetAllPicturesAsync();
            Trips.Clear();
            tripList.ForEach(x => Trips.Add(x));
            MapPins.Clear();
            mapList.ForEach(x => MapPins.Add(x));
            Pictures.Clear();
            picList.ForEach(x => Pictures.Add(x));
        }
        [RelayCommand]
        public async Task SearchTrip()
        {
            Search = Search.Trim();
            if(string.IsNullOrEmpty(Search) || string.IsNullOrWhiteSpace(Search))
            {
                DeleteSearch();
            }
            else
            {
                Trips.Clear();
                foreach (var item in await tripRepository.GetAllTripAsync())
                {
                    if (item.Name.ToLower().Contains(Search.ToLower()))
                        Trips.Add(item);
                }
            }
            Search = "";
        }

        [RelayCommand]
        public async Task DeleteSearch()
        {
            Trips.Clear();
            foreach (var item in await tripRepository.GetAllTripAsync())
            {
                Trips.Add(item);
            }
        }
        [RelayCommand]
        public async Task MapOverView()
        {
            var param = new ShellNavigationQueryParameters()
            {
                {"Trips", Trips },
                {"Pins", MapPins}
            };
            await Shell.Current.GoToAsync("mapoverview", param);
        }
    }
}
