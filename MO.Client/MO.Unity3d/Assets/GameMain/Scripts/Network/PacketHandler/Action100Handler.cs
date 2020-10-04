using GameFramework.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Network.PacketHandler
{
    public class Action100Handler : IPacketHandler
    {
        public int Id
        {
            get { return 100; }
        }

        public void Handle(object sender, Packet packet)
        {

        }
    }
}
