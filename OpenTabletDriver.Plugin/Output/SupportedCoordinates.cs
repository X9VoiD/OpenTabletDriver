using System;

namespace OpenTabletDriver.Plugin.Output
{
    [Flags]
    public enum SupportedCoordinates
    {
        None,
        Absolute,
        Relative
    }
}