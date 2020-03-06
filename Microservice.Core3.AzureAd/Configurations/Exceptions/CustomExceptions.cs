using Microsoft.AspNetCore.Http;
using System;

#pragma warning disable CA1032 // Implement standard exception constructors
namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public static class CustomExceptions
    {
        public static void Throw(int statusCode, string message = null) =>
            throw (statusCode switch
            {
                StatusCodes.Status400BadRequest => new BadRequestCustomException(message),
                StatusCodes.Status401Unauthorized => new UnauthorizedCustomException(message),
                StatusCodes.Status403Forbidden => new ForbiddenCustomException(message),
                StatusCodes.Status404NotFound => new NotFoundCustomException(message),
                StatusCodes.Status409Conflict => new ConflictCustomException(message),
                StatusCodes.Status501NotImplemented => new NotImplementedCustomException(message),
                StatusCodes.Status503ServiceUnavailable => new ServiceUnavailableCustomException(message),
                _ => new InternalServerErrorCustomException(message)
            });
    }

    public abstract class BaseCustomException : Exception
    {
        public int Code { get; }
        public string Title { get; }
        public string Detail { get; }

        protected BaseCustomException(string message, int code, string title, string detail) : base(message ?? detail)
        {
            Code = code;
            Title = title;
            Detail = detail;
        }
    }

    public class BadRequestCustomException : BaseCustomException
    {
        public BadRequestCustomException(string message = null) : base(message,
            StatusCodes.Status400BadRequest, StatusDetails.Title400, StatusDetails.Detail400)
        {
        }
    }

    public class UnauthorizedCustomException : BaseCustomException
    {
        public UnauthorizedCustomException(string message = null) : base(message,
            StatusCodes.Status401Unauthorized, StatusDetails.Title401, StatusDetails.Detail401)
        {
        }
    }

    public class ForbiddenCustomException : BaseCustomException
    {
        public ForbiddenCustomException(string message = null) : base(message,
            StatusCodes.Status403Forbidden, StatusDetails.Title403, StatusDetails.Detail403)
        {
        }
    }

    public class NotFoundCustomException : BaseCustomException
    {
        public NotFoundCustomException(string message = null) : base(message,
            StatusCodes.Status404NotFound, StatusDetails.Title404, StatusDetails.Detail404)
        {
        }
    }

    public class ConflictCustomException : BaseCustomException
    {
        public ConflictCustomException(string message = null) : base(message,
            StatusCodes.Status409Conflict, StatusDetails.Title409, StatusDetails.Detail409)
        {
        }
    }

    public class InternalServerErrorCustomException : BaseCustomException
    {
        public InternalServerErrorCustomException(string message = null) : base(message,
            StatusCodes.Status500InternalServerError, StatusDetails.Title500, StatusDetails.Detail500)
        {
        }
    }

    public class NotImplementedCustomException : BaseCustomException
    {
        public NotImplementedCustomException(string message = null) : base(message,
            StatusCodes.Status501NotImplemented, StatusDetails.Title501, StatusDetails.Detail501)
        {
        }
    }

    public class ServiceUnavailableCustomException : BaseCustomException
    {
        public ServiceUnavailableCustomException(string message = null) : base(message,
            StatusCodes.Status503ServiceUnavailable, StatusDetails.Title503, StatusDetails.Detail503)
        {
        }
    }
}
