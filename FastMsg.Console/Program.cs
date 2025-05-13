using FastMsg.Extensions;
using FastMsg.Transport.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Debug));
        services.Configure<TransportServerOptions>(options =>
        {
            options.IPAddress = IPAddress.Any;
            options.Port = 6566;
            options.NoDelay = true;
            options.Type = TransportType.TcpNetCoreServer;
        });

        services.AddTransportServerWithHostedService();
    }).UseConsoleLifetime().Build();

await host.RunAsync();
