namespace OpenTabletDriver.UI.Navigation;

public interface INavigator
{
    void Back();
    void BackToRoot();
    void Next(string page);
    void NextAsRoot(string page);
}
