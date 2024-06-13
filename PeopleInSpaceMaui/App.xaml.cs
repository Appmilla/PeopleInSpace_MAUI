using PeopleInSpaceMaui.Alerts;
using PeopleInSpaceMaui.Extensions;
using PeopleInSpaceMaui.Network;

namespace PeopleInSpaceMaui;

public partial class App : Application
{
    private INetworkStatusObserver _networkStatusObserver;
    private IDisposable _networkStatusSubscription;
    private IUserAlerts _userAlerts;
    private IConnectivity _connectivity;

    private readonly TimeSpan _snackbarDuration = TimeSpan.FromSeconds(3);
    
    private NetworkAccess? _currentNetworkAccess;
    
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Resolve the network status observer and navigation service from the DI container
        _networkStatusObserver = serviceProvider.GetRequiredService<INetworkStatusObserver>();
        _userAlerts = serviceProvider.GetRequiredService<IUserAlerts>();
        _connectivity = serviceProvider.GetRequiredService<IConnectivity>();
        
        _networkStatusSubscription = _networkStatusObserver.Start();
        
        // Subscribe to network changes and broadcast them
        _networkStatusObserver.ConnectivityNotifications.Subscribe(networkAccess =>
        {
            if (_currentNetworkAccess == networkAccess) return;
            _currentNetworkAccess = networkAccess;
                
            _userAlerts.ShowSnackbar(networkAccess != NetworkAccess.Internet
                    ? "Internet access has been lost."
                    : "Internet access has been restored.", 
                _snackbarDuration).FireAndForgetSafeAsync();
        });
        
        MainPage = new AppShell();
    }

    private void CheckInitialNetworkStatus()
    {
        _currentNetworkAccess = _connectivity.NetworkAccess;
        if (_currentNetworkAccess != NetworkAccess.Internet)
        {
            _userAlerts.ShowSnackbar("No internet access.", _snackbarDuration)
                .FireAndForgetSafeAsync();
        }
    }
    
    protected override void OnStart()
    {
        // Handle when your app starts
        CheckInitialNetworkStatus();
    }

    protected override void OnSleep()
    {
        // Handle when your app sleeps
    }

    protected override void OnResume()
    {
        // Handle when your app resumes
    }

    protected override void CleanUp()
    {
        _networkStatusSubscription?.Dispose();
    }
}