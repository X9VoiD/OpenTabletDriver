using System;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Plugin
{
    public static class DriverState
    {
        private static bool _penInRange;

        public static IOutputMode OutputMode { set; get; }
        public static TabletProperties TabletProperties { set; get; }
        public static event EventHandler<IDeviceReport> ReportRecieved;
        public static event EventHandler PenArrived;
        public static event EventHandler PenLeft;
        public static bool PenInRange
        {
            internal set
            {
                if (value)
                    PenArrived?.Invoke(null, null);
                else
                    PenLeft?.Invoke(null, null);

                _penInRange = value;
            }
            get => _penInRange;
        }

        public static void PostReport(object sender, IDeviceReport report)
        {
            ReportRecieved?.Invoke(sender, report);
        }
    }
}