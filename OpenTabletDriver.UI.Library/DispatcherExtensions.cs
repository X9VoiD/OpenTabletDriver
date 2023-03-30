using Avalonia.Threading;

namespace OpenTabletDriver.UI;

public static class DispatcherExtensions
{
    public static void ProbablySynchronousPost(this IDispatcher dispatcher, Action action, DispatcherPriority priority = default)
    {
        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            dispatcher.Post(action, priority);
        }
    }
}
