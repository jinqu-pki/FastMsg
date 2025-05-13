namespace FastMsg.Transport.Core.Models.EventArgs;

public class DataReceivedEventArgs : System.EventArgs
{
    public string SessionId { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public DateTime ReceivedTime { get; set; } = DateTime.UtcNow;
}
