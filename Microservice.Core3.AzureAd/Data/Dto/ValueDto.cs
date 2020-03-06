namespace Microservice.Core3.AzureAd.Data.Dto
{
    public class ValueDto
    {
        public string NameOfValue { get; set; }

        public ValueDto(string name = null) => NameOfValue = name;
    }
}
