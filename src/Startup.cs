﻿using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using access_protection_service.Utils.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.NetStandard.Gateways;
using Swashbuckle.AspNetCore.Swagger;
using access_protection_service.Services;

namespace access_protection_service
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc().AddMvcOptions(_ => _.AllowEmptyInputInBodyModelBinding = true);
            
            services.AddHealthChecks()
                .AddCheck<TestHealthCheck>("TestHealthCheck");                      
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "access_protection_service API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                });
            });
  
            services.AddAvailability();
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
            services.AddTransient<IAccessProtectionService, AccessProtectionService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
                       
            app.UseMiddleware<ExceptionHandling>();           
            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);
            app.UseMvc();
            app.UseSwagger();

            var swaggerPrefix = (env.EnvironmentName == "local" || env.EnvironmentName == "Development") ? string.Empty : "/accessprotectionservice";

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{swaggerPrefix}/swagger/v1/swagger.json", "access_protection_service API");
            });
        }
    }
}
