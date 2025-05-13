using FastMsg.Transport.Core.Models;
using Microsoft.Extensions.Logging;
using NetCoreServer;
using System.Net.Sockets;

namespace FastMsg.Transport.Implementations.Tcp.NetCoreServer.Internal;

internal class DefaultTcpServer : TcpServer
{
    private readonly NetCoreServerWrapper _wrapper;
    private readonly TransportServerOptions _options;

    public DefaultTcpServer(NetCoreServerWrapper wrapper, TransportServerOptions options) : base(options.IPAddress, options.Port)
    {
        _wrapper = wrapper;
        _options = options;

        OptionNoDelay = options.NoDelay;
        OptionSendBufferSize = options.SendBufferSize;
        OptionReceiveBufferSize = options.ReceiveBufferSize;
    }

    protected override TcpSession CreateSession() => new DefaultTcpSession(_wrapper);

    protected override void OnError(SocketError error)
    {
        _wrapper.Logger.LogError($"Server socket error: {error}");
        base.OnError(error);
    }
}
