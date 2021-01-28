using System.Collections.Generic;

namespace Shared.BaseModel
{
    public class PagingResponse<TEntity> : BaseEntityResponse<TEntity>
    {
        public PagingResponse()
        {
            
        }
        public PagingResponse(bool status, IList<DetailError> detailErrors) : base(status, detailErrors)
        {
        }

        public long Total { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}