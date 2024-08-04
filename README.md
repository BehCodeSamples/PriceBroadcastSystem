To set up and run the project in VS Code on macOS, follow these instructions:

Install Erlang and RabbitMQ:

Download Erlang from Erlang's official website.
Download RabbitMQ from RabbitMQ's official website.
Start RabbitMQ.

For each project (PriceService and WebSocketService), execute the following commands:

dotnet build
dotnet run
You need to install Postman for testing:

Download Postman from Postman's official website.
Create requests to interact with the API and WebSockets.

API requests are documented in Postman:

List instruments
Get instrument price
To subscribe to updates via WebSocket, use:

ws://localhost:5042/ws
To handle high load scenarios, test the situation where you have more active sockets than the configured limit. Scale the number of WebSocketService instances as needed and distribute the load among them using a load balancer, such as Kong or Kraken.
