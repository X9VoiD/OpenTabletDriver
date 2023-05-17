using Avalonia.Controls;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Controls;

public abstract class ActivatableUserControl : UserControl
{
    protected ActivatableUserControl()
    {
        NavigationMixin.Attach(this);
    }
}
