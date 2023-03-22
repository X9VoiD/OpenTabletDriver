using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Views;

public partial class PlaygroundView : Window
{
    public PlaygroundView()
    {
        InitializeComponent();
        SetTransparencyLevelHint();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void SetTransparencyLevelHint()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Build >= 22000)
        {
            // Windows 11 Mica blur
            TransparencyLevelHint = WindowTransparencyLevel.Mica;
        }
        else
        {
            // Force acrylic blur for everything else
            TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
        }
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        WindowBg.Opacity = 0.65;
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        WindowBg.Opacity = 1.0;
        base.OnLostFocus(e);
    }
}

