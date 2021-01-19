using System;
using FluentValidation.Results;
using Shared.BaseModel;
using Shared.Extensions;

namespace Shared.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(BaseResponse baseResponses)
            : base("One or more validation failures have occurred.")
        {
            BaseResponses = baseResponses;
        }

        public ValidationException(ValidationFailure failures, BaseResponse baseResponses)
            : this(baseResponses)
        {
            BaseResponses = new BaseResponse(false, failures.ErrorCode,
                failures.CustomState.ToDictionary<string, string>());
        }

        public BaseResponse BaseResponses { get; }
    }
}