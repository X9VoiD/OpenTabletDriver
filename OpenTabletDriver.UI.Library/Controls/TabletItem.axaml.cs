using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenTabletDriver.UI.Controls;

public partial class TabletItem : UserControl
{
    private string? _tabletName;

    public TabletItem()
    {
        InitializeComponent();
    }

    public DirectProperty<TabletItem, string?> TabletNameProperty =
        AvaloniaProperty.RegisterDirect<TabletItem, string?>(
            nameof(TabletName),
            o => o.TabletName,
            (o, v) => o.TabletName = v);

    public string? TabletName
    {
        get => _tabletName;
        set => SetAndRaise(TabletNameProperty, ref _tabletName, value);
    }
}
