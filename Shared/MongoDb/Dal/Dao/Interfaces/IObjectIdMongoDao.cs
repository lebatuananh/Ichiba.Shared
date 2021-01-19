using Hinox.Data.Mongo.Dal.Entities;
using MongoDB.Bson;

namespace Hinox.Data.Mongo.Dal.Dao.Interfaces
{
    public interface IObjectIdMongoDao<T> : IMongoDao<T, ObjectId> where T : BaseObjectIdMongoEntity
    {
    }
}