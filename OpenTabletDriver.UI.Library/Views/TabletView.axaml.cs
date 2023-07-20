using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using OpenTabletDriver.UI.Controls;
using OpenTabletDriver.UI.Messages;

namespace OpenTabletDriver.UI.Views;

public partial class TabletView : ActivatableUserControl
{
    private Thickness _originalMargin;
    private Thickness _originalMargin1st;

    public TabletView()
    {
        InitializeComponent();
        var messenger = Ioc.Default.GetRequiredService<IMessenger>();

        // remove horizontal margin when sidebar is hidden
        messenger.Register<TabletView, UILayoutChangedMessage>(this, (v, message) =>
        {
            if (message.Value == UILayoutChange.SidebarHidden)
            {
                if (!Application.Current!.TryGetResource("OTD_SectionEmphasizedMargin", null, out var marginObj)
                    || !Application.Current.TryGetResource("OTD_SectionEmphasizedMargin1st", null, out var margin1stObj))
                {
                    return;
                }

                var margin = (Thickness)marginObj!;
                var margin1st = (Thickness)margin1stObj!;

                _originalMargin = margin;
                _originalMargin1st = margin1st;

                var newMargin = new Thickness(0, margin.Top, 0, margin.Bottom);
                var newMargin1st = new Thickness(0, margin1st.Top, 0, margin1st.Bottom);

                Resources["OTD_SectionEmphasizedMargin"] = newMargin;
                Resources["OTD_SectionEmphasizedMargin1st"] = newMargin1st;
            }
            else if (message.Value == UILayoutChange.SidebarOpen)
            {
                Resources["OTD_SectionEmphasizedMargin"] = _originalMargin;
                Resources["OTD_SectionEmphasizedMargin1st"] = _originalMargin1st;
            }
        });
    }
}
