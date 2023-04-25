using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentAssertions;
using FluentAssertions.Events;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Tests.Navigation;

// TODO: test navigation cancels
public class NavigationServiceTests
{
    [Fact]
    public void CurrentPageIsNullByDefault()
    {
        var navServ = new NavigationService();
        navServ.CurrentPage.Should().BeNull();
    }

    [Fact]
    public void Next()
    {
        var navServ = new NavigationService();
        using var navEvMon = navServ.Monitor();

        navServ.Next("test");

        navServ.CanGoBack.Should().BeFalse();
        navServ.CurrentPage.Should().Be("test");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigation(navServ, null, "test", NavigationKind.Next)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void NextAfterNext()
    {
        var navServ = new NavigationService();
        using var navEvMon = navServ.Monitor();

        navServ.Next("test");
        navServ.Next("test2");

        navServ.CanGoBack.Should().BeTrue();
        navServ.CurrentPage.Should().Be("test2");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigation(navServ, null, "test", NavigationKind.Next)
            .AddStandardNavigation(navServ, "test", "test2", NavigationKind.Next, canGoBackChanged: true)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void BackWhenCanGoBackIsFalse()
    {
        var navServ = new NavigationService();

        navServ.Invoking(n => n.Back())
            .Should().Throw<InvalidOperationException>();

        navServ.Next("test");

        navServ.Invoking(n => n.Back())
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Back()
    {
        var navServ = new NavigationService();
        navServ.Next("test");
        navServ.Next("test2");

        // Monitor late so we don't have to deal with the events from Next
        using var navEvMon = navServ.Monitor();

        navServ.Back();

        navServ.CanGoBack.Should().BeFalse();
        navServ.CurrentPage.Should().Be("test");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigation(navServ, "test2", "test", NavigationKind.Back, canGoBackChanged: true)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void BackAfterThreeNexts()
    {
        var navServ = new NavigationService();
        navServ.Next("test");
        navServ.Next("test2");
        navServ.Next("test3");

        // Monitor late so we don't have to deal with the events from Next
        using var navEvMon = navServ.Monitor();

        navServ.Back();

        navServ.CanGoBack.Should().BeTrue();
        navServ.CurrentPage.Should().Be("test2");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigation(navServ, "test3", "test2", NavigationKind.Back)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void NextAsRootWhenEmpty()
    {
        var navServ = new NavigationService();
        using var navEvMon = navServ.Monitor();

        navServ.NextAsRoot("test");

        navServ.CanGoBack.Should().BeFalse();
        navServ.CurrentPage.Should().Be("test");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigation(navServ, null, "test", NavigationKind.NextAsRoot)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void NextAsRoot()
    {
        var navServ = new NavigationService();
        navServ.Next("test");
        navServ.Next("test2");

        // Monitor late so we don't have to deal with the events from Next
        using var navEvMon = navServ.Monitor();

        navServ.NextAsRoot("test3");

        navServ.CanGoBack.Should().BeFalse();
        navServ.CurrentPage.Should().Be("test3");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigation(navServ, "test2", "test3", NavigationKind.NextAsRoot, canGoBackChanged: true)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    [Fact]
    public void BackToRootWhenEmpty()
    {
        var navServ = new NavigationService();

        navServ.Invoking(n => n.BackToRoot())
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void BackToRoot()
    {
        var navServ = new NavigationService();

        navServ.Next("test");

        using (var scopedNavEvMon = navServ.Monitor())
        {
            navServ.BackToRoot();
            scopedNavEvMon.OccurredEvents.Should().BeEmpty();
        }

        navServ.Next("test2");

        using var navEvMon = navServ.Monitor();

        navServ.BackToRoot();

        navServ.CanGoBack.Should().BeFalse();
        navServ.CurrentPage.Should().Be("test");

        var expectedEvents = new List<OccurredEvent>()
            .AddStandardNavigation(navServ, "test2", "test", NavigationKind.BackToRoot, canGoBackChanged: true)
            .ToArray();

        AssertEvents(navEvMon, expectedEvents);
    }

    private void AssertEvents(IMonitor<NavigationService> navEvMon, params OccurredEvent[] expectedEvents)
    {
        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.Sequence) // already accounted for by WithStrictOrdering
            .Excluding(e => e.TimestampUtc));
    }
}

internal static class OccurredEventListExtensions
{
    public static List<OccurredEvent> AddStandardNavigation(
        this List<OccurredEvent> events,
        NavigationService navServ,
        string? prev,
        string curr,
        NavigationKind kind,
        bool canGoBackChanged = false)
    {
        if (canGoBackChanged)
        {

            // CanGoBack is changed after the Navigating event, so the sequence looks like:
            // - Navigating
            // - PropertyChanged (CanGoBack)
            // - Navigated
            // This is to only ever change CanGoBack once per navigation, no need to change
            // it twice in the event that the navigation is cancelled.
            events.AddRange(new OccurredEvent[]
            {
                new OccurredEvent
                {
                    EventName = nameof(NavigationService.Navigating),
                    Parameters = new object[]
                    {
                        navServ,
                        new NavigatingEventArgs(prev, curr, kind)
                    }
                },
                new OccurredEvent
                {
                    EventName = nameof(NavigationService.PropertyChanging),
                    Parameters = new object[]
                    {
                        navServ,
                        new PropertyChangingEventArgs(nameof(NavigationService.CanGoBack))
                    }
                },
                new OccurredEvent
                {
                    EventName = nameof(NavigationService.PropertyChanged),
                    Parameters = new object[]
                    {
                        navServ,
                        new PropertyChangedEventArgs(nameof(NavigationService.CanGoBack))
                    }
                },
                new OccurredEvent
                {
                    EventName = nameof(NavigationService.Navigated),
                    Parameters = new object[]
                    {
                        navServ,
                        new NavigatedEventArgs(prev, curr, kind)
                    }
                }
            });
        }
        else
        {
            events.AddRange(new OccurredEvent[]
            {
                new OccurredEvent
                {
                    EventName = nameof(NavigationService.Navigating),
                    Parameters = new object[]
                    {
                        navServ,
                        new NavigatingEventArgs(prev, curr, kind)
                    }
                },
                new OccurredEvent
                {
                    EventName = nameof(NavigationService.Navigated),
                    Parameters = new object[]
                    {
                        navServ,
                        new NavigatedEventArgs(prev, curr, kind)
                    }
                }
            });
        }

        return events;
    }
}
