using System.Numerics;

namespace OpenTabletDriver.Plugin.Output
{
    public interface IPointer
    {
        SupportedCoordinates SupportedCoordinates { get; }
        void SetCoordinate(Vector2 pos);
    }
}