using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenTabletDriver.UI.ViewModels;

public class AreaEditorViewModel : ObservableObject
{
    public Area Mapping { get; } = new();
    public ObservableCollection<Area> Targets { get; } = new();
}

public partial class Area : ObservableObject
{
    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;

    [ObservableProperty]
    private double _rotation;
}
