using System.Collections.ObjectModel;

#nullable enable

namespace OpenTabletDriver.Plugin.Settings
{
    public sealed class TabletSettings : BaseSettings
    {
        private string? _tabletName;
        private PluginSettings? _outputMode;
        private AbsoluteModeSettings? _absoluteModeSettings;
        private RelativeModeSettings? _relativeModeSettings;
        private Collection<PluginSettings>? _filters;
        private BindingSettings? _bindingSettings;

        public string? TabletName
        {
            get => _tabletName;
            set => RaiseAndSetIfChanged(ref _tabletName, value);
        }

        public PluginSettings? OutputMode
        {
            get => _outputMode;
            set => RaiseAndSetIfChanged(ref _outputMode, value);
        }

        public AbsoluteModeSettings? AbsoluteModeSettings
        {
            get => _absoluteModeSettings;
            set => RaiseAndSetIfChanged(ref _absoluteModeSettings, value);
        }

        public RelativeModeSettings? RelativeModeSettings
        {
            get => _relativeModeSettings;
            set => RaiseAndSetIfChanged(ref _relativeModeSettings, value);
        }

        public Collection<PluginSettings>? Filters
        {
            get => _filters;
            set => RaiseAndSetIfChanged(ref _filters, value);
        }

        public BindingSettings? BindingSettings
        {
            get => _bindingSettings;
            set => RaiseAndSetIfChanged(ref _bindingSettings, value);
        }
    }
}