using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Microservice.Core3.AzureAd
{
    public partial class Startup
    {
        private static void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme) // ToDo: Use your schema
                    .AddJwtBearer(AzureADDefaults.AuthenticationScheme, o =>
                    {
                        o.RequireHttpsMetadata = true;
                        o.Audience = Configuration["Security:ClientId"];
                        o.Authority = Configuration["Security:Authority"] + "/v2.0"; // ToDo: v2 only if v2
                        o.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = o.Authority,
                            ValidAudience = o.Audience // ToDo: Use ValidAudiences if many
                        };
                    });
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