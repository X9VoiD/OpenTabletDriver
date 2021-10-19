using System.Collections.ObjectModel;

#nullable enable

namespace OpenTabletDriver.Plugin.Settings
{
    public sealed class BindingSettings : BaseSettings
    {
        private float _tipActivationPressure, _eraserActivationPressure;
        private PluginSettings? _tipButton, _eraserButton, _mouseScrollUp, _mouseScrollDown;
        private Collection<PluginSettings>? _penButtons, _auxButtons, _mouseButtons;

        public float TipActivationPressure
        {
            get => _tipActivationPressure;
            set => RaiseAndSetIfChanged(ref _tipActivationPressure, value);
        }

        public PluginSettings? TipButton
        {
            get => _tipButton;
            set => RaiseAndSetIfChanged(ref _tipButton, value);
        }

        public float EraserActivationPressure
        {
            get => _eraserActivationPressure;
            set => RaiseAndSetIfChanged(ref _eraserActivationPressure, value);
        }

        public PluginSettings? EraserButton
        {
            get => _eraserButton;
            set => RaiseAndSetIfChanged(ref _eraserButton, value);
        }

        public Collection<PluginSettings>? PenButtons
        {
            get => _penButtons;
            set => RaiseAndSetIfChanged(ref _penButtons, value);
        }

        public Collection<PluginSettings>? AuxButtons
        {
            get => _auxButtons;
            set => RaiseAndSetIfChanged(ref _auxButtons, value);
        }

        public Collection<PluginSettings>? MouseButtons
        {
            get => _mouseButtons;
            set => RaiseAndSetIfChanged(ref _mouseButtons, value);
        }

        public PluginSettings? MouseScrollUp
        {
            get => _mouseScrollUp;
            set => RaiseAndSetIfChanged(ref _mouseScrollUp, value);
        }

        public PluginSettings? MouseScrollDown
        {
            get => _mouseScrollDown;
            set => RaiseAndSetIfChanged(ref _mouseScrollDown, value);
        }
    }
}