namespace OpenTabletDriver.UI.ViewModels
{
    public class BindingsViewModel : ViewModelBase
    {
        private readonly TabletViewModel _tabletViewModel;

        public BindingsViewModel(TabletViewModel tabletViewModel)
        {
            _tabletViewModel = tabletViewModel;
        }
    }
}
