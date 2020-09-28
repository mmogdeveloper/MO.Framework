using MO.Protocol;
using Orleans;

namespace MO.GrainInterfaces.Network
{
    public interface IPacketObserver : IGrainObserver
    {
        void SendPacket(MOMsg packet);
        void Close(MOMsg packet = null);
    }
}
