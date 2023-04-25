using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Views;

// TODO: add preset management
public partial class TabletsOverview : UserControl
{
    public TabletsOverview()
    {
        InitializeComponent();
        NavigationMixin.Attach(this);
    }
}
