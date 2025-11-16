using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;

namespace GeoMemories
{
    [QueryProperty(nameof(NewTrip), "NewTrip")]
    [QueryProperty(nameof(NewMapList), "MapPins")]
    [QueryProperty(nameof(NewPictureList), "Pictures")]
    public partial class NewTripViewModel : ObservableObject
    {
        private readonly string url = "https://nominatim.openstreetmap.org/";
        [ObservableProperty]
        Trip newTrip;

        public ObservableCollection<MapPin> NewMapList { get; set; }
        public ObservableCollection<Picture> NewPictureList { get; set; }

        JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        HttpClient client;

        [ObservableProperty]
        Address address;

        private Mapsui.Map map;
        public Mapsui.Map Map
        {
            get => map;
             //It is the same if i would use map = value and then call OnPropertyChanged
            set => SetProperty(ref map, value);
        }
        //A new layer on the map
        public MemoryLayer PinLayer { get; } = new MemoryLayer { Name = "Pin Layer" };
        public void newMapList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //A feature is a thing a drawing on a map, this is a list of those 
            var newFeatures = new List<IFeature>();
            foreach (var pinItem in NewMapList)
            {
                //This converts the GPS standardized "coordinates" to a standard that a map can understand
                var coord = SphericalMercator.FromLonLat(pinItem.Longitude, pinItem.Latitude);
                //Places a Geometrical Feature(like a point in my case) to a coordinate 
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
            //A full list change beacause you cannot add or delete to a MemoryLayer's Feature list
            //just completely change it
            PinLayer.Features = newFeatures;
            PinLayer.DataHasChanged();
            //Center the map according to the new Pins
            CenterMap();
        }
        public NewTripViewModel()
        {
            address = new Address();
            client = new HttpClient();
            //You must have a user-agent to call the nominatim API
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MyMauiApp/1.0 (bvdev001@gmail.com)"); 
            //These lines creates the map and adds a new OpenStreetMap layer  
            Map = new Mapsui.Map();
            Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            Map.Layers.Add(PinLayer);
            NewPictureList = new ObservableCollection<Picture>();
        }
        private void CenterMap()
        {
            //This calculated the current avg of the coordinates and centers the map there
            var center = SphericalMercator.FromLonLat(19.0402, 47.4979);
            if (NewMapList != null && NewMapList.Count != 0)
            {
                double logn = NewMapList.Average(x => x.Longitude);
                double latt = NewMapList.Average(x => x.Latitude);
                center = SphericalMercator.FromLonLat(logn, latt);
            }
            Map.Navigator.CenterOnAndZoomTo(new MPoint(center.x, center.y), resolution: 2000, 500, Mapsui.Animations.Easing.CubicOut);
        }
        [RelayCommand]
        public async Task PlacePin()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                WeakReferenceMessenger.Default.Send("Please connect to the internet to use the app.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(Address.City) && !string.IsNullOrWhiteSpace(Address.Country))
            {
                //These lines translate the address to a format that can be inserted into the url to querry then querries them to get the coordinates
                string urlsafeAdd = Uri.EscapeDataString(address.ToString());
                HttpResponseMessage response = await client.GetAsync($"{url}search?q={urlsafeAdd}&format=json&limit=1");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var res = JsonSerializer.Deserialize<List<NominatimResult>>(content, serializerOptions);
                    if (res.Count != 0)
                    {
                        NewMapList.Add(new MapPin()
                        {
                            TripID = newTrip.ID,
                            Latitude = double.Parse(res[0].lattitude),
                            Longitude = double.Parse(res[0].longitude),
                            AddressString = Address.ToString()
                        });
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send("Address not found, please check the address you entered");
                    }
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(response.StatusCode.ToString());
                }
            }
            Address = new Address();
        }
        private bool CanSave() => NewTrip != null && !string.IsNullOrWhiteSpace(NewTrip.Name) && NewTrip.EndDate >= NewTrip.StartDate;
        [RelayCommand]
        public async Task SaveTrip()
        {
            if (CanSave())
            {
                var param = new ShellNavigationQueryParameters
                {
                    {"EditedTip", NewTrip},
                    {"addedpics", NewPictureList},
                    {"addedpins",NewMapList}
                };
                await Shell.Current.GoToAsync("..", param);
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Please fill in all required fields correctly. You cannot save a trip withour a name or an ealier end date than the start date");
            }
        }
        [RelayCommand]
        public async Task CancelNewTrip()
        {
            foreach (var item in NewPictureList)
            {
                File.Delete(item.FilePath);
            }
            await Shell.Current.GoToAsync("..");
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
                NewPictureList.Add(new Picture()
                {
                    TripID = NewTrip.ID,
                    FilePath = filePath,
                    FileName = file.FileName
                });
                OnPropertyChanged(nameof(NewPictureList));
            }
            catch (Exception e)
            {
                WeakReferenceMessenger.Default.Send("An error occured while saving your picture: " + e.Message);
            }
        }
    }
}