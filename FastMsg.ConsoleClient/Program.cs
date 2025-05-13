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
        services.Configure<TransportClientOptions>(options =>
        {
            options.Address = IPAddress.Loopback.ToString();
            options.Port = 6566;
            options.NoDelay = true;
        });

        services.AddTransportClientWithHostedService();
    }).UseConsoleLifetime().Build();

await host.RunAsync();
