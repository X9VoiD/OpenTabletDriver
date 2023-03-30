using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenTabletDriver.UI.Navigation;

public abstract class NavigationViewModelBase : ObservableObject
{
    public abstract string PageName { get; }

    public virtual void OnNavigatedTo()
    {
    }

    public virtual void OnNavigatedFrom()
    {
    }
}
