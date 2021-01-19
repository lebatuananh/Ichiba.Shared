using System.Collections.Generic;

namespace Shared.BaseModel
{
    public class BaseResponse
    {
        public BaseResponse()
        {
        }

        public BaseResponse(bool status, string errorCode, IDictionary<string, string> parameters = null)
        {
            Status = status;
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        public BaseResponse(string errorCode, IDictionary<string, string> parameters = null)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        public bool Status { get; set; }

        public string ErrorCode { get; set; }
        public IDictionary<string, string> Parameters { get; set; }

        public void Successful()
        {
            Status = true;
        }

        public void Fail(string errorCode, IDictionary<string, string> parameters = null)
        {
            Status = false;
            ErrorCode = errorCode;
            Parameters = parameters;
        }
    }
}