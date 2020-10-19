using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IRoomGrain : IGrainWithIntegerKey
    {
        Task Update();
        Task RoomNotify(MOMsg msg);

        Task Reconnect(IUserGrain user);
        Task PlayerEnterRoom(IUserGrain user);
        Task PlayerLeaveRoom(IUserGrain user);
        Task PlayerReady(IUserGrain user);
        Task PlayerSendMsg(IUserGrain user, string msg);
        Task PlayerCommand(IUserGrain user, List<CommandInfo> commands);
    }
}
