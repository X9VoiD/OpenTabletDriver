using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Events;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Tests.Navigation;

public class NavigatorTests
{
    #region Raw Navigation

    [Fact]
    public void CurrentIsNullByDefault()
    {
        var nav = CreateNavigator();
        nav.Current.Should().BeNull();
    }

    [Fact]
    public void Push()
    {
        var nav = CreateNavigator();
        using var navEvMon = nav.Monitor();

        nav.Push("test" as object); // cast to ensure it uses the right overload
        nav.Current.Should().Be("test");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigationEvents(nav, null, "test", NavigationKind.Push)
            .ToArray();
    }

    [Fact]
    public void PushAfterPush()
    {
        var nav = CreateNavigator();
        using var navEvMon = nav.Monitor();

        nav.Push("test" as object);
        nav.Push("test2" as object);

        nav.CanGoBack.Should().BeTrue();
        nav.Current.Should().Be("test2");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigationEvents(nav, null, "test", NavigationKind.Push)
            .AddStandardNavigationEvents(nav, "test", "test2", NavigationKind.Push)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void Pop()
    {
        var nav = CreateNavigator();
        using var navEvMon = nav.Monitor();

        nav.CanGoBack.Should().BeFalse();
        nav.Invoking(n => n.Pop())
            .Should().Throw<InvalidOperationException>("Navigator cannot go back when it has no history");

        nav.Push("test" as object);

        nav.CanGoBack.Should().BeFalse();
        nav.Invoking(n => n.Pop())
            .Should().Throw<InvalidOperationException>();

        nav.Push("test2" as object);

        nav.CanGoBack.Should().BeTrue();
        nav.Current.Should().Be("test2");
        nav.Invoking(n => n.Pop())
            .Should().NotThrow();

        nav.CanGoBack.Should().BeFalse();
        nav.Current.Should().Be("test");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigationEvents(nav, null, "test", NavigationKind.Push)
            .AddStandardNavigationEvents(nav, "test", "test2", NavigationKind.Push)
            .AddStandardNavigationEvents(nav, "test2", "test", NavigationKind.Pop)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void PopAfterThreeNexts()
    {
        var nav = CreateNavigator();
        nav.Push("test" as object);
        nav.Push("test2" as object);
        nav.Push("test3" as object);

        // Monitor late so we don't get the initial events
        using var navEvMon = nav.Monitor();

        nav.Pop();

        nav.CanGoBack.Should().BeTrue();
        nav.Current.Should().Be("test2");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigationEvents(nav, "test3", "test2", NavigationKind.Pop)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void PushAsRoot()
    {
        var nav = CreateNavigator();
        using var navEvMon = nav.Monitor();

        nav.Push("test" as object, asRoot: true);

        nav.CanGoBack.Should().BeFalse();
        nav.Current.Should().Be("test");

        nav.Push("test2" as object);
        nav.Push("test3" as object, asRoot: true);

        nav.CanGoBack.Should().BeFalse();
        nav.Current.Should().Be("test3");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigationEvents(nav, null, "test", NavigationKind.PushAsRoot)
            .AddStandardNavigationEvents(nav, "test", "test2", NavigationKind.Push)
            .AddStandardNavigationEvents(nav, "test2", "test3", NavigationKind.PushAsRoot)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void BackToRoot()
    {
        var nav = CreateNavigator();

        nav.Invoking(n => n.Pop(toRoot: true))
            .Should().Throw<InvalidOperationException>("Navigator cannot go back to root when it has no history");

        nav.Push("test" as object);

        using (var scopedNavEvMon = nav.Monitor())
        {
            nav.Pop(toRoot: true);
            scopedNavEvMon.OccurredEvents.Should().BeEmpty();
        }

        nav.Push("test2" as object);
        nav.Push("test3" as object);

        using (var scopedNavEvMon = nav.Monitor())
        {
            nav.Pop(toRoot: true);
            nav.CanGoBack.Should().BeFalse();
            nav.Current.Should().Be("test");

            var expectedEvents = new List<OccurredEvent>()
                .AddStandardNavigationEvents(nav, "test3", "test", NavigationKind.PopToRoot)
                .ToArray();
        }
    }

    #endregion
    #region Routed Navigation

    [Fact]
    public void SingletonRoute()
    {
        var nav = CreateNavigator(sc => sc
            .AddSingletonRoute<DummyObject>("test_route")
            .AddSingletonRoute<DummyObject2>("other_nav", "test_route2"));

        nav.Push("test_route");
        nav.Current.Should().BeOfType<DummyObject>();

        var dummy = nav.Current as DummyObject;

        nav.Push("something" as object);
        nav.Pop();
        nav.Current.Should().Be(dummy);

        nav.Push("new_root" as object, asRoot: true);
        nav.Push("test_route");
        nav.Current.Should().Be(dummy, "Singleton routes should always return the same instance");

        nav.Invoking(n => n.Push("test_route2", asRoot: true))
            .Should().Throw<InvalidOperationException>("'test_route2' is not a valid route for 'test_nav' host");
    }

    [Fact]
    public void TransientRoute()
    {
        var nav = CreateNavigator(sc => sc
            .AddTransientRoute<DummyObject>("test_route")
            .AddTransientRoute<DummyObject2>("other_nav", "test_route2"));

        nav.Push("test_route");
        nav.Current.Should().BeOfType<DummyObject>();

        var dummy = nav.Current as DummyObject;

        nav.Push("something" as object);
        nav.Pop();
        nav.Current.Should().Be(dummy, "Transient routes should return the same instance until popped out of the stack");

        nav.Push("new_root" as object, asRoot: true);
        nav.Push("test_route");
        nav.Current.Should().NotBe(dummy, "Transient routes should always return a new instance");

        nav.Invoking(n => n.Push("test_route2", asRoot: true))
            .Should().Throw<InvalidOperationException>("'test_route2' is not a valid route for 'test_nav' host");
    }

    #endregion

    // TODO: test cancelling behaviour

    private static INavigator CreateNavigator()
    {
        var navFactory = new ServiceCollection()
            .UseNavigation<NavigatorFactory>()
            .BuildServiceProvider()
            .GetRequiredService<INavigatorFactory>();

        return navFactory.GetOrCreate("test_nav");
    }

    private static INavigator CreateNavigator(Action<IServiceCollection> configureServices)
    {
        var serviceCollection = new ServiceCollection()
            .UseNavigation<NavigatorFactory>();

        configureServices(serviceCollection);

        var navFactory = serviceCollection
            .BuildServiceProvider()
            .GetRequiredService<INavigatorFactory>();

        return navFactory.GetOrCreate("test_nav");
    }

    private void AssertEvents(IMonitor<INavigator> navEvMon, params OccurredEvent[] expectedEvents)
    {
        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.Sequence) // already accounted for by WithStrictOrdering
            .Excluding(e => e.TimestampUtc));
    }

    private class DummyObject { }
    private class DummyObject2 { }
}

internal static class OccurredEventListExtensions
{
    public static List<OccurredEvent> AddStandardNavigationEvents(
        this List<OccurredEvent> events,
        INavigator navigator,
        object? prev,
        object? curr,
       NavigationKind kind)
    {
        events.AddRange(new OccurredEvent[]
        {
            new OccurredEvent
            {
                EventName = nameof(INavigator.Navigating),
                Parameters = new object[]
                {
                    navigator,
                    new CancellableNavigationEventData(kind, prev, curr)
                }
            },
            new OccurredEvent
            {
                EventName = nameof(INavigator.Navigated),
                Parameters = new object[]
                {
                    navigator,
                    new NavigationEventData(kind, prev, curr)
                }
            }
        });

        return events;
    }
}
