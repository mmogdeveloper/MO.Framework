using MO.GrainInterfaces.User;
using Orleans;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IRoom : IGrainWithIntegerKey
    {
        Task RoomNotify(MOMsg msg);

        Task PlayerEnterRoom(IUser user);
        Task PlayerLeaveRoom(IUser user);
        Task PlayerReady(IUser user);
        Task PlayerGo(IUser user, Int64 x, Int64 y);
    }
}
