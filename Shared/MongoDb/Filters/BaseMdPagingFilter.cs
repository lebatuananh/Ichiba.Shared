namespace Shared.MongoDb.Filters
{
    public abstract class BaseMdPagingFilter<T> : BaseMdFilter<T>
    {
        public int Page { get; set; }
        public int Limit { get; set; }

        protected MdPagination GeneratePagination()
        {
            var pagination = new MdPagination
            {
                Skip = (Page - 1) * Limit,
                Limit = Limit
            };
            return pagination;
        }
    }
}