using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MO.Unity3d.Network.Actions
{
    public class Action100006Callback: IPacketHandler
    {
        public int Id
        {
            get { return 100006; }
        }

        public void Handle(object sender, Packet packet)
        {
            S2C100006 rep = S2C100006.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            if (rep.UserId == GameUser.Instance.UserId)
            {
                GlobalGame.IsGameStart = false;
                GameUser.Instance.Channel.Close();
            }
        }
    }
}
