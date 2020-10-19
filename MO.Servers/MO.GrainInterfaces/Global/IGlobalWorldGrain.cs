using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Global
{
    public interface IGlobalWorldGrain : IGrainWithIntegerKey
    {
        Task Notify(MOMsg msg);
        Task<int> GetCount();
        Task PlayerEnterGlobalWorld(IUserGrain user);
        Task PlayerLeaveGlobalWorld(IUserGrain user);
    }
}
