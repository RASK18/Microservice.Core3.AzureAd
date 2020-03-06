using Microservice.Core3.AzureAd.Configurations.Exceptions;
using Microservice.Core3.AzureAd.Data.Dto;
using Microservice.Core3.AzureAd.Literals;
using Microservice.Core3.AzureAd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

#pragma warning disable CA1822 // Mark members as static
namespace Microservice.Core3.AzureAd.Controllers
{
    [Authorize]
    [ApiController]
    [Route(Config.ApiController)]
    [Produces(Config.ApplicationJson)]
    [ProducesResponseType(typeof(ExceptionsResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ExceptionsResponse), StatusCodes.Status501NotImplemented)]
    public class ValueController : ControllerBase
    {
        private readonly ValueService _valueService;

        public ValueController(ValueService valueService) => _valueService = valueService;

        [HttpGet("RandomExceptionVoid")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void RandomExceptionVoid()
        {
            throw new Exception();
        }

        [HttpGet("RandomExceptionMessage")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void RandomExceptionMessage()
        {
            throw new Exception("This is a unhandle exception jeje");
        }

        [HttpGet("CustomExceptionVoid")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public void CustomExceptionVoid()
        {
            _valueService.CustomExceptionVoid();
        }

        [HttpGet("CustomExceptionMessage")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public void CustomExceptionMessage()
        {
            _valueService.CustomExceptionMessage();
        }

        [HttpGet("Normal")]
        [ProducesResponseType(typeof(ValueDto), StatusCodes.Status200OK)]
        public ValueDto Normal([FromQuery] int id)
        {
            return new ValueDto("This is a normal response");
        }

    }
}