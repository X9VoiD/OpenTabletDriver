using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTabletDriver.UI.Navigation;

public static class NavigationExtensions
{
    public static IServiceCollection UseNavigation<TNav>(this IServiceCollection services)
        where TNav : class, INavigationService
    {
        return services
            .AddSingleton<INavigationService, TNav>()
            .AddSingleton<INavigator, Navigator>()
            .AddSingleton<ReadOnlyCollection<NavigationRoute>>(sp =>
                new(sp.GetRequiredService<IEnumerable<NavigationRoute>>().ToArray()))
            .AddTransient<NavigationValueConverter>()
            .AddTransient<NavigationViewLocator>()
            .AddStartupJob<NavigationStartup>();
    }

    public static IServiceCollection AddNavigationRoute<T>(this IServiceCollection services, string route)
        where T : class
    {
        return AddRoute(services, route, typeof(T), null, ServiceLifetime.Transient);
    }

    public static IServiceCollection AddNavigationRoute<TObject, TView>(this IServiceCollection services, string route)
        where TObject : class
        where TView : Control
    {
        return AddRoute(services, route, typeof(TObject), typeof(TView), ServiceLifetime.Transient);
    }

    public static IServiceCollection AddSingletonNavigationRoute<T>(this IServiceCollection services, string route)
        where T : class
    {
        return AddRoute(services, route, typeof(T), null, ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddSingletonNavigationRoute<TObject, TView>(this IServiceCollection services, string route)
        where TObject : class
        where TView : Control
    {
        return AddRoute(services, route, typeof(TObject), typeof(TView), ServiceLifetime.Singleton);
    }

    private static IServiceCollection AddRoute(
        this IServiceCollection services,
        string route,
        Type objectType,
        Type? viewType,
        ServiceLifetime lifetime)
    {
        services.Add(ServiceDescriptor.Describe(objectType, objectType, lifetime));
        services.AddSingleton(new NavigationRoute(route, objectType, viewType));
        return services;
    }

    private class NavigationStartup : IStartupJob
    {
        private readonly INavigationService _navigationService;
        private readonly NavigationValueConverter _converter;
        private readonly NavigationViewLocator _viewLocator;

        public NavigationStartup(INavigationService navigationService, NavigationValueConverter converter,
            NavigationViewLocator viewLocator)
        {
            _navigationService = navigationService;
            _converter = converter;
            _viewLocator = viewLocator;
        }

        public void Run()
        {
            Application.Current!.Resources.Add("NavigationService", _navigationService);
            Application.Current!.Resources.Add("NavigationValueConverter", _converter);
            Application.Current!.DataTemplates.Insert(0, _viewLocator);
        }
    }
}
