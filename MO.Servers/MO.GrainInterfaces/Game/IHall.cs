using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System;
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
