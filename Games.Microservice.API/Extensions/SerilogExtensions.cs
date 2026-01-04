using Serilog;
using Serilog.Formatting.Json;
using Serilog.Events;
using Serilog.Enrichers.Span;

namespace Users.Microservice.API.Extensions
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(
            HostBuilderContext context,
            IServiceProvider services,
            LoggerConfiguration configuration)
        {
            configuration

                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)


                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()

                .Enrich.WithSpan()

                .Enrich.WithProperty(
                    "Application",
                    context.HostingEnvironment.ApplicationName)
                .Enrich.WithProperty(
                    "Environment",
                    context.HostingEnvironment.EnvironmentName)

                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] " +
                    "[TraceId:{TraceId}] " +
                    "{Message:lj}{NewLine}{Exception}")

                .WriteTo.File(
                    new JsonFormatter(renderMessage: true),
                    path: "logs/log-.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    shared: true);
        }
    }
}
