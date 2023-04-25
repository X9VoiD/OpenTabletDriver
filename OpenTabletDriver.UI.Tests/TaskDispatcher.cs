using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace OpenTabletDriver.UI.Tests;

public class TaskDispatcher : IDispatcher
{
    public bool CheckAccess()
    {
        return true;
    }

    public async Task InvokeAsync(Action action, DispatcherPriority priority = default)
    {
        await Task.Run(action);
    }

    public async Task InvokeAsync(Func<Task> function, DispatcherPriority priority = default)
    {
        await Task.Run(function);
    }

    public void Post(Action action, DispatcherPriority priority = default)
    {
        Task.Run(action);
    }

    public void Post(SendOrPostCallback action, object arg, DispatcherPriority priority = default)
    {
        Task.Run(() => action(arg));
    }

    public void VerifyAccess()
    {
    }
}
