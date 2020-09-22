using Orleans;
using ProtoMessage;

namespace MO.GrainInterfaces.Network
{
    public interface IClientboundPacketObserver : IGrainObserver
    {
        void ReceivePacket(MOMsg packet);

        void OnClosed();
    }
}
