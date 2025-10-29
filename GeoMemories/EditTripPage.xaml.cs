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
        VM.Init();
        base.OnNavigatedTo(args);
        VM.MapRefesh();
        VM.MapPinsDraft.CollectionChanged += VM.MapPinsDraft_CollectionChanged;
        
    }
    protected override void OnDisappearing()
    {
        VM.MapPinsDraft.CollectionChanged -= VM.MapPinsDraft_CollectionChanged;
        base.OnDisappearing();
    }
}