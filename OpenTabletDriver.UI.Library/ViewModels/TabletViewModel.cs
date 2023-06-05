using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.ViewModels;

public partial class TabletViewModel : ActivatableViewModelBase
{
    private readonly IDaemonService _daemonService;
    private readonly ITabletService _tabletService;
    private readonly IDispatcher _dispatcher;
    private bool _modified;
    private bool _saved = true;

    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private PluginDto? _selectedOutputMode;

    [ObservableProperty]
    private bool _isAbsoluteMode;

    [ObservableProperty]
    private bool _isRelativeMode;

    // Absolute Mode Settings
    [ObservableProperty]
    private AreaDisplayViewModel _displayArea = null!;

    [ObservableProperty]
    private TabletAreaDisplayViewModel _tabletArea = null!;

    // Relative Mode Settings
    [ObservableProperty]
    private double _sensitivityX;

    [ObservableProperty]
    private double _sensitivityY;

    [ObservableProperty]
    private double _relativeModeRotation;

    [ObservableProperty]
    private double _resetDelay;

    public Profile Profile => _tabletService.Profile;
    public int TabletId => _tabletService.TabletId;
    public string Name => _tabletService.Name;
    public ObservableCollection<PluginDto> OutputModes { get; } = new();
    public ObservableCollection<PluginSettingViewModel> OutputModeSettings { get; } = new();

    public bool Modified
    {
        get => _modified;
        set
        {
            SetProperty(ref _modified, value);
            ApplyCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            DiscardCommand.NotifyCanExecuteChanged();
        }
    }

    public bool Saved
    {
        get => _saved;
        set
        {
            SetProperty(ref _saved, value);
            SaveCommand.NotifyCanExecuteChanged();
        }
    }

    public bool CanSave => !Saved;

    public TabletViewModel(ITabletService tabletService)
    {
        _daemonService = Ioc.Default.GetRequiredService<IDaemonService>();
        _dispatcher = Ioc.Default.GetRequiredService<IDispatcher>();
        _tabletService = tabletService;

        // propagate daemon-broadcasted profile changes to the UI
        // TODO: there's currently no fast way to check if this profile is the one
        // that UI just sent to daemon, so disable this for now to save CPU cycles

        // _tabletService.HandleProperty(
        //     nameof(_tabletService.Profile),
        //     s => s.Profile,
        //     (s, p) => _dispatcher.Post(async () => await ReadFromProfileAsync()),
        //     invokeOnCreation: false);

        // propagate plugin changes as well
        _daemonService.PluginContexts.CollectionChanged += (s, e) =>
        {
            // no need to optimize this since it's rarely called
            var lastSelectedOutputMode = SelectedOutputMode;

            ReadOutputModes();

            if (lastSelectedOutputMode is not null)
            {
                var selectedOutputMode = OutputModes.FirstOrDefault(m => m.Path == lastSelectedOutputMode?.Path);
                SelectedOutputMode = selectedOutputMode ?? OutputModes.FirstOrDefault();
            }
        };

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        ReadOutputModes();
        await ReadFromProfileAsync();
        IsInitialized = true;
    }

    partial void OnSelectedOutputModeChanged(PluginDto? value)
    {
        OutputModeSettings.Clear();

        if (value is null)
            return;

        IsAbsoluteMode = value.IsAbsoluteMode();
        IsRelativeMode = value.IsRelativeMode();

        var settings = value.GetCustomOutputModeSettings()
            .Select(s => PluginSettingViewModel.CreateBindable(value, Profile, p => p.OutputMode[s.PropertyName])!)
            .Where(s => s is not null);

        OutputModeSettings.AddRange(settings!);
    }

    [RelayCommand(CanExecute = nameof(Modified))]
    private async Task Apply()
    {
        await WriteToProfileAsync();
        Modified = false;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        if (Modified)
            await WriteToProfileAsync();

        await _daemonService.Instance!.SaveSettings();
        Modified = false;
        Saved = true;
    }

    [RelayCommand(CanExecute = nameof(Modified))]
    private async Task Discard()
    {
        await ReadFromProfileAsync();
        Modified = false;
        Saved = false;
    }

    [RelayCommand]
    private async Task Reset()
    {
        await _tabletService.ResetProfile();
    }

