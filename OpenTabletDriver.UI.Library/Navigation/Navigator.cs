namespace OpenTabletDriver.UI.Navigation;

public sealed class Navigator : INavigator
{
    private readonly INavigationService _navigationService;

    public Navigator(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public string? CurrentPage => _navigationService.CurrentPage;

    public void Back()
    {
        _navigationService.Back();
    }

    public void BackToRoot()
    {
        _navigationService.BackToRoot();
    }

    public void Next(string page)
    {
        _navigationService.Next(page);
    }

    public void NextAsRoot(string page)
    {
        _navigationService.NextAsRoot(page);
    }
}
