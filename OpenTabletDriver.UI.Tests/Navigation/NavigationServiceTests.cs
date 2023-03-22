using System;
using System.ComponentModel;
using FluentAssertions;
using FluentAssertions.Events;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Tests.Navigation;

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

        var expectedEvents = new[]
        {
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 0,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs(null, "test", NavigationKind.Next)
                }
            }
        };

        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.TimestampUtc));
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

        var expectedEvents = new[]
        {
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 0,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs(null, "test", NavigationKind.Next)
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanging),
                Sequence = 1,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangingEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanged),
                Sequence = 2,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangedEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 3,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs("test", "test2", NavigationKind.Next)
                }
            }
        };

        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.TimestampUtc));
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

        var expectedEvents = new[]
        {
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanging),
                Sequence = 0,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangingEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanged),
                Sequence = 1,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangedEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 2,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs("test2", "test", NavigationKind.Back)
                }
            }
        };

        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.TimestampUtc));
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

        var expectedEvents = new[]
        {
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 0,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs("test3", "test2", NavigationKind.Back)
                }
            }
        };

        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.TimestampUtc));
    }

    [Fact]
    public void NextAsRootWhenEmpty()
    {
        var navServ = new NavigationService();
        using var navEvMon = navServ.Monitor();

        navServ.NextAsRoot("test");

        navServ.CanGoBack.Should().BeFalse();
        navServ.CurrentPage.Should().Be("test");

        var expectedEvents = new[]
        {
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 0,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs(null, "test", NavigationKind.NextAsRoot)
                }
            }
        };

        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.TimestampUtc));
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

        var expectedEvents = new[]
        {
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanging),
                Sequence = 0,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangingEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanged),
                Sequence = 1,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangedEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 2,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs("test2", "test3", NavigationKind.NextAsRoot)
                }
            }
        };

        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.TimestampUtc));
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

        var expectedEvents = new[]
        {
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanging),
                Sequence = 0,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangingEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.PropertyChanged),
                Sequence = 1,
                Parameters = new object[]
                {
                    navServ,
                    new PropertyChangedEventArgs(nameof(NavigationService.CanGoBack))
                }
            },
            new OccurredEvent
            {
                EventName = nameof(NavigationService.Navigated),
                Sequence = 2,
                Parameters = new object[]
                {
                    navServ,
                    new NavigatedEventArgs("test2", "test", NavigationKind.BackToRoot)
                }
            }
        };

        navEvMon.OccurredEvents.Should().BeEquivalentTo(expectedEvents, options => options
            .WithStrictOrdering()
            .Excluding(e => e.TimestampUtc));
    }
}
