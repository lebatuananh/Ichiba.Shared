namespace Shared.RedisCache.Interfaces
{
    public interface IRedisConfig
    {
        string Server { get; set; }
        string Password { get; set; }
        int DbId { get; set; }
        int LogDbId { get; set; }
    }

    public interface IRedisStorage<RedisConfig> : IRedisStorage
        where RedisConfig : IRedisConfig
    {
    }
}