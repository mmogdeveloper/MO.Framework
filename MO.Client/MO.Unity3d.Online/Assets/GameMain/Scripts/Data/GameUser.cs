using GameFramework.Network;
using Google.Protobuf;
using MO.Unity3d.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace MO.Unity3d.Data
{
    public class GameUser
    {
        public static GameUser Instance { get; }
        static GameUser()
        {
            Instance = new GameUser();
        }

        public INetworkChannel Channel { get; private set; }
        public Dictionary<long, PlayerData> Players { get; private set; }
        public PlayerData CurPlayer { get; private set; }
        public void Initiation(PlayerData playerData)
        {
            if (Channel == null)
                Channel = GameEntry.Network.CreateNetworkChannel("Global", ServiceType.Tcp, new NetworkChannelHelper());
            if (Players == null)
                Players = new Dictionary<long, PlayerData>();
            else
                Players.Clear();
            CurPlayer = playerData;
            Players[playerData.UserId] = CurPlayer;
        }

        public long UserId { get { return CurPlayer.UserId; } }
        public string UserName { get { return CurPlayer.UserName; } }
        public string Token { get; set; }
        private int _msgId;
        public int MsgId
        {
            get
            {
                _msgId++;
                return _msgId;
            }
        }
        public int FrameCount { get; set; }
        public int RoomId { get; set; }

        public void SendPackage(IMessage content)
        {
            Channel.Send(content.BuildPacket());
        }
    }
}
