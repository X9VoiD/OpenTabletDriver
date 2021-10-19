using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace OpenTabletDriver.Plugin.Settings
{
    public abstract class BaseSettings : INotifyPropertyChanged
    {
        // private bool _throttling;
        private ImmutableHashSet<string?> _supressedEvents = ImmutableHashSet<string?>.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Supress(string propertyName)
        {
            ImmutableInterlocked.Update(ref _supressedEvents, static (supressedEvents, pName) =>
            {
                return !supressedEvents.Contains(pName)
                    ? supressedEvents.Add(pName)
                    : supressedEvents;
            }, propertyName);
        }

        protected void Unsupress(string propertyName)
        {
            ImmutableInterlocked.Update(ref _supressedEvents, static (supressedEvents, pName) =>
            {
                return supressedEvents.Contains(pName)
                    ? supressedEvents.Remove(pName)
                    : supressedEvents;
            }, propertyName);
        }

        protected bool IsSupressed(string propertyName)
        {
            return _supressedEvents.Contains(propertyName);
        }

        // protected void ApplyThrottling()
        // {

        // }

        protected void RaiseAndSetIfChanged<T>(ref T obj, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (_supressedEvents.Contains(propertyName) && EqualityComparer<T>.Default.Equals(obj, newValue))
            {
                obj = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}