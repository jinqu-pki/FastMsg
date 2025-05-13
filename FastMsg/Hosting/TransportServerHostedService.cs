using FastMsg.Transport.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FastMsg.Hosting;

internal class TransportServerHostedService : IHostedService
{
    private readonly ITransportServer _transportServer;
    private readonly ILogger<TransportServerHostedService> _logger;

    public TransportServerHostedService(ITransportServer transportServer, ILogger<TransportServerHostedService> logger)
    {
        _transportServer = transportServer;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _transportServer.StartAsync(cancellationToken);

        _transportServer.ClientConnected += (s, e) =>
        {
            _logger.LogInformation($"Client connected: {e.SessionId}");
        };

        _transportServer.ClientDisconnected += (s, e) =>
        {
            _logger.LogInformation($"Client disconnected: {e.SessionId}");
        };

        _transportServer.DataReceived += async (s, e) =>
        {
            _logger.LogInformation($"Data received from {e.SessionId}: {Encoding.UTF8.GetString(e.Data)} ");
            // Process the received data here
            // For example, echo it back to the client
            await _transportServer.SendAsync(e.SessionId, e.Data, cancellationToken);
        };
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _transportServer.StopAsync(cancellationToken);
    }
}
