// (c) American Software, Inc. All rights reserved.

using System.Threading.Tasks;
using Amsoftware.E2E.Supply.WebApiHelloWorldService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Amsoftware.E2E.Supply.WebApiHelloWorldService.Tests.Controllers
{
    public class WebApiHelloWorldControllerTests
    {
        [Fact]
        public async Task GetEoqWithValidRequestShouldReturnOk()
        {
            //arrange
            var eoqRequest = new EoqRequest { D = 10000, K = 40D, h = 5D };
            var factory = new WebApiHelloWorldControllerFactory();
            var controller = factory.Controller;

            //act
            var response = await controller.GetEoq(eoqRequest);

            //assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetEoqWithKnownRequestValuesShouldReturnKnownResponseValue()
        {
            //arrange
            var eoqRequest = new EoqRequest { D = 10000, K = 40D, h = 5D };
            double expectedEoq = 400D;

            var factory = new WebApiHelloWorldControllerFactory();
            var controller = factory.Controller;

            //act
            var response = await controller.GetEoq(eoqRequest);

            //assert
            var okResponse = (OkObjectResult)response;
            Assert.NotNull(okResponse.Value);
            Assert.IsType<EoqResponse>(okResponse.Value);

            var eoqResponse = (EoqResponse)okResponse.Value;
            Assert.Equal(expectedEoq, eoqResponse.Eoq);
        }


        [Fact]
        public async Task GetEoqWithZerohValueShouldReturnBadRequest()
        {
            //arrange
            var eoqRequest = new EoqRequest { D = 10000, K = 40, h = 0D };

            var factory = new WebApiHelloWorldControllerFactory();
            var controller = factory.Controller;

            //act
            var response = await controller.GetEoq(eoqRequest);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            var objectResult = (BadRequestObjectResult)response;
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }
    }
}
