using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels;

public partial class IntegerViewModel : PluginSettingViewModel
{
    [ObservableProperty]
    private int _value;

    public bool Slider { get; }
    public int Min { get; }
    public int Max { get; }
    public int Step { get; }

    public IntegerViewModel(PluginSetting setting, PluginSettingMetadata metadata, Func<Profile, PluginSetting> binding)
        : base(setting, metadata, binding)
    {
        if (metadata.Attributes.ContainsKey("min"))
        {
            Slider = true;
            Min = int.Parse(metadata.Attributes["min"]);
            Max = int.Parse(metadata.Attributes["max"]);
            Step = int.Parse(metadata.Attributes["step"]);
        }
    }

    protected override void OnSettingChanged(PluginSetting newPluginSetting)
    {
        Value = newPluginSetting.GetValue<int>();
    }

    partial void OnValueChanged(int value)
    {
        // Min and Max validation is handled by the view
        PluginSetting.SetValue(value);
    }
}
