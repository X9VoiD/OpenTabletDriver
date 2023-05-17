using System.Diagnostics;
using static OpenTabletDriver.UI.TraceUtility;

namespace OpenTabletDriver.UI.ViewModels;

public abstract class ActivatableViewModelBase : ViewModelBase
{
    private Action<CompositeDisposable>? _whenActivatedAction;
    private CompositeDisposable? _whenActivatedDisposables;
    private bool _isActivated;

    public event EventHandler? Activated;
    public event EventHandler? Deactivated;

    public virtual void OnActivated()
    {
        PrintTrace(this, "Activated");
        Debug.Assert(!_isActivated);
        _isActivated = true;
        InvokeActivated();
        Activated?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnDeactivated()
    {
        PrintTrace(this, "Deactivated");
        Debug.Assert(_isActivated);
        _isActivated = false;
        DisposeObjects();
        Deactivated?.Invoke(this, EventArgs.Empty);
    }

    protected void WhenActivated(Action<CompositeDisposable> whenNavigatedTo)
    {
        _whenActivatedAction = whenNavigatedTo;

        // Dispose if there's some disposables left over
        DisposeObjects();
        // Invoke if we're currently navigated to
        InvokeActivated();
    }

    private void InvokeActivated()
    {
        if (_whenActivatedAction is not null && _isActivated)
        {
            PrintTrace(this, "Invoking activation action");
            _whenActivatedDisposables = new CompositeDisposable();
            _whenActivatedAction(_whenActivatedDisposables);
        }
    }

    private void DisposeObjects()
    {
        var whenNavigatedToDisposables = Interlocked.Exchange(ref _whenActivatedDisposables, null);
        if (whenNavigatedToDisposables is not null)
        {
            PrintTrace(this, "Disposing activation action");
            whenNavigatedToDisposables.Dispose();
        }
    }
}
