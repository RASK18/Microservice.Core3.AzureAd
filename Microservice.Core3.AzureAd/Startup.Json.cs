using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microservice.Core3.AzureAd
{
    public partial class Startup
    {
        private static void ConfigureJsonSettings(IMvcBuilder builder)
        {
            DefaultContractResolver snakeCase = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
            DefaultContractResolver camelCase = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };

            // Json Settings for all responses
            builder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = snakeCase;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            // Json Settings for all static Serialize and Deserialize
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = camelCase,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}