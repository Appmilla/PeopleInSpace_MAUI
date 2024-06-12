using System.Reactive;
using Akavache;
using Microsoft.Extensions.Logging;
using PeopleInSpaceMaui.Apis;
using PeopleInSpaceMaui.Reactive;
using PeopleInSpaceMaui.Repositories;
using PeopleInSpaceMaui.ViewModels;
using PeopleInSpaceMaui.Views;
using ReactiveUI;
using Refit;

namespace PeopleInSpaceMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Akavache.Registrations.Start("PeopleInSpace");

        RxApp.DefaultExceptionHandler = new AnonymousObserver<Exception>(ex =>
        {
            App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        });
        
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        builder.Services.AddSingleton<IBlobCache>(BlobCache.LocalMachine);
        
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddScoped<MainPageViewModel>();
        builder.Services.AddTransient<DetailPage>();
        builder.Services.AddScoped<DetailPageViewModel>();
        builder.Services.AddSingleton<ICrewRepository, CrewRepository>();
        builder.Services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
        builder.Services.AddRefitClient<ISpaceXApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.spacexdata.com/v4"));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}