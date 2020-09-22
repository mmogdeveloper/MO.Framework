using Google.Protobuf;
using MO.Algorithm.Actions.Enum;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Algorithm.Actions
{
    public static class PacketExtensions
    {
        public static MOMsg ParseResult(this MOMsg packet,
            ErrorType errorType = ErrorType.Success,
            string errorInfo = "")
        {
            packet.Token = string.Empty;
            packet.Content = ByteString.Empty;
            packet.ErrorCode = (int)errorType;
            packet.ErrorInfo = errorInfo;
            return packet;
        }
    }
}
