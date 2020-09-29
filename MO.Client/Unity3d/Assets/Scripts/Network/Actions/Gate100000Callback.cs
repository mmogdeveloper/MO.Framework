using GameFramework.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGameFramework.Runtime;

namespace Assets.Scripts.Network.Actions
{
    public class Gate100000Callback : IPacketHandler
    {
        public int Id
        {
            get { return 100000; }
        }

        public void Handle(object sender, Packet packet)
        {
            Log.Info(string.Format("{0},网关连接成功", GameUser.Instance.PlayerData.UserId));
        }
    }
}
