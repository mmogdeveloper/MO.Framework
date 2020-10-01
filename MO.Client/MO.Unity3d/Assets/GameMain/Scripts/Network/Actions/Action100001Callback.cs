using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Network.Actions
{
    public class Action100001Callback : IPacketHandler
    {
        public int Id
        {
            get { return 100001; }
        }

        public void Handle(object sender, Packet packet)
        {
            Log.Info("{0},进入成功", GameUser.Instance.UserName);
            var rep = S2C100001.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            foreach (var item in rep.UserPoints)
            {
                if (!GameUser.Instance.Players.ContainsKey(item.UserId))
                {
                    var newPlayer = new PlayerData();
                    newPlayer.UserId = item.UserId;
                    newPlayer.UserName = item.UserName;
                    GameUser.Instance.Players.Add(item.UserId, newPlayer);
                }
            }
        }
    }
}
