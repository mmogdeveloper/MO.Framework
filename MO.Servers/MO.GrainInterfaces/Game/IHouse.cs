using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface IHouse : IGrainWithIntegerKey
    {
        Task AddUser(IUser user);
        Task RemoveUser(IUser user);
        Task HouseNotify(MOMsg msg);
    }
}