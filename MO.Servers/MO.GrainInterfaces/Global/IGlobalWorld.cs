using MO.GrainInterfaces.User;
using Orleans;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
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
