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
EXPOSE 8080
# EXPOSE 5042
# ENV ASPNETCORE_HTTP_PORTS=5042
# EXPOSE 5277
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebSocketService.dll"]
