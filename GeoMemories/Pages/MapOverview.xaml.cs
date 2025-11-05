namespace GeoMemories.Pages;

public partial class MapOverview : ContentPage
{
    MapOverviewViewModel viewModel;
	public MapOverview(MapOverviewViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        viewModel.Init();
        viewModel.MapRefresh();
    }
}