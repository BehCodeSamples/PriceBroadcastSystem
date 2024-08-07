using System.Net.WebSockets;
using FinancialInstrumentService.Models;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FinancialInstrumentService.Services
{
    /// <summary>
    /// Provides WebSocket services including handling connections and broadcasting messages.
    /// </summary>
    public class WebSocketService
    {
        private const int BufferSize = 1024 * 4; // 4 KB buffer size

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new ConcurrentDictionary<Guid, WebSocket>();
        private readonly ILogger<WebSocketService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketService"/> class.
        /// Sets up the RabbitMQ connection and declares the price updates queue.
        /// </summary>
        /// <param name="logger">The logger to use for logging messages and errors.</param>
        public WebSocketService(ILogger<WebSocketService> logger)
        {
            _logger = logger;

            try
            {
                var factory = new ConnectionFactory() { HostName = "rabbitmq" };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare a queue for price updates
                _channel.QueueDeclare(queue: "price_updates",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Set up a consumer to receive messages
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var priceUpdate = JsonSerializer.Deserialize<PriceUpdate>(message);

                        // Broadcast to WebSocket clients
                        if (priceUpdate != null)
                        {
                            _logger.LogInformation("Received price update for symbol: {Symbol}.", priceUpdate.Symbol);
                            await BroadcastToClientsAsync(priceUpdate);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from RabbitMQ.");
                    }
                };
                _channel.BasicConsume(queue: "price_updates",
                                     autoAck: true,
                                     consumer: consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing WebSocketService.");
                throw;
            }
        }

        /// <summary>
        /// Handles the WebSocket connection, accepting the WebSocket request and managing the connection.
        /// </summary>
        /// <param name="context">The HTTP context containing the WebSocket request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleWebSocketAsync(HttpContext context)
        {
            try
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var clientId = Guid.NewGuid();
                    _clients.TryAdd(clientId, webSocket);

                    _logger.LogInformation("New WebSocket connection established. Active connections: {ConnectionCount}", _clients.Count);

                    // Manage the WebSocket connection
                    await ManageWebSocketConnectionAsync(clientId, webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    _logger.LogWarning("Invalid WebSocket request.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling WebSocket request.");
                context.Response.StatusCode = 500;
            }
        }

        /// <summary>
        /// Broadcasts a price update to all connected WebSocket clients.
        /// </summary>
        /// <param name="priceUpdate">The price update to broadcast.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task BroadcastToClientsAsync(PriceUpdate priceUpdate)
        {
            try
            {
                // Efficiently broadcast price updates to all connected WebSocket clients
                var priceData = $"Symbol: {priceUpdate.Symbol}, Price: {priceUpdate.Price}, Timestamp: {priceUpdate.Timestamp}";
                var buffer = Encoding.UTF8.GetBytes(priceData);

                _logger.LogInformation("Broadcasting message to {ClientCount} clients.", _clients.Count);

                var tasks = _clients.Values.Select(async client =>
                {
                    if (client.State == WebSocketState.Open)
                    {
                        await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                });

                // Wait for all messages to be sent
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting to WebSocket clients.");
            }
        }

        /// <summary>
        /// Manages a WebSocket connection, including receiving messages and handling disconnections.
        /// </summary>
        /// <param name="clientId">The unique identifier for the client.</param>
        /// <param name="webSocket">The WebSocket to manage.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ManageWebSocketConnectionAsync(Guid clientId, WebSocket webSocket)
        {
            var connectionStart = DateTime.UtcNow;
            try
            {
                var buffer = new byte[BufferSize];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // Handle messages from clients (if needed)

                // If WebSocket is closed, remove the client
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _clients.TryRemove(clientId, out _);
                    var connectionDuration = DateTime.UtcNow - connectionStart;
                    _logger.LogInformation("WebSocket connection closed after {Duration}. Active connections: {ConnectionCount}", connectionDuration, _clients.Count);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error managing WebSocket connection.");
                _clients.TryRemove(clientId, out _);
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Error occurred", CancellationToken.None);
            }
        }
    }
}
