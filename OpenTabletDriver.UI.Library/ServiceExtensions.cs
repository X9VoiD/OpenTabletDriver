using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI;

public static class ServiceExtensions
{
    public static IServiceCollection AddStartupJob<T>(this IServiceCollection services)
        where T : class, IStartupJob
    {
        return services.AddTransient<IStartupJob, T>();
    }

    public static IServiceCollection WithDefaultServices(this IServiceCollection services)
    {
        return services
            .AddTransient<App>()
            .AddSingleton<AppDataContext>()
            .AddSingleton<IRpcClient<IDriverDaemon>>(_ => new RpcClient<IDriverDaemon>("OpenTabletDriver.Daemon"))
            .AddSingleton<IDaemonService, DaemonService>()
            .UseNavigation<NavigationService>();
    }
}
