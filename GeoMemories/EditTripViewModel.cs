using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace GeoMemories
{
    [QueryProperty(nameof(EditedTrip), "EditedTrip")]
    [QueryProperty(nameof(MapPins), "MapPins")]
    [QueryProperty(nameof(Pictures), "Pictures")]
    public partial class EditTripViewModel : ObservableObject
    {
        [ObservableProperty]
        Trip editedTrip;
        [ObservableProperty]
        Trip draft;

        public ObservableCollection<MapPin> MapPins { get; set; }
        public ObservableCollection<Picture> Pictures { get; set; }


        public ObservableCollection<MapPin> MapPinsDraft { get; set; }
        public ObservableCollection<Picture> PicturesDraft { get; set; }

        [ObservableProperty]
        Address address;

        private readonly string url = "https://nominatim.openstreetmap.org/";
        HttpClient client = new HttpClient();
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public void Init()
        {
            Draft = EditedTrip.GetCopy();
            foreach (var item in MapPins)
            {
                if (item.TripID == EditedTrip.ID)
                    MapPinsDraft.Add(item.GetCopy());
            }
            foreach (var item in Pictures)
            {
                if (item.TripID == EditedTrip.ID)
                    PicturesDraft.Add(item.GetCopy());
            }
        }
        private Mapsui.Map map;
        public Mapsui.Map Map
        {
            get => map;
            set => SetProperty(ref map, value);
        }
        public MemoryLayer PinLayer { get; set; } = new MemoryLayer { Name = "Pin Layer" };
       private bool CanSave() => Draft != null && !string.IsNullOrWhiteSpace(Draft.Name) && Draft.EndDate >= Draft.StartDate;
        [RelayCommand]
        public async Task Save()
        {
            if (CanSave())
            {
                var param = new ShellNavigationQueryParameters
                {
                    {"EditedTip", Draft},
                    {"addedpics", PicturesDraft},
                    {"addedpins", MapPinsDraft}
                };
                await Shell.Current.GoToAsync("..", param);
            }
            else
            {
                WeakReferenceMessenger.Default.Send("You cannot save a trip, that has no name or the end date is earlier than the start date");
            }
        }
        [RelayCommand]
        public async Task CancelEdit()
        {
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        public void DeletePin(MapPin pin)
        {
            MapPinsDraft.Remove(pin);
            MapRefesh();
        }
        public EditTripViewModel()
        {
            Map = new Mapsui.Map();
            //Adding a new OpenStreetMap layer to the map
            Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            //This creates a point on the map and centers it to it
            var center = SphericalMercator.FromLonLat(19.0402, 47.4979);
            Map.Home = n => n.CenterOnAndZoomTo(new MPoint(center.x, center.y), resolution: 2000, 500, Mapsui.Animations.Easing.CubicOut);
            //Adding a plus memorylayer to the map, where the pins will be
            Map.Layers.Add(PinLayer);
            MapPinsDraft = new ObservableCollection<MapPin>();
            PicturesDraft = new ObservableCollection<Picture>();
            Address = new Address();
            //Creating a user agent so that it can use the nominatim API
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MyMauiApp/1.0 (bvdev001@gmail.com)");
        }
        public void MapRefesh()
        {
            //A feature is a thing/drawing on a map, this is a list of those
            var newFeatures = new List<IFeature>();
            foreach (var item in MapPinsDraft)
            {
                //This converts the GPS standardized "coordinates" to a standard that a map can understand
                var coord = SphericalMercator.FromLonLat(item.Longitude, item.Latitude);
                //Pairs a Geometrical Feature(like a point in my case) to a coordinate 
                var geofeature = new Mapsui.Nts.GeometryFeature(new NetTopologySuite.Geometries.Point(coord.x, coord.y));
                //Puts a styling to the geometrical feature
                geofeature.Styles.Add(new Mapsui.Styles.SymbolStyle
                {
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                    SymbolType = Mapsui.Styles.SymbolType.Ellipse,
                    SymbolScale = 0.5
                });
                newFeatures.Add(geofeature);
            }
            //A full list change beacause you cannot add or delete to a MemoryLayer's feature list just completely change it
            PinLayer.Features = newFeatures;
            PinLayer.DataHasChanged();
        }

        [RelayCommand]
        public async Task SavePin()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                WeakReferenceMessenger.Default.Send("Please connect to the internet to use the app.");
                return;
            }
            if (Address != null && !string.IsNullOrWhiteSpace(Address.City) && !string.IsNullOrWhiteSpace(Address.Country))
            {
                //Converts the given address to coordinates
                HttpResponseMessage response = await client.GetAsync($"{url}search?q={Uri.EscapeDataString(Address.ToString())}&format=json&limit=1");

                if (response.IsSuccessStatusCode)
                {
                    var ctn = await response.Content.ReadAsStringAsync();
                    var res = JsonSerializer.Deserialize<List<NominatimResult>>(ctn, options);
                    if (res.Count > 0)
                    {
                        MapPinsDraft.Add(new MapPin
                        {
                            TripID = EditedTrip.ID,
                            Latitude = double.Parse(res[0].lattitude),
                            Longitude = double.Parse(res[0].longitude),
                            AddressString = Address.ToString()
                        });
                        Address = new Address();
                        MapRefesh();
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send("Invalid Address, please enter a valid one");
                    }

                }
                else
                {
                    WeakReferenceMessenger.Default.Send("Error converting the Address to the Coordinates: " + response.StatusCode);
                    Address = new Address();
                }
            }
        }
        [RelayCommand]
        public void DeletePic(Picture pic)
        {
            PicturesDraft.Remove(pic);
        }
        [RelayCommand]
        public async Task TakePhoto()
        {
            if (MediaPicker.IsCaptureSupported)
            {
                FileResult? pic = await MediaPicker.Default.CapturePhotoAsync();
                if (pic != null)
                {
                    await SavePhoto(pic);
                }
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Your device's camere is not supported");
            }
        }
        [RelayCommand]
        public async Task PickPhoto()
        {
            FileResult? pic = await MediaPicker.Default.PickPhotoAsync();
            if (pic != null)
            {
                await SavePhoto(pic);
            }
        }
        public async Task SavePhoto(FileResult file)
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, file.FileName);
                using Stream sf = await file.OpenReadAsync();
                using FileStream fs = File.Create(filePath);
                await sf.CopyToAsync(fs);
                PicturesDraft.Add(new Picture()
                {
                    TripID = EditedTrip.ID,
                    FilePath = filePath,
                    FileName = file.FileName
                });
            }
            catch (Exception e)
            {
                WeakReferenceMessenger.Default.Send("An error occured while saving your picture: " + e.Message);
            }
        }
    }
}
