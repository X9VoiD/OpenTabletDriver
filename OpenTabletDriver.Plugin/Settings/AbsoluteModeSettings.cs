#nullable enable

namespace OpenTabletDriver.Plugin.Settings
{
    public sealed class AbsoluteModeSettings : BaseSettings
    {
        private AreaSettings? _display, _tablet;
        private bool _clipping, _areaLimiting, _lockAr;

        public AreaSettings? Display
        {
            get => _display;
            set => RaiseAndSetIfChanged(ref _display, value);
        }

        public AreaSettings? Tablet
        {
            get => _tablet;
            set => RaiseAndSetIfChanged(ref _tablet, value);
        }

        public bool EnableClipping
        {
            get => _clipping;
            set => RaiseAndSetIfChanged(ref _clipping, value);
        }

        public bool EnableAreaLimiting
        {
            get => _areaLimiting;
            set => RaiseAndSetIfChanged(ref _areaLimiting, value);
        }

        public bool LockAspectRatio
        {
            get => _lockAr;
            set => RaiseAndSetIfChanged(ref _lockAr, value);
        }
    }
}