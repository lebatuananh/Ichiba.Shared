using System;
using Microsoft.AspNetCore.Http;

namespace Shared.Extensions
{
    public static class HttpContextExtentions
    {
        public static string GetFullDomain(this HttpContext context)
        {
            var domain = context.Request.Host.Value;
            var scheme = context.Request.Scheme;
            var delimiter = Uri.SchemeDelimiter;
            var fullDomainToUse = scheme + delimiter + domain;
            return fullDomainToUse;
        }
    }
}