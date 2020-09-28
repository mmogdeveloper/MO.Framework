using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IRoom : IGrainWithIntegerKey
    {
        Task RoomNotify(MOMsg msg);

        Task Reconnect(IUser user);
        Task PlayerEnterRoom(IUser user);
        Task PlayerLeaveRoom(IUser user);
        Task PlayerReady(IUser user);
        Task PlayerGo(IUser user, double x, double y);
    }
}
