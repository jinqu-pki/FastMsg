using FastMsg.Transport.Core.Models;
using FastMsg.Transport.Core.Models.EventArgs;

namespace FastMsg.Transport.Core.Interfaces;

public interface ITransportServer : IDisposable
{
    #region Ctrl
    // Server status
    bool IsRunning { get; }
    // Start the server
    Task StartAsync(CancellationToken ct = default);
    // Stop the server
    Task StopAsync(CancellationToken ct = default);
    #endregion

    #region Events
    // New client connection event
    event EventHandler<ClientConnectedEventArgs> ClientConnected;
    // Client disconnection event
    event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
    // Data received event
    event EventHandler<DataReceivedEventArgs> DataReceived;
    #endregion

    #region Configuration
    TransportServerOptions Options { get; }
    #endregion

    #region Data Transmission
    // Send data to specific client
    Task SendAsync(string sessionId, byte[] data, CancellationToken ct = default);
    // Generic version with auto-serialization
    Task SendAsync<T>(string sessionId, T message, CancellationToken ct = default) where T : class;
    // Broadcast to all clients
    Task BroadcastAsync(byte[] data, CancellationToken ct = default);
    // Generic broadcast
    Task BroadcastAsync<T>(T message, CancellationToken ct = default) where T : class;
    // Send to specific group
    Task MulticastAsync(IEnumerable<string> sessionIds, byte[] data, CancellationToken ct = default);
    #endregion

    #region Session Management
    // Get all active sessions
    IReadOnlyCollection<string> ActiveSessions { get; }

    // Force disconnect specific session
    Task DisconnectSessionAsync(string sessionId);

    // Get session information
    SessionInfo? GetSessionInfo(string sessionId);
    #endregion
}
