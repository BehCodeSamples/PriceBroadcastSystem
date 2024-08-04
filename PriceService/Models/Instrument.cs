namespace FinancialInstrumentService.Models
{
    /// <summary>
    /// Represents a financial instrument with a symbol and name.
    /// </summary>
    public class Instrument
    {
        /// <summary>
        /// Gets or sets the symbol of the financial instrument.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the name of the financial instrument.
        /// </summary>
        public string Name { get; set; }
    }
}
