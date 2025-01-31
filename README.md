# Setup and Run Instructions 

1. **Navigate to the Root Directory**

   Open your terminal or command prompt and navigate to the root directory of your solution where the `docker-compose.yml` file is located.

2. **Start Services**

   Run the following command to build and start all services:

   ```bash
   docker-compose up --build
   ```

   This command will automatically build the Docker images for your services and start RabbitMQ, PriceService, and WebSocketService within a Docker network.

3. **Install Postman**

   Download Postman from the [Postman's official website](https://www.postman.com/downloads/).

4. **Create Requests**

   Create requests to interact with the API and WebSockets:

   - API requests:
     - [List instruments](http://localhost:5277/instruments/v1/list)
     - [Get instrument price](http://localhost:5277/instruments/price/BTCUSD)

   - WebSocket request:
     - `ws://localhost:5042/ws`

5. **Handle High Load Scenarios**

   To handle high load scenarios, test situations where you have more active sockets than the configured limit. Scale the number of WebSocketService instances as needed and distribute the load among them using a load balancer, such as [Kong](https://docs.konghq.com/gateway/latest/introduction/) or [Kraken](https://kraken.io/).

