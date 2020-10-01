using GameFramework.Network;
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
        public Dictionary<long, PlayerData> Players { get; set; }
        public PlayerData CurPlayer { get; set; }
        public void Initiation()
        {
            Channel = GameEntry.Network.CreateNetworkChannel("Global", ServiceType.Tcp, new NetworkChannelHelper());
            CurPlayer = new PlayerData();
            Players = new Dictionary<long, PlayerData>();
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

        public bool IsGameStart { get; set; }
    }
}
