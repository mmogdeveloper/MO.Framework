using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MO.Unity3d.Network.PacketHandler
{
    public class Action100008Handler : IPacketHandler
    {
        public int Id
        {
            get { return 100008; }
        }

        public void Handle(object sender, Packet packet)
        {
            S2C100008 rep = S2C100008.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            if (rep.UserId == GameUser.Instance.UserId)
                return;

            PlayerData playerData;
            if (GameUser.Instance.Players.TryGetValue(rep.UserId, out playerData))
            {
                GameEntry.Entity.ShowEntity<PlayerChatEntity>(
                    GameEntry.Entity.GenerateSerialId(),
                    "Assets/GameMain/Entities/PlayerChatMsg.prefab",
                    "DefaultEntityGroup", new MsgUserData(playerData.UserName, rep.Content));
            }
        }
    }
}
