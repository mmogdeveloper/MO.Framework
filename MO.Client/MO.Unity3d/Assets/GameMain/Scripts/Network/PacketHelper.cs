using Google.Protobuf;
using MO.Common.Security;
using MO.Protocol;
using MO.Unity3d.Data;
using System;

namespace MO.Unity3d.Network
{
    public static class PacketHelper
    {
        public static MOPacket BuildPacket(IMessage content)
        {
            var actionId = Int32.Parse(content.GetType().Name.Substring(3));
            MOMsg msg = new MOMsg();
            msg.UserId = GameUser.Instance.UserId;
            msg.Token = GameUser.Instance.Token;
            msg.MsgId = GameUser.Instance.MsgId;
            msg.ActionId = actionId;
            if (content != null)
                msg.Content = content.ToByteString();
            var data = msg.ToByteString();
            msg.Sign = CryptoHelper.MD5_Encrypt(string.Format("{0}{1}", data, GameConfig.Md5Key)).ToLower();
            MOPacket packet = new MOPacket(msg);
            return packet;
        }
    }
}

