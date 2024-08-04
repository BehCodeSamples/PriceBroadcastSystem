using FinancialInstrumentService.Models;

namespace FinancialInstrumentService.Services
{
    /// <summary>
    /// Defines a contract for fetching price updates for financial instruments.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Asynchronously fetches the current price update for the specified financial instrument symbol.
        /// </summary>
        /// <param name="symbol">The symbol of the financial instrument (e.g., "EURUSD", "BTCUSD").</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="PriceUpdate"/> object 
        /// with the latest price information for the specified symbol.
        /// </returns>
        Task<PriceUpdate> FetchPriceUpdateAsync(string symbol);
    }
}