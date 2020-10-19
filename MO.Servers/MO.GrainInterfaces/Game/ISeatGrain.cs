using MO.GrainInterfaces.User;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Game
{
    public interface ISeatGrain : IGrainWithIntegerKey
    {
        Task SitDown(IUserGrain user);
        Task SitUp(IUserGrain user);
    }
}
