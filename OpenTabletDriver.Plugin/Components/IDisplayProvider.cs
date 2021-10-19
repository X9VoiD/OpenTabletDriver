using System;
using OpenTabletDriver.Plugin.Platform.Display;

#nullable enable

namespace OpenTabletDriver.Plugin.Components
{
    public interface IDisplayProvider
    {
        event EventHandler? DisplayChanged;
        IVirtualScreen VirtualDisplay { get; }
    }
}