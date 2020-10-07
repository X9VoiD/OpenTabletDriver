using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Timers;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    public abstract class Interpolator : IDisposable
    {
        protected Interpolator(ITimer scheduler)
        {
            this.scheduler = scheduler;
            scheduler.Elapsed += Interpolate;
            scheduler.Start();
            Info.Driver.ReportRecieved += HandleReport;
        }

        public abstract ITabletReport Interpolate();
        public abstract void UpdateState(ITabletReport report);

        protected bool enabled, inRange;
        protected ITimer scheduler;
        protected DateTime lastTime;
        protected ISyntheticReport syntheticReport;
        protected readonly object stateLock = new object();

        
        [Property("Hertz"), Unit("hz")]
        public uint Hertz { get; set; } = 1000;

        public virtual bool Enabled
        {
            set
            {
                this.enabled = value;
                if (value)
                {
                    scheduler.Interval = 1000 / this.Hertz;
                    scheduler.Start();
                }
                else
                {
                    scheduler.Stop();
                }
            }
            get => enabled;
        }

        protected virtual void HandleReport(IDeviceReport report)
        {
            lastTime = DateTime.UtcNow;
            if (report is ITabletReport tabletReport && !(report is ISyntheticReport))
            {
                if (Info.Driver.TabletProperties.ActiveReportID.IsInRange(tabletReport.ReportID))
                {
                    lock (this.stateLock)
                    {
                        this.inRange = true;

                        if (Enabled)
                        {
                            syntheticReport = new SyntheticTabletReport(tabletReport);
                            UpdateState(tabletReport);
                        }
                    }
                }
                else
                {
                    this.inRange = false;
                }
            }
        }

        protected virtual void Interpolate(object sender, object e)
        {
            lock (this.stateLock)
            {
                if ((DateTime.UtcNow - lastTime).TotalMilliseconds < 150 && this.inRange)
                {
                    var report = Interpolate();
                    Info.Driver.OnReport(report);
                }
            }
        }

        public virtual void Dispose()
        {
            Enabled = false;
        }
    }
}