using FastMsg.Hosting;
using FastMsg.Transport.Core.Interfaces;
using FastMsg.Transport.Core.Models;
using FastMsg.Transport.Implementations.Tcp.NetCoreServer;
using Microsoft.Extensions.DependencyInjection;

namespace FastMsg.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransportServer(this IServiceCollection services, Action<TransportServerOptions> configureOptions = null)
    {
        services.AddOptions<TransportServerOptions>()
            .Configure(configureOptions ?? (opt => { }));

        services.AddSingleton<ITransportServer, NetCoreServerWrapper>();

        return services;
    }

    public static IServiceCollection AddTransportServerWithHostedService(this IServiceCollection services, Action<TransportServerOptions> configureOptions = null)
    {
        services.AddTransportServer(configureOptions);
        services.AddHostedService<TransportServerHostedService>();

        return services;
    }

    public static IServiceCollection AddTransportClient(this IServiceCollection services, Action<TransportClientOptions> configureOptions = null)
    {
        services.AddOptions<TransportClientOptions>()
            .Configure(configureOptions ?? (opt => { }));
        services.AddSingleton<ITransportClient, NetCoreClientWrapper>();
        return services;
    }

    public static IServiceCollection AddTransportClientWithHostedService(this IServiceCollection services, Action<TransportClientOptions> configureOptions = null)
    {
        services.AddTransportClient(configureOptions);
        services.AddHostedService<TransportClientWorker>();
        return services;
    }
}
