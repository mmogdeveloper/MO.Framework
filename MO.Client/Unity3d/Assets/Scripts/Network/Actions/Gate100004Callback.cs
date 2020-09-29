using GameFramework.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGameFramework.Runtime;

namespace Assets.Scripts.Network.Actions
{
    public class Gate100004Callback: IPacketHandler
    {
        public int Id
        {
            get { return 100004; }
        }

        public void Handle(object sender, Packet packet)
        {
            Log.Info("100004Callback");
        }
    }
}
