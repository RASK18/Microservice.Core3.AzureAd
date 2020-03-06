using Microservice.Core3.AzureAd.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Core3.AzureAd
{
    public partial class Startup
    {
        private static void InjectDependencies(IServiceCollection services)
        {
            services.AddScoped<ValueService>();
        }
    }
}
