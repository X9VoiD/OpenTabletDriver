using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.ViewModels
{
    /// <summary>
    /// Represents the main view model and is responsible for
    /// managing the daemon connection and the tablet and tool view models.
    /// </summary>
    public sealed partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IDaemonService _daemonService;
        private readonly INavigator _navigator;
        private string? _currentRoute;

        [ObservableProperty]
        private string _title;

        /// <summary>
        /// Gets or sets a boolean indicating whether the daemon is connected.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand),
                                    nameof(ResetSettingsCommand),
                                    nameof(GetPresetsCommand),
                                    nameof(ApplyPresetCommand),
                                    nameof(SaveAsPresetCommand))]
        private bool _isConnected;

        /// <summary>
        /// Gets or sets a boolean indicating whether the side pane is open.
        /// </summary>
        [ObservableProperty]
        private bool _sidePaneOpen;

        /// <summary>
        /// Gets or sets an array of currently available displays.
        /// </summary>
        [ObservableProperty]
        private ImmutableArray<DisplayDto> _displays;

        /// <summary>
        /// Gets or sets an array of the settings currently applied for each tool.
        /// </summary>
        [ObservableProperty]
        private ImmutableArray<PluginSettings> _toolSettings;

        /// <summary>
        /// Gets or sets an array that contains information about the plugins currently loaded and ways to configure them.
        /// </summary>
        [ObservableProperty]
        private ImmutableArray<PluginContextDto> _plugins;

        public string Version { get; } = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        /// <summary>
        /// Gets an observable collection of tablet view models.
        /// </summary>
        public ObservableCollection<TabletViewModel> Tablets { get; } = new();

        /// <summary>
        /// Gets an observable collection of tool view models.
        /// </summary>
        public ObservableCollection<ToolViewModel> Tools { get; } = new();

        /// <summary>
        /// Gets an observable collection of preset names.
        /// </summary>
        public ObservableCollection<string> Presets { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel(IDaemonService daemonService, INavigator navigator)
        {
            _title = "OpenTabletDriver v" + Version;
            _daemonService = daemonService;
            _daemonService.PropertyChanged += HandleDaemonServiceOnPropertyChanged;
            _navigator = navigator;
            _navigator.NextAsRoot(AppRoutes.DaemonConnectionRoute);

            // Connection-time setup is done on OnIsConnectedChanged()
            Dispatcher.UIThread.InvokeAsync(attemptDaemonConnection).ConfigureAwait(false);

            async Task attemptDaemonConnection()
            {
                try
                {
                    await _daemonService!.ConnectAsync();
                }
                catch
                {
                    // TODO: log
                }
            }
        }

        private void HandleDaemonServiceOnPropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (e.PropertyName == nameof(IDaemonService.State))
                {
                    switch (_daemonService.State)
                    {
                        case DaemonState.Connecting:
                            break;
                        case DaemonState.Disconnected:
                            break;
                        default:
                            break;
                    }

                    IsConnected = _daemonService.State == DaemonState.Connected;
                }
            });
        }

        public MainWindowViewModel()
        {
            // TODO: setup design-time data
            _daemonService = null!;
            _navigator = null!;
            _title = "OpenTabletDriver";
        }

        /// <summary>
        /// Initializes the view model by subscribing to events and getting the initial data.
        /// </summary>
        /// <returns>An awaitable task that completes when all setup is complete.</returns>
        private async Task InitializeAsync()
        {
            _daemonService.Instance!.TabletAdded += Daemon_TabletAdded;
            _daemonService.Instance.TabletRemoved += Daemon_TabletRemoved;
            _daemonService.Instance.ToolsChanged += Daemon_ToolsChanged;
            Displays = (await _daemonService.Instance.GetDisplays()).ToImmutableArray();
            ToolSettings = (await _daemonService.Instance.GetToolSettings()).ToImmutableArray();
            Plugins = (await _daemonService.Instance.GetPlugins()).ToImmutableArray();
            await SetupTabletViewModels();
            await SetupToolViewModels();
            await GetPresetsAsync();
        }

        /// <summary>
        /// Saves the currently applied settings.
        /// </summary>
        /// <returns>An awaitable task that completes when daemon is done saving settings.</returns>
        [RelayCommand(CanExecute = nameof(IsConnected))]
        private async Task SaveSettingsAsync()
        {
            // TODO: apply profiles of tablets with dirty profiles.
            // TODO: apply settings of tools with dirty settings.
            Debug.Assert(_daemonService.Instance != null);
            await _daemonService.Instance.SaveSettings();
        }

        /// <summary>
        /// Resets the settings to its defaults.
        /// </summary>
        /// <returns>An awaitable task that completes when daemon is done resetting settings.</returns>
        [RelayCommand(CanExecute = nameof(IsConnected))]
        private async Task ResetSettingsAsync()
        {
            Debug.Assert(_daemonService.Instance != null);
            await _daemonService.Instance.ResetSettings();
        }

        /// <summary>
        /// Gets the current presets. This updates the <see cref="Presets"/> collection.
        /// </summary>
        /// <returns>An awaitable task that completes when <see cref="Presets"/> is updated.</returns>
        [RelayCommand(CanExecute = nameof(IsConnected))]
        private async Task GetPresetsAsync()
        {
            // TODO: switch to using immutable array for presets?
            Debug.Assert(_daemonService.Instance != null);
            Presets.Clear();
            foreach (var preset in await _daemonService.Instance.GetPresets())
                Presets.Add(preset);
        }

        /// <summary>
        /// Applies a preset.
        /// </summary>
        /// <param name="preset">The name of the preset to apply.</param>
        /// <returns>An awaitable task that completes when daemon is done applying the preset.</returns>
        [RelayCommand(CanExecute = nameof(IsConnected))]
        private async Task ApplyPresetAsync(string? preset)
        {
            Debug.Assert(_daemonService.Instance != null);
            if (preset is not null && Presets.Contains(preset))
                await _daemonService.Instance.ApplyPreset(preset);
        }

        /// <summary>
        /// Saves the currently applied settings as a preset.
        /// </summary>
        /// <param name="preset">The name of the preset to save.</param>
        /// <returns>An awaitable task that completes when daemon is done saving the preset.</returns>
        [RelayCommand(CanExecute = nameof(IsConnected))]
        private async Task SaveAsPresetAsync(string? preset)
        {
            Debug.Assert(preset != null);
            Debug.Assert(_daemonService.Instance != null);
            await _daemonService.Instance.SaveAsPreset(preset);
        }

        private async Task SetupTabletViewModels()
        {
            foreach (var tabletId in await _daemonService.Instance!.GetTablets())
            {
                await AddTabletViewModel(tabletId);
            }
        }

        private async Task SetupToolViewModels()
        {
            foreach (var tool in await _daemonService.Instance!.GetToolSettings())
            {
                AddToolViewModel(tool);
            }
        }

        private async Task AddTabletViewModel(int tabletId)
        {
            var tabletHandler = await TabletService.CreateAsync(_daemonService.Instance!, tabletId);
            var tabletViewModel = new TabletViewModel(tabletHandler);
            Tablets.Add(tabletViewModel);
        }

        private void AddToolViewModel(PluginSettings tool)
        {
            var toolDescriptor = _plugins
                .SelectMany(c => c.Plugins)
                .FirstOrDefault(p => p.Path == tool.Path);

            var toolViewModel = new ToolViewModel(toolDescriptor, tool);
            Tools.Add(toolViewModel);
        }

        private void Daemon_TabletAdded(object? sender, int tabletId)
        {
            AddTabletViewModel(tabletId).ConfigureAwait(false);
        }

        private void Daemon_TabletRemoved(object? sender, int tabletId)
        {
            for (var i = 0; i < Tablets.Count; i++)
            {
                if (Tablets[i].TabletId == tabletId)
                {
                    Tablets.RemoveAt(i);
                    return;
                }
            }
        }

        private void Daemon_ToolsChanged(object? sender, IEnumerable<PluginSettings> tools)
        {
            foreach (var newToolSetting in tools)
            {
                var tool = Tools.FirstOrDefault(t => t.Settings.Path == newToolSetting.Path);
                if (tool is null)
                    continue;

                tool.Settings = newToolSetting;
            }
        }

        partial void OnIsConnectedChanged(bool value)
        {
            if (value)
            {
                InitializeAsync().ConfigureAwait(false);
            }
            else
            {
                Displays = ImmutableArray<DisplayDto>.Empty;
                ToolSettings = ImmutableArray<PluginSettings>.Empty;
                Plugins = ImmutableArray<PluginContextDto>.Empty;
                Tablets.Clear();
                Tools.Clear();
                Presets.Clear();
            }
        }
    }
}
