namespace OpenTabletDriver.UI.Navigation;

public class NavigatedEventArgs : EventArgs
{
    public NavigatedEventArgs(string? prev, string curr, NavigationKind kind)
    {
        Kind = kind;
        PreviousPage = prev;
        Page = curr;
    }

    public NavigationKind Kind { get; }
    public string? PreviousPage { get; }
    public string Page { get; }
}

public class NavigatingEventArgs : EventArgs
{
    public NavigatingEventArgs(string? prev, string curr, NavigationKind kind)
    {
        Kind = kind;
        PreviousPage = prev;
        Page = curr;
    }

    public NavigationKind Kind { get; }
    public string? PreviousPage { get; }
    public string Page { get; }
    public bool Cancel { get; set; }
}
