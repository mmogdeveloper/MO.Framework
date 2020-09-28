using Google.Protobuf;
using MO.Algorithm.Actions.Enum;
using MO.Protocol;

namespace MO.Algorithm
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
