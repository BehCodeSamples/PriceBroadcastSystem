using FinancialInstrumentService.Models;

namespace FinancialInstrumentService.Services
{
    public class MockDataProvider : IDataProvider
    {
        public Task<PriceUpdate> FetchPriceUpdateAsync(string symbol)
        {
            // Example using a mocked response
            var priceUpdate = new PriceUpdate
            {
                Symbol = symbol,
                Price = 30000.00m,
                Timestamp = DateTime.UtcNow
            };

            return Task.FromResult(priceUpdate);
        }
    }
}
