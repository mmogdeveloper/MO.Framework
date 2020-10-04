using GameFramework;
using GameFramework.Network;
using MO.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Network
{
    public class MOPacketHeader : IPacketHeader
    {
        public int PacketLength { get;private set; }

        public MOPacketHeader(int packetLength)
        {
            PacketLength = packetLength;
        }
    }
}
