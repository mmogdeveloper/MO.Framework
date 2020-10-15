using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IRoom : IGrainWithIntegerKey
    {
        Task Update();
        Task RoomNotify(MOMsg msg);

        Task Reconnect(IUser user);
        Task PlayerEnterRoom(IUser user);
        Task PlayerLeaveRoom(IUser user);
        Task PlayerReady(IUser user);
        Task PlayerSendMsg(IUser user, string msg);
        Task PlayerCommand(IUser user, List<CommandInfo> commands);
    }
}
