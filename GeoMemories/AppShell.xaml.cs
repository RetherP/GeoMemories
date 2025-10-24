namespace GeoMemories
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("edittrip", typeof(EditTripPage));
            Routing.RegisterRoute("newtrip", typeof(NewTripPage));
        }
    }
}
