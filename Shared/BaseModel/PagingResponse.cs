using System.Collections.Generic;

namespace Shared.BaseModel
{
    public class PagingResponse<TEntity> : BaseEntityResponse<TEntity>
    {
        public PagingResponse(bool status, string errorCode, IDictionary<string, string> parameters = null) : base(
            status,
            errorCode, parameters)
        {
        }

        public PagingResponse()
        {
        }

        public PagingResponse(string errorCode, IDictionary<string, string> parameters = null) : base(errorCode,
            parameters)
        {
        }

        public long Total { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}