    private async Task WriteToProfileAsync()
    {
        var profile = _tabletService.Profile;

        // propagate DisplayArea and TabletArea changes to the profile
        var outputMode = profile.OutputMode;
        outputMode.Path = SelectedOutputMode!.Path;

        var input = outputMode["Input"];

        input.SetValue(new {
            XPosition = TabletArea.Mapping.X,
            YPosition = TabletArea.Mapping.Y,
            Width = (double)TabletArea.Mapping.Width,
            Height = (double)TabletArea.Mapping.Height,
            Rotation = (double)TabletArea.Mapping.Rotation,
        });

        var output = outputMode["Output"];

        var xOffset = Math.Abs(Math.Min(DisplayArea.MaximumBounds.X, 0));
        var yOffset = Math.Abs(Math.Min(DisplayArea.MaximumBounds.Y, 0));

        var x = xOffset + DisplayArea.Mapping.X + DisplayArea.Mapping.Width / 2.0;
        var y = yOffset + DisplayArea.Mapping.Y + DisplayArea.Mapping.Height / 2.0;

        output.SetValue(new
        {
            XPosition = x,
            YPosition = y,
            Width = (double)DisplayArea.Mapping.Width,
            Height = (double)DisplayArea.Mapping.Height,
        });

        var lockAspectRatio = outputMode["LockAspectRatio"];
        lockAspectRatio.SetValue(TabletArea.LockAspectRatio);

        var areaClipping = outputMode["AreaClipping"];
        areaClipping.SetValue(TabletArea.ClipInput);

        var areaLimiting = outputMode["AreaLimiting"];
        areaLimiting.SetValue(TabletArea.DropInput);

        var lockToBounds = outputMode["LockToBounds"];
        lockToBounds.SetValue(TabletArea.RestrictToMaximumBounds);

        var sensitivity = outputMode["Sensitivity"];
        sensitivity.SetValue(new
        {
            X = SensitivityX,
            Y = SensitivityY,
        });

        var rotation = outputMode["Rotation"];
        rotation.SetValue(RelativeModeRotation);

        var resetDelay = outputMode["ResetDelay"];
        resetDelay.SetValue(TimeSpan.FromMilliseconds(ResetDelay));

        // no need to propagate OutputModeSettings changes to the profile,
        // they are already bound to it.

        // send the updated profile to the daemon
        await _tabletService.ApplyProfile();
    }

    private async Task ReadFromProfileAsync()
    {
        if (DisplayArea != null)
        {
            DisplayArea.PropertyChanged -= HandleSettingsChanged;
            DisplayArea.Mapping.PropertyChanged -= HandleSettingsChanged;
        }

        if (TabletArea != null)
        {
            TabletArea.PropertyChanged -= HandleSettingsChanged;
            TabletArea.Mapping.PropertyChanged -= HandleSettingsChanged;
        }

        var daemon = _daemonService.Instance!;
        var displayDtos = await daemon.GetDisplays();
        var displayBounds = displayDtos
            .OrderBy(d => d.Index)
            .Select(d => Bounds.FromDto(d));
        var profile = _tabletService.Profile;
        PluginSettings? outputMode = profile.OutputMode;

        var tabletWidth = _tabletService.Configuration.Specifications.Digitizer!.Width;
        var tabletHeight = _tabletService.Configuration.Specifications.Digitizer!.Height;
        var tabletBounds = new Bounds(0, 0, tabletWidth, tabletHeight, 0);
        var tabletMapping = new Mapping(0, 0, tabletWidth, tabletHeight, 0, centerOrigin: true);

        var tabletAreaSetting = outputMode["Input"].Value!;

        DisplayArea = new AreaDisplayViewModel(displayBounds, null, "Full Virtual Desktop");
        TabletArea = new TabletAreaDisplayViewModel(DisplayArea, new Bounds[] { tabletBounds }, tabletMapping, "Full Area");

        SetOutputModeDefaultsIfNeeded(profile, TabletArea, DisplayArea);

        // propagate profile data to DisplayArea and TabletArea

        var displayAreaSetting = outputMode["Output"].Value!;
        var displayAreaOffsetX = Math.Abs(Math.Min(DisplayArea.MaximumBounds.X, 0));
        var displayAreaOffsetY = Math.Abs(Math.Min(DisplayArea.MaximumBounds.Y, 0));
        DisplayArea.Mapping.Width = (double)displayAreaSetting["Width"]!;
        DisplayArea.Mapping.Height = (double)displayAreaSetting["Height"]!;
        DisplayArea.Mapping.X = -displayAreaOffsetX + (double)displayAreaSetting["XPosition"]! - DisplayArea.Mapping.Width / 2.0;
        DisplayArea.Mapping.Y = -displayAreaOffsetY + (double)displayAreaSetting["YPosition"]! - DisplayArea.Mapping.Height / 2.0;

        DisplayArea.PropertyChanged += HandleSettingsChanged;
        DisplayArea.Mapping.PropertyChanged += HandleSettingsChanged;

        TabletArea.Mapping.Width = (double)tabletAreaSetting["Width"]!;
        TabletArea.Mapping.Height = (double)tabletAreaSetting["Height"]!;
        TabletArea.Mapping.X = (double)tabletAreaSetting["XPosition"]!;
        TabletArea.Mapping.Y = (double)tabletAreaSetting["YPosition"]!;
        TabletArea.Mapping.Rotation = (double)tabletAreaSetting["Rotation"]!;

        TabletArea.LockAspectRatio = (bool)outputMode["LockAspectRatio"].Value!;
        TabletArea.ClipInput = (bool)outputMode["AreaClipping"].Value!;
        TabletArea.DropInput = (bool)outputMode["AreaLimiting"].Value!;
        TabletArea.RestrictToMaximumBounds = (bool)outputMode["LockToBounds"].Value!;

        TabletArea.PropertyChanged += HandleSettingsChanged;
        TabletArea.Mapping.PropertyChanged += HandleSettingsChanged;

        var sensitivity = outputMode["Sensitivity"].Value!;
        SensitivityX = (double)sensitivity["X"]!;
        SensitivityY = (double)sensitivity["Y"]!;
        RelativeModeRotation = (double)outputMode["Rotation"].Value!;
        ResetDelay = ((TimeSpan)outputMode["ResetDelay"].Value!).TotalMilliseconds;

        var pluginDto = _daemonService.FindPlugin(outputMode.Path); // should never be null, has to be handled daemon-side
        Debug.Assert(pluginDto is not null);

        SelectedOutputMode = pluginDto;
    }

