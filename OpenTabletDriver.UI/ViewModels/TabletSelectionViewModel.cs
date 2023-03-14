using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenTabletDriver.UI.ViewModels;

public partial class TabletSelectionViewModel : ViewModelBase
{
    [ObservableProperty]
    private TabletViewModel? _selectedTablet;
}
