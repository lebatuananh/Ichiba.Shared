using System;
using System.Collections.Generic;
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

        public ValidationException(IEnumerable<ValidationFailure> failures, BaseResponse baseResponses)
            : this(baseResponses)
        {
            var detailErrors = new List<DetailError>();
            foreach (var validationFailure in failures)
                detailErrors.Add(new DetailError(validationFailure.ErrorCode,
                    validationFailure.CustomState.ToDictionary<string, string>()));

            BaseResponses = new BaseResponse(false, detailErrors);
        }

        public BaseResponse BaseResponses { get; }
    }
}