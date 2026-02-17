using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Infrastructure.Elasticsearch
{
    public static class ElasticClientFactory
    {
        public static IElasticClient Create(IConfiguration config)
        {
            var settings = new ConnectionSettings(new Uri(config["ElasticSearch:Url"]))
                              .DefaultIndex(config["ElasticSearch:Index"]);

            return new ElasticClient(settings);

        }
    }
}
