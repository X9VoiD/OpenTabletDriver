using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    internal class SyntheticTabletReport : ISyntheticTabletReport
    {
        public byte[] Raw { get; }
        public uint ReportID { get; }
        public Vector2 Position { get; set; }
        public uint Pressure { get; set; }
        public bool[] PenButtons { get; set; }

        public SyntheticTabletReport(ITabletReport report)
        {
            this.Raw = report.Raw;
            this.ReportID = report.ReportID;
            this.Position = report.Position;
            this.Pressure = report.Pressure;
            this.PenButtons = report.PenButtons;
        }
    }
}