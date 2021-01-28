using System.Collections.Generic;

namespace Shared.BaseModel
{
    public class DetailError

    {
        public DetailError(string errorCode, IDictionary<string, string> parameters)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        public string ErrorCode { get; }
        public IDictionary<string, string> Parameters { get; }
    }

    public class BaseResponse
    {
        public BaseResponse(bool status, IList<DetailError> detailErrors)
        {
            Status = status;
            DetailErrors = detailErrors;
        }

        public BaseResponse()
        {
        }

        public bool Status { get; set; }

        public IList<DetailError> DetailErrors { get; set; }

        public void Successful()
        {
            Status = true;
        }

        public void Fail(IList<DetailError> detailErrors)
        {
            Status = false;
            DetailErrors = detailErrors;
        }
    }
}