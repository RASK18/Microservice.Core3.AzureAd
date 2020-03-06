namespace Microservice.Core3.AzureAd.Data.Models
{
    public class Value
    {
        public string Id { get; set; }

        public Value(string id = null) => Id = id;
    }
}