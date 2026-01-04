using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Games.Microservice.API.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetryTracing(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var serviceName = configuration["Service:Name"] ?? "Users.Microservice";
            var serviceVersion = "1.0.0";

            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(
                                    serviceName: serviceName,
                                    serviceVersion: serviceVersion))

                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })

                        .AddHttpClientInstrumentation()

                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                        })

                        .AddConsoleExporter()

                        .AddJaegerExporter(jaegerOptions =>
                        {
                            jaegerOptions.AgentHost = configuration["Jaeger:Host"] ?? "localhost";
                            jaegerOptions.AgentPort = int.Parse(configuration["Jaeger:Port"] ?? "6831");
                        });
                });

            return services;
        }
    }
}
