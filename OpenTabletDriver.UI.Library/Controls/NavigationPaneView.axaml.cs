using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Controls;

public partial class NavigationPaneView : UserControl
{
    public NavigationPaneView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<NavigationPaneViewModel>();
        var vm = (NavigationPaneViewModel)DataContext;

        ConnectDisjointedList(vm);
    }

    private void ConnectDisjointedList(NavigationPaneViewModel vm)
    {
        // Since the settings button is not actually part of the navigation pane,
        // we will need to manually handle the click event.
        SettingsButton.PointerPressed += (object? s, PointerPressedEventArgs e) =>
            vm.SettingsOpened = e.GetCurrentPoint(s as Visual).Properties.IsLeftButtonPressed;
    }
}
