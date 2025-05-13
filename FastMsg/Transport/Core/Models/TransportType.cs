namespace FastMsg.Transport.Core.Models;

public enum TransportType
{
    TcpNetCoreServer,  // 基于NetCoreServer的TCP
    TcpSystemSockets,  // 基于System.Net.Sockets的TCP
    UdpSystem,         // 系统UDP实现
    WebSocket,         // WebSocket
    InMemory,          // 内存传输(用于测试)
    QUIC               // 未来扩展支持
}