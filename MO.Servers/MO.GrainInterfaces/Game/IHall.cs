using MO.GrainInterfaces.User;
using Orleans;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IHall : IGrainWithIntegerKey
    {
        Task<Guid> JoinHall(IUser user);
        Task<Guid> LeaveHall(IUser user);
        Task HallNotify(MOMsg msg);
    }
}
