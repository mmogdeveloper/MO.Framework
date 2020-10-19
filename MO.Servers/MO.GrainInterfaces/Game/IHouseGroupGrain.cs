using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IHouseGroupGrain : IGrainWithIntegerKey
    {
        Task AddUser(IUserGrain user);
        Task RemoveUser(IUserGrain user);
        Task HouseGroupNotify(MOMsg msg);
    }
}
