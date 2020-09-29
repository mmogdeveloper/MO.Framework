using GameFramework.Network;
using MO.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PacketHeader : IPacketHeader
{
    public int PacketLength { get; }
    public PacketHeader(int packetLength)
    {
        PacketLength = packetLength;
    }
}
