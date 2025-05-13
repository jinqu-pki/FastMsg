using FastMsg.Transport.Core.Abstracts;
using FastMsg.Transport.Core.Models;
using FastMsg.Transport.Implementations.Tcp.NetCoreServer.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;

namespace FastMsg.Transport.Implementations.Tcp.NetCoreServer;

public class NetCoreClientWrapper : AbstractTransportClient
{
    private TcpClient? _client;

    public NetCoreClientWrapper(IOptions<TransportClientOptions> options, ILogger<NetCoreClientWrapper> logger) : base(logger)
    {
        _client = new DefaultTcpClient(this, options.Value);
    }

    public override async Task ConnectAsync(CancellationToken ct = default)
    {
        if (IsConnected) return;
        _client?.ConnectAsync();
        IsConnected = true;
    }

    public override async Task DisconnectAsync(CancellationToken ct = default)
    {
        if (!IsConnected) return;
        _client?.DisconnectAsync();
        IsConnected = false;
    }

    protected override async Task SendDataAsync(byte[] data, CancellationToken ct = default)
    {
        try
        {
            _client.SendAsync(data);
        }
        catch (Exception ex)
        {
            OnErrorOccurred(ex, "Error sending data");
            await DisconnectAsync(ct);
            throw;
        }
    }

    internal void HandleConnected()
    {
        OnConnected(EventArgs.Empty);
    }

    internal void HandleDisconnected()
    {
        OnDisconnected(EventArgs.Empty);
    }

    internal void HandleDataReceived(byte[] buffer, long offset, long size)
    {
        var data = new byte[size];
        Array.Copy(buffer, offset, data, 0, size);

        OnDataReceived(data);
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            _client?.Dispose();
            _client = null;
        }

        base.Dispose(disposing);
        _isDisposed = true;
    }
}
