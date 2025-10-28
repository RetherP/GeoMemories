using CommunityToolkit.Mvvm.Messaging;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.UI.Maui;

namespace GeoMemories;

public partial class NewTripPage : ContentPage
{
    NewTripViewModel vm;
    MemoryLayer pins;
    public NewTripPage(NewTripViewModel vm)
    {
        InitializeComponent();
        this.vm = vm;
        BindingContext = this.vm;
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        vm.newMapList.CollectionChanged += vm.OnPinCollectionChanged;
    }
}