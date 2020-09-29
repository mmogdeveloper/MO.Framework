using GameFramework.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Network.Actions
{
    /// <summary>
    /// 心跳包
    /// </summary>
    public class Gate1Callback : IPacketHandler
    {
        public int Id
        {
            get { return 1; }
        }

        public void Handle(object sender, Packet packet)
        {

        }
    }
}
