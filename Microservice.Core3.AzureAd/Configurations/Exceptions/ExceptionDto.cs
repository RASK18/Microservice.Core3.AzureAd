using Newtonsoft.Json;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public class ExceptionDto
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Message { get; set; }

        // ReSharper disable once UnusedMember.Global
        public ExceptionDto() { }

        public ExceptionDto(BaseCustomException custom)
        {
            Title = custom.Title;
            Detail = custom.Detail;
            Message = custom.Message;
        }

        public override string ToString()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }
    }
}