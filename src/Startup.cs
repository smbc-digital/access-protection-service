using System.Diagnostics.CodeAnalysis;
using access_protection_service.Utils.ServiceCollectionExtensions;
using access_protection_service.Utils.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.AspNetCore.Availability.Middleware;
using StockportGovUK.NetStandard.Gateways;

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
            services.AddControllers()
                    .AddNewtonsoftJson()
                    .AddMvcOptions(_ => _.AllowEmptyInputInBodyModelBinding = true);
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
            services.AddSwagger();
            services.AddHealthChecks()
                    .AddCheck<TestHealthCheck>("TestHealthCheck");               
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

            app.UseMiddleware<Availability>();
            app.UseMiddleware<ApiExceptionHandling>();          

            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{(env.IsEnvironment("local") ? string.Empty : "/accessprotectionservice")}/swagger/v1/swagger.json", "Access protection service API");
            });
        }
    }
}
