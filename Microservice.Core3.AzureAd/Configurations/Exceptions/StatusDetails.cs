namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public static class StatusDetails
    {
        public const string Title400 = "Bad Request";
        public const string Detail400 = "Server couldn't understand that, something was sent incorrectly";

        public const string Title401 = "Unauthorized";
        public const string Detail401 = "Valid authentication credentials are required";

        public const string Title403 = "Forbidden";
        public const string Detail403 = "You don't have permission to access";

        public const string Title404 = "Not Found";
        public const string Detail404 = "We couldn't find that";

        public const string Title409 = "Conflict";
        public const string Detail409 = "Something conflicts with your request, check the current status of resources";

        public const string Title500 = "Internal Server Error";
        public const string Detail500 = "Something has gone wrong";

        public const string Title501 = "Not Implemented";
        public const string Detail501 = "This functionality is not supported yet";

        public const string Title503 = "Service Unavailable";
        public const string Detail503 = "Server is currently unable to handle your request due to a temporary overload or scheduled maintenance, try it again later";
    }
}