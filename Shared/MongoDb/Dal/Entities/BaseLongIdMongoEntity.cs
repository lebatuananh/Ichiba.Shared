using MongoDB.Bson.Serialization.Attributes;
using Shared.MongoDb.Dal.IdGenerators;

namespace Hinox.Data.Mongo.Dal.Entities
{
    public abstract class BaseLongIdMongoEntity : IMongoEntity<long>
    {
        [BsonId(IdGenerator = typeof(LongObjectIdGenerator))]
        public virtual long Id { get; set; }
    }
}