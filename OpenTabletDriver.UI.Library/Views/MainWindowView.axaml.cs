using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Views;

public partial class MainWindowView : Window
{
    private readonly INavigationService _navigationService;
    private bool _hasTransparency;

    public MainWindowView()
    {
        InitializeComponent();
        SetTransparencyLevelHint();

        _navigationService = Application.Current!.Resources.TryGetResource("NavigationService", null, out var s)
            ? (INavigationService)s!
            : throw new Exception("Failed to get NavigationService from resources");
        HookBackButtonOpacityHandler();
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

        _hasTransparency = ActualTransparencyLevel != WindowTransparencyLevel.None;
        if (_hasTransparency)
        {
            Activated += (sender, e) =>
            {
                WindowBg.Opacity = 0.65;
            };

            Deactivated += (sender, e) =>
            {
                WindowBg.Opacity = 1.0;
            };
        }
        else
        {
            // If transparency is not supported, remove the opacity animation
            WindowBg.IsVisible = false;
        }
    }

    private void HookBackButtonOpacityHandler()
    {
        _navigationService.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(_navigationService.CanGoBack))
            {
                HandleBackButtonOpacity();
            }
        };
        HandleBackButtonOpacity();
    }

    private void HandleBackButtonOpacity()
    {
        BackButton.Opacity = _navigationService.CanGoBack ? 1.0 : 0.5;
    }
}
