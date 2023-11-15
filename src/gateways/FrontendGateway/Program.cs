using FrontendGateway;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var builder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config
                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("ocelot.json", false, true)
                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (hostingContext.HostingEnvironment.EnvironmentName == "Development")
            {
                config.AddJsonFile("appsettings.Local.json", true, true);
            }
        })
        .UseSerilog((_, config) =>
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

builder.Build().Run();