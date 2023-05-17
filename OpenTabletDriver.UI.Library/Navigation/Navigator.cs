using System.Diagnostics;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTabletDriver.UI.Navigation;

public class Navigator : INavigator
{
    private readonly Stack<object> _navStack = new(5);
    private readonly IServiceProvider _serviceProvider;

    private CancellableNavigationEventData? _navigating;

    public object? Current => _navStack.TryPeek(out var curr) ? curr : null;
    public bool CanGoBack { get; private set; }
    public NavigationRouteCollection Routes { get; }

    public event EventHandler<CancellableNavigationEventData>? Navigating;
    public event EventHandler<NavigationEventData>? Navigated;

    public Navigator(IServiceProvider serviceProvider, NavigationRouteCollection routes)
    {
        _serviceProvider = serviceProvider;
        Routes = routes;
    }

    public void Push(object routeObject, bool asRoot = false)
    {
        ArgumentNullException.ThrowIfNull(routeObject);
        var kind = asRoot ? NavigationKind.PushAsRoot : NavigationKind.Push;
        Navigate(kind, routeObject);
    }

    public void Push(string routeName, bool asRoot = false)
    {
        var route = Routes.FirstOrDefault(r => r.Name == routeName);
        if (route is null)
        {
            Debug.WriteLine($"Cannot find route descriptor for route name '{routeName}'");
            var navigation404 = Routes.FirstOrDefault(r => r.Name == "404");

            var routeObject = navigation404 is not null
                ? CreateRouteObject(navigation404)
                : throw new InvalidOperationException($"Cannot find route descriptor for route name '{routeName}' and no 404 route is registered");

            Push(routeObject, asRoot);
        }
        else
        {
            Push(CreateRouteObject(route), asRoot);
        }
    }

    public void Pop(bool toRoot = false)
    {
        if (!toRoot && !CanGoBack)
            throw new InvalidOperationException($"Already root");
        if (toRoot && _navStack.Count == 0)
            throw new InvalidOperationException("Empty navigation stack");
        if (toRoot && _navStack.Count == 1)
            return; // already root
        var kind = toRoot ? NavigationKind.PopToRoot : NavigationKind.Pop;
        Navigate(kind, null);
    }

    private void Navigate(NavigationKind kind, object? next)
    {
        if (_navigating is not null)
        {
            // If a navigation is already in progress, cancel it.
            // This happens when a navigation is triggered from Navigating event handler.
            TraceUtility.PrintTrace(this, $"Cancelling navigation from {_navigating.Current} to {next}");
            _navigating.Cancel = true;
        }

        var prev = Current;
        var curr = kind switch
        {
            NavigationKind.Pop => _navStack.ElementAt(1), // curr will be the page before the top of the stack
            NavigationKind.PopToRoot => _navStack.Last(), // curr will be the root page
            _ => next != null ? next : throw new ArgumentNullException(nameof(next))
        };

        var eventArg = new CancellableNavigationEventData(kind, prev, curr);
        _navigating = eventArg;
        Navigating?.Invoke(this, eventArg);
        if (!eventArg.Cancel)
        {
            switch (kind)
            {
                case NavigationKind.Push:
                    _navStack.Push(next!);
                    break;
                case NavigationKind.Pop:
                    _navStack.Pop();
                    break;
                case NavigationKind.PushAsRoot:
                    _navStack.Clear();
                    _navStack.Push(next!);
                    break;
                case NavigationKind.PopToRoot:
                    _navStack.Clear();
                    _navStack.Push(curr);
                    break;
            }

            _navigating = null; // at this point, navigation is "complete"
            CanGoBack = _navStack.Count > 1;
            Navigated?.Invoke(this, new NavigationEventData(kind, prev, curr));
        }
    }

    private object CreateRouteObject(NavigationRoute route)
    {
        if (route.ViewType is not null)
        {
            var view = (Control)Activator.CreateInstance(route.ViewType)!;
            view.DataContext = _serviceProvider.GetRequiredService(route.ObjectType);
            return view;
        }
        else
        {
            return _serviceProvider.GetRequiredService(route.ObjectType);
        }
    }
}
