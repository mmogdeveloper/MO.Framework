﻿using GameFramework.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Network.PacketHandler
{
    /// <summary>
    /// 心跳包
    /// </summary>
    public class Action1Handler : IPacketHandler
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
