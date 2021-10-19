using System;

#nullable enable

namespace OpenTabletDriver.Plugin.Settings
{
    public sealed class RelativeModeSettings : BaseSettings
    {
        private float _xSensitivity = 10, _ySensitivity = 10, _relativeRotation;
        private TimeSpan _resetTime = TimeSpan.FromMilliseconds(15);

        public float XSensitivity
        {
            get => _xSensitivity;
            set => RaiseAndSetIfChanged(ref _xSensitivity, value);
        }

        public float YSensitivity
        {
            get => _ySensitivity;
            set => RaiseAndSetIfChanged(ref _ySensitivity, value);
        }

        public float RelativeRotation
        {
            get => _relativeRotation;
            set => RaiseAndSetIfChanged(ref _relativeRotation, value);
        }

        public TimeSpan ResetTime
        {
            get => _resetTime;
            set => RaiseAndSetIfChanged(ref _resetTime, value);
        }
    }
}