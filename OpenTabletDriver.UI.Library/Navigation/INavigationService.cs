using System.ComponentModel;

namespace OpenTabletDriver.UI.Navigation;

/// <summary>
/// Provides functionality to navigate between pages based on a stack of strings.
/// </summary>
/// <remarks>
/// The strings used to navigate can be arbitrary, there is no hierarchy or structure to them.
/// It is used to identify the pages to navigate to, and are not used to determine
/// the flow of navigation. However, nothing prevents you from using hierarchical strings
/// for example:
/// <list type="bullet">
///   <item>Main</item>
///   <item>Settings/General</item>
///   <item>Settings/Advanced/Plugins</item>
/// </list>
/// Just make sure to use the exact same strings and invoke the correct navigation methods
/// according to your defined hierarchy.
/// </remarks>
public interface INavigationService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the current page, or null if <see cref="Next(string)"/> or <see cref="NextAsRoot(string)"/>
    /// is not invoked yet.
    /// </summary>
    string? CurrentPage { get; }

    /// <summary>
    /// Gets a boolean indicating whether the navigation stack has more than one page.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Occurs when the navigation stack is changing.
    /// </summary>
    event EventHandler<NavigatingEventArgs>? Navigating;

    /// <summary>
    /// Occurs when the navigation stack changes.
    /// </summary>
    event EventHandler<NavigatedEventArgs>? Navigated;

    /// <summary>
    /// Navigates to the previous page in the stack.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="CanGoBack"/> is <see langword="false"/>.</exception>
    void Back();

    /// <summary>
    /// Navigates to the root page in the stack.
    /// </summary>
    /// <remarks>
    /// This method will do nothing if the stack contains only one page.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="CurrentPage"/> is <see langword="null"/>.</exception>
    void BackToRoot();

    /// <summary>
    /// Navigates to the specified registered page.
    /// </summary>
    /// <param name="page">The registered page to navigate to.</param>
    void Next(string page);

    /// <summary>
    /// Clears the navigation stack and navigates to the specified registered page.
    /// </summary>
    /// <param name="page">The page to navigate to.</param>
    /// <remarks>
    /// Only this method can replace the current root page.
    /// </remarks>
    void NextAsRoot(string page);
}
