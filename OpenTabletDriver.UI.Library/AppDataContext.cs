using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI;

public partial class AppDataContext : ObservableObject
{
    public MainWindowViewModel MainWindowViewModel { get; }

    public AppDataContext(MainWindowViewModel mainWindowViewModel)
    {
        MainWindowViewModel = mainWindowViewModel;
    }
}
