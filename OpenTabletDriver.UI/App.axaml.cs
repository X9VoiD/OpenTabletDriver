using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.UI.Services;
using OpenTabletDriver.UI.ViewModels;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI;

public class App : Application
{
    private IServiceProvider _serviceProvider;

    public App()
    {
        Debug.Assert(Current == null);

        _serviceProvider = new ServiceCollection()
            .AddSingleton<DaemonService>(_ =>
            {
                var rpcClient = new RpcClient<IDriverDaemon>("OpenTabletDriver.Daemon");
                return DaemonService.FromRpc(rpcClient);
            })
            .AddSingleton<MainWindowViewModel>()
            .BuildServiceProvider();

        Current = this;
    }

    public new static App? Current { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            RemoveAvaloniaValidationPlugin();

            desktop.MainWindow = new MainWindowView
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void RemoveAvaloniaValidationPlugin()
    {
        var pluginToRemove = Type.GetType("Avalonia.Data.Core.Plugins.DataAnnotationsValidationPlugin, Avalonia.Base", true);
        var dataValidators = BindingPlugins.DataValidators;

        for (var i = 0; i < dataValidators.Count;)
        {
            var pluginType = dataValidators[i].GetType();
            if (pluginType.IsAssignableTo(pluginToRemove))
            {
                dataValidators.RemoveAt(i);
                continue;
            }

            i++;
        }
    }
}
