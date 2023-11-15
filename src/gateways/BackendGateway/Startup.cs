using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
namespace BackendGateway;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddOcelot(Configuration);

        services.AddJwtAuthentication(Configuration); // JWT Configuration

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddHealthChecks()
            .AddMongoDb(
                mongodbConnectionString: (
                    Configuration.GetSection("mongo").Get<MongoOptions>()
                    ?? throw new Exception("mongo configuration section not found")
                ).ConnectionString,
                name: "mongo",
                failureStatus: HealthStatus.Unhealthy
            );

        services.AddHealthChecksUI().AddInMemoryStorage();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<RequestResponseLogging>();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();

        app.UseHealthChecks("/healthz", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecksUI();

        var option = new RewriteOptions();
        option.AddRedirect("^$", "healthchecks-ui");
        app.UseRewriter(option);

        await app.UseOcelot();
    }
}