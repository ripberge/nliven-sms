using Serilog;
using SmsService.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddHealthChecks();
builder.Services.ConfigureApiServices(builder.Configuration);

var app = builder.Build();

// Configure middleware
app.UseHealthChecks("/health/live");
app.UseHealthChecks("/health/ready");

// Custom health endpoint
app.MapGet("/health", () => new { status = "healthy" }).WithName("Health");

app.MapHealthChecks("/health");

app.Run();

Log.CloseAndFlush();
