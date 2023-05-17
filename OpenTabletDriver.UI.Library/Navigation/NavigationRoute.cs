using Microsoft.Extensions.DependencyInjection;

namespace OpenTabletDriver.UI.Navigation;

public record NavigationRoute(
    string? Host,
    string Name,
    Type ObjectType,
    Type? ViewType
);
