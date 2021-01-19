namespace Hinox.Data.Mongo.Dal.Entities
{
    public interface IMongoEntity<T>
    {
        T Id { get; set; }
    }
}