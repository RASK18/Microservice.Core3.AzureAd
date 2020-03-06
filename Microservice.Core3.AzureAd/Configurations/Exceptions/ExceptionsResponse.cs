using Newtonsoft.Json;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public class ExceptionsResponse
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Message { get; set; }

        public string Method { get; set; }
        public string Request { get; set; }
        public string Body { get; set; }

        public override string ToString()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }
    }
}