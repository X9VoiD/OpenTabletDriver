namespace OpenTabletDriver.UI.Navigation;

public sealed class Navigation404ViewModel : NavigationViewModelBase
{
    public override string PageName => "404";

    public string Message { get; }

    public Navigation404ViewModel(INavigator navigator)
    {
        Message = $"The page '{navigator.CurrentPage}' does not exist.";
    }
}
