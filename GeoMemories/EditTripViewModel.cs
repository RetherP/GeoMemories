using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using SkiaSharp.Views.Maui.Controls.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    [QueryProperty(nameof(EditedTrip),"EditedTrip")]
    [QueryProperty(nameof(MapPins), "MapPins")]
    [QueryProperty(nameof(Pictures), "Pictures")]
    public partial class EditTripViewModel : ObservableObject
    {
        //TODO:
        /*
         * Finish the UI, and all the necessary save commands
         * Add pin delete methodology --> DeletePin func finish
         * Add image handling
         */
        [ObservableProperty]
        Trip editedTrip;
        [ObservableProperty]
        Trip draft;

        public ObservableCollection<MapPin> MapPins { get; set; }
        public ObservableCollection<Picture> Pictures { get; set; }

        
        public ObservableCollection<MapPin> MapPinsDraft { get; set; }
        public ObservableCollection<Picture> PicturesDraft { get; set; }
        
        
        public void Init()
        {
            Draft = editedTrip.GetCopy();
            MapPinsDraft = new ObservableCollection<MapPin>();
            foreach (var item in MapPins)
            {
                if (item.TripID == EditedTrip.ID)
                    MapPinsDraft.Add(item.GetCopy());
            }
            PicturesDraft =  new ObservableCollection<Picture>();
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
        [RelayCommand]
        public async Task Save()
        {
            var param = new ShellNavigationQueryParameters
            {
                {"EditedTip", Draft},
                {"addedpics", PicturesDraft},
                {"addedpins", MapPinsDraft}
            };
            await Shell.Current.GoToAsync("..", param);
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
        }
        public EditTripViewModel()
        {
            Map = new Mapsui.Map();
            Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            var center = SphericalMercator.FromLonLat(19.0402, 47.4979);
            Map.Home = n => n.CenterOnAndZoomTo(new MPoint(center.x, center.y), resolution: 2000, 500, Mapsui.Animations.Easing.CubicOut);
            Map.Layers.Add(PinLayer);
        }
        public void MapRefesh()
        {
            var newFeatures = new List<IFeature>();
            foreach (var item in MapPinsDraft)
            {
                var coord = SphericalMercator.FromLonLat(item.Longitude, item.Latitude);
                var geofeature = new Mapsui.Nts.GeometryFeature(new NetTopologySuite.Geometries.Point(coord.x, coord.y));
                geofeature.Styles.Add(new Mapsui.Styles.SymbolStyle
                {
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                    SymbolType = Mapsui.Styles.SymbolType.Ellipse,
                    SymbolScale = 0.5
                });
                newFeatures.Add(geofeature);
            }
            PinLayer.Features = newFeatures;
            PinLayer.DataHasChanged();
        }
        public void MapPinsDraft_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MapRefesh();
        }
    }
}
