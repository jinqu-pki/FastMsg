using FastMsg.Transport.Core.Models;

namespace FastMsg.Transport.Core.Models.EventArgs;

public class ClientDisconnectedEventArgs : System.EventArgs
{
    public string SessionId { get; set; }
    public DateTime DisconnectedTime { get; set; } = DateTime.UtcNow;
    public DisconnectReason Reason { get; set; } = DisconnectReason.Normal;
}
