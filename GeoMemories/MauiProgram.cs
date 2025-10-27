using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using SkiaSharp.Views.Maui.Controls.Hosting;


namespace GeoMemories
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp(true)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<EditTripViewModel>();
            builder.Services.AddTransient<EditTripPage>();
            builder.Services.AddTransient<NewTripViewModel>();
            builder.Services.AddTransient<NewTripPage>();
            builder.Services.AddSingleton<IMemoryDB, MemoryDB>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
