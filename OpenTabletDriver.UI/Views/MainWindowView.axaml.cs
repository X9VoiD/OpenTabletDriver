using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace OpenTabletDriver.UI.Views;

public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        InitializeComponent();
        SetTransparencyLevelHint();
    }

    private void SetTransparencyLevelHint()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Build >= 22000)
        {
            // Windows 11 Mica blur
            TransparencyLevelHint = WindowTransparencyLevel.Mica;
            return;
        }

        // Force acrylic blur for everything else
        TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
    }
}
