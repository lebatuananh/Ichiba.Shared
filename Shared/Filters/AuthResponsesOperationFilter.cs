using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Filters
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType is null) return;
            var authAttributes = context.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();

            if (!authAttributes.Any()) return;
            operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});
        }
    }
}