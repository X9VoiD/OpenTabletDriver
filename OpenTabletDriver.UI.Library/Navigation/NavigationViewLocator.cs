using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace OpenTabletDriver.UI.Navigation;

public class NavigationViewLocator : IDataTemplate
{
    private readonly ReadOnlyCollection<NavigationRoute> _routes;

    public NavigationViewLocator(ReadOnlyCollection<NavigationRoute> routes)
    {
        _routes = routes;
    }

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var paramType = param.GetType();

        var route = _routes.FirstOrDefault(r => r.ObjectType == paramType);
        if (route?.ViewType is null)
            return null;

        var view = Activator.CreateInstance(route.ViewType) as Control;
        return view;
    }

    public bool Match(object? data)
    {
        return data is NavigationViewModelBase;
    }
}
