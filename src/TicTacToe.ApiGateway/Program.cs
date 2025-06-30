using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Unified CORS policy
var allowedOrigins = builder.Configuration["AllowedCorsOrigins"]?.Split(';') ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Allow all localhost/127.0.0.1 ports in dev
            policy.SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:") ||
                origin.StartsWith("https://localhost:") ||
                origin.StartsWith("http://127.0.0.1:") ||
                origin.StartsWith("https://127.0.0.1:")
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        }
        else
        {
            // Use strict origins in prod
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

var app = builder.Build();

// Use unified CORS policy
app.UseCors("DevelopmentPolicy");

// Map reverse proxy
app.MapReverseProxy();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("API Gateway is healthy"));

app.Run();

// Make Program accessible for testing
namespace TicTacToe.ApiGateway {
    public partial class Program { }
} 