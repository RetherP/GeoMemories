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
}