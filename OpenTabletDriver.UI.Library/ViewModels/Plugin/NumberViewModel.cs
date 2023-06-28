using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels.Plugin;

public partial class NumberViewModel : PluginSettingViewModel
{
    [ObservableProperty]
    private double _value;

    public bool Slider { get; }
    public double Min { get; }
    public double Max { get; }
    public double Step { get; }

    public NumberViewModel(PluginSetting setting, PluginSettingMetadata metadata, Func<Profile, PluginSetting> binding)
        : base(setting, metadata, binding)
    {
        if (metadata.Attributes.ContainsKey("min"))
        {
            Slider = true;
            Min = double.Parse(metadata.Attributes["min"]);
            Max = double.Parse(metadata.Attributes["max"]);
            Step = double.Parse(metadata.Attributes["step"]);
        }
    }

    protected override void OnSettingChanged(PluginSetting newPluginSetting)
    {
        Value = newPluginSetting.GetValue<double>();
    }

    partial void OnValueChanged(double value)
    {
        // Min and Max validation is handled by the view
        PluginSetting.SetValue(value);
    }
}
