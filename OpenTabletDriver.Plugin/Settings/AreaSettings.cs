#nullable enable

namespace OpenTabletDriver.Plugin.Settings
{
    public sealed class AreaSettings : BaseSettings
    {
        private float _width, _height, _x, _y, _rotation;

        public float Width
        {
            get => _width;
            set => RaiseAndSetIfChanged(ref _width, value);
        }

        public float Height
        {
            get => _height;
            set => RaiseAndSetIfChanged(ref _height, value);
        }

        public float X
        {
            get => _x;
            set => RaiseAndSetIfChanged(ref _x, value);
        }

        public float Y
        {
            get => _y;
            set => RaiseAndSetIfChanged(ref _y, value);
        }

        public float Rotation
        {
            get => _rotation;
            set => RaiseAndSetIfChanged(ref _rotation, value);
        }
    }
}