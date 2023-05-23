using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Channels;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.Logging;
using OpenTabletDriver.UI.Models;

namespace OpenTabletDriver.UI.Services;

public enum DaemonState
{
    Disconnected,
    Connecting,
    Connected
}

public interface IDaemonService : INotifyPropertyChanged, INotifyPropertyChanging
{
    DaemonState State { get; }
    IDriverDaemon? Instance { get; }
    ObservableCollection<ITabletService> Tablets { get; }
    Task ConnectAsync();
    Task ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    Task ReconnectAsync();
    Task ReconnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
}

public partial class DaemonService : ObservableObject, IDaemonService
{
    private record TabletIdListChange(
        TabletIdListChangeType Change,
        int Id
    );

    private enum TabletIdListChangeType
    {
        Add,
        Remove
    }

    private readonly IRpcClient<IDriverDaemon> _rpcClient;
    private readonly IDispatcher _dispatcher;
    private DaemonState _state;
    private IDriverDaemon? _instance;
    private bool _suppressStateChangedEvents;
    private SemaphoreSlim _connectSemaphore = new(1, 1);
    private Channel<TabletIdListChange> _tabletIdListChangeSerializer = Channel.CreateUnbounded<TabletIdListChange>();
    private Task _idManagerTask;
    private CancellationTokenSource _idManagerTaskRunning = new();

    public DaemonState State
    {
        get => _state;
        private set => SetProperty(ref _state, value);
    }

    public IDriverDaemon? Instance
    {
        get => _instance;
        private set => SetProperty(ref _instance, value);
    }

    public ObservableCollection<ITabletService> Tablets { get; } = new();

    public DaemonService(IRpcClient<IDriverDaemon> rpcClient, IDispatcher dispatcher)
    {
        _rpcClient = rpcClient;
        _dispatcher = dispatcher;
        rpcClient.Connected += OnConnected;
        rpcClient.Disconnected += OnDisconnected;

        _idManagerTask = Task.Run(StartIdManagerAsync);
    }

    private async Task StartIdManagerAsync()
    {
        var reader = _tabletIdListChangeSerializer.Reader;
        await foreach (var change in reader.ReadAllAsync(_idManagerTaskRunning.Token))
        {
            switch (change.Change)
            {
                case TabletIdListChangeType.Add:
                    await CreateTabletService(_instance!, change.Id);
                    break;
                case TabletIdListChangeType.Remove:
                    RemoveTabletService(change.Id);
                    break;
            }
        }

        async Task CreateTabletService(IDriverDaemon daemon, int tabletId)
        {
            var tabletService = await TabletService.CreateAsync(daemon, tabletId);
            _dispatcher.Post(() => Tablets.Add(tabletService));
        }

        void RemoveTabletService(int tabletId)
        {
            var tabletService = Tablets.First(tablet => tablet.TabletId == tabletId);
            _dispatcher.Post(() => Tablets.Remove(tabletService));
        }
    }

    public Task ConnectAsync()
    {
        return ConnectAsync(TimeSpan.FromSeconds(5));
    }

    public async Task ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (!_rpcClient.IsConnected)
        {
            State = DaemonState.Connecting;
            try
            {
                await _rpcClient.ConnectAsync(timeout, cancellationToken);
            }
            catch
            {
                State = DaemonState.Disconnected;
                throw;
            }
        }
    }

    public Task ReconnectAsync()
    {
        return ReconnectAsync(TimeSpan.FromSeconds(5));
    }

    public async Task ReconnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        _suppressStateChangedEvents = true;
        State = DaemonState.Connecting;
        _rpcClient.Disconnect();
        await _rpcClient.ConnectAsync(timeout, cancellationToken);
        _suppressStateChangedEvents = false;
    }

    private void OnConnected(object? _, EventArgs args)
    {
        var daemon = _rpcClient.Instance!;
        daemon.TabletAdded += (sender, tabletId) => QueueChange(TabletIdListChangeType.Add, tabletId);
        daemon.TabletRemoved += (sender, tabletId) => QueueChange(TabletIdListChangeType.Remove, tabletId);

        _dispatcher.ProbablySynchronousPost(() =>
        {
            Log.Output += Log_Output;
            Instance = daemon;
            if (!_suppressStateChangedEvents)
                State = DaemonState.Connected;
        });

        _ = Task.Run(async () =>
        {
            (await daemon.GetTablets()).ForEach(tabletId => QueueChange(TabletIdListChangeType.Add, tabletId));
        });
    }

    private void OnDisconnected(object? _, EventArgs args)
    {
        using (_connectSemaphore.Lock())
        {
            Instance = null;
            Log.Output -= Log_Output;

            if (!_suppressStateChangedEvents)
                State = DaemonState.Disconnected;
        }
    }

    private void Log_Output(object? _, LogMessage message)
    {
        _instance!.WriteMessage(message).ConfigureAwait(false);
    }

    private void QueueChange(TabletIdListChangeType change, int id)
    {
        var success = _tabletIdListChangeSerializer.Writer.TryWrite(new TabletIdListChange(change, id));
        Debug.Assert(success);
    }
}
