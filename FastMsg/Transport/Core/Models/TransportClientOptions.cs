using System.Net;

namespace FastMsg.Transport.Core.Models;

public sealed class TransportClientOptions
{
    public string Address { get; set; } = IPAddress.Loopback.ToString();

    public int Port { get; set; } = 6566;
    public bool NoDelay { get; set; } = true;
    public int SendBufferSize { get; set; } = 1024 * 1024; // 1 MB
    public int ReceiveBufferSize { get; set; } = 1024 * 1024; // 1 MB
    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(30);

}
