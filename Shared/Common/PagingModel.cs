namespace Shared.Common
{
    public class PagingModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public virtual Paging ToPaging()
        {
            return new Paging
            {
                PageIndex = PageIndex,
                PageSize = PageSize
            };
        }
    }
}