using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTabletDriver.UI.Navigation;

public static class NavigationExtensions
{
    public static IServiceCollection UseNavigation<TNav>(this IServiceCollection services)
        where TNav : class, INavigationService
    {
        services.AddSingleton<INavigationService, TNav>();
        services.AddSingleton<INavigator, Navigator>();
        services.AddTransient<NavigationValueConverter>();
        return services;
    }

    public static IServiceCollection AddTransientNavigationRoute<T>(
        this IServiceCollection services,
        string route,
        Func<object, object>? dataContextConverter = null)
            where T : Control
    {
        services.AddTransient<T>();
        services.AddSingleton(new NavigationRoute(route, typeof(T), dataContextConverter));
        return services;
    }

    public static IServiceCollection AddSingletonNavigationRoute<T>(
        this IServiceCollection services,
        string route,
        Func<object, object>? dataContextConverter = null)
            where T : Control
    {
        services.AddSingleton<T>();
        services.AddSingleton(new NavigationRoute(route, typeof(T), dataContextConverter));
        return services;
    }
}
