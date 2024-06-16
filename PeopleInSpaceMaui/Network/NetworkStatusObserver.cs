using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PeopleInSpaceMaui.Network;

public interface INetworkStatusObserver: IDisposable
{
    void Start();
    IObservable<NetworkAccess> ConnectivityNotifications { get; }
}

public class NetworkStatusObserver(IConnectivity connectivity) : INetworkStatusObserver
{
    private readonly Subject<NetworkAccess> _connectivityNotifications = new();
    private IDisposable? _subscription;

    public IObservable<NetworkAccess> ConnectivityNotifications => _connectivityNotifications;

    public void Start()
    {
        _subscription = Observable.FromEventPattern<EventHandler<ConnectivityChangedEventArgs>, ConnectivityChangedEventArgs>(
                handler => connectivity.ConnectivityChanged += handler,
                handler => connectivity.ConnectivityChanged -= handler)
            .Select(eventPattern => eventPattern.EventArgs.NetworkAccess)
            .Subscribe(_connectivityNotifications);
    }
    
    public void Dispose()
    {
        _subscription?.Dispose();
        _connectivityNotifications.Dispose();
    }
}