namespace OpenTabletDriver.UI.NavigationV2;

public interface INavigatorFactory
{
    INavigator GetOrCreate(string navHostName);
}
