using AutoMapper;
using Microservice.Core3.AzureAd.Configurations.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microservice.Core3.AzureAd
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public static IConfiguration Configuration { get; set; }

        public static void ConfigureServices(IServiceCollection services)
        {
            ConfigureCors(services);
            ConfigureSwagger(services);
            InjectDependencies(services);
            ConfigureAuthentication(services);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            IMvcBuilder builder = services.AddControllers();
            ConfigureJsonSettings(builder);

            // External Custom 400 Bad Request with error message
            builder.ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = c =>
                throw new BadRequestCustomException(c.ModelState.Values.ToList().FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage));
        }

        // ReSharper disable once UnusedMember.Global
        public static void Configure(IApplicationBuilder app)
        {
            // Internal Custom Errors
            app.UseMiddleware<ExceptionsMiddleware>();

            // External Custom Errors
            app.UseStatusCodePages(c =>
            {
                int status = c.HttpContext.Response.StatusCode;
                CustomExceptions.Throw(status);
                return Task.CompletedTask;
            });

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            ConfigureSwagger(app);
        }
    }
}
