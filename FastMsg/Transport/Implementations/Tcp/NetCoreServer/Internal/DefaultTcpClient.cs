using FastMsg.Transport.Core.Models;
using NetCoreServer;

namespace FastMsg.Transport.Implementations.Tcp.NetCoreServer.Internal;

internal class DefaultTcpClient : TcpClient
{
    private readonly NetCoreClientWrapper _wrapper;
    private readonly TransportClientOptions _options;

    public DefaultTcpClient(NetCoreClientWrapper wrapper, TransportClientOptions options) : base(options.Address, options.Port)
    {
        _wrapper = wrapper;

        OptionNoDelay = options.NoDelay;
        OptionSendBufferSize = options.SendBufferSize;
        OptionReceiveBufferSize = options.ReceiveBufferSize;
    }

    protected override void OnConnected()
    {
        _wrapper.HandleConnected();
    }

    protected override void OnDisconnected()
    {
        _wrapper.HandleDisconnected();
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        _wrapper.HandleDataReceived(buffer, offset, size);
    }
}
