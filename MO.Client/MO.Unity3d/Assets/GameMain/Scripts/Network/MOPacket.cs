using GameFramework.Network;
using MO.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Network
{
    public class MOPacket : Packet
    {
        public override int Id
        {
            get
            {
                return Packet.ActionId;
            }
        }
        public MOMsg Packet { get;private set; }

        public MOPacket(MOMsg msg)
        {
            Packet = msg;
        }

        public override void Clear()
        {
            Packet = null;
        }
    }
}
