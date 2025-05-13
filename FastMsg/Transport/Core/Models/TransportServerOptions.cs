using System.Net;

namespace FastMsg.Transport.Core.Models;

public sealed class TransportServerOptions
{
    public TransportType Type { get; set; } = TransportType.TcpNetCoreServer;

    public IPAddress IPAddress { get; set; } = IPAddress.Loopback;
    public int Port { get; set; } = 6566;

    public bool NoDelay { get; set; } = true;
    public int SendBufferSize { get; set; } = 1024 * 1024; // 1 MB
    public int ReceiveBufferSize { get; set; } = 1024 * 1024; // 1 MB
    public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
