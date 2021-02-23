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
                o.OAuthUsePkce(); // ToDo: No need if Implicit
                o.DisplayOperationId();
                o.DocumentTitle = ApiName;
                o.OAuthClientId(Configuration["Security:ClientId"]);
                o.SwaggerEndpoint($"{BasePath}/swagger/{TitleV1}/swagger.json", " V1");
            });
        }

        private static void AddSecurity(SwaggerGenOptions o)
        {
            const string securityName = "OAuth2";
            const string authEndpoint = "/oauth2/v2.0"; // ToDo: Change if necessary
            string tokenUrl = Configuration["Security:Authority"] + authEndpoint + "/token";
            string authUrl = Configuration["Security:Authority"] + authEndpoint + "/authorize";
            Dictionary<string, string> scope = new Dictionary<string, string>
            {
                { $"api://{Configuration["Security:ClientId"]}/access_as_user", "Access as User" } // ToDo: Remember change this
            };

            o.AddSecurityDefinition(securityName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow // ToDo: Check if Implicit
                    {
                        TokenUrl = new Uri(tokenUrl),
                        AuthorizationUrl = new Uri(authUrl),
                        Scopes = scope
                    }
                }
            });

            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = securityName, Type = ReferenceType.SecurityScheme } },
                    scope.Select(s => s.Key).ToList()
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
