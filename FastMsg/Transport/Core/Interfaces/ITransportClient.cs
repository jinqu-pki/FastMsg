using FastMsg.Transport.Core.Models.EventArgs;

namespace FastMsg.Transport.Core.Interfaces;

public interface ITransportClient : IDisposable
{
    #region Connection management
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);
    #endregion

    #region Data transmission
    Task SendAsync(byte[] data, CancellationToken ct);
    Task SendAsync<T>(T message, CancellationToken ct) where T : class;
    #endregion

    #region Events
    event EventHandler Connected;
    event EventHandler Disconnected;
    event EventHandler<DataReceivedEventArgs> DataReceived;
    event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;
    #endregion
}
