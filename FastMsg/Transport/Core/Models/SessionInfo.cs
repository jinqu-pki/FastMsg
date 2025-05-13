using System.Net;

namespace FastMsg.Transport.Core.Models;

public class SessionInfo
{
    public string SessionId { get; set; }
    public IPEndPoint RemoteEndPoint { get; set; }
    public DateTime ConnectedTime { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
}
