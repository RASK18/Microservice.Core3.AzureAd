using Newtonsoft.Json;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public class CustomExceptionDto
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Message { get; set; }

        public CustomExceptionDto(CustomException ex)
        {
            Title = ex.Title;
            Detail = ex.Detail;
            Message = ex.Message;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}