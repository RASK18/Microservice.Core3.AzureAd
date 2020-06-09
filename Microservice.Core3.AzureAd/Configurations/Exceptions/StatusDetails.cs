using System.Collections.Generic;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public static class StatusDetails
    {
        private const string BadRequest = "Server couldn't understand that, something was sent incorrectly";
        private const string Unauthorized = "Valid authentication credentials are required";
        private const string Forbidden = "You don't have permission to access";
        private const string NotFound = "We couldn't find that";
        private const string MethodNotAllowed = "The method received is known but not supported by the target resource";
        private const string Conflict = "Something conflicts with your request, check the current status of resources";
        private const string InternalServerError = "Something has gone wrong";
        private const string NotImplemented = "This functionality is not supported yet";
        private const string ServiceUnavailable = "Server is currently unable to handle your request due to a " +
                                                  "temporary overload or scheduled maintenance, try it again later";
        private const string TimeOut = "The server did not receive a timely response from another required server";

        private static readonly Dictionary<Types, string> Details = new Dictionary<Types, string>
        {
            { Types.BadRequest, BadRequest },
            { Types.Unauthorized, Unauthorized },
            { Types.Forbidden, Forbidden },
            { Types.NotFound, NotFound },
            { Types.MethodNotAllowed, MethodNotAllowed },
            { Types.Conflict, Conflict },
            { Types.InternalServerError, InternalServerError },
            { Types.NotImplemented, NotImplemented },
            { Types.ServiceUnavailable, ServiceUnavailable },
            { Types.TimeOut, TimeOut }
        };

        public static string Get(Types type) => Details[type];
    }

    public enum Types
    {
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        Conflict = 409,
        InternalServerError = 500,
        NotImplemented = 501,
        ServiceUnavailable = 503,
        TimeOut = 504
    }
}
