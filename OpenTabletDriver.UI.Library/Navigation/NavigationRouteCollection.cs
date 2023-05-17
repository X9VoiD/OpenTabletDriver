using System.Collections.ObjectModel;

namespace OpenTabletDriver.UI.Navigation;

public class NavigationRouteCollection : ObservableCollection<NavigationRoute>
{
    public NavigationRouteCollection()
    {
    }

    public NavigationRouteCollection(IEnumerable<NavigationRoute> routes) : base(routes)
    {
    }
}
