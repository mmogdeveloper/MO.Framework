using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.Global;
using MO.GrainInterfaces.Network;
using Orleans;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    public interface IUser : IGrainWithIntegerKey
    {
        Task BindPacketObserver(IPacketObserver observer);
        Task UnbindPacketObserver();

        Task Notify(MOMsg packet);

        Task SubscribeGlobal(Guid streamId);
        Task UnsubscribeGlobal();

        Task SubscribeRoom(Guid streamId);
        Task UnsubscribeRoom();

        Task SetRoomId(int roomId);
        Task<int> GetRoomId();
    }
}
