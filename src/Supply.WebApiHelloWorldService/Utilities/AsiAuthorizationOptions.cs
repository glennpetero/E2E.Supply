// (c) American Software, Inc. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Utilities
{
    [ExcludeFromCodeCoverage]
    public class AsiAuthorizationOptions : JwtBearerOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ApiScope { get; set; } = string.Empty;

    }
}
