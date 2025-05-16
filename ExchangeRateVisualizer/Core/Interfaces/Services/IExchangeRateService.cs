
namespace Core.Interfaces.Services
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRate>> FetchLatestRatesAsync(string baseCurrency);
    }
}