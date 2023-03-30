using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenTabletDriver.UI.Models;

// Do not use MVVM source gen here, as it will cause STJ source gen to fail to
// generate a correct serializer for this class.
public partial class UISettings : ObservableObject
{
    private bool _firstLaunch = true;
    private bool _autoStart;
    private bool _kaomoji = true;

    public bool FirstLaunch
    {
        get => _firstLaunch;
        set => SetProperty(ref _firstLaunch, value);
    }

    public bool AutoStart
    {
        get => _autoStart;
        set => SetProperty(ref _autoStart, value);
    }

    public bool Kaomoji
    {
        get => _kaomoji;
        set => SetProperty(ref _kaomoji, value);
    }
}
