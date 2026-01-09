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
            var serviceName =
                configuration["Service:Name"] ?? "games-microservice";

            var otlpEndpoint =
                configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
                ?? configuration["OpenTelemetry:OtlpEndpoint"]
                ?? "http://localhost:4317";

            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(
                                    serviceName: serviceName,
                                    serviceVersion: "1.0.0"))

                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })

                        .AddHttpClientInstrumentation()

                        .AddEntityFrameworkCoreInstrumentation()

                        .AddOtlpExporter(opt =>
                        {
                            opt.Endpoint = new Uri(otlpEndpoint);
                        });
                });

            return services;
        }
    }
}
