using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels.Plugin;

public partial class EnumViewModel : PluginSettingViewModel
{
    [ObservableProperty]
    private string? _selectedValue;

    public string[] Choices { get; }

    public EnumViewModel(PluginSetting setting, PluginSettingMetadata metadata, Func<Profile, PluginSetting> binding)
        : base(setting, metadata, binding)
    {
        Choices = metadata.Attributes["choices"].Split(';');
    }

    protected override void OnSettingChanged(PluginSetting newPluginSetting)
    {
        SelectedValue = newPluginSetting.GetValue<string>();
    }

    partial void OnSelectedValueChanged(string? value)
    {
        PluginSetting.SetValue(value);
    }
}
