using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.ViewModels;

public class UISettingsViewModel : NavigationViewModelBase
{
    public override string PageName => "Settings";

    public UISettingsViewModel(IUISettingsProvider settingsProvider)
    {
        settingsProvider.WhenLoaded(Initialize);
    }

    private void Initialize(IUISettingsProvider provider, UISettings settings)
    {

    }
}
