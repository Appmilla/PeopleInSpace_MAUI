using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace PeopleInSpaceMaui.Alerts;

public interface IUserAlerts    
{
    Task ShowToast(string message);
    
    Task ShowSnackbar(string message, TimeSpan duration);
}

public class UserAlerts : IUserAlerts
{
    public async Task ShowToast(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        
        var duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);

        await toast.Show(cancellationTokenSource.Token);
    }

    public async Task ShowSnackbar(string message, TimeSpan duration)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#FFDE1920"), // FF for full opacity, followed by RGB
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.White,
            CornerRadius = new CornerRadius(4),
            Font = Font.SystemFontOfSize(14),
            ActionButtonFont = Font.SystemFontOfSize(14)
        };
        
        var snackbar = Snackbar.Make(message, duration: duration, visualOptions:snackbarOptions);

        await snackbar.Show(cancellationTokenSource.Token);
    }
}