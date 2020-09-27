using System;

namespace OpenTabletDriver.Plugin.Timers
{
    /// <summary>
    /// MicroStopwatch class
    /// </summary>
    public class MicroStopwatch : System.Diagnostics.Stopwatch
    {
        readonly double _microSecPerTick =
            1000000D / Frequency;

        public MicroStopwatch()
        {
            if (!IsHighResolution)
            {
                throw new PlatformNotSupportedException(
                    "On this system the high-resolution " +
                    "performance counter is not available"
                );
            }
        }

        public long ElapsedMicroseconds
        {
            get
            {
                return (long)(ElapsedTicks * _microSecPerTick);
            }
        }
    }
}