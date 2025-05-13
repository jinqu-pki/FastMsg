using FastMsg.Transport.Core.Interfaces;
using FastMsg.Transport.Core.Models;
using FastMsg.Transport.Core.Models.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastMsg.Hosting;

internal class TransportClientWorker : BackgroundService
{
    private readonly ITransportClient _transportClient;
    private readonly ILogger<TransportClientWorker> _logger;
    private readonly IOptions<TransportClientOptions> _options;

    public TransportClientWorker(ITransportClient transportClient, ILogger<TransportClientWorker> logger, IOptions<TransportClientOptions> options)
    {
        _transportClient = transportClient;
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _transportClient.Connected += (s, e) => _logger.LogInformation("Connected to server");
        _transportClient.Disconnected += (s, e) => _logger.LogInformation("Disconnected from server");
        _transportClient.DataReceived += OnDataReceived;
        _transportClient.ErrorOccurred += OnErrorOccurred;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_transportClient.IsConnected)
                {
                    await _transportClient.ConnectAsync(stoppingToken);
                }

                await _transportClient.SendAsync(new { Message = "Hello", Time = DateTime.UtcNow }, stoppingToken);
                await Task.Delay(_options.Value.HeartbeatInterval, stoppingToken);
                await _transportClient.SendAsync(new { }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Communication error");
                await Task.Delay(_options.Value.ReconnectDelay, stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disconnecting client...");
        await _transportClient.DisconnectAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    private void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        _logger.LogInformation($"Received data: {e.Data.Length} bytes");
    }

    private void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
    {
        _logger.LogError(e.Exception, e.ErrorMessage);
    }
}
