using GameFramework.Event;
using GameFramework.Network;
using Google.Protobuf;
using MO.Protocol;
using MO.Unity3d.Network.Actions;
using System;
using System.IO;
using System.Reflection;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Network
{
    public class NetworkChannelHelper : INetworkChannelHelper
    {
        private INetworkChannel _networkChannel;
        private MOPacket _heartPacket;
        public int PacketHeaderLength { get { return 2; } }

        public void Initialize(INetworkChannel networkChannel)
        {
            _networkChannel = networkChannel;

            Type packetHandlerBaseType = typeof(IPacketHandler);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || types[i].IsAbstract)
                {
                    continue;
                }
                if (types[i].GetInterface(typeof(IPacketHandler).Name) == packetHandlerBaseType)
                {
                    IPacketHandler packetHandler = (IPacketHandler)Activator.CreateInstance(types[i]);
                    _networkChannel.RegisterHandler(packetHandler);
                }
            }

            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
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
                if (_heartPacket == null)
                    _heartPacket = PacketHelper.BuildPacket(new C2S1());
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
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);

            _networkChannel = null;
        }

        private void OnNetworkConnected(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkConnectedEventArgs ne = (UnityGameFramework.Runtime.NetworkConnectedEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' connected, local address '{1}', remote address '{2}'.", ne.NetworkChannel.Name, ne.NetworkChannel.Socket.LocalEndPoint.ToString(), ne.NetworkChannel.Socket.RemoteEndPoint.ToString());

            _networkChannel.Send(PacketHelper.BuildPacket(new C2S100000()));
        }

        private void OnNetworkClosed(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkClosedEventArgs ne = (UnityGameFramework.Runtime.NetworkClosedEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' closed.", ne.NetworkChannel.Name);
        }

        private void OnNetworkMissHeartBeat(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs ne = (UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' miss heart beat '{1}' times.", ne.NetworkChannel.Name, ne.MissCount.ToString());

            if (ne.MissCount < 2)
            {
                return;
            }

            ne.NetworkChannel.Close();
        }

        private void OnNetworkError(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkErrorEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' error, error code is '{1}', error message is '{2}'.", ne.NetworkChannel.Name, ne.ErrorCode.ToString(), ne.ErrorMessage);

            ne.NetworkChannel.Close();
        }

        private void OnNetworkCustomError(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkCustomErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkCustomErrorEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }
        }
    }
}
