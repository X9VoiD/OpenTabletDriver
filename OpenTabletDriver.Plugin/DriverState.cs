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
        public static event EventHandler<bool> PenArrived;
        public static bool PenInRange
        {
            internal set
            {
                PenArrived?.Invoke(null, value);
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