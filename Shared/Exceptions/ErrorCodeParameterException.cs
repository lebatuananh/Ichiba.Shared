using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Exceptions
{
    public class ErrorCodeParameterException : ErrorCodeException
    {
        public ErrorCodeParameterException(string errorCode,
            IDictionary<string, string> parameters)
            : this(errorCode, parameters, null)
        {
        }

        public ErrorCodeParameterException(string errorCode,
            IDictionary<string, string> parameters,
            Exception innerException)
            : base(errorCode,
                $"{typeof(ErrorCodeParameterException)} with error code: {errorCode}, parameters: [{(parameters == null ? string.Empty : string.Join(", ", parameters.Select(item => $"{item.Key} = {item.Value}")))}]",
                innerException)
        {
            Parameters = parameters;
        }

        public IDictionary<string, string> Parameters { get; }
    }
}