using FastMsg.Transport.Core.Interfaces;
using FastMsg.Transport.Core.Models.EventArgs;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace FastMsg.Transport.Core.Abstracts;

public abstract class AbstractTransportClient : ITransportClient
{
    protected bool _isDisposed;

    private readonly ILogger _logger;

    public event EventHandler Connected;
    public event EventHandler Disconnected;
    public event EventHandler<DataReceivedEventArgs> DataReceived;
    public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

    protected AbstractTransportClient(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ILogger Logger => _logger;
    public bool IsConnected { get; protected set; }

    public abstract Task ConnectAsync(CancellationToken ct = default);

    public abstract Task DisconnectAsync(CancellationToken ct = default);

    public virtual Task SendAsync(byte[] data, CancellationToken ct = default)
    {
        if (!IsConnected) throw new InvalidOperationException("Client is not connected");

        return SendDataAsync(data, ct);
    }

    public Task SendAsync<T>(T message, CancellationToken ct = default) where T : class
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        var data = SerializeMessage(message);
        return SendAsync(data, ct);
    }

    protected abstract Task SendDataAsync(byte[] data, CancellationToken ct = default);

    protected virtual void OnConnected(EventArgs e)
    {
        IsConnected = true;
        Connected?.Invoke(this, e);
    }

    protected virtual void OnDisconnected(EventArgs e)
    {
        IsConnected = false;
        Disconnected?.Invoke(this, e);
    }

    protected virtual void OnDataReceived(byte[] data)
    {
        DataReceived?.Invoke(this, new DataReceivedEventArgs
        {
            Data = data,
            ReceivedTime = DateTime.UtcNow
        });
    }

    protected virtual void OnErrorOccurred(Exception ex, string? message = null)
    {
        ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs
        {
            Exception = ex,
            ErrorMessage = message ?? ex.Message
        });

        _logger.LogError(ex, message ?? "Client error occurred");
    }

    protected virtual byte[] SerializeMessage<T>(T message) where T : class
    {
        // Default implementation uses JSON serialization
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
    }

    protected virtual T DeserializeMessage<T>(byte[] data) where T : class
    {
        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        _isDisposed = true;
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~AbstractTransportClient()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        DisconnectAsync().Wait();
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
