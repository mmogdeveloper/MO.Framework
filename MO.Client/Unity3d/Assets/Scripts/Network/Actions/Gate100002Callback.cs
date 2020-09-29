using GameFramework.Network;
using MO.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Network.Actions
{
    public class Gate100002Callback: IPacketHandler
    {
        public int Id
        {
            get { return 100002; }
        }

        public void Handle(object sender, Packet packet)
        {
            S2C100002 rep = S2C100002.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            if (rep.UserId == GameUser.Instance.CurPlayer.UserId)
                return;

            if (!GameUser.Instance.ViewPlayers.ContainsKey(rep.UserId))
            {
                var newPlayer = new PlayerData();
                newPlayer.UserId = rep.UserId;
                newPlayer.GameObject = (GameObject)Resources.Load("Player");
                newPlayer.GameObject.name = rep.UserId.ToString();
                GameObject prefabInstance = GameObject.Instantiate(newPlayer.GameObject);
                prefabInstance.transform.parent = GameObject.Find("Canvas").gameObject.transform;
                GameUser.Instance.ViewPlayers.Add(rep.UserId, newPlayer);
            }
        }
    }
}
