using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.UIExtension;
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
                    GameUser.Instance.CurPlayer.Position = new Vector3(item.Vector.X, item.Vector.Y, item.Vector.Z);
                    GameUser.Instance.CurPlayer.Rotate = new Vector3(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
                    GameUser.Instance.CurPlayer.ShowEntity();
                }
                else
                {
                    if (!GameUser.Instance.Players.ContainsKey(item.UserId))
                    {
                        var newPlayer = new PlayerData();
                        newPlayer.UserId = item.UserId;
                        newPlayer.UserName = item.UserName;
                        newPlayer.Position = new Vector3(item.Vector.X, item.Vector.Y, item.Vector.Z);
                        newPlayer.Rotate = new Vector3(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
                        GameUser.Instance.Players.Add(item.UserId, newPlayer);
                        newPlayer.ShowEntity();
                    }
                }
            }
        }
    }
}
