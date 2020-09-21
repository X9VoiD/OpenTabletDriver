using OpenTabletDriver.Plugin.Attributes;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    public abstract class Interpolator
    {
        public abstract void Interpolate(InterpolatorArgs args);
        public abstract void NewReport(InterpolatorArgs args);

        [UnitProperty("Hertz", "hz")]
        public uint Hertz { get; set; } = 1000;
    }
}