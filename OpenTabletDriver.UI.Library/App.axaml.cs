using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI;

public class App : Application
{
    private IEnumerable<IStartupJob>? _startupJobs;

    public App(IServiceProvider provider, IEnumerable<IStartupJob> startupJobs)
    {
        Ioc.Default.ConfigureServices(provider); // allow use of Ioc.Default for DI
        _startupJobs = startupJobs;

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            ExceptionDispatchInfo.Throw(e.Exception); // forcibly crash
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

    public override void OnFrameworkInitializationCompleted()
    {
        RemoveAvaloniaValidationPlugin();
        RunStartupJobs();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindowView();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void RunStartupJobs()
    {
        if (_startupJobs is null)
            return;

        foreach (var job in _startupJobs)
            job.Run();

        _startupJobs = null;
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
