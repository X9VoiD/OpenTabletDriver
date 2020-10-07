using System;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Plugin
{
    public interface IDriver
    {
        event Action<bool> Reading;
        event Action<IDeviceReport> ReportRecieved;

        bool EnableInput { set; get; }
        TabletProperties TabletProperties { get; }
        IOutputMode OutputMode { set; get; }

        bool TryMatch(TabletProperties tablet);
        void OnReport(IDeviceReport report);
    }
}