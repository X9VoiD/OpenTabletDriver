using System.Runtime.InteropServices;
using OpenTabletDriver.Plugin.Timers.Fallback;
using OpenTabletDriver.Plugin.Timers.Windows;

namespace OpenTabletDriver.Plugin.Timers
{
    public static class Timer
    {
        public static ITimer NewTimer()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsTimer();
            else
                return new MicroTimer(); // no platform-specific timer, fallback to busy wait
        }
    }
}