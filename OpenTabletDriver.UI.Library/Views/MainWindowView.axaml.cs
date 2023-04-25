using System.Runtime.InteropServices;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Views;

public partial class MainWindowView : Window
{
    private readonly INavigationService _navigationService;
    private bool _hasTransparency;

    public MainWindowView()
    {
        InitializeComponent();
        _navigationService = Ioc.Default.GetRequiredService<INavigationService>();
        SetTransparencyLevelHint();
        HookBackButtonOpacityHandler();
        DataContext = Ioc.Default.GetService<MainWindowViewModel>();

        this.PointerPressed += (sender, e) =>
        {
            if (!e.Handled)
            {
                App.Current?.FocusManager?.Focus(null);
            }
        };
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
            Activated += (_, _) => WindowBg.Opacity = 0.65;
            Deactivated += (_, _) => WindowBg.Opacity = 1.0;
            WindowBg.Opacity = 0.65; // assume active
        }
        else
        {
            // If transparency is not supported, remove the opacity animation
            WindowBg.IsVisible = false;
        }
    }

    private void HookBackButtonOpacityHandler()
    {
        _navigationService.HandleProperty(
            nameof(_navigationService.CanGoBack),
            service => service.CanGoBack,
            (service, canGoBack) => BackButton.Opacity = canGoBack ? 1.0 : 0.5
        );
    }
}
