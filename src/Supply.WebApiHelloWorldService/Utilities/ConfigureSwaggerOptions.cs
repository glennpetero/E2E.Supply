// (c) American Software, Inc. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Utilities
{
    [ExcludeFromCodeCoverage]
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo {
                        Title = "Amsoftware.E2E.Supply.WebApiHelloWorldService",
                        Version = description.ApiVersion.ToString(),
                        Description = "The webapihelloworld service is a basic service that provides an example REST API for calculations."
                    });
            }
        }
    }
}
