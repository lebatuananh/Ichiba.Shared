using MongoDB.Driver;

namespace Shared.MongoDb.Filters
{
    public class MdFilterSpecification<T>
    {
        public FilterDefinition<T> Filter { get; set; }
        public SortDefinition<T> Sort { get; set; }
        public MdPagination Pagination { get; set; }
    }

    public class MdPagination
    {
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}