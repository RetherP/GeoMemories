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

        private Mapsui.Map _map;
        public Mapsui.Map Map
        {
            get => _map;
            //Ez teljesen ugyan olyan mintha _map = value és utána OnPropertyChanged lenne
            //csak egybefogja, kevesebb a hibaesély.
            set => SetProperty(ref _map, value);
        }
        //Egy új réteg a térképen
        public MemoryLayer PinLayer { get; } = new MemoryLayer { Name = "Pin Layer" };
        public void newMapList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //A Feature egy térképen lévő elemek listája
            var newFeatures = new List<IFeature>();
            foreach (var pinItem in NewMapList)
            {
                //Ez azért kell hogy a GPS-es szabványból a térkép által értelmezett szabványt készítsünk
                var coord = SphericalMercator.FromLonLat(pinItem.Longitude, pinItem.Latitude);
                //Adott x,y helyre rak egy speciális geometriai alakot.
                var geofeature = new Mapsui.Nts.GeometryFeature(new NetTopologySuite.Geometries.Point(coord.x, coord.y));
                //a fentebb lévő alakzatak ad egy stílust
                geofeature.Styles.Add(new Mapsui.Styles.SymbolStyle
                {
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                    SymbolType = Mapsui.Styles.SymbolType.Ellipse,
                    SymbolScale = 0.5
                });
                newFeatures.Add(geofeature);
            }
            //Teljes listacsere mivel a memorylayer nem engedi azt hogy töröljünk vagy hozzáadjunk elemeket.
            PinLayer.Features = newFeatures;
            PinLayer.DataHasChanged();
        }
        public NewTripViewModel()
        {
            address = new Address();
            client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MyMauiApp/1.0 (vbelya07@gmail.com)");
            Map = new Mapsui.Map();
            Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            var center = SphericalMercator.FromLonLat(19.0402, 47.4979);
            Map.Home = n => n.CenterOnAndZoomTo(new MPoint(center.x, center.y), resolution: 2000, 500, Mapsui.Animations.Easing.CubicOut);
            Map.Layers.Add(PinLayer);
            NewPictureList = new ObservableCollection<Picture>();
        }
        [RelayCommand]
        public async Task PlacePin()
        {
            if (!string.IsNullOrWhiteSpace(Address.City) && !string.IsNullOrWhiteSpace(Address.City))
            {
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
        private bool CanSave()
        {
            return NewTrip != null && !string.IsNullOrWhiteSpace(NewTrip.Name) && NewTrip.EndDate >= NewTrip.StartDate;
        }
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
                WeakReferenceMessenger.Default.Send("Please fill in all required fields correctly");
            }
        }
        [RelayCommand]
        public async Task CancelNewTrip()
        {
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
                //A file lokációja, egyedi névvel.
                string filePath = Path.Combine(FileSystem.AppDataDirectory, file.FileName);
                //Nyit egy streamet és kapcsolatot a nyers adattal
                using Stream sf = await file.OpenReadAsync();
                //Elkészíti az új filet
                using FileStream fs = File.Create(filePath);
                //A forrásból a célbe belemásolja az adatokat.
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