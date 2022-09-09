// (c) American Software, Inc. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Utilities
{
    [ExcludeFromCodeCoverage]
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly AsiAuthorizationOptions _asiAuthOptions;

        public AuthorizeCheckOperationFilter(AsiAuthorizationOptions asiAuthOptions)
        {
            _asiAuthOptions = asiAuthOptions;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            bool hasAuthorize = context.MethodInfo.DeclaringType != null 
                                    && (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                                    || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());

            if (!hasAuthorize)
            {
                return;
            }
            
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            }

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new() {
                    [
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"}
                        }
                    ] = new[] { _asiAuthOptions.Audience }
                }
            };
        }
    }
}
