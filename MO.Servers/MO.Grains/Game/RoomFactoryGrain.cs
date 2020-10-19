using Google.Protobuf;
using MO.GrainInterfaces.Config;
using MO.GrainInterfaces.Game;
using MO.Protocol;
using Orleans;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class RoomFactoryGrain : Grain, IRoomFactoryGrain
    {
        public async Task<int> CreateRoom(ProtoRoomInfo roomInfo)
        {
            int roomId = await RedisHelper.LPopAsync<int>(RedisConstants.LKeyRedis_RoomId);
            await RedisHelper.HSetAsync(RedisConstants.HKeyRedis_Room, roomId.ToString(), roomInfo.ToByteArray());
            return roomId;
        }

        public async Task ReleaseRoom(int roomId)
        {
            await RedisHelper.DelAsync(RedisConstants.HKeyRedis_Room, roomId.ToString());
            await RedisHelper.RPushAsync(RedisConstants.LKeyRedis_RoomId, roomId);
        }

        public async Task<ProtoRoomInfo> GetRoomInfo(int roomId)
        {
            var bytes = await RedisHelper.HGetAsync<byte[]>(RedisConstants.HKeyRedis_Room, roomId.ToString());
            return ProtoRoomInfo.Parser.ParseFrom(bytes);
        }

        public async Task<bool> RoomExist(int roomId)
        {
            return await RedisHelper.HExistsAsync(RedisConstants.HKeyRedis_Room, roomId.ToString());
        }
    }
}