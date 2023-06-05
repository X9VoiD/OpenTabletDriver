using Avalonia.Controls;
using Avalonia.Styling;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Controls;

public abstract class ActivatableUserControl : UserControl, IStyleable
{
    protected ActivatableUserControl()
    {
        NavigationMixin.Attach(this);
    }

    Type IStyleable.StyleKey => typeof(ActivatableUserControl);
}
