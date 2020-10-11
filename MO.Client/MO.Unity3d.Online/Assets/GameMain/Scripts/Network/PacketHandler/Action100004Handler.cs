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
                    GameUser.Instance.CurPlayer.ServerX = item.Vector.X;
                    GameUser.Instance.CurPlayer.ServerY = item.Vector.Y;
                    GameUser.Instance.CurPlayer.ServerZ = item.Vector.Z;
                    GameUser.Instance.CurPlayer.ServerRX = item.Rotation.X;
                    GameUser.Instance.CurPlayer.ServerRY = item.Rotation.Y;
                    GameUser.Instance.CurPlayer.ServerRZ = item.Rotation.Z;
                }
                else
                {
                    PlayerData gamePlayer;
                    if (GameUser.Instance.Players.TryGetValue(item.UserId, out gamePlayer))
                    {
                        gamePlayer.ServerX = item.Vector.X;
                        gamePlayer.ServerY = item.Vector.Y;
                        gamePlayer.ServerZ = item.Vector.Z;
                        gamePlayer.ServerRX = item.Rotation.X;
                        gamePlayer.ServerRY = item.Rotation.Y;
                        gamePlayer.ServerRZ = item.Rotation.Z;
                    }
                }
            }
        }
    }
}
