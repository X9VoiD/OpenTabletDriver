using System.Numerics;

namespace OpenTabletDriver.Plugin.Tablet.Interpolator
{
    public class InterpolatorArgs
    {
        public Vector2 Position { get; set; }
        public uint Pressure { get; set; }
        public double TimeDelta { get; private set; }

        public InterpolatorArgs(Vector2 Position, uint Pressure, double TimeDelta)
        {
            this.Position = Position;
            this.Pressure = Pressure;
            this.TimeDelta = TimeDelta;
        }
    }
}