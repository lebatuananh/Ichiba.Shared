using Hinox.Data.Mongo.Dal.Dao.Interfaces;
using Hinox.Data.Mongo.Dal.Entities;

namespace Shared.MongoDb.Dal.Dao.Interfaces
{
    public interface ILongIdMongoDao<T> : IMongoDao<T, long> where T : BaseLongIdMongoEntity
    {
    }
}