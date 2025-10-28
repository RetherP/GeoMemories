using CommunityToolkit.Mvvm.Messaging;

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
            WeakReferenceMessenger.Default.Register<MainPage, string>(this, (r, m) =>
            {
                r.DisplayAlert("Alert", m, "OK");
            });
        }
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            await viewModel.InitAsync();
            base.OnNavigatedTo(args);
        }
    }
}
