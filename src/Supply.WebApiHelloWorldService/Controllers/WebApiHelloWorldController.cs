// (c) American Software, Inc. All rights reserved.

using System;
using System.Threading.Tasks;
using Amsoftware.E2E.Supply.WebApiHelloWorldService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Controllers
{
    [Route("v{version:apiVersion}/webapihelloworld")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Authorize]
    public class WebApiHelloWorldController : ControllerBase
    {
        private readonly ILogger<WebApiHelloWorldController> _logger;

        public WebApiHelloWorldController(ILogger<WebApiHelloWorldController> logger)
        {
            _logger = logger;
        }

        
        [HttpGet]             
        public Task<IActionResult> GetHelloWorld()
        {           
            return Task.FromResult((IActionResult)Ok("Hello World!"));
        }


        /// <summary>
        /// Gets the economic order quantity for the specified request values.
        /// </summary>
        /// <param name="eoqRequest"></param>
        /// <returns><see cref="EoqResponse"/></returns>
        [HttpGet("eoq", Name = "GetEoq")]
        [ProducesResponseType(typeof(EoqResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public Task<IActionResult> GetEoq([FromQuery] EoqRequest eoqRequest)
        {
            _logger.LogTrace($"Eoq calculation request: {eoqRequest}");

            // Validation is done here for demonstration purposes only
            // See the Range attribute on EoqRequest.h property for preferred model validation method
            if (eoqRequest.h <= 0)
            {
                ModelState.AddModelError("h", "h must be greater than 0.");
                return Task.FromResult((IActionResult)ValidationProblem(statusCode: StatusCodes.Status400BadRequest));
            }

            double result = Math.Sqrt(2 * eoqRequest.D * eoqRequest.K / eoqRequest.h);
            var eoqResponse = new EoqResponse { Eoq = result };
            return Task.FromResult((IActionResult)Ok(eoqResponse));
        }
    }
}
