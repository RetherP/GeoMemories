using CommunityToolkit.Mvvm.Messaging;
using Mapsui;
using Mapsui.Projections;

namespace GeoMemories;

public partial class NewTripPage : ContentPage
{
    NewTripViewModel vm;
    public NewTripPage(NewTripViewModel vm)
    {
        InitializeComponent();
        this.vm = vm;
        BindingContext = this.vm;
        var layer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer();
        myMap.Map.Layers.Add(layer);
        var center = SphericalMercator.FromLonLat(19.0402, 47.4979);
        myMap.Map.Home = n => n.CenterOnAndZoomTo(new MPoint(center.x,center.y), resolution: 2000, 500, Mapsui.Animations.Easing.CubicOut);
    }
}