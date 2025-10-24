using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoMemories
{
    [QueryProperty(nameof(EditedTrip),"EditedTrip")]
    [QueryProperty(nameof(MapPins), "MapPins")]
    [QueryProperty(nameof(Pictures), "Pictures")]
    public partial class EditTripViewModel : ObservableObject
    {
        [ObservableProperty]
        Trip editedTrip;
        [ObservableProperty]
        Trip draft;

        ObservableCollection<MapPin> MapPins;
        ObservableCollection<Picture> Pictures;

        ObservableCollection<MapPin> MapPinsDraft;
        ObservableCollection<Picture> PicturesDraft;

        public void Init()
        {
            Draft = editedTrip.GetCopy();
            MapPinsDraft = new ObservableCollection<MapPin>(MapPins.Where(x => x.ID == EditedTrip.ID).Select(x => x.GetCopy()));
            PicturesDraft =  new ObservableCollection<Picture>(PicturesDraft.Where(x => x.ID == EditedTrip.ID).Select(x => x.GetCopy()));
        }

        [RelayCommand]
        public async Task Save()
        {
            var param = new ShellNavigationQueryParameters
            {
                {"EditedTip", Draft},
                {"addedpics", PicturesDraft},
                {"addedpins",MapPinsDraft}
            };
            await Shell.Current.GoToAsync("..", param);
        }
        [RelayCommand]
        public async Task CancelEdit()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
