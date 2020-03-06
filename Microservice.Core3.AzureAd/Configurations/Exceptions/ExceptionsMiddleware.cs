﻿using AutoMapper;
using Microservice.Core3.AzureAd.Literals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public class ExceptionsMiddleware
    {
        private readonly IMapper _mapper;
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionsMiddleware> _logger;

        public ExceptionsMiddleware(RequestDelegate next, IMapper mapper, ILogger<ExceptionsMiddleware> logger)
        {
            _next = next;
            _mapper = mapper;
            _logger = logger;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
                throw;
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            if (!(exception is BaseCustomException customException))
                customException = ConvertToCustom(context.Response.StatusCode, exception.Message);

            ExceptionsResponse error = _mapper.Map<ExceptionsResponse>(customException);

            HttpResponse response = context.Response;
            response.StatusCode = customException.Code;
            response.ContentType = Config.ApplicationJson;
            await response.WriteAsync(error.ToString());

            error.Method = context.Request.Method;
            error.Request = exception.Source + " --> " + context.Request.Path;

            string[] bodyMethods = { "POST", "PUT", "PATCH" };
            if (bodyMethods.Contains(error.Method))
                error.Body = await ReadBody(context);

            _logger.LogError("\r\n" + error);
        }

        private static BaseCustomException ConvertToCustom(int statusCode, string message) =>
            statusCode switch
            {
                StatusCodes.Status400BadRequest => new BadRequestCustomException(message),
                StatusCodes.Status401Unauthorized => new UnauthorizedCustomException(message),
                StatusCodes.Status403Forbidden => new ForbiddenCustomException(message),
                StatusCodes.Status404NotFound => new NotFoundCustomException(message),
                StatusCodes.Status409Conflict => new ConflictCustomException(message),
                StatusCodes.Status501NotImplemented => new NotImplementedCustomException(message),
                StatusCodes.Status503ServiceUnavailable => new ServiceUnavailableCustomException(message),
                _ => new InternalServerErrorCustomException(message)
            };

        private static async Task<string> ReadBody(HttpContext context)
        {
            context.Request.Body.Position = 0;

            using StreamReader reader = new StreamReader(context.Request.Body);
            string body = await reader.ReadToEndAsync();
            return body;
        }

    }
}