using System.Net;

namespace FastMsg.Transport.Core.Models.EventArgs;

public class ClientConnectedEventArgs : System.EventArgs
{
    public string SessionId { get; set; }
    public DateTime ConnectedTime { get; set; } = DateTime.UtcNow;
    public IPEndPoint RemoteEndPoint { get; set; }
}