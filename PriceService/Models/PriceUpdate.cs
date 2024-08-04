namespace FinancialInstrumentService.Models
{
    /// <summary>
    /// Represents a price update for a financial instrument.
    /// </summary>
    public class PriceUpdate
    {
        /// <summary>
        /// Gets or sets the symbol of the financial instrument.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the current price of the financial instrument.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the price update was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
