// (c) American Software, Inc. All rights reserved.

using Amsoftware.E2E.Supply.WebApiHelloWorldService.Controllers;
using Microsoft.Extensions.Logging;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Tests.Controllers
{
    internal class WebApiHelloWorldControllerDependencies
    {
        public ILogger<WebApiHelloWorldController> Logger { get; set; }
    }
}
