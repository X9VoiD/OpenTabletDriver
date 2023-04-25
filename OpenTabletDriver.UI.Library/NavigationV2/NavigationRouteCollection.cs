using System.Collections.ObjectModel;

namespace OpenTabletDriver.UI.NavigationV2;

public class NavigationRouteCollection : ObservableCollection<NavigationRoute>
{
    public NavigationRouteCollection()
    {
    }

    public NavigationRouteCollection(IEnumerable<NavigationRoute> routes) : base(routes)
    {
    }
}
