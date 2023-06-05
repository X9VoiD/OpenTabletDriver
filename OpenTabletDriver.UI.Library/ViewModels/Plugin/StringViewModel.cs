using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels;

public partial class StringViewModel : PluginSettingViewModel
{
    [ObservableProperty]
    private string? _value;

    public StringViewModel(PluginSetting setting, PluginSettingMetadata metadata, Func<Profile, PluginSetting> binding)
        : base(setting, metadata, binding)
    {
    }

    protected override void OnSettingChanged(PluginSetting newPluginSetting)
    {
        Value = newPluginSetting.GetValue<string>();
    }

    partial void OnValueChanged(string? value)
    {
        PluginSetting.SetValue(value);
    }
}
