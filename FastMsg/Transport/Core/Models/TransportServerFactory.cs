using FastMsg.Transport.Core.Interfaces;
using FastMsg.Transport.Implementations.Tcp.NetCoreServer;
using Microsoft.Extensions.DependencyInjection;

namespace FastMsg.Transport.Core.Models;

public class TransportServerFactory : ITransportServerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TransportServerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITransportServer Create(TransportType type)
    {
        return type switch
        {
            TransportType.TcpNetCoreServer => _serviceProvider.GetRequiredService<NetCoreServerWrapper>(),
            //TransportType.TcpSystemSockets => new SystemSocketsTcpAdapter(),
            //TransportType.UdpSystem => new SystemUdpTransport(),
            //TransportType.WebSocket => new WebSocketTransport(),
            //TransportType.InMemory => new InMemoryTransport(),
            _ => throw new NotSupportedException($"Transport type {type} is not supported")
        };
    }
}