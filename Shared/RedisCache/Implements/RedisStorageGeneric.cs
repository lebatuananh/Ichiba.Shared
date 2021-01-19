using Shared.RedisCache.Interfaces;

namespace Shared.RedisCache.Implements
{
    public class RedisStorage<TRedisConfig> : RedisStorage, IRedisStorage<TRedisConfig>
        where TRedisConfig : IRedisConfig
    {
        public RedisStorage(RedisConnection<TRedisConfig> redisConnection)
            : base(redisConnection.WriteConnection, redisConnection.ReadConnection)
        {
        }
    }
}