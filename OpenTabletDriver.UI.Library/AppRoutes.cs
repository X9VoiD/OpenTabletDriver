using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI;

public static class AppRoutes
{
    public static IServiceCollection AddApplicationRoutes(this IServiceCollection services)
    {
        return services
            .AddSingleton<MainWindowViewModel>()
            .AddNavigationRoute<PlaceholderViewModel, PlaceholderView>("MainPlaceholder");
    }
}
