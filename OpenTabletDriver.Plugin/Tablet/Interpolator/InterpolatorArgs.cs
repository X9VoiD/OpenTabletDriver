using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    public class InterpolatorArgs
    {
        public Vector2? Position { internal get; set; }
        public uint? Pressure { internal get; set; }
    }
}