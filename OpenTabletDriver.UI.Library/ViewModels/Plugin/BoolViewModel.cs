using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels;

public partial class BoolViewModel : PluginSettingViewModel
{
    [ObservableProperty]
    private bool _value;

    public BoolViewModel(PluginSetting setting, PluginSettingMetadata metadata, Func<Profile, PluginSetting> binding)
        : base(setting, metadata, binding)
    {
    }

    protected override void OnSettingChanged(PluginSetting newPluginSetting)
    {
        Value = newPluginSetting.GetValue<bool>();
    }

    partial void OnValueChanged(bool value)
    {
        PluginSetting.SetValue(value);
    }
}
