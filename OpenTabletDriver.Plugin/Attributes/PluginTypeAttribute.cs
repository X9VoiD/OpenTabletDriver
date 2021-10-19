using System;

#nullable enable

namespace OpenTabletDriver.Plugin
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    internal class PluginTypeAttribute : Attribute
    {
        public PluginLifetime Lifetime { get; }

        public PluginTypeAttribute(PluginLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }

    internal enum PluginLifetime
    {
        Singleton,
        Scoped,
        Transient
    }
}