using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.BaseModel;
using Shared.Exceptions;

namespace Shared.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilter()
        {
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                {typeof(ValidationException), HandleValidationException},
                {typeof(NotFoundException), HandleNotFoundException},
                {typeof(ErrorCodeException), HandleErrorCodeException},
                {typeof(ErrorCodeParameterException), HandleErrorCodeParameterException}
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            var type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            if (context.Exception is ValidationException exception)
                context.Result = new BadRequestObjectResult(exception.BaseResponses);

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            if (context.Exception is NotFoundException exception)
            {
                var details = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "The specified resource was not found.",
                    Detail = exception.Message
                };

                context.Result = new NotFoundObjectResult(details);
            }

            context.ExceptionHandled = true;
        }

        private void HandleErrorCodeException(ExceptionContext context)
        {
            if (context.Exception is ErrorCodeException exception)
                context.Result = new BadRequestObjectResult(new BaseResponse
                {
                    ErrorCode = exception.ErrorCode,
                    Status = false
                });

            context.ExceptionHandled = true;
        }

        private void HandleErrorCodeParameterException(ExceptionContext context)
        {
            if (context.Exception is ErrorCodeParameterException exception)
                context.Result = new BadRequestObjectResult(new BaseResponse
                {
                    ErrorCode = exception.ErrorCode,
                    Status = false,
                    Parameters = exception.Parameters
                });

            context.ExceptionHandled = true;
        }
    }
}