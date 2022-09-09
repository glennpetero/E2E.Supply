// (c) American Software, Inc. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Utilities.HealthChecks
{
    //sourced from https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-5.0#customize-output
    [ExcludeFromCodeCoverage]
    public static class HealthCheckResponseWriter
    {
        public static Task WriteAsync(HttpContext? context, HealthReport? result)
        {
            if (context == null)
            {
                return Task.CompletedTask;
            }

            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions {
                Indented = false
            };

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, options))
            {
                writer.WriteStartObject();
                if (result != null)
                {
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteStartObject("results");
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }

            var json = Encoding.UTF8.GetString(stream.ToArray());
            return context.Response.WriteAsync(json);
        }
    }
}
