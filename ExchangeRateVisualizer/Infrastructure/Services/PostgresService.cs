using Core;
using Core.Interfaces.Services;
using Npgsql;

namespace Infrastructure.Services
{
    public class PostgresService : IDatabaseService
    {
        private readonly string _connectionString;
        public PostgresService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AddExchangeRateAsync(IEnumerable<ExchangeRate> exchangeRates)
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
    }
}
