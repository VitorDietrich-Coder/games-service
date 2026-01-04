using FluentValidation;
using Games.Microservice.Application.Behaviours;
using Games.Microservice.Infrastructure.Persistence;
using MediatR;
 
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssemblyContaining<GamesDbContext>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
        });
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(UnhandledExceptionBehaviour<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CorrelationBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>));

        return services;
    }

}
public sealed class ApplicationAssemblyMarker { }



