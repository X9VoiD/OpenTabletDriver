using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.UI.Controls;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.Services;
using OpenTabletDriver.UI.ViewModels;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI;

public class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        Debug.Assert(Current == null);

        // TODO: split service registration to several extension methods
        _serviceProvider = new ServiceCollection()
            .AddSingleton<IRpcClient<IDriverDaemon>>(_ => new RpcClient<IDriverDaemon>("OpenTabletDriver.Daemon"))
            .AddSingleton<IDaemonService, DaemonService>()
            // .AddTransient<MainWindowViewModel>()
            .AddTransient<PlaygroundViewModel>()
            .AddTransient<PlaygroundView>()
            .UseNavigation<NavigationService>()
            .AddTransientNavigationRoute<PlayGContent>("PG")
            .BuildServiceProvider();

        Current = this;

        var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

        Resources.Add("NavigationService", navigationService);
        Resources.Add("NavigationValueConverter", _serviceProvider.GetRequiredService<NavigationValueConverter>());

        Dispatcher.UIThread.Post(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            navigationService.Next("PG");
        });
    }

    public new static App Current { get; private set; } = null!;

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

            // desktop.MainWindow = new MainWindowView
            // {
            //     DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>(),
            // };

            DataContext = _serviceProvider.GetRequiredService<PlaygroundViewModel>();

            desktop.MainWindow = new PlaygroundView();
            desktop.MainWindow.Bind(StyledElement.DataContextProperty, this[!DataContextProperty]);
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
