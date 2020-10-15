using GameFramework.Network;
using Google.Protobuf;
using MO.Unity3d.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Data
{
    public static class GlobalGame
    {
        public const string Md5Key = "8B203C98ADD54BACA9857C5DC4DFF603";

        public const string LoginUrl = "http://localhost:8001";

        public static bool IsGameStart = false;

        public static int FrameCount;
        public static INetworkChannel Channel = GameEntry.Network.CreateNetworkChannel("Global", ServiceType.Tcp, new NetworkChannelHelper());
        public static void SendPackage(IMessage content)
        {
            Channel.Send(content.BuildPacket());
        }
    }
}
