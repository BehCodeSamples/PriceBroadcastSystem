using FinancialInstrumentService.Services; 
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.UseUrls("http://localhost:5277");

// Add services to the container.
builder.Services.AddSingleton<IDataProvider, MockDataProvider>();
builder.Services.AddSingleton<IPriceService, PriceService>(); // Register PriceService as a singleton
builder.Services.AddControllers();  // Register controllers

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Add more logging providers if needed (e.g., Debug, EventSource)

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Finance tools API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finance tools API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}


app.UseHttpsRedirection();
app.UseRouting();

var cts = new CancellationTokenSource();
app.Lifetime.ApplicationStopping.Register(() =>
{
    cts.Cancel();
});

var priceService = app.Services.GetRequiredService<IPriceService>();
Task.Run(() => priceService.FetchAndCachePricesAsync(cts.Token));

app.MapControllers();

app.Run();

