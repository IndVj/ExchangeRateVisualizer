using System.Text.Json;
using Core;
using Core.Interfaces.Services;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class ElasticService : IIndexingService
    {
        private readonly string _indexName = "exchange-rates";
        private readonly ElasticLowLevelClient _elasticClient;

        public ElasticService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Elasticsearch") ??
                throw new InvalidOperationException("Elastic search connection string is null");

            var settings = new ConnectionConfiguration(new Uri(connectionString))
              .RequestTimeout(TimeSpan.FromSeconds(30));

            _elasticClient = new ElasticLowLevelClient(settings);
        }
         

        public async Task IndexExchangeRatesAsyc(IEnumerable<ExchangeRate> exchangeRates)
        {

            foreach (var exchangeRate in exchangeRates) 
            { 
                var jsonData = JsonSerializer.Serialize(exchangeRate);

                var indexResponse = await _elasticClient.IndexAsync<StringResponse>(_indexName, PostData.String(jsonData));

                if (!indexResponse.Success)
                {
                    throw new ElasticsearchClientException($"Failed to index document: {indexResponse.DebugInformation}");
                }

            }
        }
    }
}
