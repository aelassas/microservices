using CartMicroservice.Repository;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Middleware;
using MongoDB.Driver;
using System;

namespace CartMicroservice;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddJwtAuthentication(Configuration); // JWT Configuration

        services.AddMongoDb(Configuration);

        services.AddSingleton<ICartRepository>(sp =>
          new CartRepository(sp.GetService<IMongoDatabase>() ?? throw new Exception("IMongoDatabase not found"))
        );

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT token with the prefix Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
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
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog V1");
        });

        var option = new RewriteOptions();
        option.AddRedirect("^$", "swagger");
        app.UseRewriter(option);

        app.UseHealthChecks("/healthz", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecksUI();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseMiddleware<JwtMiddleware>(); // JWT Middleware

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}