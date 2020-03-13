namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public class ExceptionModel : ExceptionDto
    {
        public string Method { get; set; }
        public string Request { get; set; }
        public string Body { get; set; }

        public ExceptionModel(ExceptionDto dto)
        {
            Title = dto.Title;
            Detail = dto.Detail;
            Message = dto.Message;
        }
    }
}