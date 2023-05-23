using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OpenTabletDriver.UI;

public sealed class CompositeDisposable : Collection<IDisposable>, IDisposable
{
    public void Dispose()
    {
        this.ForEach(disposable => disposable.Dispose());
    }

    public static CompositeDisposable From(IDisposable disposable)
    {
        var composite = new CompositeDisposable();
        composite.Add(disposable);
        return composite;
    }

    public static CompositeDisposable From(IDisposable disposable1, IDisposable disposable2)
    {
        var composite = new CompositeDisposable();
        composite.Add(disposable1);
        composite.Add(disposable2);
        return composite;
    }

    public static CompositeDisposable From(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3)
    {
        var composite = new CompositeDisposable();
        composite.Add(disposable1);
        composite.Add(disposable2);
        composite.Add(disposable3);
        return composite;
    }

    public static CompositeDisposable From(IDisposable disposable1, IDisposable disposable2, IDisposable disposable3, IDisposable disposable4)
    {
        var composite = new CompositeDisposable();
        composite.Add(disposable1);
        composite.Add(disposable2);
        composite.Add(disposable3);
        composite.Add(disposable4);
        return composite;
    }

    public static CompositeDisposable From(params IDisposable[] disposables)
    {
        var composite = new CompositeDisposable();
        composite.AddRange(disposables);
        return composite;
    }
}

public static class CompositeDisposableExtensions
{
    public static void DisposeWith(this IDisposable disposable, CompositeDisposable composite)
    {
        composite.Add(disposable);
    }
}

public static class SynchronizationExtensions
{
    public static async Task<SemaphoreSlimLock> LockAsync(this SemaphoreSlim sem)
    {
        await sem.WaitAsync();
        return new SemaphoreSlimLock(sem);
    }

    public static SemaphoreSlimLock Lock(this SemaphoreSlim sem)
    {
        sem.Wait();
        return new SemaphoreSlimLock(sem);
    }

    public struct SemaphoreSlimLock : IDisposable
    {
        private readonly SemaphoreSlim _sem;

        public SemaphoreSlimLock(SemaphoreSlim sem)
        {
            _sem = sem;
        }

        public void Dispose()
        {
            _sem.Release();
        }
    }
}
