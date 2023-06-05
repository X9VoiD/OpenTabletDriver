using OpenTabletDriver.Daemon.Contracts;

namespace OpenTabletDriver.UI.ViewModels;

public abstract class PluginSettingViewModel : ViewModelBase
{
    private readonly Func<Profile, PluginSetting> _binding;
    protected PluginSetting PluginSetting { get; private set; }
    protected PluginSettingMetadata Metadata { get; }

    public string FriendlyName => Metadata.FriendlyName;
    public string? Description => Metadata.ShortDescription;
    public string? ToolTip => Metadata.LongDescription;

    protected PluginSettingViewModel(PluginSetting setting, PluginSettingMetadata metadata, Func<Profile, PluginSetting> binding)
    {
        _binding = binding;
        PluginSetting = setting;
        Metadata = metadata;
    }

    public void ChangeContext(Profile profile)
    {
        var oldPluginSetting = PluginSetting;
        PluginSetting = _binding(profile);
        OnSettingChanged(PluginSetting);
    }

    protected abstract void OnSettingChanged(PluginSetting newPluginSetting);

    public static PluginSettingViewModel? CreateBindable(PluginDto pluginDto, Profile profile, Func<Profile, PluginSetting> binding)
    {
        var pluginSetting = binding(profile);
        var sourceProperty = pluginSetting.Property;
        var metadata = pluginDto.SettingsMetadata.FirstOrDefault(x => x.PropertyName == sourceProperty);
        if (metadata == null)
            return null;

        return metadata.Type switch
        {
            SettingType.Boolean => new BoolViewModel(pluginSetting, metadata, binding),
            SettingType.Enum => new EnumViewModel(pluginSetting, metadata, binding),
            SettingType.Integer => new IntegerViewModel(pluginSetting, metadata, binding),
            SettingType.Number => new NumberViewModel(pluginSetting, metadata, binding),
            SettingType.String => new StringViewModel(pluginSetting, metadata, binding),
            _ => null
        };
    }
}
