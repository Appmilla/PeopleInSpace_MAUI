using CommunityToolkit.Maui.Core;

namespace PeopleInSpaceMaui.Navigation;

using CommunityToolkit.Maui.Alerts;

public interface INavigationService
{
    Task NavigateAsync(string route);

    Task DisplayToast(string message);
}

public class NavigationService : INavigationService
{
    public async Task NavigateAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task DisplayToast(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);

        await toast.Show(cancellationTokenSource.Token);
    }
}