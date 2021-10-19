using System;

namespace OpenTabletDriver.Plugin
{
    /// <summary>
    /// A plugin that will be started up and kept running until OpenTabletDriver is closed.
    /// </summary>
    [PluginType(PluginLifetime.Singleton)]
    public interface ITool : IDisposable
    {
        bool Initialize();
    }
}