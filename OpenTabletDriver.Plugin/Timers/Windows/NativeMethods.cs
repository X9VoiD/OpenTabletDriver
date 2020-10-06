using System;
using System.Runtime.InteropServices;

namespace OpenTabletDriver.Plugin.Timers.Windows
{
    [Flags]
    public enum EventType : uint
    {
        TIME_ONESHOT = 0,      //Event occurs once, after uDelay milliseconds.
        TIME_PERIODIC = 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TimeCaps
    {
        public uint wPeriodMin;
        public uint wPeriodMax;
    };

    internal delegate void TimerCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);

    internal static class NativeMethods
    {
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeGetDevCaps(ref TimeCaps timeCaps, uint sizeTimeCaps);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeEndPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeSetEvent(uint msDelay, uint msResolution, TimerCallback handler,
            IntPtr data, EventType eventType);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeKillEvent(uint timerEventId);
    }
}