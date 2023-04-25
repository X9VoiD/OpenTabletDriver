using Microsoft.Extensions.DependencyInjection;

namespace OpenTabletDriver.UI.NavigationV2;

public record NavigationRoute(
    string? Host,
    string Name,
    Type ObjectType,
    Type? ViewType
);
