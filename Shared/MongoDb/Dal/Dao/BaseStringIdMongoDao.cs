using Hinox.Data.Mongo.Dal.Dao.Interfaces;
using Hinox.Data.Mongo.Dal.Entities;

namespace Hinox.Data.Mongo.Dal.Dao
{
    public abstract class BaseStringIdMongoDao<T> : BaseMongoDao<T, string>, IStringIdMongoDao<T>
        where T : BaseStringIdMongoEntity
    {
        public BaseStringIdMongoDao(IMongoDbFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}