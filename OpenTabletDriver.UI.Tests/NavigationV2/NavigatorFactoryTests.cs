using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.UI.NavigationV2;

namespace OpenTabletDriver.UI.Tests.NavigationV2;

public class NavigatorFactoryTests
{
    [Fact]
    public void TestGetOrCreate()
    {
        var serviceProvider = new ServiceCollection()
            .UseNavigation<NavigatorFactory>()
            .BuildServiceProvider();
        var navFactory = serviceProvider.GetRequiredService<INavigatorFactory>();

        var nav = navFactory.GetOrCreate("test");
        navFactory.GetOrCreate("test").Should().BeSameAs(nav, "NavigatorFactory should return the same navigator for the same host name");
        navFactory.GetOrCreate("test2").Should().NotBeSameAs(nav, "NavigatorFactory should return a different navigator for a different host name");
    }
}
