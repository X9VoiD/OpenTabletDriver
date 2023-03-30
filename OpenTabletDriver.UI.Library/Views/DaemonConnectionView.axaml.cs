using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Views;

public partial class DaemonConnectionView : UserControl
{
    public DaemonConnectionView()
    {
        InitializeComponent();
        NavigationMixin.Attach(this);
        Window.SizeChangedEvent.AddClassHandler<DaemonConnectionView>(HandleClientSizeChanged);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var vm = (DaemonConnectionViewModel)DataContext!;
        vm.QolHintText.CollectionChanged += HandleItemsChanged;
        base.OnAttachedToVisualTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var vm = (DaemonConnectionViewModel)DataContext!;
        vm.QolHintText.CollectionChanged -= HandleItemsChanged;
        base.OnDetachedFromVisualTree(e);
    }

    private void HandleItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            Scroller.ScrollToEnd();
        }
    }

    private static void HandleClientSizeChanged(DaemonConnectionView view, SizeChangedEventArgs args)
    {
        view.Scroller.ScrollToEnd();
    }
}

