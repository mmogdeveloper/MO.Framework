using GameFramework.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Network.Actions
{
    public class Action100Callback: IPacketHandler
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
