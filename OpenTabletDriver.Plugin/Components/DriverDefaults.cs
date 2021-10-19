using System;
using OpenTabletDriver.Plugin.Output;

#nullable enable

namespace OpenTabletDriver.Plugin.Components
{
    /// <summary>
    /// Defines properties that will be used for generating functional default settings.
    /// </summary>
    public class DriverDefaults
    {
        public Type OutputMode { get; }
        public Type Pointer { get; }
        public Type TipBinding { get; }

        public DriverDefaults(Type outputMode, Type pointer, Type tipBinding)
        {
            OutputMode = Verify<IOutputMode>(outputMode);
            Pointer = Verify<IPointer>(pointer);
            TipBinding = Verify<IBinding>(tipBinding);
        }

        private static Type Verify<T>(Type type)
        {
            return type.IsAssignableTo(typeof(T))
                ? type
                : throw new ArgumentException($"{type.FullName} is not assignable to {typeof(T).FullName}");
        }
    }
}