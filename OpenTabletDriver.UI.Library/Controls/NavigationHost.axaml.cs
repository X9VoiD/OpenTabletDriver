using System.Globalization;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.Controls;

/// <summary>
/// Displays <see cref="ContentControl.Content"/> according to a <see cref="FuncDataTemplate"/>.
/// Has different possible transitions for different navigation types.
/// </summary>
public partial class NavigationHost : UserControl
{
    private static int _sentinel;
    private readonly NavigationContext _context = new();
    private INavigationService? _navigationService;

    public NavigationHost()
    {
        if (Interlocked.Increment(ref _sentinel) > 1)
            throw new InvalidOperationException("Only one instance of NavigationHost can exist at a time.");

        InitializeComponent();
        PART_TransitioningContentControl.DataContext = _context;
    }

    public static readonly DirectProperty<NavigationHost, INavigationService?> NavigationServiceProperty =
        AvaloniaProperty.RegisterDirect<NavigationHost, INavigationService?>(
            nameof(NavigationService),
            o => o.NavigationService,
            (o, v) => o.NavigationService = v
        );

    public INavigationService? NavigationService
    {
        get => _navigationService;
        set => SetAndRaise(NavigationServiceProperty, ref _navigationService, value);
    }

    public static readonly StyledProperty<IPageTransition?> NextTransitionProperty =
        AvaloniaProperty.Register<NavigationHost, IPageTransition?>(nameof(NextTransition),
            new CrossFade(TimeSpan.FromSeconds(0.125)));

    public IPageTransition? NextTransition
    {
        get => GetValue(NextTransitionProperty);
        set => SetValue(NextTransitionProperty, value);
    }

    public static readonly StyledProperty<IPageTransition?> BackTransitionProperty =
        AvaloniaProperty.Register<NavigationHost, IPageTransition?>(nameof(BackTransition),
            new CrossFade(TimeSpan.FromSeconds(0.125)));

    public IPageTransition? BackTransition
    {
        get => GetValue(BackTransitionProperty);
        set => SetValue(BackTransitionProperty, value);
    }

    public static readonly StyledProperty<IPageTransition?> NextAsRootTransitionProperty =
        AvaloniaProperty.Register<NavigationHost, IPageTransition?>(nameof(NextAsRootTransition));

    public IPageTransition? NextAsRootTransition
    {
        get => GetValue(NextAsRootTransitionProperty);
        set => SetValue(NextAsRootTransitionProperty, value);
    }

    public static readonly StyledProperty<IPageTransition?> BackToRootTransitionProperty =
        AvaloniaProperty.Register<NavigationHost, IPageTransition?>(nameof(BackToRootTransition));

    public IPageTransition? BackToRootTransition
    {
        get => GetValue(BackToRootTransitionProperty);
        set => SetValue(BackToRootTransitionProperty, value);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        PART_TransitioningContentControl.DataContext = _context;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == NavigationServiceProperty)
        {
            if (change.OldValue is INavigationService oldService)
                oldService.Navigated -= HandleNavigated;

            if (change.NewValue is INavigationService newService)
            {
                newService.Navigated += HandleNavigated;
            }
        }
        base.OnPropertyChanged(change);
    }

    private void HandleNavigated(object? sender, NavigatedEventArgs e)
    {
        PART_TransitioningContentControl.PageTransition = e.Kind switch
        {
            NavigationKind.Next => NextTransition,
            NavigationKind.Back => BackTransition ?? NextTransition,
            NavigationKind.NextAsRoot => NextAsRootTransition ?? NextTransition,
            NavigationKind.BackToRoot => BackToRootTransition ?? BackTransition ?? NextTransition,
            _ => throw new InvalidOperationException("Invalid navigation kind.")
        };

        _context.Page = e.Page;
    }

    internal static void ResetSentinel()
    {
        _sentinel = 0;
    }
}

internal partial class NavigationContext : ObservableObject
{
    [ObservableProperty]
    private string? _page;
}
