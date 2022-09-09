// (c) American Software, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using Amsoftware.E2E.Supply.WebApiHelloWorldService.Utilities;
using Amsoftware.E2E.Supply.WebApiHelloWorldService.Utilities.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddControllers();

            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            var asiAuthOptions = Configuration.GetSection("IdentityServer").Get<AsiAuthorizationOptions>();
            services.AddSingleton(asiAuthOptions);

            ConfigureApiVersioning(services);
            ConfigureSwaggerServices(services, asiAuthOptions);
            ConfigureIdentityGateway(services, asiAuthOptions);

            services.AddApplicationInsightsTelemetry();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider,
            AsiAuthorizationOptions asiAuthOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                ConfigureSwaggerForApp(app, apiVersionDescriptionProvider, asiAuthOptions);
            }

            app.UseForwardedHeaders();

            app.UseHttpsRedirection();

            app.UseCors("allow-all");

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions {
                    ResponseWriter = HealthCheckResponseWriter.WriteAsync
                });
            });
        }

        private static void ConfigureApiVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(config => {
                config.ReportApiVersions = false;
            });

            services.AddVersionedApiExplorer(options => {
                // format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                // replace the version parameter with actual version number in routes in swagger ui
                options.SubstituteApiVersionInUrl = true;
            });
        }

        private static void ConfigureSwaggerServices(IServiceCollection services, AsiAuthorizationOptions asiAuthOptions)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(c => {
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows {
                        AuthorizationCode = new OpenApiOAuthFlow {
                            AuthorizationUrl = new Uri($"{asiAuthOptions.Authority}/connect/authorize"),
                            TokenUrl = new Uri($"{asiAuthOptions.Authority}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { asiAuthOptions.ApiScope ?? string.Empty, asiAuthOptions.ApiScope ?? string.Empty }
                            }
                        }
                    }
                });
                
                // tells swagger to add the oauth scheme to all operations with Authorize attribute
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        private void ConfigureSwaggerForApp(IApplicationBuilder app, 
            IApiVersionDescriptionProvider apiVersionDescriptionProvider,
            AsiAuthorizationOptions asiAuthOptions)
        {
            app.UseSwagger(options => {
                // swagger gen doesn't take into account the additional prefix header when behind a reverse proxy
                // this next bit sets the server element as defined in openapi3 spec
                // the server baseUrl is prepended to the operation url in swagger ui calls
                // 
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1298
                // https://vmsdurano.com/fixing-swagger-ui-when-in-behind-proxy/
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore#modify-swagger-with-request-context

                options.PreSerializeFilters.Add((doc, req) => {
                    if (!req.Headers.ContainsKey("X-Forwarded-Host"))
                    {
                        return;
                    }

                    string? pathBase = req.Headers.TryGetValue("X-Forwarded-Prefix", out var prefixes)
                        ? prefixes.FirstOrDefault()
                        : string.Empty;

                    doc.Servers = new List<OpenApiServer>
                    {
                        new()
                        {
                            Url = $"{req.Scheme}://{req.Host.Value}{pathBase}"
                        }
                    };
                });
            });

            app.UseSwaggerUI(options => {
   
                // get all non-deprecated api versions ([ApiVersion] annotation)
                // and add a swagger doc for each
                // descending order is needed so that swagger ui shows the most recent by default
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions
                    .OrderByDescending(v => v.ApiVersion.MajorVersion)
                    .ThenByDescending(v => v.ApiVersion.MinorVersion)
                    .Where(v => !v.IsDeprecated))
                {
                    // path is relative to route prefix set below
                    options.SwaggerEndpoint(
                        $"{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }

                options.RoutePrefix = "swagger";

                options.OAuthClientId(asiAuthOptions.ClientId);
                options.OAuthAppName(asiAuthOptions.Audience);
                options.OAuthUsePkce();
            });
        }

        private void ConfigureIdentityGateway(IServiceCollection services, AsiAuthorizationOptions asiAuthOptions)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                    Configuration.Bind("IdentityServer", options);
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true
                    };
                });

            //enable cors
            services.AddCors(cfg =>
            {
                cfg.AddPolicy("allow-all", p =>
                {
                    p.AllowAnyOrigin();
                    p.AllowAnyMethod();
                    p.AllowAnyHeader();
                });
            });
        }
    }
}
