namespace Shared.MongoDb.Filters
{
    public abstract class BaseMdFilter<T>
    {
        public abstract MdFilterSpecification<T> GenerateFilterSpecification();
    }
}