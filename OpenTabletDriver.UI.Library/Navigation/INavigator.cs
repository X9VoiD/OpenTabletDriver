namespace OpenTabletDriver.UI.Navigation;

public interface INavigator
{
    string? CurrentPage { get; }
    void Back();
    void BackToRoot();
    void Next(string page);
    void NextAsRoot(string page);
}
