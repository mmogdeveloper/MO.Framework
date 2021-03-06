﻿using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Network.PacketHandler
{
    public class Action100000Handler : IPacketHandler
    {
        public int Id
        {
            get { return 100000; }
        }

        public void Handle(object sender, Packet packet)
        {
            Log.Info("{0},网关连接成功", GameUser.Instance.UserName);
            GlobalGame.IsGameStart = true;
            GlobalGame.SendPackage(new C2S100001() { RoomId = GameUser.Instance.RoomId });
        }
    }
}
