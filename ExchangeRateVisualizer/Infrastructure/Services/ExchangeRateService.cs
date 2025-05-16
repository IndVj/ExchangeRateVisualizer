using System.Text.Json;
using System.Text.Json.Serialization;
using Core;
using Core.Interfaces.Services;

namespace Infrastructure.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ExchangeRate>> FetchLatestRatesAsync(string baseCurrency)
        {

            var reponse = await _httpClient.GetAsync($"https://open.er-api.com/v6/latest/{baseCurrency}");

            reponse.EnsureSuccessStatusCode();

            var responseJson = await reponse.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ExchangeRateApiResponse>(responseJson);

            if (apiResponse == null)
            {
                return Enumerable.Empty<ExchangeRate>();
            }

            return apiResponse.Rates.Select(t => new ExchangeRate
            {
                BaseCurrency = baseCurrency,
                TargetCurrency = t.Key,
                Rate = t.Value,
                RetreivedAt = DateTime.UtcNow

            });
        }

        private class ExchangeRateApiResponse
        {
            [JsonPropertyName("base_code")]
            public string BaseCode { get; set; } = default!;

            [JsonPropertyName("rates")]
            public Dictionary<string, decimal> Rates { get; set; } = default!;
        }

    }
}
