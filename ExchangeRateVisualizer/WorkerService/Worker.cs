using Core.Interfaces.Services;
using Infrastructure.Services;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IDatabaseService _databaseService;
        private readonly IIndexingService _elasticService;
        private readonly int intervalminutes;
        private const string BaseCurrency = "EUR";

        public Worker(ILogger<Worker> logger, IExchangeRateService exchangeRateService, 
                      IDatabaseService databaseService, IIndexingService elasticService, 
                      IConfiguration configuration)
        {
            _logger = logger;
            _exchangeRateService = exchangeRateService;
            _databaseService = databaseService;
            _elasticService = elasticService;
            this.intervalminutes = configuration.GetValue<int>("FetchSettings:IntervalMinutes"); ;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    _logger.LogInformation("Fetching exchange rates...");

                    var exchangeRates = await _exchangeRateService.FetchLatestRatesAsync(BaseCurrency);

                    _logger.LogInformation("Saving exchange rates...");

                    await _databaseService.AddExchangeRateAsync(exchangeRates);

                    _logger.LogInformation("Indexing exchange rates...");

                    await _elasticService.IndexExchangeRatesAsyc(exchangeRates);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing exchange rates.");
                }


                await Task.Delay(TimeSpan.FromMinutes(intervalminutes), stoppingToken);
            }
        }
    }
}
