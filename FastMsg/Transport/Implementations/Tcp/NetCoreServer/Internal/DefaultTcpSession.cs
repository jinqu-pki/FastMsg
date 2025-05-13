using Microsoft.Extensions.Logging;
using NetCoreServer;
using System.Net.Sockets;

namespace FastMsg.Transport.Implementations.Tcp.NetCoreServer.Internal;

internal class DefaultTcpSession : TcpSession
{
    private readonly NetCoreServerWrapper _wrapper;

    public DefaultTcpSession(NetCoreServerWrapper _wrapper) : base(_wrapper.Server)
    {
        this._wrapper = _wrapper;
    }

    protected override void OnConnected()
    {
        _wrapper.HandleSessionConnected(this);
    }

    protected override void OnDisconnected()
    {
        _wrapper.HandleSessionDisconnected(this);
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        _wrapper.HandleDataReceived(this, buffer, offset, size);
    }

    protected override void OnError(SocketError error)
    {
        _wrapper.Logger.LogError($"Session socket error: {error}");
        base.OnError(error);
    }
}
