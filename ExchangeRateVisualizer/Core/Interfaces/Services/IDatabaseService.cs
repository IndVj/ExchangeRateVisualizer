namespace Core.Interfaces.Services
{
    public interface IDatabaseService
    {
        Task<bool> AddExchangeRateAsync(IEnumerable<ExchangeRate> exchangeRates);
    }
}