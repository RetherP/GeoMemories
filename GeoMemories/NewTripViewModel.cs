using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace GeoMemories
{
    [QueryProperty(nameof(NewTrip), "NewTrip")]
    [QueryProperty(nameof(newMapList), "MapPins")]
    [QueryProperty(nameof(newPictureList), "Pictures")]
    public partial class NewTripViewModel : ObservableObject
    {
        private readonly string url = "https://nominatim.openstreetmap.org/";
        [ObservableProperty]
        Trip newTrip;

        public ObservableCollection<MapPin> newMapList { get; set; }
        public ObservableCollection<Picture> newPictureList { get; set; }

        JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        HttpClient client;

        [ObservableProperty]
        Address address;
        public NewTripViewModel()
        {
            address = new Address();
            client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MyMauiApp/1.0 (vbelya07@gmail.com)");
        }
        [RelayCommand]
        public async Task PlacePin()
        {
            string urlsafeAdd = Uri.EscapeDataString(address.ToString());
            HttpResponseMessage response = await client.GetAsync($"{url}search?q={urlsafeAdd}&format=json&limit=1");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<List<NominatimResult>>(content, serializerOptions);
                if (res.Count != 0)
                {
                    newMapList.Add(new MapPin()
                    {
                        TripID = newTrip.ID,
                        Latitude = double.Parse(res[0].lattitude),
                        Longitude = double.Parse(res[0].longitude),
                        Title = Address.ToString()
                    });
                }
                else
                {
                    WeakReferenceMessenger.Default.Send("Address not found, please check the address you entered");
                }
            }
            else
            {
                WeakReferenceMessenger.Default.Send(response.StatusCode.ToString());
            }
            Address = new Address();
        }
        private bool CanSave()
        {
            return NewTrip != null && !string.IsNullOrWhiteSpace(NewTrip.Name) && NewTrip.EndDate >= NewTrip.StartDate;
        }
        [RelayCommand]
        public async Task SaveTrip()
        {
            if (CanSave())
            {
                var param = new ShellNavigationQueryParameters
                {
                    {"EditedTip", NewTrip},
                    {"addedpics", newPictureList},
                    {"addedpins",newMapList}
                };
                await Shell.Current.GoToAsync("..", param);
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Please fill in all required fields correctly");
            }
        }
        [RelayCommand]
        public async Task CancelNewTrip()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}