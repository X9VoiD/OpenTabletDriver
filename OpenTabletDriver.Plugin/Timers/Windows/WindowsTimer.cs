using System;
using System.Runtime.InteropServices;

namespace OpenTabletDriver.Plugin.Timers.Windows
{
    using static NativeMethods;
    internal class WindowsTimer : ITimer
    {
        private uint timerId;
        private TimerCallback callbackDelegate;
        private readonly object stateLock = new object();

        public void Start()
        {
            lock (stateLock)
            {
                var caps = new TimeCaps();
                timeGetDevCaps(ref caps, (uint)Marshal.SizeOf(caps));
                timeBeginPeriod(Math.Clamp((uint)Interval, caps.wPeriodMin, caps.wPeriodMax));
                Enabled = true;

                callbackDelegate = Callback;
                timerId = timeSetEvent(1, 1, callbackDelegate, IntPtr.Zero, EventType.TIME_PERIODIC);
            }
        }

        public void Stop()
        {
            lock (stateLock)
            {
                timeKillEvent(timerId);
                timeEndPeriod((uint)Interval);
                Enabled = false;
            }
        }

        public bool Stop(int milliseconds)
        {
            Stop();
            return true; // waiting not implemented
        }

        private void Callback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
        {
            if (Enabled)
                Elapsed(this, null);
        }

        public bool Enabled { private set; get; }

        public long Interval { set; get; } = 1;

        public event EventHandler Elapsed;
    }
}