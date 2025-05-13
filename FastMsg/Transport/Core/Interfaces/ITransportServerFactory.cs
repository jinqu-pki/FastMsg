using FastMsg.Transport.Core.Models;

namespace FastMsg.Transport.Core.Interfaces;

public interface ITransportServerFactory
{
    ITransportServer Create(TransportType type);
}
