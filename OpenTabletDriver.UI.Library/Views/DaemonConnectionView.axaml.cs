﻿using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
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

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is DaemonConnectionViewModel vm)
        {
            vm.QolHintText.CollectionChanged += HandleItemsChanged;
            vm.ConnectCommand.HandleProperty(
                nameof(IAsyncRelayCommand.IsRunning),
                c => c.IsRunning,
                (c, running) =>
                {
                    this.Cursor = running
                        ? new Cursor(StandardCursorType.Wait)
                        : new Cursor(StandardCursorType.Arrow);
                }
            );
        }
        base.OnDataContextChanged(e);
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

