using Nest;

namespace Games.Microservice.API.Elasticsearch;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("ElasticSearch");

        var uri = settings["Url"];
        var index = settings["Index"];

        if (string.IsNullOrWhiteSpace(uri))
            throw new InvalidOperationException("ElasticSearch:Uri not configured");

        var connectionSettings = new ConnectionSettings(new Uri(uri))
            .DefaultIndex(index)
            .EnableApiVersioningHeader()
            .PrettyJson();

        var client = new ElasticClient(connectionSettings);

        services.AddSingleton<IElasticClient>(client);

        return services;
    }
}
