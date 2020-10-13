using Google.Protobuf;
using MO.Algorithm.Enum;
using MO.Protocol;

namespace MO.GrainInterfaces.Extensions
{
    public static class PacketExtension
    {
        public static MOMsg ParseResult(this MOMsg packet,
            MOErrorType errorType = MOErrorType.Success,
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
