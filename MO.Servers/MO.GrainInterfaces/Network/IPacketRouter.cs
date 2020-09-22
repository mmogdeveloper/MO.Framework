using Google.Protobuf;
using Orleans;
using ProtoMessage;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Network
{
    public interface IPacketRouter : IGrainWithGuidKey
    {
        Task SendPacket(MOMsg packet);
        Task Disconnect();
    }
}
