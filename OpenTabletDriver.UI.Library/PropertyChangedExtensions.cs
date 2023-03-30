using System.ComponentModel;

namespace OpenTabletDriver.UI;

internal static class PropertyChangedExtensions
{
    public static WeakRefPropertyChangedHandler<TSource, TTarget> HandlePropertyWeak<TSource, TTarget>(
        this TSource source,
        string sourceProperty,
        Func<TSource, TTarget> getter,
        Action<TSource, TTarget> action)
            where TSource : class, INotifyPropertyChanged
    {
        return new WeakRefPropertyChangedHandler<TSource, TTarget>(source, sourceProperty, getter, action);
    }

    public static StrongRefPropertyChangedHandler<TSource, TTarget> HandleProperty<TSource, TTarget>(
        this TSource source,
        string sourceProperty,
        Func<TSource, TTarget> getter,
        Action<TSource, TTarget> action)
            where TSource : class, INotifyPropertyChanged
    {
        return new StrongRefPropertyChangedHandler<TSource, TTarget>(source, sourceProperty, getter, action);
    }
}

internal readonly struct WeakRefPropertyChangedHandler<TSource, TTarget> : IDisposable
    where TSource : class, INotifyPropertyChanged
{
    private readonly WeakReference<TSource> _source;
    private readonly string _sourceProperty;
    private readonly Func<TSource, TTarget> _getter;
    private readonly Action<TSource, TTarget> _action;

    public WeakRefPropertyChangedHandler(TSource source, string sourceProperty, Func<TSource, TTarget> getter, Action<TSource, TTarget> action)
    {
        _source = new WeakReference<TSource>(source);
        _sourceProperty = sourceProperty;
        _getter = getter;
        _action = action;

        source.PropertyChanged += HandlePropertyChanged;
        action(source, getter(source)); // Invoke action on creation
    }

    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_source.TryGetTarget(out var source) && e.PropertyName == _sourceProperty)
            _action(source, _getter(source));
    }

    public void Dispose()
    {
        if (_source.TryGetTarget(out var source))
            source.PropertyChanged -= HandlePropertyChanged;
    }
}

internal readonly struct StrongRefPropertyChangedHandler<TSource, TTarget> : IDisposable
    where TSource : class, INotifyPropertyChanged
{
    private readonly TSource _source;
    private readonly string _sourceProperty;
    private readonly Func<TSource, TTarget> _getter;
    private readonly Action<TSource, TTarget> _action;

    public StrongRefPropertyChangedHandler(TSource source, string sourceProperty, Func<TSource, TTarget> getter, Action<TSource, TTarget> action)
    {
        _source = source;
        _sourceProperty = sourceProperty;
        _getter = getter;
        _action = action;

        source.PropertyChanged += HandlePropertyChanged;
        action(source, getter(source)); // Invoke action on creation
    }

    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == _sourceProperty)
            _action(_source, _getter(_source));
    }

    public void Dispose()
    {
        _source.PropertyChanged -= HandlePropertyChanged;
    }
}
