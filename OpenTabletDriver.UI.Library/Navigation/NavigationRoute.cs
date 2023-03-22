namespace OpenTabletDriver.UI.Navigation;

public record NavigationRoute(
    string Route,
    Type ControlType,
    Func<object, object>? DataContextConverter = null
);
