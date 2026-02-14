using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Network
{
    public interface IPacketObserver : IGrainObserver
    {
        Task SendPacket(MOMsg packet);
        Task Close(MOMsg packet = null);
    }
}
