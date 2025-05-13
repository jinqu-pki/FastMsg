using FastMsg.Transport.Core.Abstracts;
using FastMsg.Transport.Core.Models;
using FastMsg.Transport.Core.Models.EventArgs;
using FastMsg.Transport.Implementations.Tcp.NetCoreServer.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;
using System.Collections.Concurrent;
using System.Net;

namespace FastMsg.Transport.Implementations.Tcp.NetCoreServer;

public class NetCoreServerWrapper : AbstractTransportServer
{
    private readonly ConcurrentDictionary<string, TcpSession> _serverSessions = new();

    public NetCoreServerWrapper(IOptions<TransportServerOptions> options, ILogger<NetCoreServerWrapper> logger) : base(options, logger)
    {
        _server = new DefaultTcpServer(this, options.Value);
    }

    private TcpServer? _server;

    public TcpServer Server => _server ?? throw new InvalidOperationException("Server is not initialized.");

    public override async Task StartAsync(CancellationToken ct = default)
    {
        if (IsRunning) return;

        _server?.Start();
        IsRunning = true;
        _logger.LogInformation($"Server started on {_options.IPAddress}:{_options.Port}");
    }

    public override async Task StopAsync(CancellationToken ct = default)
    {
        if (!IsRunning) return;

        // Disconnect all sessions first
        await Task.WhenAll(_sessions.Keys.Select(sessionId => DisconnectSessionAsync(sessionId)));

        _server?.Stop();
        IsRunning = false;
        _logger.LogInformation("Server stopped");
    }

    protected override async Task DisconnectSessionInternalAsync(string sessionId, DisconnectReason reason)
    {
        if (_serverSessions.TryRemove(sessionId, out var session))
        {
            session.Disconnect();
            _logger.LogDebug($"Disconnected session {sessionId} (Reason: {reason})");
        }
    }

    internal void HandleSessionConnected(TcpSession session)
    {
        var sessionId = session.Id.ToString();
        _serverSessions[sessionId] = session;

        OnClientConnected(new ClientConnectedEventArgs()
        {
            SessionId = sessionId,
            ConnectedTime = DateTime.UtcNow,
            RemoteEndPoint = session.Socket.RemoteEndPoint as IPEndPoint,
        });
    }

    internal void HandleSessionDisconnected(TcpSession session)
    {
        var sessionId = session.Id.ToString();
        _serverSessions.TryRemove(sessionId, out _);

        OnClientDisconnected(new ClientDisconnectedEventArgs()
        {
            SessionId = sessionId,
            DisconnectedTime = DateTime.UtcNow,
            Reason = DisconnectReason.Normal,
        });
    }

    internal void HandleDataReceived(TcpSession session, byte[] buffer, long offset, long size)
    {
        var sessionId = session.Id.ToString();
        var data = new byte[size];
        Array.Copy(buffer, offset, data, 0, size);

        OnDataReceived(new DataReceivedEventArgs()
        {
            SessionId = sessionId,
            Data = data,
            ReceivedTime = DateTime.UtcNow,
        });
    }

    protected override async Task SendDataAsync(string sessionId, byte[] data, CancellationToken ct = default)
    {
        if (_serverSessions.TryGetValue(sessionId, out var serverSession))
        {
            try
            {
                serverSession.SendAsync(data);

                if (_sessions.TryGetValue(sessionId, out var session))
                {
                    session.BytesSent += data.Length;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending data to session {sessionId}");
                await DisconnectSessionInternalAsync(sessionId, DisconnectReason.Error);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed) { return; }

        if (disposing)
        {
            // Dispose managed resources
            _server?.Dispose();
            _server = null;
        }

        base.Dispose(disposing);

        _isDisposed = true;
    }
}
