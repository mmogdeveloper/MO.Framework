using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IHouseGrain : IGrainWithIntegerKey
    {
        Task AddUser(IUserGrain user);
        Task RemoveUser(IUserGrain user);
        Task HouseNotify(MOMsg msg);
    }
}