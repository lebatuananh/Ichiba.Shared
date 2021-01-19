using System.Collections.Generic;

namespace Shared.BaseModel
{
    public class BaseEntityResponse<TEntity> : BaseResponse
    {
        public BaseEntityResponse()
        {
        }

        public BaseEntityResponse(bool status, string errorCode, IDictionary<string, string> parameters = null) : base(
            status,
            errorCode, parameters)
        {
        }

        public BaseEntityResponse(string errorCode, IDictionary<string, string> parameters = null) : base(errorCode,
            parameters)
        {
        }

        public TEntity Data { get; set; }

        public virtual BaseEntityResponse<TEntity> SetData(TEntity data)
        {
            Data = data;
            Status = true;
            return this;
        }
    }
}