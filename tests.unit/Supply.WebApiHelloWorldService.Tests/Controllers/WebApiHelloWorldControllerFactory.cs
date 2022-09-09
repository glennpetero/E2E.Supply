// (c) American Software, Inc. All rights reserved.

using System;
using Amsoftware.E2E.Supply.WebApiHelloWorldService.Controllers;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Tests.Controllers
{
    internal class WebApiHelloWorldControllerFactory
    {
        public WebApiHelloWorldController Controller { get; init; }
        public WebApiHelloWorldControllerDependencies Dependencies { get; init; }

        public WebApiHelloWorldControllerFactory(Action<WebApiHelloWorldControllerDependencies> options = null)
        {
            var dependencies = new WebApiHelloWorldControllerDependencies {
                Logger = A.Fake<ILogger<WebApiHelloWorldController>>()
            };
            options?.Invoke(dependencies);

            Dependencies = dependencies;
            Controller = new WebApiHelloWorldController(Dependencies.Logger);
        }
    }
}
