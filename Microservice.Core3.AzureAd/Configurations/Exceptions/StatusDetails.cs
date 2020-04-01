using System.Collections.Generic;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public static class StatusDetails
    {
        private const string BadRequest = "Server couldn't understand that, something was sent incorrectly";
        private const string Unauthorized = "Valid authentication credentials are required";
        private const string Forbidden = "You don't have permission to access";
        private const string NotFound = "We couldn't find that";
        private const string Conflict = "Something conflicts with your request, check the current status of resources";
        private const string InternalServerError = "Something has gone wrong";
        private const string NotImplemented = "This functionality is not supported yet";
        private const string ServiceUnavailable = "Server is currently unable to handle your request due to a " +
                                                  "temporary overload or scheduled maintenance, try it again later";

        private static readonly Dictionary<Type, string> Details = new Dictionary<Type, string>
        {
            { Type.BadRequest, BadRequest },
            { Type.Unauthorized, Unauthorized },
            { Type.Forbidden, Forbidden },
            { Type.NotFound, NotFound },
            { Type.Conflict, Conflict },
            { Type.InternalServerError, InternalServerError },
            { Type.NotImplemented, NotImplemented },
            { Type.ServiceUnavailable, ServiceUnavailable }
        };

        public static string Get(Type type) => Details[type];
    }

    public enum Type
    {
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        InternalServerError = 500,
        NotImplemented = 501,
        ServiceUnavailable = 503
    }
}