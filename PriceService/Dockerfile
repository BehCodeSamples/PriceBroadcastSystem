# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the .NET 8.0 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Build the runtime image
FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y netcat-openbsd
COPY wait-for-it.sh /app/
RUN chmod +x /app/wait-for-it.sh
# Copy and build your application...
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["/app/wait-for-it.sh", "rabbitmq", "5672", "dotnet", "PriceService.dll"]
