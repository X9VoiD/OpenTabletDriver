using System;
using System.IO;
using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.Tests.Models;

public class UISettingsProviderTests
{
    private const string DefaultSettings = """
    {
        "FirstLaunch": true,
        "AutoStart": false,
        "Kaomoji": true
    }
    """;

    [Fact]
    public void SettingsStartLoadingOnConstruct()
    {
        using var environment = new EnvWithSettings(DefaultSettings);
        var settingsProvider = new UISettingsProvider(environment.Environment);
    }

    private class EnvWithSettings : IDisposable
    {
        public EnvWithSettings(string uiSettings)
        {
            string tempDir;
            do
            {
                tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
            while(Directory.Exists(tempDir));

            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "ui-settings.json"), uiSettings);
            Environment = new UIEnvironment(new string[0], tempDir);
        }

        public UIEnvironment Environment { get; }

        public void Dispose()
        {
            Directory.Delete(Environment.AppDataPath, true);
        }
    }
}
