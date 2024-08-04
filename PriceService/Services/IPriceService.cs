using FinancialInstrumentService.Models;

/// <summary>
/// Interface for handling price-related operations and data caching.
/// </summary>
public interface IPriceService
{
    /// <summary>
    /// Retrieves a list of all available financial instruments.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Instrument"/> objects.</returns>
    IEnumerable<Instrument> GetAvailableInstruments();

    /// <summary>
    /// Retrieves the current price update for a specified financial instrument.
    /// </summary>
    /// <param name="symbol">The symbol of the financial instrument (e.g., "EURUSD", "BTCUSD").</param>
    /// <returns>
    /// A <see cref="PriceUpdate"/> object containing the latest price information for the specified symbol, 
    /// or <c>null</c> if the symbol is not found.
    /// </returns>
    PriceUpdate GetPrice(string symbol);

    /// <summary>
    /// Starts an asynchronous task that fetches and caches price updates from an external data source.
    /// This method runs in an infinite loop until a cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task FetchAndCachePricesAsync(CancellationToken cancellationToken);
}