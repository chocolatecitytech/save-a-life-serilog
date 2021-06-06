using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace working_with_serilog
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .AddEnvironmentVariables()
            .Build();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .UseKestrel()
                .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Verbose)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
            .Enrich.FromLogContext()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
            .WriteTo.File("C:\\Temp\\logs\\working-with-serilog.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
            .CreateLogger();
            try
            {
                var webHost = BuildWebHost(args);
                webHost.Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
