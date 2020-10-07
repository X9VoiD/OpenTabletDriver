using System;
using System.Threading;
using OpenTabletDriver.Plugin.Timers;

namespace OpenTabletDriver.Interop.Timer
{
    public class MicroTimer : ITimer
    {
        public event EventHandler Elapsed;

        private Thread threadTimer = null;
        private long ignoreEventIfLateBy = long.MaxValue;
        private long timerIntervalInMicroSec = 0;
        private bool stopTimer = true;

        public MicroTimer()
        {
        }

        public MicroTimer(long timerIntervalInMicroseconds)
        {
            Interval = timerIntervalInMicroseconds;
        }

        private long MicroInterval
        {
            get => Interlocked.Read(
                    ref this.timerIntervalInMicroSec);
        }

        public long Interval
        {
            get => Interlocked.Read(
                    ref this.timerIntervalInMicroSec) / 1000;
            set => Interlocked.Exchange(
                    ref this.timerIntervalInMicroSec, value * 1000);
        }

        public long IgnoreEventIfLateBy
        {
            get => Interlocked.Read(
                    ref this.ignoreEventIfLateBy);
            set => Interlocked.Exchange(
                    ref this.ignoreEventIfLateBy, value <= 0 ? long.MaxValue : value);
        }

        public bool Enabled => this.threadTimer != null && this.threadTimer.IsAlive;

        public void Start()
        {
            if (Enabled || MicroInterval <= 0)
            {
                return;
            }

            this.stopTimer = false;

            this.threadTimer = new Thread(NotificationTimer)
            {
                Priority = ThreadPriority.Highest
            };
            this.threadTimer.Start();
        }

        public void Stop()
        {
            this.stopTimer = true;
        }

        public bool Stop(int timeoutInMilliSec)
        {
            this.stopTimer = true;

            if (!Enabled)
                return true;
            else
                return this.threadTimer.Join(timeoutInMilliSec);
        }

        void NotificationTimer()
        {
            int timerCount = 0;
            long nextNotification = 0;
            long elapsedMicroseconds;

            MicroStopwatch microStopwatch = new MicroStopwatch();
            microStopwatch.Start();

            while (!this.stopTimer)
            {
                nextNotification += MicroInterval;
                timerCount++;

                while ((elapsedMicroseconds = microStopwatch.ElapsedMicroseconds)
                        < nextNotification)
                {
                    Thread.Sleep(0);
                }

                long timerLateBy = elapsedMicroseconds - nextNotification;

                if (timerLateBy >= IgnoreEventIfLateBy)
                {
                    continue;
                }

                Elapsed(this, null);
            }

            microStopwatch.Stop();
        }
    }
}