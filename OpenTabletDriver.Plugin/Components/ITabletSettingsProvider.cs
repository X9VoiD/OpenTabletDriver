#nullable enable

using OpenTabletDriver.Plugin.Settings;

namespace OpenTabletDriver.Plugin.Components
{
    public interface ITabletSettingsProvider
    {
        TabletSettings GetDefaultSettings();
        TabletSettings? GetTabletDefaultSettings(string tabletName);
    }
}