
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;

namespace PeopleInSpaceMaui.Network;

public interface INetworkStatusObserver
{
    IDisposable Start();

    IObservable<NetworkAccess> ConnectivityNotifications { get; }
}



public class NetworkStatusObserver(IConnectivity connectivity) : INetworkStatusObserver, IDisposable
{
    readonly Subject<NetworkAccess> _connectivityNotifications = new();
        
    public IObservable<NetworkAccess> ConnectivityNotifications => _connectivityNotifications;

    public IDisposable Start()
    {
        return Observable.FromEventPattern<EventHandler<ConnectivityChangedEventArgs>, ConnectivityChangedEventArgs>(
                handler => connectivity.ConnectivityChanged += handler,
                handler => connectivity.ConnectivityChanged -= handler)
            .Select(eventPattern => eventPattern.EventArgs.NetworkAccess)
            .Subscribe(_connectivityNotifications);
    }
    
    public void Dispose()
    {
        _connectivityNotifications.Dispose();
    }
    
}