using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MO.Unity3d.Network.PacketHandler
{
    public class Action100002Handler : IPacketHandler
    {
        public int Id
        {
            get { return 100002; }
        }

        public void Handle(object sender, Packet packet)
        {
            S2C100002 rep = S2C100002.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            if (rep.UserId == GameUser.Instance.UserId)
                return;

            if (!GameUser.Instance.Players.ContainsKey(rep.UserId))
            {
                var newPlayer = new PlayerData();
                newPlayer.UserId = rep.UserId;
                newPlayer.UserName = rep.UserName;
                GameUser.Instance.Players.Add(rep.UserId, newPlayer);
            }
        }
    }
}
