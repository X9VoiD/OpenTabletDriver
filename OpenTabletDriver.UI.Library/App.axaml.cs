using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI;

public class App : Application
{
    private IEnumerable<IStartupJob>? _startupJobs;

    public App(AppDataContext appContext, IEnumerable<IStartupJob> startupJobs)
    {
        _startupJobs = startupJobs;
        DataContext = appContext;

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Debug.WriteLine("aaaaa");
            Debug.WriteLine(e.Exception);
        };
    }

    public App()
    {
        Debug.Fail("Should never be called by user-code");
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void RunStartupJobs()
    {
        if (_startupJobs is null)
            return;

        foreach (var job in _startupJobs)
            job.Run();

        _startupJobs = null;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        RemoveAvaloniaValidationPlugin();
        RunStartupJobs();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindowView
            {
                DataContext = ((AppDataContext)DataContext!).MainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void RemoveAvaloniaValidationPlugin()
    {
        var pluginToRemove = Type.GetType("Avalonia.Data.Core.Plugins.DataAnnotationsValidationPlugin, Avalonia.Base", true);
        var dataValidators = BindingPlugins.DataValidators;

        for (var i = 0; i < dataValidators.Count; i++)
        {
            var pluginType = dataValidators[i].GetType();
            if (!pluginType.IsAssignableTo(pluginToRemove))
                continue;

            dataValidators.RemoveAt(i);
            return;
        }
    }
}
