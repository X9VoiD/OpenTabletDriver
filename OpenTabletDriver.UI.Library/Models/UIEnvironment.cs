namespace OpenTabletDriver.UI.Models;

public class UIEnvironment
{
    /// <summary>
    /// Gets an array of arguments passed to the application.
    /// </summary>
    public string[] Args { get; }

    /// <summary>
    /// Gets the path to the application's data directory.
    /// </summary>
    public string AppDataPath { get; }

    public UIEnvironment(string[] args, string appDataPath)
    {
        Args = args;
        AppDataPath = appDataPath;

        if (!Directory.Exists(appDataPath))
            Directory.CreateDirectory(appDataPath);
    }

    public static UIEnvironment Create(string[] args)
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OpenTabletDriver.UI"
        );
        return new UIEnvironment(args, appDataPath);
    }
}
