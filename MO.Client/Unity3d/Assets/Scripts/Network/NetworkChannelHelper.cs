using Assets.Scripts.Network.Actions;
using GameFramework.Network;
using Google.Protobuf;
using MO.Common.Security;
using MO.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class NetworkChannelHelper : INetworkChannelHelper
{
    private INetworkChannel _networkChannel;
    private MOPacket _heartPacket;
    public int PacketHeaderLength { get { return 2; } }

    public void Initialize(INetworkChannel networkChannel)
    {
        _heartPacket = PacketHelper.BuildPacket(1);
        _networkChannel = networkChannel;
        _networkChannel.RegisterHandler(new Gate1Callback());
        _networkChannel.RegisterHandler(new Gate100000Callback());
    }

    public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
    {
        customErrorData = null;
        MOMsg msg = null;
        try
        {
            var buffer = new byte[packetHeader.PacketLength];
            source.Read(buffer, 0, buffer.Length);
            msg = MOMsg.Parser.ParseFrom(ByteString.CopyFrom(buffer));
        }
        catch (Exception ex)
        {
            customErrorData = ex.Message;
        }
        return new MOPacket(msg);
    }

    public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
    {
        customErrorData = null;
        int packetLength = 0;
        try
        {
            var bufferLen = new byte[PacketHeaderLength];
            source.Read(bufferLen, 0, bufferLen.Length);
            packetLength = BitConverter.ToInt16(bufferLen, 0);
        }
        catch (Exception ex)
        {
            customErrorData = ex.Message;
        }
        return new PacketHeader(packetLength);
    }

    public void PrepareForConnecting()
    {

    }

    public bool SendHeartBeat()
    {
        if (_networkChannel.Connected)
        {
            _networkChannel.Send(_heartPacket);
        }
        return false;
    }

    public bool Serialize<T>(T packet, Stream destination) where T : Packet
    {
        var moPacket = packet as MOPacket;
        if (moPacket != null)
        {
            var content = moPacket.Packet.ToByteArray();
            var buffer = new byte[content.Length + PacketHeaderLength];
            var len = BitConverter.GetBytes((short)content.Length);
            Array.Copy(len, buffer, PacketHeaderLength);
            Array.Copy(content, 0, buffer, PacketHeaderLength, content.Length);
            destination.Write(buffer, 0, buffer.Length);
            return true;
        }
        return false;
    }

    public void Shutdown()
    {

    }
}
