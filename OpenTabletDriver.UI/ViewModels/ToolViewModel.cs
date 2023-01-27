using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels
{
    public partial class ToolViewModel : ViewModelBase
    {
        [ObservableProperty]
        private PluginDto? _descriptor;

        [ObservableProperty]
        private PluginSettings _settings;

        public ToolViewModel(PluginDto? toolDescriptor, PluginSettings toolSettings)
        {
            _descriptor = toolDescriptor;
            _settings = toolSettings;
        }
    }
}
