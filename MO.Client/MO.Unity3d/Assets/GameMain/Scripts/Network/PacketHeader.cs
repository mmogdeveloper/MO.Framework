using GameFramework.Network;
using MO.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Network
{
    public class PacketHeader : IPacketHeader
    {
        public int PacketLength { get; }
        public PacketHeader(int packetLength)
        {
            PacketLength = packetLength;
        }
    }
}
