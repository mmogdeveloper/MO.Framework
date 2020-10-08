using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Network.PacketHandler
{
    public class Action100004Handler : IPacketHandler
    {
        public int Id
        {
            get { return 100004; }
        }

        public void Handle(object sender, Packet packet)
        {
            S2C100004 rep = S2C100004.Parser.ParseFrom(((MOPacket)packet).Packet.Content);

            foreach (var item in rep.UserPoints)
            {
                if (item.UserId == GameUser.Instance.UserId)
                {
                    GameUser.Instance.CurPlayer.X = item.Vector.X;
                    GameUser.Instance.CurPlayer.Y = item.Vector.Y;
                    GameUser.Instance.CurPlayer.Z = item.Vector.Z;
                    GameUser.Instance.CurPlayer.RX = item.Rotation.X;
                    GameUser.Instance.CurPlayer.RY = item.Rotation.Y;
                    GameUser.Instance.CurPlayer.RZ = item.Rotation.Z;
                }
                else
                {
                    PlayerData gamePlayer;
                    if (GameUser.Instance.Players.TryGetValue(item.UserId, out gamePlayer))
                    {
                        gamePlayer.X = item.Vector.X;
                        gamePlayer.Y = item.Vector.Y;
                        gamePlayer.Z = item.Vector.Z;
                        gamePlayer.RX = item.Rotation.X;
                        gamePlayer.RY = item.Rotation.Y;
                        gamePlayer.RZ = item.Rotation.Z;
                    }
                }
            }
        }
    }
}
