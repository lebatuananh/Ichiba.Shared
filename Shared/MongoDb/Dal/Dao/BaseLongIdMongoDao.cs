using Hinox.Data.Mongo.Dal.Dao.Interfaces;
using Hinox.Data.Mongo.Dal.Entities;
using Shared.MongoDb.Dal.Dao.Interfaces;

namespace Hinox.Data.Mongo.Dal.Dao
{
    public abstract class BaseLongIdMongoDao<T> : BaseMongoDao<T, long>, ILongIdMongoDao<T>
        where T : BaseLongIdMongoEntity
    {
        public BaseLongIdMongoDao(IMongoDbFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}