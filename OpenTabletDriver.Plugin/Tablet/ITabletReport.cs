using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet
{
    public interface ITabletReport : IDeviceReport
    {
        uint ReportID { get; }
        Vector2 Position { get; set; }
        uint Pressure { get; set; }
        bool[] PenButtons { get; }
    }
}