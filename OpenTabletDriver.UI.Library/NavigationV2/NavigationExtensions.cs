using System.Diagnostics;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTabletDriver.UI.NavigationV2;

public static class NavigationExtensions
{
    public static IServiceCollection UseNavigation<TNav>(this IServiceCollection services)
        where TNav : class, INavigatorFactory
    {
        return services.AddSingleton<INavigatorFactory, TNav>();
    }

    public static IServiceCollection AddTransientRoute<TObject>(this IServiceCollection services, string route)
    {
        return services.AddRoute(route, null, typeof(TObject), null, ServiceLifetime.Transient);
    }

    public static IServiceCollection AddTransientRoute<TObject, TView>(this IServiceCollection services, string route)
        where TView : Control
    {
        return services.AddRoute(route, null, typeof(TObject), typeof(TView), ServiceLifetime.Transient);
    }

    public static IServiceCollection AddTransientRoute<TObject>(this IServiceCollection services, string navHostName, string route)
    {
        return services.AddRoute(route, navHostName, typeof(TObject), null, ServiceLifetime.Transient);
    }

    public static IServiceCollection AddTransientRoute<TObject, TView>(this IServiceCollection services, string navHostName, string route)
        where TView : Control
    {
        return services.AddRoute(route, navHostName, typeof(TObject), typeof(TView), ServiceLifetime.Transient);
    }

    public static IServiceCollection AddSingletonRoute<TObject>(this IServiceCollection services, string route)
    {
        return services.AddRoute(route, null, typeof(TObject), null, ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddSingletonRoute<TObject, TView>(this IServiceCollection services, string route)
        where TView : Control
    {
        return services.AddRoute(route, null, typeof(TObject), typeof(TView), ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddSingletonRoute<TObject>(this IServiceCollection services, string navHostName, string route)
    {
        return services.AddRoute(route, navHostName, typeof(TObject), null, ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddSingletonRoute<TObject, TView>(this IServiceCollection services, string navHostName, string route)
        where TView : Control
    {
        return services.AddRoute(route, navHostName, typeof(TObject), typeof(TView), ServiceLifetime.Singleton);
    }

    private static IServiceCollection AddRoute(
        this IServiceCollection services,
        string route,
        string? navHostName,
        Type objectType,
        Type? viewType,
        ServiceLifetime lifetime)
    {
        Debug.Assert(viewType is null || viewType.IsAssignableTo(typeof(Control)));
        services.Add(ServiceDescriptor.Describe(objectType, objectType, lifetime));
        services.AddSingleton(new NavigationRoute(navHostName, route, objectType, viewType));
        return services;
    }
}
