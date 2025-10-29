using CommunityToolkit.Mvvm.Messaging;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.UI.Maui;

namespace GeoMemories;

public partial class NewTripPage : ContentPage
{
    NewTripViewModel vm;
    public NewTripPage(NewTripViewModel vm)
    {
        InitializeComponent();
        this.vm = vm;
        BindingContext = this.vm;
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        vm.NewMapList.CollectionChanged += vm.newMapList_CollectionChanged;
        base.OnNavigatedTo(args);
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        vm.NewMapList.CollectionChanged -= vm.newMapList_CollectionChanged;
        base.OnNavigatedFrom(args);
    }
}