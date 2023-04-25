using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenTabletDriver.UI.Navigation;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly Stack<string> _navStack = new(5);

    [ObservableProperty]
    private bool _canGoBack;

    public string? CurrentPage => _navStack.TryPeek(out var page) ? page : null;

    public event EventHandler<NavigatedEventArgs>? Navigated;
    public event EventHandler<NavigatingEventArgs>? Navigating;

    public void Back()
    {
        if (!CanGoBack)
            throw new InvalidOperationException($"Cannot go back from the root page: '{CurrentPage}'");

        Navigate(NavigationKind.Back);
    }

    public void BackToRoot()
    {
        // Clear the stack and add the root page
        // If we're already at the root, don't do anything
        if (_navStack.Count == 0)
            throw new InvalidOperationException("Cannot go back to root from an empty navigation stack");
        if (_navStack.Count == 1)
            return; // already root

        Navigate(NavigationKind.BackToRoot);
    }

    public void Next(string page)
    {
        Navigate(NavigationKind.Next, page);
    }

    public void NextAsRoot(string page)
    {
        Navigate(NavigationKind.NextAsRoot, page);
    }

    private void Navigate(NavigationKind kind, string? nextPage = null, NavigationViewModelBase? navViewModel = null)
    {
        var prev = CurrentPage;
        var curr = kind switch
        {
            NavigationKind.Back => _navStack.ElementAt(1), // curr will be the page before the top of the stack
            NavigationKind.BackToRoot => _navStack.Last(), // curr will be the root page
            _ => nextPage != null ? nextPage : throw new ArgumentNullException(nameof(nextPage))
        };

        var naving = new NavigatingEventArgs(prev, curr, kind);
        Navigating?.Invoke(this, naving);
        if (!naving.Cancel)
        {
            switch (kind)
            {
                case NavigationKind.Back:
                    _navStack.Pop();
                    break;
                case NavigationKind.BackToRoot:
                case NavigationKind.NextAsRoot:
                    _navStack.Clear();
                    _navStack.Push(curr);
                    break;
                case NavigationKind.Next:
                    _navStack.Push(curr);
                    break;
            }

            // Can only go back if there's more than one page in the stack to ensure
            // there's always a page to show
            CanGoBack = _navStack.Count > 1;
            Navigated?.Invoke(this, new NavigatedEventArgs(prev, curr, kind));
        }
    }

    private delegate string NavigatingDelegate();
}
