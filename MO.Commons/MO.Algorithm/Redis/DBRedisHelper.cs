using CSRedis;

namespace MO.Algorithm.Redis
{
    //public static class TokenRedis
    //{
    //    public static CSRedisClient Client { get; private set; }
    //    public static void Initialization(string redisConfig)
    //    {
    //        Client = new CSRedisClient(redisConfig);
    //    }
    //}

    public static class DataRedis
    {
        public static CSRedisClient Client { get; private set; }
        public static void Initialization(string redisConfig)
        {
            Client = new CSRedisClient(redisConfig);
        }
    }
}