namespace OpenTabletDriver.UI.ViewModels
{
    public class FiltersViewModel : ViewModelBase
    {
        private readonly TabletViewModel _tabletViewModel;

        public FiltersViewModel(TabletViewModel tabletViewModel)
        {
            _tabletViewModel = tabletViewModel;
        }
    }
}
