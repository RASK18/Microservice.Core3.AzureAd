using Microservice.Core3.AzureAd.Configurations.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microservice.Core3.AzureAd
{
    public partial class Startup
    {
        private const string TitleV1 = "v1";
        private static readonly string ApiName = Assembly.GetExecutingAssembly().GetName().Name;

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                string filePath = Path.Combine(AppContext.BaseDirectory, ApiName + ".xml");
                o.IncludeXmlComments(filePath);
                o.OperationFilter<AddDefaultValues>();
                o.OperationFilter<AddErrorResponses>();
                o.SwaggerDoc(TitleV1, new OpenApiInfo { Title = ApiName, Version = "v1" });
                AddSecurity(o);
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        private static void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseSwagger(o =>
                o.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    swaggerDoc.Servers.Add(new OpenApiServer { Url = BasePath })));

            app.UseSwaggerUI(o =>
            {
                o.OAuthUsePkce();
                o.DisplayOperationId();
                o.DocumentTitle = ApiName;
                o.OAuthClientId(Configuration["AzureAd:ClientId"]);
                o.SwaggerEndpoint($"{BasePath}/swagger/{TitleV1}/swagger.json", " V1");
            });
        }

        private static void AddSecurity(SwaggerGenOptions o)
        {
            string scope = $"api://{Configuration["AzureAd:ClientId"]}/access_as_user"; // ToDo: Remember change this
            string authUrl = Configuration["AzureAd:Instance"] + Configuration["AzureAd:TenantId"] + "/oauth2/v2.0/authorize";
            string tokenUrl = Configuration["AzureAd:Instance"] + Configuration["AzureAd:TenantId"] + "/oauth2/v2.0/token";

            o.AddSecurityDefinition("aad-jwt", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authUrl),
                        TokenUrl = new Uri(tokenUrl),
                        Scopes = new Dictionary<string, string> { { scope, "Access as User" } }
                    }
                }
            });

            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Id = "aad-jwt", Type = ReferenceType.SecurityScheme }
                    },
                    new List<string> { scope }
                }
            });
        }

    }

    public class AddDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            IList<OpenApiSchema> schemas = context.SchemaRepository.Schemas
                                           .Select(s => s.Value)
                                           .SelectMany(v => v.Properties)
                                           .Select(p => p.Value)
                                           .Where(v => v.Default != null)
                                           .ToList();

            foreach (OpenApiSchema schema in schemas)
                schema.Example = schema.Default;
        }
    }

    public class AddErrorResponses : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(CustomExceptionDto), context.SchemaRepository);

            OpenApiReference reference = new OpenApiReference { Id = nameof(CustomExceptionDto), Type = ReferenceType.Schema };
            OpenApiSchema schema = new OpenApiSchema { Reference = reference };
            OpenApiMediaType mediaType = new OpenApiMediaType { Schema = schema };
            Dictionary<string, OpenApiMediaType> content = new Dictionary<string, OpenApiMediaType> { { "application/json", mediaType } };

            operation.Responses.Add("400", new OpenApiResponse { Description = "Bad Request", Content = content });
            operation.Responses.Add("500", new OpenApiResponse { Description = "Internal Server Error", Content = content });

            bool? haveAuth = context.MethodInfo?.DeclaringType?
                             .GetCustomAttributes(true)
                             .Union(context.MethodInfo?.GetCustomAttributes(true))
                             .OfType<AuthorizeAttribute>()
                             .Any();

            if (haveAuth != true) return;

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized", Content = content });
        }
    }

}
