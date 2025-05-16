

namespace Core.Interfaces.Services
{
    public interface IIndexingService
    {
        Task IndexExchangeRatesAsyc(IEnumerable<ExchangeRate> exchangeRates);
    }
}