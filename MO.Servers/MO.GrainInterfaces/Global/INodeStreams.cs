using Orleans;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Global
{
    public interface INodeStreams : IGrainWithIntegerKey
    {
        Task PublishStream(MOMsg msg);
    }
}
