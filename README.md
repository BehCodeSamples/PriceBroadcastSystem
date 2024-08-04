# Project Setup and Usage

## Prerequisites

1. **Install Erlang and RabbitMQ**

   Follow the installation instructions for Erlang and RabbitMQ for macOS. You can find them at the following links:
   - [Erlang Installation Guide](https://www.erlang.org/doc/install.html)
   - [RabbitMQ Installation Guide](https://www.rabbitmq.com/install-homebrew.html)

2. **Start RabbitMQ**

   After installation, start RabbitMQ using the command:
   ```bash
   rabbitmq-server

3. **Running the Projects**

   3.1 *Build the Project*
   
   Navigate to the project directory and run:
   ```
   dotnet build
   ```

   3.1 *Run the Project*
   
   Start the project with::
   ```
   dotnet run
   ```
   
5. **Testing the API**

    5.1 *Install Postman*

   Download and install Postman for testing API endpoints.

    5.2 *Create API Requests*

   Use the following endpoints for API requests:
   
  ```http
   GET http://localhost:5277/instruments/v1/list
  ```

  ```http
   GET http://localhost:5277/instruments/price/BTCUSD
  ```  

7. **Testing WebSocket**

   Use a WebSocket client to connect to:
   
  ```http
   ws://localhost:5042/ws
  ```
   
9. **Handling High Load**

    To handle high load scenarios:

9.1 *Monitor Active Sockets*

Check if you have more active sockets than the limit.

9.1 *Scale WebSocketService Instances*

Deploy additional instances of WebSocketService as needed.

9.1 *Use a Load Balancer*

Distribute the load between instances using a load balancer. Examples include:

Kong
Kraken
Configure the load balancer to manage traffic and balance the load effectively.
