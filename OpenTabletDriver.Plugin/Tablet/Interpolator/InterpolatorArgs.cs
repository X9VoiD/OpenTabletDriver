using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    public class InterpolatorArgs
    {
        public Vector2 Position { get; set; }
        public uint Pressure { get; set; }

        public InterpolatorArgs(Vector2 Position, uint Pressure)
        {
            this.Position = Position;
            this.Pressure = Pressure;
        }
    }
}