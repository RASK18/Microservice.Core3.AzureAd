using Microservice.Core3.AzureAd.Literals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public class ExceptionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionsMiddleware> _logger;

        public ExceptionsMiddleware(RequestDelegate next, ILogger<ExceptionsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // ReSharper disable once UnusedMember.Global 
        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                await _next.Invoke(context);
            }
            catch (CustomException customEx)
            {
                await HandleException(context, customEx);
            }
            catch (Exception ex)
            {
                CustomException customEx = new CustomException(Type.InternalServerError, ex.Message) { Source = ex.Source };
                await HandleException(context, customEx);
            }
        }

        private async Task HandleException(HttpContext context, CustomException customEx)
        {
            HttpResponse response = context.Response;
            response.StatusCode = customEx.Code;
            response.ContentType = Config.ApplicationJson;
            CustomExceptionDto dto = new CustomExceptionDto(customEx);
            await response.WriteAsync(dto.ToString());

            await customEx.AddRequest(context.Request);
            _logger.LogError("\r\n" + customEx);
        }
    }
}