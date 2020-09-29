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
    public class Gate100001Callback : IPacketHandler
    {
        public int Id
        {
            get { return 100001; }
        }

        public void Handle(object sender, Packet packet)
        {
            Log.Info("{0},进入成功", GameUser.Instance.CurPlayer.UserId);
            var rep = S2C100001.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            foreach (var item in rep.UserPoints)
            {
                if (item.UserId == GameUser.Instance.CurPlayer.UserId)
                {
                    GameUser.Instance.CurPlayer.X = item.X;
                    GameUser.Instance.CurPlayer.Y = item.Y;
                    GameUser.Instance.CurPlayer.GameObject.transform.position = new Vector3(item.X, item.Y, 0);
                }
                else
                {
                    if (!GameUser.Instance.ViewPlayers.ContainsKey(item.UserId))
                    {
                        var newPlayer = new PlayerData();
                        newPlayer.UserId = item.UserId;
                        newPlayer.GameObject = (GameObject)Resources.Load("Player");
                        newPlayer.GameObject.name = item.UserId.ToString();
                        GameObject prefabInstance = GameObject.Instantiate(newPlayer.GameObject);
                        prefabInstance.transform.parent = GameObject.Find("Canvas").gameObject.transform;
                        GameUser.Instance.ViewPlayers.Add(item.UserId, newPlayer);
                    }
                }
            }
        }
    }
}
