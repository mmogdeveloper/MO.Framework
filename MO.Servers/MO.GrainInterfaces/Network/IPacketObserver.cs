using Orleans;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MO.GrainInterfaces.Network
{
    public interface IPacketObserver : IGrainObserver
    {
        void SendPacket(MOMsg packet);
        void Close(MOMsg packet = null);
    }
}
