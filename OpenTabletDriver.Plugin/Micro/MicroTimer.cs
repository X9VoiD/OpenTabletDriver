using System.Threading;

namespace OpenTabletDriver.Plugin.Micro
{
    /// <summary>
    /// MicroTimer class
    /// </summary>
    public class MicroTimer
    {
        public delegate void MicroTimerElapsedEventHandler(
                             object sender,
                             MicroTimerEventArgs timerEventArgs);
        public event MicroTimerElapsedEventHandler MicroTimerElapsed;

        Thread _threadTimer = null;
        long _ignoreEventIfLateBy = long.MaxValue;
        long _timerIntervalInMicroSec = 0;
        bool _stopTimer = true;

        public MicroTimer()
        {
        }

        public MicroTimer(long timerIntervalInMicroseconds)
        {
            Interval = timerIntervalInMicroseconds;
        }

        public long Interval
        {
            get => Interlocked.Read(
                    ref _timerIntervalInMicroSec);
            set => Interlocked.Exchange(
                    ref _timerIntervalInMicroSec, value);
        }

        public long IgnoreEventIfLateBy
        {
            get => Interlocked.Read(
                    ref _ignoreEventIfLateBy);
            set => Interlocked.Exchange(
                    ref _ignoreEventIfLateBy, value <= 0 ? long.MaxValue : value);
        }

        public bool Enabled
        {
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
            get
            {
                return _threadTimer != null && _threadTimer.IsAlive;
            }
        }

        public void Start()
        {
            if (Enabled || Interval <= 0)
            {
                return;
            }

            _stopTimer = false;

            void threadStart()
            {
                NotificationTimer(ref _timerIntervalInMicroSec,
                                  ref _ignoreEventIfLateBy,
                                  ref _stopTimer);
            }

            _threadTimer = new Thread(threadStart)
            {
                Priority = ThreadPriority.Highest
            };
            _threadTimer.Start();
        }

        public void Stop()
        {
            _stopTimer = true;
        }

        public void StopAndWait()
        {
            StopAndWait(Timeout.Infinite);
        }

        public bool StopAndWait(int timeoutInMilliSec)
        {
            _stopTimer = true;

            if (!Enabled || _threadTimer.ManagedThreadId ==
                Thread.CurrentThread.ManagedThreadId)
            {
                return true;
            }

            return _threadTimer.Join(timeoutInMilliSec);
        }

        public void Abort()
        {
            _stopTimer = true;

            if (Enabled)
            {
                _threadTimer.Abort();
            }
        }

        void NotificationTimer(ref long timerIntervalInMicroSec,
                               ref long ignoreEventIfLateBy,
                               ref bool stopTimer)
        {
            int timerCount = 0;
            long nextNotification = 0;

            MicroStopwatch microStopwatch = new MicroStopwatch();
            microStopwatch.Start();

            while (!stopTimer)
            {
                long callbackFunctionExecutionTime =
                    microStopwatch.ElapsedMicroseconds - nextNotification;

                long timerIntervalInMicroSecCurrent =
                    Interlocked.Read(ref timerIntervalInMicroSec);
                long ignoreEventIfLateByCurrent =
                    Interlocked.Read(ref ignoreEventIfLateBy);

                nextNotification += timerIntervalInMicroSecCurrent;
                timerCount++;
                long elapsedMicroseconds;

                while ((elapsedMicroseconds = microStopwatch.ElapsedMicroseconds)
                        < nextNotification)
                {
                    Thread.Sleep(0);
                }

                long timerLateBy = elapsedMicroseconds - nextNotification;

                if (timerLateBy >= ignoreEventIfLateByCurrent)
                {
                    continue;
                }

                MicroTimerEventArgs microTimerEventArgs =
                     new MicroTimerEventArgs(timerCount,
                                             elapsedMicroseconds,
                                             timerLateBy,
                                             callbackFunctionExecutionTime);
                MicroTimerElapsed(this, microTimerEventArgs);
            }

            microStopwatch.Stop();
        }
    }
}