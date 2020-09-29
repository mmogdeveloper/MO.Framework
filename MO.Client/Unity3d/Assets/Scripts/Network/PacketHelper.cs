using Google.Protobuf;
using MO.Common.Security;
using MO.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class PacketHelper
{
    public static MOPacket BuildPacket(int actionId, IMessage content = null)
    {
        MOMsg msg = new MOMsg();
        msg.UserId = GameUser.Instance.PlayerData.UserId;
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
