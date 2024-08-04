using FinancialInstrumentService.Services;  // Assuming you have a Services namespace for services
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddSingleton<WebSocketService>();  // Register WebSocketService

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Financial Instrument Service",
        Version = "v1",
        Description = "API for managing financial instruments. WebSocket endpoint available for real-time updates."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financial Instrument Service v1");
        c.RoutePrefix = string.Empty; // To serve the Swagger UI at the root URL
    });
}

app.UseWebSockets();

// Add WebSocket endpoint
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        var webSocketService = context.RequestServices.GetRequiredService<WebSocketService>();
        await webSocketService.HandleWebSocketAsync(context);
    }
    else
    {
        await next();
    }
});

app.Run();
