using System;
using Moq;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Tests.Navigation;

public class NavigatorTests
{
    [Fact]
    public void DelegatesToService()
    {
        // Navigator should delegate its functionality to the navigation service.
        WithCleanNavigationService((mock, nav) =>
        {
            nav.Back();
            mock.Verify(m => m.Back(), Times.Once);
        });

        WithCleanNavigationService((mock, nav) =>
        {
            nav.BackToRoot();
            mock.Verify(m => m.BackToRoot(), Times.Once);
        });

        WithCleanNavigationService((mock, nav) =>
        {
            nav.Next("test");
            mock.Verify(m => m.Next("test"), Times.Once);
        });

        WithCleanNavigationService((mock, nav) =>
        {
            nav.NextAsRoot("test");
            mock.Verify(m => m.NextAsRoot("test"), Times.Once);
        });
    }

    private static void WithCleanNavigationService(Action<Mock<INavigationService>, Navigator> action)
    {
        var mockNavServ = new Mock<INavigationService>();
        var nav = new Navigator(mockNavServ.Object);
        action(mockNavServ, nav);
    }
}
