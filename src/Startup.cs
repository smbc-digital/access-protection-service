using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using access_protection_service.Utils.ServiceCollectionExtensions;
using access_protection_service.Utils.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.NetStandard.Gateways;
using Microsoft.Extensions.Hosting;

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
            services.AddMvc().AddMvcOptions(_ => _.AllowEmptyInputInBodyModelBinding = true);
            services.AddSwagger();
            
            services.AddHealthChecks()
                .AddCheck<TestHealthCheck>("TestHealthCheck");                      
            
            services.AddAvailability();
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
            services.RegisterServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseMiddleware<ExceptionHandling>();           
            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);
            app.UseSwagger();

            var swaggerPrefix = (env.IsEnvironment("local") || env.IsEnvironment("Development") ? string.Empty : "/accessprotectionservice");

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{swaggerPrefix}/swagger/v1/swagger.json", "access_protection_service API");
            });
        }
    }
}
