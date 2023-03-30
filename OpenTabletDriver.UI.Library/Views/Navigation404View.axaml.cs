using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Views;

public partial class Navigation404View : UserControl
{
    public Navigation404View()
    {
        InitializeComponent();
        NavigationMixin.Attach(this);
    }
}
