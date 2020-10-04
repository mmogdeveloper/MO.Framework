using Google.Protobuf;
using MO.Common.Security;
using MO.Protocol;
using MO.Unity3d.Data;
using System;

namespace MO.Unity3d.Network
{
    public static class PacketHelper
    {
        public static MOPacket BuildPacket(this IMessage content)
        {
            var actionId = Int32.Parse(content.GetType().Name.Substring(3));
            MOMsg msg = new MOMsg();
            msg.ActionId = actionId;
            msg.UserId = GameUser.Instance.UserId;
            msg.Token = GameUser.Instance.Token;
            msg.MsgId = GameUser.Instance.MsgId;
            if (content != null)
                msg.Content = content.ToByteString();
            var data = msg.ToByteString();
            msg.Sign = CryptoHelper.MD5_Encrypt(string.Format("{0}{1}", data, GlobalGame.Md5Key)).ToLower();
            return new MOPacket(msg);
        }

        public static MOPacket BuildHeartPacket(this MOPacket packet)
        {
            packet.Packet.ActionId = 1;
            packet.Packet.UserId = GameUser.Instance.UserId;
            packet.Packet.Token = GameUser.Instance.Token;
            packet.Packet.MsgId = GameUser.Instance.MsgId;
            var data = packet.Packet.ToByteString();
            packet.Packet.Sign = CryptoHelper.MD5_Encrypt(string.Format("{0}{1}", data, GlobalGame.Md5Key)).ToLower();
            return packet;
        }
    }
}

