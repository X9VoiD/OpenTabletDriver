namespace OpenTabletDriver.UI.NavigationV2;

public class NavigatorFactory : INavigatorFactory
{
    private readonly IServiceProvider _provider;
    private readonly NavigationRoute[] _staticRoutes;

    public NavigatorFactory(IServiceProvider provider, IEnumerable<NavigationRoute> routes)
    {
        _provider = provider;
        _staticRoutes = routes.ToArray();
    }

    public INavigator GetOrCreate(string navHostName)
    {
        var routes = _staticRoutes.Where(r => r.Host is null || r.Host == navHostName);
        var routeCollection = new NavigationRouteCollection(routes);
        return new Navigator(_provider, routeCollection);
    }
}
