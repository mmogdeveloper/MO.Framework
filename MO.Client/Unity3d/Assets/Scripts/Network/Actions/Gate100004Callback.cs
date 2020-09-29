using GameFramework.Network;
using MO.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Assets.Scripts.Network.Actions
{
    public class Gate100004Callback: IPacketHandler
    {
        public int Id
        {
            get { return 100004; }
        }

        public void Handle(object sender, Packet packet)
        {
            S2C100004 rep = S2C100004.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            if (rep.UserId == GameUser.Instance.CurPlayer.UserId)
                return;

            PlayerData gamePlayer;
            if (GameUser.Instance.ViewPlayers.TryGetValue(rep.UserId, out gamePlayer))
            {
                gamePlayer.GameObject.transform.position = new Vector3(rep.X, rep.Y, 0);
            }
        }
    }
}
