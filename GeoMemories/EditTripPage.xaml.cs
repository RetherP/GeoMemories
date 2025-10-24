namespace GeoMemories;

public partial class EditTripPage : ContentPage
{
    EditTripViewModel VM;
	public EditTripPage(EditTripViewModel vm)
	{
		InitializeComponent();
        VM = vm;
        BindingContext = VM;
	}
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        VM.Init();
    }
}