using System.Collections.Immutable;
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
    private readonly ImmutableArray<NavigationRoute> _routes;

    public NavigationValueConverter(IServiceProvider provider, IEnumerable<NavigationRoute> routes)
    {
        _provider = provider;
        _routes = routes.ToImmutableArray();
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
            Debug.WriteLine("Cannot find navigation route for page '{page}'.");
            return BindingNotification.Null;
        }

        var control = (Control)_provider.GetRequiredService(route.ControlType);

        var mainDataContext = App.Current?.DataContext;
        Debug.Assert(mainDataContext != null, "App.Current.DataContext is null");

        // TODO: verify that this is working as intended
        var dataContext = route.DataContextConverter?.Invoke(mainDataContext);
        control.DataContext = dataContext ?? mainDataContext;
        return control;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new BindingNotification(
            new NotSupportedException("ConvertBack is not supported for NavigationValueConverter"),
            BindingErrorType.Error
        );
    }
}
