using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenTabletDriver.UI.ViewModels;
using SkiaSharp;

namespace OpenTabletDriver.UI.Controls;

public partial class AreaEditor : SkiaCanvas
{
    private bool _isTweening;

    public AreaEditor()
    {
        InitializeComponent();
    }

    protected override void OnRenderAction(SKCanvas skCanvas, SKSurface skSurface)
    {
        if (DataContext is not AreaEditorViewModel vm)
            return;


    }
}
