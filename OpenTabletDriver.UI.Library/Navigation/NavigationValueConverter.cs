using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTabletDriver.UI.Navigation;

/// <summary>
/// Converts a string to a <see cref="Control"/> based on the <see cref="NavigationRoute"/>s.
/// </summary>
public class NavigationValueConverter : IValueConverter
{
    private readonly IServiceProvider _provider;
    private readonly ReadOnlyCollection<NavigationRoute> _routes;

    public NavigationValueConverter(IServiceProvider provider, ReadOnlyCollection<NavigationRoute> routes)
    {
        _provider = provider;
        _routes = routes;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return BindingNotification.Null;

        if (value is not string page)
        {
            Debug.Fail($"Cannot convert value of type '{value?.GetType().Name}' to a navigation route.");
            return BindingNotification.Null;
        }

        var route = _routes.FirstOrDefault(r => r.Route == page);
        if (route is null)
        {
            Debug.WriteLine($"Cannot find navigation route for page '{page}'.");
            var navigation404 = _routes.FirstOrDefault(r => r.Route == "404");

            return navigation404 is not null
                ? _provider.GetRequiredService(navigation404.ObjectType)
                : BindingNotification.Null;
        }

        return _provider.GetRequiredService(route.ObjectType);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new BindingNotification(
            new NotSupportedException("ConvertBack is not supported for NavigationValueConverter"),
            BindingErrorType.Error
        );
    }
}
