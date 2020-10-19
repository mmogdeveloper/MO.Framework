using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IHallGrain : IGrainWithIntegerKey
    {
        Task<Guid> JoinHall(IUserGrain user);
        Task<Guid> LeaveHall(IUserGrain user);
        Task HallNotify(MOMsg msg);
    }
}
