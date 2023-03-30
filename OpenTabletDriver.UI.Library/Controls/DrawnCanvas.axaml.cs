using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace OpenTabletDriver.UI.Controls;

public partial class DrawnCanvas : UserControl
{
    public DrawnCanvas()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public Action<DrawingContext>? RenderAction;

    protected virtual void OnRenderAction(DrawingContext context)
    {
        RenderAction?.Invoke(context);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ContentProperty)
            throw new InvalidOperationException("Content is not supported on SkiaCanvas");

        base.OnPropertyChanged(change);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
    }
}
