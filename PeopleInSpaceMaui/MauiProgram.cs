using Microsoft.Extensions.Logging;
using PeopleInSpaceMaui.Apis;
using PeopleInSpaceMaui.Queries;
using PeopleInSpaceMaui.Reactive;
using PeopleInSpaceMaui.ViewModels;
using Refit;

namespace PeopleInSpaceMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddScoped<MainPageViewModel>();
        builder.Services.AddSingleton<IPeopleInSpaceQuery, PeopleInSpaceQuery>();
        builder.Services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
        builder.Services.AddRefitClient<ISpaceXApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.spacexdata.com/v4"));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}