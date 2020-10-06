using System;
using OpenTabletDriver.Plugin.Timers;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    public static class InterpolationEngine
    {
        private static bool _enabled;
        public static bool Enabled
        {
            set
            {
                _enabled = value;
                if (value && ActiveInterpolator != null)
                {
                    Scheduler.Interval = 1000 / ActiveInterpolator.Hertz;
                }
                else
                    Scheduler?.Stop();
            }
            get => _enabled;
        }
        public static Interpolator ActiveInterpolator { get; set; }

        private static ITimer Scheduler;
        private static DateTime LastTime;
        private static InterpolatedTabletReport SynthReport;
        private static readonly object StateLock = new object();

        public static void Initialize()
        {
            try
            {
                Scheduler = Timer.NewTimer();
            }
            catch
            {
                Log.Write("Interpolator", "No high resolution timer is available", LogLevel.Error);
            }

            Scheduler.Elapsed += Interpolate;

            DriverState.PenArrived += (sender, o) =>
            {
                if (o && Enabled && !Scheduler.Enabled)
                {
                    Scheduler.Start();
                }
                else
                {
                    Scheduler.Stop();
                }
            };
        }

        public static void HandleReport(IDeviceReport report)
        {
            LastTime = DateTime.UtcNow;
            if (report is ITabletReport tabletReport)
            {
                if (DriverState.TabletProperties.ActiveReportID.IsInRange(tabletReport.ReportID))
                {
                    lock (StateLock)
                    {
                        if (!DriverState.PenInRange)
                        {
                            DriverState.PenInRange = true;
                        }

                        SynthReport = new InterpolatedTabletReport(tabletReport);

                        if (Enabled)
                        {
                            ActiveInterpolator.NewReport(tabletReport.Position, tabletReport.Pressure);
                            return;
                        }
                    }
                }
            }
            SendReport(report);
        }

        private static void Interpolate(object sender, object _)
        {
            lock (StateLock)
            {
                if ((DateTime.UtcNow - LastTime).TotalMilliseconds < 150)
                {
                    var interpolatorArgs = new InterpolatorArgs();
                    ActiveInterpolator.Interpolate(interpolatorArgs);

                    if (interpolatorArgs.Position.HasValue)
                        SynthReport.Position = interpolatorArgs.Position.Value;
                    if (interpolatorArgs.Pressure.HasValue)
                        SynthReport.Pressure = interpolatorArgs.Pressure.Value;

                    SendReport(SynthReport);
                }
                else
                {
                    if (DriverState.PenInRange)
                        DriverState.PenInRange = false;
                }
            }
        }

        private static void SendReport(IDeviceReport report)
        {
            DriverState.OutputMode.Read(report);
            if (DriverState.OutputMode is IBindingHandler<IBinding> handler)
                handler.HandleBinding(report);
        }
    }
}