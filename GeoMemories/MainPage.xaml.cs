namespace GeoMemories
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel viewModel;
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = this.viewModel;
        }
        private async void MainPage_OnLoaded(object? sender, EventArgs e)
        {
            await viewModel.InitAsync();
        }
    }
}
