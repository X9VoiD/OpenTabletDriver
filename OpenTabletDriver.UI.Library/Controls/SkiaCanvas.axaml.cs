using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;

namespace OpenTabletDriver.UI.Controls;

public partial class SkiaCanvas : UserControl
{
    public SkiaCanvas()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public event Action<SKCanvas, SKSurface>? RenderAction;

    protected virtual void OnRenderAction(SKCanvas skCanvas, SKSurface skSurface)
    {
        RenderAction?.Invoke(skCanvas, skSurface);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ContentProperty)
            throw new InvalidOperationException("Content is not supported on SkiaCanvas");

        base.OnPropertyChanged(change);
    }

    // public override void Render(DrawingContext context)
    // {
    //     context.Custom
    //     var skia = context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>();
    //     if (skia == null)
    //         return;

    //     using (var lease = skia.Lease())
    //     {
    //         var skCanvas = lease.SkCanvas;
    //         var skSurface = lease.SkSurface;
    //         if (skCanvas is not null && skSurface is not null)
    //             OnRenderAction(skCanvas, skSurface);
    //     }
    //     base.Render(context);
    // }
}
