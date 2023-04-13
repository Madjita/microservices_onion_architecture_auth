using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthBLL.Filters;

public sealed class AuthorizeCheckOperationFilter : IOperationFilter
{
    private const string SecuritySchemeId = "Bearer";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        var hasAllowAnonymous =
            context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
            || context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (!hasAuthorize || hasAllowAnonymous)
            return;

        // if (!operation.Responses.ContainsKey("401"))
        //     operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
        // if (!operation.Responses.ContainsKey("403"))
        //     operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});
        // if (!operation.Responses.ContainsKey("404"))
        //     operation.Responses.Add("404", new OpenApiResponse {Description = "NotFound"});

        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = SecuritySchemeId}
        };

        operation.Security.Add(new OpenApiSecurityRequirement {[scheme] = new List<string>()});
    }
}