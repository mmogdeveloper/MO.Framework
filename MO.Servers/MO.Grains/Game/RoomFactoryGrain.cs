using Google.Protobuf;
using MO.Algorithm.Config;
using MO.Algorithm.Redis;
using MO.GrainInterfaces.Game;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class RoomFactoryGrain : Grain, IRoomFactory
    {
        public async Task<int> CreateRoom(ProtoRoomInfo roomInfo)
        {
            int roomId = await DataRedis.Client.LPopAsync<int>(RedisConstants.LKeyRedis_RoomId);
            await DataRedis.Client.HSetAsync(RedisConstants.HKeyRedis_Room, roomId.ToString(), roomInfo.ToByteArray());
            return roomId;
        }

        public async Task ReleaseRoom(int roomId)
        {
            await DataRedis.Client.DelAsync(RedisConstants.HKeyRedis_Room, roomId.ToString());
            await DataRedis.Client.RPushAsync(RedisConstants.LKeyRedis_RoomId, roomId);
        }

        public async Task<ProtoRoomInfo> GetRoomInfo(int roomId)
        {
            var bytes = await DataRedis.Client.HGetAsync<byte[]>(RedisConstants.HKeyRedis_Room, roomId.ToString());
            return ProtoRoomInfo.Parser.ParseFrom(bytes);
        }

        public async Task<bool> RoomExist(int roomId)
        {
            return await DataRedis.Client.HExistsAsync(RedisConstants.HKeyRedis_Room, roomId.ToString());
        }
    }
}