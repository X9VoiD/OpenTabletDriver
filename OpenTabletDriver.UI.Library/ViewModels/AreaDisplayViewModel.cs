using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels;

public class AreaDisplayViewModel : ObservableObject
{
    public AreaDisplayViewModel(IEnumerable<Bounds> bounds, Mapping? mapping, string? maxBoundsName = null)
    {
        var canvas = new Canvas();
        Bounds = new(bounds.ToArray());

        var minX = Bounds.Min(b => b.X);
        var minY = Bounds.Min(b => b.Y);
        var offsetX = Math.Abs(Math.Min(minX, 0));
        var offsetY = Math.Abs(Math.Min(minY, 0));
        var maxWidth = Bounds.Max(b => b.X + b.Width + offsetX);
        var maxHeight = Bounds.Max(b => b.Y + b.Height + offsetY);

        MaximumBounds = new Bounds(
            minX,
            minY,
            maxWidth,
            maxHeight,
            0,
            maxBoundsName ?? "Full Area"
        );

        Mapping = mapping ?? new Mapping(
            MaximumBounds.X,
            MaximumBounds.Y,
            MaximumBounds.Width,
            MaximumBounds.Height,
            0
        );
    }

    public ReadOnlyCollection<Bounds> Bounds { get; }
    public Bounds MaximumBounds { get; }
    public Mapping Mapping { get; }
}

public partial class Mapping : ObservableObject
{
    [ObservableProperty]
    private int _x;

    [ObservableProperty]
    private int _y;

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    [ObservableProperty]
    private float _rotation;

    public Mapping(int x, int y, int width, int height, float rotation)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;
        _rotation = rotation;
    }
}

public partial class Bounds : Mapping
{
    [ObservableProperty]
    private string? _name;

    public Bounds(int x, int y, int width, int height, float rotation, string? name) : base(x, y, width, height, rotation)
    {
        _name = name;
    }

    public static Bounds FromDto(DisplayDto displayDto)
    {
        var x = (int)displayDto.X;
        var y = (int)displayDto.Y;
        var width = (int)displayDto.Width;
        var height = (int)displayDto.Height;

        return new(
            x,
            y,
            width,
            height,
            0,
            $"Display {displayDto.Index}" // OTD does not support display names yet so create a
        );
    }
}
