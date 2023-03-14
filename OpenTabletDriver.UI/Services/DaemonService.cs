using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.Logging;

namespace OpenTabletDriver.UI.Services;

public partial class DaemonService : ObservableObject
{
    private Func<IDriverDaemon> _daemonFactory;
    private IDriverDaemon? _instance;
    private TaskCompletionSource _completionSource = new();

    [ObservableProperty]
    private bool _isConnected;

    public IDriverDaemon Instance => _instance ?? throw new InvalidOperationException("Daemon is not connected");

    private DaemonService(Func<IDriverDaemon> daemonFactory, bool isAlreadyConnected = false)
    {
        _daemonFactory = daemonFactory;
        if (isAlreadyConnected)
            _completionSource.SetResult();
    }

    /// <summary>
    /// Instantiates a new <see cref="DaemonService"/> from an RPC client.
    /// </summary>
    /// <param name="rpcClient">The RPC client to use.</param>
    /// <returns>An instance of <see cref="DaemonService"/></returns>
    public static DaemonService FromRpc(RpcClient<IDriverDaemon> rpcClient)
    {
        var daemonService = new DaemonService(() => rpcClient.Instance!);
        rpcClient.Connected += (_, _) => daemonService.OnConnected();
        rpcClient.Disconnected += (_, _) => daemonService.OnDisconnected();
        return daemonService;
    }

    /// <summary>
    /// Instantiates a new <see cref="DaemonService"/> from a factory.
    /// </summary>
    /// <param name="daemonFactory">The factory that provides an instance of <see cref="IDriverDaemon"/>.</param>
    /// <param name="onConnectedDelegate">When this method returns, contains a delegate to use to tell this service
    /// that the factory is ready to return an instance.</param>
    /// <param name="onDisconnectedDelegate">When this method returns, contains a delegate to use to tell this service
    /// that the instance returned by factory is now invalid.</param>
    /// <returns>An instance of <see cref="DaemonService"/>.</returns>
    public static DaemonService FromFactory(Func<IDriverDaemon> daemonFactory, out Action onConnectedDelegate,
        out Action onDisconnectedDelegate)
    {
        var daemonService = new DaemonService(daemonFactory);
        onConnectedDelegate = daemonService.OnConnected;
        onDisconnectedDelegate = daemonService.OnDisconnected;
        return daemonService;
    }

    public async Task ConnectAsync()
    {
        await _completionSource.Task;
    }

    private void OnConnected()
    {
        _instance = _daemonFactory();
        _completionSource.SetResult();
        Log.Output += Log_Output;
        IsConnected = true;
    }

    private void OnDisconnected()
    {
        _instance = null;
        _completionSource = new TaskCompletionSource();
        Log.Output -= Log_Output;
        IsConnected = false;
    }

    private void Log_Output(object? _, LogMessage message)
    {
        _instance!.WriteMessage(message).ConfigureAwait(false);
    }
}
