using Avalonia.Controls;

namespace OpenTabletDriver.UI.Navigation;

public static class NavigationMixin
{
    public static void Attach(Control control)
    {
        control.AttachedToVisualTree += (s, e) =>
        {
            if (control.DataContext is NavigationViewModelBase vm)
            {
                vm.OnNavigatedTo();
            }
        };

        control.DetachedFromVisualTree += (s, e) =>
        {
            if (control.DataContext is NavigationViewModelBase vm)
            {
                vm.OnNavigatedFrom();
            }
        };
    }
}
