using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.UI.Maui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories.Pages
{
    [QueryProperty(nameof(Trips),"Trips")]
    [QueryProperty(nameof(Pins),"Pins")]
    public partial class MapOverviewViewModel : ObservableObject
    {
        public ObservableCollection<MapPin> Pins { get; set; }
        public ObservableCollection<Trip> Trips { get; set; }
        public ObservableCollection<MapPin> DraftPins {  get; set; }
        public MemoryLayer PinLayer { get; } = new MemoryLayer { Name = "Pin Layer" };
        private Mapsui.Map map;
        public Mapsui.Map Map
        {
            get => map;
            set
            {
                map = value;
                OnPropertyChanged();
            }
        }
        [ObservableProperty]
        DateTime start = DateTime.Now;
        [ObservableProperty]
        DateTime end = DateTime.Now;
        public MapOverviewViewModel()
        {
            Map = new Mapsui.Map();
            Pins = new ObservableCollection<MapPin>();
            DraftPins = new ObservableCollection<MapPin>();
            Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            Map.Layers.Add(PinLayer);
        }

        private void CenterMap()
        {
            if (DraftPins != null && DraftPins.Count != 0)
            {
                double logn= DraftPins.Average(x => x.Longitude);
                double latt = DraftPins.Average(x => x.Latitude);
                var center = SphericalMercator.FromLonLat(logn, latt);
                Map.Navigator.CenterOnAndZoomTo(new MPoint(center.x, center.y), resolution: 2000, 500, Mapsui.Animations.Easing.CubicOut);
            }
        }
        public void MapRefresh()
        {
            //A feature is a thing a drawing on a map, this is a list of those 
            var newFeatures = new List<IFeature>();
            foreach (var pinItem in DraftPins)
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
            CenterMap();
        }
        public void Init()
        {
            DraftPins.Clear();
            foreach(var pinItem in Pins)
            {
                DraftPins.Add(pinItem);
            }
        }
        [RelayCommand]
        public async Task MainPage()
        {
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        public async Task Search()
        {
            DraftPins.Clear();
            foreach(var pinItem in Pins)
            {
                var tmp = Trips.Where(x => x.ID == pinItem.TripID).FirstOrDefault();
                if (DateTime.Compare(tmp.StartDate, Start)>=0 && DateTime.Compare(tmp.EndDate,End)<=0)
                {
                    DraftPins.Add(pinItem);
                }
            }
            MapRefresh();
            CenterMap();
        }
        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        public async Task DeleteSearch()
        {
            DraftPins.Clear();
            foreach (var item in Pins)
            {
                DraftPins.Add(item);
            }
            MapRefresh();
            CenterMap();
        }
    }
}
