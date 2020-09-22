using Orleans;
using ProtoMessage;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Network
{
    public interface IClientboundPacketSink : IGrainWithGuidKey
    {
        Task Subscribe(IClientboundPacketObserver observer);

        Task UnSubscribe(IClientboundPacketObserver observer);

        Task SendPacket(MOMsg packet);

        Task Close();
    }
}
