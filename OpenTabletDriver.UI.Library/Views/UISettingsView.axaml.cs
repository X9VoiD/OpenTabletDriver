using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Views;

public partial class UISettingsView : UserControl
{
    public UISettingsView()
    {
        InitializeComponent();
        NavigationMixin.Attach(this);
    }
}

