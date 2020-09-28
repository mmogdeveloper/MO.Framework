using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Global
{
    public interface IGlobalWorld : IGrainWithIntegerKey
    {
        Task Notify(MOMsg msg);
        Task<int> GetCount();
        Task PlayerEnterGlobalWorld(IUser user);
        Task PlayerLeaveGlobalWorld(IUser user);
    }
}
