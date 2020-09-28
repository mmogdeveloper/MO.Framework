using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IRoomFactory : IGrainWithIntegerKey
    {
        Task<int> CreateRoom(ProtoRoomInfo roomInfo);
        Task ReleaseRoom(int roomId);
        Task<ProtoRoomInfo> GetRoomInfo(int roomId);
        Task<bool> RoomExist(int roomId);
    }
}
