using Hinox.Data.Mongo.Dal.Entities;

namespace Hinox.Data.Mongo.Dal.Dao.Interfaces
{
    public interface IStringIdMongoDao<T> : IMongoDao<T, string> where T : BaseStringIdMongoEntity
    {
    }
}