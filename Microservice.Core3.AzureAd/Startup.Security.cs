using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Microservice.Core3.AzureAd
{
    public partial class Startup
    {
        private static void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddJwtBearer("AzureAD", o =>
                    {
                        o.Audience = Configuration["AzureAd:ClientId"];
                        o.Authority = Configuration["AzureAd:Instance"] + Configuration["AzureAd:TenantId"];

                        o.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidAudience = o.Audience,
                            ValidIssuer = o.Authority + "/v2.0"
                        };
                    }
                );
        }

        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy",
                    b => b.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }
    }
}