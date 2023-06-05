using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels;

public partial class NumberViewModel : PluginSettingViewModel
{
    [ObservableProperty]
    private float _value;

    public bool Slider { get; }
    public float Min { get; }
    public float Max { get; }
    public float Step { get; }

    public NumberViewModel(PluginSetting setting, PluginSettingMetadata metadata, Func<Profile, PluginSetting> binding)
        : base(setting, metadata, binding)
    {
        if (metadata.Attributes.ContainsKey("min"))
        {
            Slider = true;
            Min = float.Parse(metadata.Attributes["min"]);
            Max = float.Parse(metadata.Attributes["max"]);
            Step = float.Parse(metadata.Attributes["step"]);
        }
    }

    protected override void OnSettingChanged(PluginSetting newPluginSetting)
    {
        Value = newPluginSetting.GetValue<float>();
    }

    partial void OnValueChanged(float value)
    {
        // Min and Max validation is handled by the view
        PluginSetting.SetValue(value);
    }
}
