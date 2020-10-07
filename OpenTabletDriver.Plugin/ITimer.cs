using System;

namespace OpenTabletDriver.Plugin.Timers
{
    public interface ITimer
    {
        public void Start();
        public void Stop();
        public bool Stop(int milliseconds);
        public bool Enabled { get; }
        public long Interval { get; set; }
        public event EventHandler Elapsed;
    }
}