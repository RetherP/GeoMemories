using GeoMemories.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using SkiaSharp.Views.Maui.Controls.Hosting;
using System.Globalization;


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
            builder.Services.AddTransient<NewTripViewModel>();
            builder.Services.AddTransient<NewTripPage>();
            builder.Services.AddTransient<EditTripViewModel>();
            builder.Services.AddTransient<EditTripPage>();
            builder.Services.AddTransient<MapOverviewViewModel>();
            builder.Services.AddTransient<MapOverview>();
            builder.Services.AddSingleton<IMemoryDB, MemoryDB>();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
