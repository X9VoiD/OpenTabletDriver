using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenTabletDriver.UI.Navigation;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly Stack<string> _navStack = new(5);

    [ObservableProperty]
    private bool _canGoBack;

    public string? CurrentPage => _navStack.TryPeek(out var page) ? page : null;
    public event EventHandler<NavigatedEventArgs>? Navigated;

    public void Back()
    {
        if (!CanGoBack)
            throw new InvalidOperationException($"Cannot go back from the root page: '{CurrentPage}'");

        var prev = _navStack.Pop();

        // Can only go back if there's more than one page in the stack to ensure
        // there's always a page to show
        CanGoBack = _navStack.Count > 1;
        OnNavigated(new NavigatedEventArgs(prev, _navStack.Peek(), NavigationKind.Back));
    }

    public void BackToRoot()
    {
        // Clear the stack and add the root page
        // If we're already at the root, don't do anything
        string? prev = null;
        if (_navStack.Count == 0)
            throw new InvalidOperationException("Cannot go back to root from an empty navigation stack");
        if (_navStack.Count == 1)
            return; // already root
        while (_navStack.Count > 1)
            prev = _navStack.Pop();
        CanGoBack = false;
        OnNavigated(new NavigatedEventArgs(prev, _navStack.Peek(), NavigationKind.BackToRoot));
    }

    public void Next(string page)
    {
        var prev = CurrentPage;
        _navStack.Push(page);
        CanGoBack = _navStack.Count > 1; // see Back()
        OnNavigated(new NavigatedEventArgs(prev, page, NavigationKind.Next));
    }

    public void NextAsRoot(string page)
    {
        var prev = CurrentPage;
        _navStack.Clear();
        _navStack.Push(page);
        CanGoBack = false;
        OnNavigated(new NavigatedEventArgs(prev, page, NavigationKind.NextAsRoot));
    }

    protected virtual void OnNavigated(NavigatedEventArgs e)
    {
        Navigated?.Invoke(this, e);
    }
}
