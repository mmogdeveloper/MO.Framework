using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IHallGrain : IGrainWithIntegerKey
    {
        Task<string> JoinHall(IUserGrain user);
        Task<string> LeaveHall(IUserGrain user);
        Task HallNotify(MOMsg msg);
    }
}
