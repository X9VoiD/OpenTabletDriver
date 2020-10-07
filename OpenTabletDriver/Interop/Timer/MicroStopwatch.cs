using System;

namespace OpenTabletDriver.Interop.Timer
{
    public class MicroStopwatch : System.Diagnostics.Stopwatch
    {
        readonly double microSecPerTick =
            1000000.0 / Frequency;

        public MicroStopwatch()
        {
            if (!IsHighResolution)
            {
                throw new PlatformNotSupportedException(
                    "The high-resolution performance counter " +
                    "is not available on this system"
                );
            }
        }

        public long ElapsedMicroseconds
        {
            get
            {
                return (long)(ElapsedTicks * this.microSecPerTick);
            }
        }
    }
}