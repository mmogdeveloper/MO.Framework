using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Network.PacketHandler
{
    public class Action100001Handler : IPacketHandler
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
                if (GameUser.Instance.UserId == item.UserId)
                {
                    GameUser.Instance.CurPlayer.X = item.Vector.X;
                    GameUser.Instance.CurPlayer.Y = item.Vector.Y;
                    GameUser.Instance.CurPlayer.Z = item.Vector.Z;
                    GameUser.Instance.CurPlayer.RX = item.Rotation.X;
                    GameUser.Instance.CurPlayer.RY = item.Rotation.Y;
                    GameUser.Instance.CurPlayer.RZ = item.Rotation.Z;
                    GameEntry.Entity.ShowEntity<PlayerEntity>(
                        GameUser.Instance.CurPlayer.EntityId,
                        "Assets/GameMain/Entities/Self.prefab", "DefaultEntityGroup",
                        GameUser.Instance.CurPlayer);
                }
                else
                {
                    if (!GameUser.Instance.Players.ContainsKey(item.UserId))
                    {
                        var newPlayer = new PlayerData();
                        newPlayer.UserId = item.UserId;
                        newPlayer.UserName = item.UserName;
                        newPlayer.X = item.Vector.X;
                        newPlayer.Y = item.Vector.Y;
                        newPlayer.Z = item.Vector.Z;
                        newPlayer.RX = item.Rotation.X;
                        newPlayer.RY = item.Rotation.Y;
                        newPlayer.RZ = item.Rotation.Z;
                        GameUser.Instance.Players.Add(item.UserId, newPlayer);
                        GameEntry.Entity.ShowEntity<PlayerEntity>(newPlayer.EntityId,
                            "Assets/GameMain/Entities/Player.prefab", "DefaultEntityGroup", newPlayer);
                    }
                }
            }
        }
    }
}
