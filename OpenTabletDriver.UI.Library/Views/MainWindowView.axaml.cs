using Avalonia;
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

    public MainWindowView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        _navigator = Ioc.Default.GetRequiredService<INavigatorFactory>().GetOrCreate(AppRoutes.MainHost);
        _dispatcher = Ioc.Default.GetRequiredService<IDispatcher>();

        HookInputEvents();
        this.BootstrapTransparency((MainWindowViewModel)DataContext!, _dispatcher);

        // ensure we have window-level focus on startup
        Activate();
        Focus();
    }

    private void HookInputEvents()
    {
        PointerPressed += (sender, e) =>
        {
            if (e.Handled)
                return;

            e.Handled = true;
            // TODO: is there a need to implement forward?
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == Avalonia.Input.PointerUpdateKind.XButton1Pressed
                && _navigator.CanGoBack)
            {
                _navigator.Pop();
                // TODO: horrible name, also don't rely on to-be-internal IPseudoClasses interface
                ((IPseudoClasses)BackButtonButton.Classes).Set(":pressed", true);
                _dispatcher.Post(async () =>
                {
                    await Task.Delay(100);
                    ((IPseudoClasses)BackButtonButton.Classes).Set(":pressed", false);
                });
            }

            Application.Current?.FocusManager?.Focus(null);
        };
    }
}
