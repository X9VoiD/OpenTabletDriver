using System;
using OpenTabletDriver.Plugin.Settings;

#nullable enable

namespace OpenTabletDriver.Plugin.Components
{
    public class TabletSettingsProvider : ITabletSettingsProvider
    {
        private readonly IDisplayProvider _displayProvider;
        private readonly DriverDefaults _driverDefaults;

        public event EventHandler<TabletSettingsCreatedEventArgs>? TabletSettingsCreated;

        public TabletSettingsProvider(IDisplayProvider displayProvider, DriverDefaults driverDefaults)
        {
            _displayProvider = displayProvider;
            _driverDefaults = driverDefaults;
        }

        public TabletSettings GetDefaultSettings()
        {
            var virtualDisplay = _displayProvider.VirtualDisplay;
            return new TabletSettings()
            {
                OutputMode = PluginSettings.FromType(_driverDefaults.OutputMode),
                AbsoluteModeSettings = new AbsoluteModeSettings()
                {
                    Display = new AreaSettings()
                    {
                        Width = virtualDisplay.Width,
                        Height = virtualDisplay.Height,
                        X = virtualDisplay.Position.X,
                        Y = virtualDisplay.Position.Y,
                    },
                    EnableClipping = true
                },
                RelativeModeSettings = new RelativeModeSettings()
                {
                    XSensitivity = 10,
                    YSensitivity = 10,
                    ResetTime = TimeSpan.FromMilliseconds(20)
                },
                BindingSettings = new BindingSettings()
                {
                    TipButton = PluginSettings.FromType(_driverDefaults.TipBinding)
                }
            };
        }

        public TabletSettings GetInitialTabletSettings(string tabletName)
        {
            var tabletSettings = GetDefaultSettings();
            tabletSettings.TabletName = tabletName;
            TabletSettingsCreated?.Invoke(this, new TabletSettingsCreatedEventArgs(tabletSettings));
            return tabletSettings;
        }
    }

    public class TabletSettingsCreatedEventArgs : EventArgs
    {
        public TabletSettings Settings { get; }

        public TabletSettingsCreatedEventArgs(TabletSettings settings)
        {
            Settings = settings;
        }
    }
}