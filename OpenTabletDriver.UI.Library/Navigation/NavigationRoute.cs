namespace OpenTabletDriver.UI.Navigation;

public record NavigationRoute(
    string Route,
    Type ObjectType,
    Type? ViewType
);
