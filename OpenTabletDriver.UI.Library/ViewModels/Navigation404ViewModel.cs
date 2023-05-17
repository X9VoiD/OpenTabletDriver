using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Navigation;

public sealed class Navigation404ViewModel : ActivatableViewModelBase
{
    public string Message { get; }

    public Navigation404ViewModel()
    {
        Message = $"Oops! The page you're looking for doesn't exist yet.";
    }
}
