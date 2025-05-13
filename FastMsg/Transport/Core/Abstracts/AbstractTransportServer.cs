using FastMsg.Transport.Core.Interfaces;
using FastMsg.Transport.Core.Models;
using FastMsg.Transport.Core.Models.EventArgs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace FastMsg.Transport.Core.Abstracts;

public abstract class AbstractTransportServer : ITransportServer
{
    protected readonly ILogger _logger;
    protected readonly TransportServerOptions _options;
    protected readonly ConcurrentDictionary<string, SessionInfo> _sessions = new();

    public event EventHandler<ClientConnectedEventArgs> ClientConnected;
    public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
    public event EventHandler<DataReceivedEventArgs> DataReceived;

    public bool IsRunning { get; protected set; }
    public ILogger Logger => _logger;
    public TransportServerOptions Options => _options;
    public IReadOnlyCollection<string> ActiveSessions => _sessions.Keys.ToList().AsReadOnly();

    protected AbstractTransportServer(IOptions<TransportServerOptions> options, ILogger logger)
    {
        _options = options?.Value ?? new TransportServerOptions();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract Task StartAsync(CancellationToken ct = default);
    public abstract Task StopAsync(CancellationToken ct = default);

    public virtual Task SendAsync(string sessionId, byte[] data, CancellationToken ct = default)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            return SendDataAsync(sessionId, data, ct);
        }

        return Task.CompletedTask;
    }

    public virtual Task SendAsync<T>(string sessionId, T message, CancellationToken ct = default) where T : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        var data = SerializeMessage(message);
        return SendAsync(sessionId, data, ct);
    }

    public virtual Task BroadcastAsync(byte[] data, CancellationToken ct = default)
    {
        var tasks = _sessions.Keys.Select(sessionId => SendDataAsync(sessionId, data));
        return Task.WhenAll(tasks);
    }

    public virtual Task BroadcastAsync<T>(T message, CancellationToken ct = default) where T : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        var data = SerializeMessage(message);
        return BroadcastAsync(data);
    }

    public virtual Task MulticastAsync(IEnumerable<string> sessionIds, byte[] data, CancellationToken ct = default)
    {
        var tasks = sessionIds
            .Where(id => _sessions.ContainsKey(id))
            .Select(sessionId => SendDataAsync(sessionId, data));
        return Task.WhenAll(tasks);
    }

    public Task DisconnectSessionAsync(string sessionId)
    {
        return DisconnectSessionInternalAsync(sessionId, DisconnectReason.Forced);
    }

    protected abstract Task DisconnectSessionInternalAsync(string sessionId, DisconnectReason forced);

    public virtual SessionInfo? GetSessionInfo(string sessionId)
    {
        return _sessions.TryGetValue(sessionId, out var session) ? session : null;
    }

    protected abstract Task SendDataAsync(string sessionId, byte[] data, CancellationToken ct = default);

    protected virtual byte[] SerializeMessage<T>(T message) where T : class
    {
        // Default implementation uses JSON serialization
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
    }

    protected virtual T DeserializeMessage<T>(byte[] data) where T : class
    {
        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
    }

    protected virtual void OnClientConnected(ClientConnectedEventArgs e)
    {
        _sessions[e.SessionId] = new SessionInfo
        {
            SessionId = e.SessionId,
            RemoteEndPoint = e.RemoteEndPoint,
            ConnectedTime = e.ConnectedTime
        };

        ClientConnected?.Invoke(this, e);
        _logger.LogInformation($"Client connected: {e.SessionId} from {e.RemoteEndPoint}");
    }

    protected virtual void OnClientDisconnected(ClientDisconnectedEventArgs e)
    {
        _sessions.TryRemove(e.SessionId, out _);

        ClientDisconnected?.Invoke(this, e);
        _logger.LogInformation($"Client disconnected: {e.SessionId} (Reason: {e.Reason})");
    }

    protected virtual void OnDataReceived(DataReceivedEventArgs e)
    {
        if (_sessions.TryGetValue(e.SessionId, out var session))
        {
            session.BytesReceived += e.Data.Length;
        }

        DataReceived?.Invoke(this, e);
    }

    #region Dispose
    protected bool _isDisposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
        }

        _isDisposed = true;
    }
    #endregion
}
