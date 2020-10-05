using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    public interface IUserIdFactory : IGrainWithIntegerKey
    {
        Task<Int64> GetNewUserId();
    }
}
