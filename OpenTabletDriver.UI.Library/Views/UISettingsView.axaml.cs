using Avalonia.Controls;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Views;

public partial class UISettingsView : UserControl
{
    public UISettingsView()
    {
        InitializeComponent();
        NavigationMixin.Attach(this);
    }
}
