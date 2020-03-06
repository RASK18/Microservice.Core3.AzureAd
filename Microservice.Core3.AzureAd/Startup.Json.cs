using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microservice.Core3.AzureAd
{
    public partial class Startup
    {
        private static void ConfigureJsonSettings(IMvcBuilder builder)
        {
            DefaultContractResolver resolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };

            // Json Settings for all responses
            builder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = resolver;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            // Json Settings for all static Serialize and Deserialize
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = resolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
    }
}
