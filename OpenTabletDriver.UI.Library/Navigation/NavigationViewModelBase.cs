using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using static OpenTabletDriver.UI.TraceUtility;

namespace OpenTabletDriver.UI.Navigation;

public abstract class NavigationViewModelBase : ObservableObject
{
    private Action<CompositeDisposable>? _whenNavigatedToAction;
    private CompositeDisposable? _whenNavigatedToDisposables;
    private bool _isCurrentNavigation;

    public abstract string PageName { get; }
    public virtual bool ShowPageName => true;

    public event EventHandler? NavigatedTo;
    public event EventHandler? NavigatedFrom;

    public virtual void OnNavigatedTo()
    {
        PrintTrace(this, "Navigated to");
        Debug.Assert(!_isCurrentNavigation);
        _isCurrentNavigation = true;
        InvokeWhenNavigatedTo();
        NavigatedTo?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnNavigatedFrom()
    {
        PrintTrace(this, "Navigated from");
        Debug.Assert(_isCurrentNavigation);
        _isCurrentNavigation = false;
        DisposeObjects();
        NavigatedFrom?.Invoke(this, EventArgs.Empty);
    }

    protected void WhenNavigatedTo(Action<CompositeDisposable> whenNavigatedTo)
    {
        var whenNavigatedToAction = Interlocked.Exchange(ref _whenNavigatedToAction, whenNavigatedTo);

        // Dispose if there's some disposables left over
        DisposeObjects();
        // Invoke if we're currently navigated to
        InvokeWhenNavigatedTo();
    }

    private void InvokeWhenNavigatedTo()
    {
        if (_whenNavigatedToAction is not null && _isCurrentNavigation)
        {
            PrintTrace(this, "Invoking navigation action");
            _whenNavigatedToDisposables = new CompositeDisposable();
            _whenNavigatedToAction(_whenNavigatedToDisposables);
        }
    }

    private void DisposeObjects()
    {
        var whenNavigatedToDisposables = Interlocked.Exchange(ref _whenNavigatedToDisposables, null);
        if (whenNavigatedToDisposables is not null)
        {
            PrintTrace(this, "Disposing navigation action");
            whenNavigatedToDisposables.Dispose();
        }
    }

}
