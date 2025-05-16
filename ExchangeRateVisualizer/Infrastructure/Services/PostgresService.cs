using Core;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Services
{
    public class PostgresService : IDatabaseService
    {
        private readonly string _connectionString;

        public PostgresService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Postgress conection string is null");
        }

        public async Task<bool> AddExchangeRateAsync(IEnumerable<ExchangeRate> exchangeRates)
        {

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                foreach (var exchangeRate in exchangeRates)
                {
                    var command = new NpgsqlCommand(
                        @"INSERT INTO exchange_rates (base_currency, target_currency, rate, retrieved_at)
                      VALUES (@base_currency, @target_currency, @rate, @retrieved_at)", connection);

                    command.Parameters.AddWithValue("@base_currency", exchangeRate.BaseCurrency);
                    command.Parameters.AddWithValue("@target_currency", exchangeRate.TargetCurrency);
                    command.Parameters.AddWithValue("@rate", exchangeRate.Rate);
                    command.Parameters.AddWithValue("@retrieved_at", exchangeRate.RetreivedAt);

                    await command.ExecuteNonQueryAsync();

                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in PostgressExecution:" + ex);

                
            }

        }
    }
}
