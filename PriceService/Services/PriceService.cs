using System.Text;
using System.Text.Json;
using FinancialInstrumentService.Models;
using RabbitMQ.Client;

namespace FinancialInstrumentService.Services
{
    public class PriceService : IPriceService
    {
        private readonly Dictionary<string, PriceUpdate> _priceCache = new();
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<PriceService> _logger;
        private readonly IDataProvider _dataProvider;

        public PriceService(
            ILogger<PriceService> logger,
            IDataProvider dataProvider)
        {
            _logger = logger;
            _dataProvider = dataProvider;

            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare a queue for price updates
                _channel.QueueDeclare(queue: "price_updates",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                _logger.LogInformation("PriceService initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize PriceService.");
                throw;
            }
        }

        public IEnumerable<Instrument> GetAvailableInstruments() //TODO use DB and cache here 
        {
            try
            {
                // Simulate fetching instruments
                var instruments = new List<Instrument>
                {
                    new Instrument { Symbol = "EURUSD", Name = "Euro / US Dollar" },
                    new Instrument { Symbol = "USDJPY", Name = "US Dollar / Japanese Yen" },
                    new Instrument { Symbol = "BTCUSD", Name = "Bitcoin / US Dollar" }
                };

                _logger.LogInformation("Retrieved available instruments successfully.");
                return instruments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get available instruments.");
                throw;
            }
        }

        public PriceUpdate GetPrice(string symbol)
        {
            try
            {
                if (_priceCache.TryGetValue(symbol, out var priceUpdate))
                {
                    _logger.LogInformation("Cache hit for symbol {Symbol}: {Price}", symbol, priceUpdate.Price);
                    return priceUpdate;
                }
                else
                {
                    _logger.LogWarning("Cache miss for symbol {Symbol}.", symbol);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get price for symbol: {symbol}.");
                throw;
            }
        }

        public async Task FetchAndCachePricesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var startTime = DateTime.UtcNow;
                    var newPriceUpdate = await _dataProvider.FetchPriceUpdateAsync("BTCUSD");

                    // Cache the price update
                    _priceCache[newPriceUpdate.Symbol] = newPriceUpdate;

                    var message = JsonSerializer.Serialize(newPriceUpdate);
                    var body = Encoding.UTF8.GetBytes(message);

                    // Publish the price update message to the queue
                    _channel.BasicPublish(exchange: "",
                                         routingKey: "price_updates",
                                         basicProperties: null,
                                         body: body);

                    var duration = DateTime.UtcNow - startTime;
                    _logger.LogInformation("Published price update for {Symbol}: {Price}. Fetch duration: {Duration} ms", newPriceUpdate.Symbol, newPriceUpdate.Price, duration.TotalMilliseconds);

                    await Task.Delay(5000, cancellationToken); // Poll every 5 seconds
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch and cache prices.");
                }
            }
        }
    }
}
