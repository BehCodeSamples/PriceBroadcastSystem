using Microsoft.AspNetCore.Mvc;
using FinancialInstrumentService.Models;

/// <summary>
/// API controller for handling requests related to financial instruments and their prices.
/// </summary>
[ApiController]
[Route("[controller]/v1")]
public class InstrumentsController : ControllerBase
{
    private readonly IPriceService _priceService;
    private readonly ILogger<InstrumentsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstrumentsController"/> class.
    /// </summary>
    /// <param name="priceService">An instance of <see cref="IPriceService"/> used to retrieve price information.</param>
    public InstrumentsController(IPriceService priceService, ILogger<InstrumentsController> logger)
    {
        _priceService = priceService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a list of all available financial instruments.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Instrument"/> objects.</returns>
    [HttpGet("list")]
    public IActionResult GetInstruments()
    {
        try
        {
            var instruments = _priceService.GetAvailableInstruments();
            return Ok(instruments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the list of instruments.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the list of instruments.");
        }
    }

    /// <summary>
    /// Retrieves the current price update for a specified financial instrument.
    /// </summary>
    /// <param name="symbol">The symbol of the financial instrument (e.g., "EURUSD", "BTCUSD").</param>
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing a <see cref="PriceUpdate"/> object with the latest price information for the specified symbol.
    /// Returns a 404 Not Found response if the symbol is not found.
    /// </returns>
    [HttpGet("price/{symbol}")]
    public ActionResult<PriceUpdate> GetPrice(string symbol)
    {
        try
        {
            var priceUpdate = _priceService.GetPrice(symbol);
            if (priceUpdate != null)
            {
                return Ok(priceUpdate);
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the price update for symbol {Symbol}.", symbol);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the price update.");
        }
    }
}
