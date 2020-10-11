namespace MO.GrainInterfaces.Config
{
    public class RedisConstants
    {
        /// <summary>
        /// 全局UserId唯一值
        /// 存储类型key-value
        /// </summary>
        public const string SKeyRedis_UserId = "Global_UserId";
        /// <summary>
        /// 全局RoomId
        /// 存储类型list
        /// </summary>
        public const string LKeyRedis_RoomId = "Global_RoomId";
        /// <summary>
        /// 全局Room
        /// 存储类型HSet
        /// </summary>
        public const string HKeyRedis_Room = "Global_HRoom";
    }
}
