using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Views;

public partial class MainWindowView : Window
{
    private readonly INavigator _navigator;
    private readonly IDispatcher _dispatcher;
    private bool _windowTransparency = false;

    public MainWindowView()
    {
        InitializeComponent();
        NavigationHost.NavigationHostName = AppRoutes.MainHost;
        DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        _navigator = Ioc.Default.GetRequiredService<INavigatorFactory>().GetOrCreate(AppRoutes.MainHost);
        _dispatcher = Ioc.Default.GetRequiredService<IDispatcher>();

        // UI/Service events
        HookInputEvents();

        // ViewModel events
        var vm = (MainWindowViewModel)DataContext!;
        BootstrapTransparency(vm);

        Activate(); // ensure we have window-level focus on startup
    }

    private void HookInputEvents()
    {
        // Change focus to nothing when clicking on the background
        this.PointerPressed += (sender, e) =>
        {
            if (!e.Handled)
            {
                App.Current?.FocusManager?.Focus(null);
            }
        };

        // Change back button opacity when navigation state changes
        void handleOpacity(bool canGoBack) => BackButton.Opacity = canGoBack ? 1.0 : 0.5;

        _navigator.Navigated += (_, _) => handleOpacity(_navigator.CanGoBack);
        handleOpacity(_navigator.CanGoBack);
    }

    private void BootstrapTransparency(MainWindowViewModel vm)
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

        Activated += (_, _) => { if (_windowTransparency) WindowBg.Opacity = 0.65; };
        Deactivated += (_, _) => { if (_windowTransparency) WindowBg.Opacity = 1.0; };

        vm.HandleProperty(
            nameof(vm.TransparencyEnabled),
            vm => vm.TransparencyEnabled,
            (vm, transparencyEnabled) => _dispatcher.ProbablySynchronousPost(() =>
            {
                if (transparencyEnabled)
                {
                    EnableWindowTransparency();
                }
                else
                {
                    DisableWindowTransparency();
                }
            })
        );
    }

    private void EnableWindowTransparency()
    {
        if (ActualTransparencyLevel != WindowTransparencyLevel.None)
        {
            _windowTransparency = true;
            AcrylicBorder.IsVisible = true;
            WindowBg.Opacity = 0.65;
        }
        else
        {
            // If transparency is not supported, disable it
            DisableWindowTransparency();
        }
    }

    private void DisableWindowTransparency()
    {
        _windowTransparency = false;
        WindowBg.Opacity = 1.0;
    }
}
