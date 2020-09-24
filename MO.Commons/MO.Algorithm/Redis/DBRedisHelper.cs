using CSRedis;
using MO.Common;
using MO.Common.Config;
using System;
using System.Collections.Generic;
using System.Text;

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