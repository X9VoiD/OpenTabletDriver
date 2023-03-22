using Avalonia.Controls;
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

    public event Action<SKCanvas, SKSurface>? RenderAction;

    protected virtual void OnRenderAction(SKCanvas skCanvas, SKSurface skSurface)
    {
        RenderAction?.Invoke(skCanvas, skSurface);
    }

    public override void Render(DrawingContext context)
    {
        var skia = context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>();
        if (skia == null)
            return;

        using (var lease = skia.Lease())
        {
            var skCanvas = lease.SkCanvas;
            var skSurface = lease.SkSurface;
            if (skCanvas is not null && skSurface is not null)
                OnRenderAction(skCanvas, skSurface);
        }
        base.Render(context);
    }
}
