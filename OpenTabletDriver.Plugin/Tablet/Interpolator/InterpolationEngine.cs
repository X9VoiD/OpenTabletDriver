using System;
using OpenTabletDriver.Plugin.Micro;

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
                if (value && !Scheduler.Enabled && ActiveInterpolator != null)
                {
                    Scheduler.Interval = 1000 * 1000 / ActiveInterpolator.Hertz;
                    Scheduler.Start();
                }
                else
                    Scheduler.Stop();
            }
            get => _enabled;
        }

        public static Interpolator ActiveInterpolator { get; set; }
        public static MicroTimer Scheduler { get; set; }

        private static DateTime LastTime;
        private static ITabletReport SynthReport;
        private static double NextReport, InterpTime;
        private static readonly object StateLock = new object();

        public static void Initialize()
        {
            try
            {
                Scheduler = new MicroTimer();
            }
            catch
            {
                Log.Write("Interpolator", "No high resolution clock is available", LogLevel.Error);
            }

            Scheduler.MicroTimerElapsed += Interpolate;
            Scheduler.IgnoreEventIfLateBy = 1000;

            DriverState.PenArrived += (sender, o) =>
            {
                if (Enabled)
                {
                    Scheduler.Start();
                }
            };
            DriverState.PenLeft += (sender, o) =>
            {
                Scheduler.Stop();
                InterpTime = 0;
            };
        }

        public static void HandleReport(IDeviceReport report)
        {
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

                        InterpTime = 0;
                        var now = DateTime.UtcNow;
                        NextReport += ((now - LastTime).TotalMilliseconds - NextReport) / 10;
                        LastTime = now;
                        SynthReport = tabletReport;

                        if (Enabled)
                        {
                            var interpolatorArgs = new InterpolatorArgs(tabletReport.Position, tabletReport.Pressure);
                            ActiveInterpolator.NewReport(interpolatorArgs);
                            SynthReport.Position = interpolatorArgs.Position;
                            SynthReport.Pressure = interpolatorArgs.Pressure;
                        }
                        else
                        {
                            SendReport(tabletReport);
                        }
                    }
                }
            }
        }

        private static void Interpolate(object sender, MicroTimerEventArgs _)
        {
            lock (StateLock)
            {
                InterpTime = (DateTime.UtcNow - LastTime).TotalMilliseconds;
                var InterpState = InterpTime / NextReport;
                if (InterpState < 8)
                {
                    var interpolatorArgs = new InterpolatorArgs(SynthReport.Position, SynthReport.Pressure);
                    ActiveInterpolator.Interpolate(interpolatorArgs);

                    SendReport(SynthReport);
                }
                else
                {
                    if (DriverState.PenInRange)
                        DriverState.PenInRange = false;
                }
            }
        }

        private static void SendReport(ITabletReport report)
        {
            DriverState.OutputMode.Read(report);
            if (DriverState.OutputMode is IBindingHandler<IBinding> handler)
                handler.HandleBinding(report);
        }
    }
}