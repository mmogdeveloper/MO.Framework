using GameFramework.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Network.Actions
{
    public class Gate100006Callback: IPacketHandler
    {
        public int Id
        {
            get { return 100006; }
        }

        public void Handle(object sender, Packet packet)
        {

        }
    }
}
