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
    public class Action100006Handler : IPacketHandler
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
                GameUser.Instance.Players.Clear();
            }
            else
            {
                PlayerData player;
                if (GameUser.Instance.Players.TryGetValue(rep.UserId, out player))
                {
                    GameUser.Instance.Players.Remove(rep.UserId);
                    GameEntry.Entity.HideEntity(player.EntityId);
                }
            }
        }
    }
}
