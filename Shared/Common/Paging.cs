namespace Shared.Common
{
    public class Paging
    {
        private const int DEFAULT_PAGE_INDEX = 1;
        private const int DEFAULT_PAGE_SIZE = 10;

        private int pageIndex;

        private int pageSize;

        public int PageIndex
        {
            get => pageIndex;
            set => pageIndex = value >= DEFAULT_PAGE_INDEX ? value : DEFAULT_PAGE_INDEX;
        }

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > 0 ? value : DEFAULT_PAGE_SIZE;
        }

        public int Total { get; set; }

        public int FromIndex => (pageIndex - 1) * PageSize;
    }
}