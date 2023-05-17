using Avalonia.Controls;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Navigation;

public static class NavigationMixin
{
    public static void Attach(Control control)
    {
        control.AttachedToVisualTree += (s, e) =>
        {
            if (control.DataContext is ActivatableViewModelBase vm)
            {
                vm.OnActivated();
            }
        };

        control.DetachedFromVisualTree += (s, e) =>
        {
            if (control.DataContext is ActivatableViewModelBase vm)
            {
                vm.OnDeactivated();
            }
        };
    }
}
