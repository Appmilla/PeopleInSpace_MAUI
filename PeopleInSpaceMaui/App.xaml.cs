using PeopleInSpaceMaui.Alerts;
using PeopleInSpaceMaui.Extensions;
using PeopleInSpaceMaui.Network;

namespace PeopleInSpaceMaui;

public partial class App : Application
{
    private readonly INetworkStatusObserver _networkStatusObserver;
    private readonly IUserAlerts _userAlerts;
    private readonly IConnectivity _connectivity;

    private readonly TimeSpan _snackbarDuration = TimeSpan.FromSeconds(3);
    
    private NetworkAccess? _currentNetworkAccess;
    
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        
        _networkStatusObserver = serviceProvider.GetRequiredService<INetworkStatusObserver>();
        _userAlerts = serviceProvider.GetRequiredService<IUserAlerts>();
        _connectivity = serviceProvider.GetRequiredService<IConnectivity>();
        
        _networkStatusObserver.Start();
        
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
        CheckInitialNetworkStatus();
    }

    protected override void CleanUp()
    {
        _networkStatusObserver.Dispose();
    }
}