# Setup and Run Instructions

## 1. Install Erlang and RabbitMQ

- Download Erlang from the [Erlang's official website](https://www.erlang.org/downloads).
- Download RabbitMQ from the [RabbitMQ's official website](https://www.rabbitmq.com/download.html).

## 2. Start RabbitMQ

## 3. Build and Run Projects

For each project (PriceService and WebSocketService), execute the following commands:

```bash
dotnet build
dotnet run
```

## 4. Install Postman

Download Postman from the [Postman's official website](https://www.postman.com/downloads/).

## 5. Create Requests

Create requests to interact with the API and WebSockets.

API requests are documented in Postman:

- [List instruments](http://localhost:5277/instruments/v1/list)
- [Get instrument price](http://localhost:5277/instruments/price/BTCUSD)

To subscribe to updates via WebSocket, use:

- `ws://localhost:5042/ws`

## 6. Handle High Load Scenarios

To handle high load scenarios, test the situation where you have more active sockets than the configured limit. Scale the number of WebSocketService instances as needed and distribute the load among them using a load balancer, such as [Kong](https://docs.konghq.com/gateway/latest/introduction/) or [Kraken](https://kraken.io/).
