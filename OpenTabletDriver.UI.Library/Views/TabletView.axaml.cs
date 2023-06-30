using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using OpenTabletDriver.UI.Controls;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Views;

public partial class TabletView : ActivatableUserControl
{
    public TabletView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is TabletViewModel vm)
        {
        }
    }
}
