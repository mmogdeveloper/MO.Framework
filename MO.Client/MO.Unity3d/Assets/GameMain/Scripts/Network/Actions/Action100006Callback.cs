using GameFramework.Network;
using MO.Protocol;
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
            //S2C100006 rep = S2C100006.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            //if (rep.UserId == GameUser.Instance.CurPlayer.UserId)
            //    return;
            //PlayerData player = null;
            //if (GameUser.Instance.ViewPlayers.TryGetValue(rep.UserId, out player))
            //{
            //    //GameObject.Destroy(player.GameObject);
            //    //GameUser.Instance.ViewPlayers.Remove(rep.UserId);
            //}
        }
    }
}
