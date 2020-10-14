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
            GameUser.Instance.RoomId = rep.RoomId;
            foreach (var item in rep.UserPoints)
            {
                if (GameUser.Instance.UserId == item.UserId)
                {
                    GameUser.Instance.CurPlayer.ServerX = item.Vector.X;
                    GameUser.Instance.CurPlayer.ServerY = item.Vector.Y;
                    GameUser.Instance.CurPlayer.ServerZ = item.Vector.Z;
                    GameUser.Instance.CurPlayer.ServerRX = item.Rotation.X;
                    GameUser.Instance.CurPlayer.ServerRY = item.Rotation.Y;
                    GameUser.Instance.CurPlayer.ServerRZ = item.Rotation.Z;
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
                        newPlayer.ServerX = item.Vector.X;
                        newPlayer.ServerY = item.Vector.Y;
                        newPlayer.ServerZ = item.Vector.Z;
                        newPlayer.ServerRX = item.Rotation.X;
                        newPlayer.ServerRY = item.Rotation.Y;
                        newPlayer.ServerRZ = item.Rotation.Z;
                        GameUser.Instance.Players.Add(item.UserId, newPlayer);
                        GameEntry.Entity.ShowEntity<PlayerEntity>(newPlayer.EntityId,
                            "Assets/GameMain/Entities/Player.prefab", "DefaultEntityGroup", newPlayer);
                    }
                }
            }
        }
    }
}
