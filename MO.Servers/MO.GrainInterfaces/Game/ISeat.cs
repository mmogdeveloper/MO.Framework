using MO.GrainInterfaces.User;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface ISeat : IGrainWithIntegerKey
    {
        Task SitDown(IUser user);
        Task SitUp(IUser user);
    }
}
