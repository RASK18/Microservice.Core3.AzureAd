using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

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
                o.SwaggerDoc(TitleV1, new OpenApiInfo { Title = ApiName, Version = "v1" });

                string filePath = Path.Combine(AppContext.BaseDirectory, ApiName + ".xml");
                o.IncludeXmlComments(filePath);

                AddSecurity(o);
            });
        }

        private static void AddSecurity(SwaggerGenOptions o)
        {
            string scope = $"api://{Configuration["AzureAd:ClientId"]}/access_as_user";
            string authUrl = Configuration["AzureAd:Instance"] + Configuration["AzureAd:TenantId"] + "/oauth2/v2.0/authorize";

            o.AddSecurityDefinition("aad-jwt", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authUrl),
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

        private static void ConfigureSwagger(IApplicationBuilder app)
        {
            // This is to make it work with my kubernetes structure, you can use: app.UseSwagger();
            string processName = Process.GetCurrentProcess().ProcessName;
            bool isLocal = processName == "iisexpress" || processName == ApiName;
            string basePath = isLocal ? "" : "/FoLdEr_SeRvEr_NaMe"; // ToDo: Remember change this

            app.UseSwagger(o =>
                o.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    swaggerDoc.Servers.Add(new OpenApiServer { Url = basePath })));

            app.UseSwaggerUI(o =>
            {
                o.DisplayOperationId();
                o.DocumentTitle = ApiName;
                o.SwaggerEndpoint($"{basePath}/swagger/{TitleV1}/swagger.json", " V1");
            });
        }

    }
}
