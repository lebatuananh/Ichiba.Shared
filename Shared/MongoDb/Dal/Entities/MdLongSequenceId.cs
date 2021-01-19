using Hinox.Data.Mongo.Dal.Entities;
using Shared.MongoDb.Attributes;

namespace Shared.MongoDb.Dal.Entities
{
    [MdCollection(Name = "LongSequenceIds")]
    public class MdLongSequenceId : BaseStringIdMongoEntity
    {
        public long CurrentValue { get; set; }
    }
}