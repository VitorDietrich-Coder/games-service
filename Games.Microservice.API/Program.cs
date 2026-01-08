using Elastic.Clients.Elasticsearch;
using Games.Microservice.API.Elasticsearch;
using Games.Microservice.API.Extensions;
using Games.Microservice.Application.EventConsumers;
using Games.Microservice.Domain.Interfaces;
using Games.Microservice.Infrastructure.Elasticsearch;
using Games.Microservice.Infrastructure.EventStore;
using Games.Microservice.Infrastructure.Interfaces;
using Games.Microservice.Infrastructure.Messaging;
using Games.Microservice.Infrastructure.Persistence;
using Games.Microservice.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using Users.Microservice.API.Extensions;

var builder = WebApplication.CreateBuilder(args);



builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<GamesDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped);


builder.Services.AddScoped<ApplicationDbContextInitialiser>();
builder.Services.AddOpenTelemetryTracing(builder.Configuration);
builder.Services.AddApiVersioningConfiguration();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerServices();

builder.Services.AddGlobalCorsPolicy();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddElasticSearch(builder.Configuration);
builder.Services.AddScoped<IGameSearchRepository, GameSearchRepository>();
builder.Services.AddScoped<IUserPurchaseSearchRepository, UserPurchaseSearchRepository>();

builder.Services.AddHealthChecks();

builder.Services.AddApplication();

builder.Services.AddAuthorization();

builder.Host.UseSerilog((context, services, configuration) =>
{
    SerilogExtensions.ConfigureSerilog(context, services, configuration);
});
builder.Services.AddCustomAuthentication(builder.Configuration);

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IEventStore, StoredEvents>();
builder.Services.AddScoped<PaymentConfirmedConsumer>();

builder.Services.AddHostedService<PaymentCompletedRabbitConsumer>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    return new RabbitMqEventBus(builder.Configuration);
});
 
builder.Services.AddSingleton(sp =>
{
    var settings = new ElasticsearchClientSettings(
        new Uri(builder.Configuration["ElasticSearch:Uri"]!)
    );

    return new ElasticsearchClient(settings);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        initialiser.Initialise();
        initialiser.Seed();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialisation.");

        throw;
    }

}

// Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Games API v1");
//        c.RoutePrefix = string.Empty;
//    });
//}


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Games API v1");

    options.RoutePrefix = string.Empty;
});

app.UseRouting();


app.UseHttpsRedirection();


app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseMiddleware<UnauthorizedResponseMiddleware>();
app.UseAuthorization();

 
app.MapControllers();
 

app.Run();