    // should use right after creating the area display view models
    private static void SetOutputModeDefaultsIfNeeded(Profile profile, AreaDisplayViewModel tabletArea, AreaDisplayViewModel displayArea)
    {
        // if AbsoluteMode settings are not set, set them to defaults
        var outputMode = profile.OutputMode;
        if (!outputMode["Input"].Value?.HasValues ?? true)
        {
            var input = outputMode["Input"];

            input.SetValue(new {
                XPosition = tabletArea.Mapping.X,
                YPosition = tabletArea.Mapping.Y,
                Width = (double)tabletArea.Mapping.Width,
                Height = (double)tabletArea.Mapping.Height,
                Rotation = (double)tabletArea.Mapping.Rotation,
            });

            var output = outputMode["Output"];

            var xOffset = Math.Abs(Math.Min(displayArea.MaximumBounds.X, 0));
            var yOffset = Math.Abs(Math.Min(displayArea.MaximumBounds.Y, 0));

            var x = xOffset + displayArea.Mapping.X + displayArea.Mapping.Width / 2.0;
            var y = yOffset + displayArea.Mapping.Y + displayArea.Mapping.Height / 2.0;

            output.SetValue(new
            {
                XPosition = x,
                YPosition = y,
                Width = (double)displayArea.Mapping.Width,
                Height = (double)displayArea.Mapping.Height,
            });

            var lockAspectRatio = outputMode["LockAspectRatio"];
            lockAspectRatio.SetValue(false);

            var areaClipping = outputMode["AreaClipping"];
            areaClipping.SetValue(true);

            var areaLimiting = outputMode["AreaLimiting"];
            areaLimiting.SetValue(false);

            var lockToBounds = outputMode["LockToBounds"];
            lockToBounds.SetValue(true);
        }
        // if RelativeMode settings are not set, set them to defaults
        if (!outputMode["Sensitivity"].Value?.HasValues ?? true)
        {
            var sensitivity = outputMode["Sensitivity"];
            sensitivity.SetValue(new
            {
                X = 10,
                Y = 10,
            });

            var rotation = outputMode["Rotation"];
            rotation.SetValue(0);

            var resetDelay = outputMode["ResetDelay"];
            resetDelay.SetValue(TimeSpan.FromMilliseconds(10));
        }
    }

    private void ReadOutputModes()
    {
        OutputModes.Clear();

        var outputModes = _daemonService.PluginContexts
            .SelectMany(pCtx => pCtx.Plugins)
            .Where(p => p.IsOutputMode());

        OutputModes.AddRange(outputModes);
    }

    partial void OnSensitivityXChanged(double value) => HandleSettingsChanged(null, null!);
    partial void OnSensitivityYChanged(double value) => HandleSettingsChanged(null, null!);
    partial void OnResetDelayChanged(double value) => HandleSettingsChanged(null, null!);
    partial void OnRelativeModeRotationChanged(double value) => HandleSettingsChanged(null, null!);

    private void HandleSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        Modified = true;
        Saved = false;
    }
}
