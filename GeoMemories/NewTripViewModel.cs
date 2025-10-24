using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace GeoMemories
{
    [QueryProperty(nameof(newTrip),"NewTrip")]
    [QueryProperty(nameof(newMapList),"MapPins")]
    [QueryProperty(nameof(newPictureList),"Pictures")]
    public partial class NewTripViewModel:ObservableObject
    {
        [ObservableProperty]
        Trip newTrip;
        
        ObservableCollection<MapPin> newMapList;
        ObservableCollection<Picture> newPictureList;

        [RelayCommand]
        public async Task SaveTrip()
        {

            var param = new ShellNavigationQueryParameters
            {
                {"EditedTip", newTrip},
                {"addedpics", newPictureList},
                {"addedpins",newMapList}
            };
            await Shell.Current.GoToAsync("..", param);
        }
        [RelayCommand]
        public async Task CancelNewTrip()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}