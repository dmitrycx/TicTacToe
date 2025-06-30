using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.AddServiceDefaults();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms<WebSocketTransform>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseServiceDefaults();

// Use CORS
app.UseCors("AllowAll");

// Map reverse proxy
app.MapReverseProxy();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("API Gateway is healthy"));

app.Run();

// Custom transform for WebSocket support
public class WebSocketTransform : ITransformProvider
{
    public void ValidateRoute(TransformRouteValidationContext context)
    {
        // No validation needed for this transform
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
        // No validation needed for this transform
    }

    public void Apply(TransformBuilderContext context)
    {
        // Add WebSocket support headers
        context.AddRequestTransform(async context =>
        {
            // Forward WebSocket headers
            if (context.HttpContext.Request.Headers.ContainsKey("Upgrade"))
            {
                context.ProxyRequest.Headers.Add("Upgrade", context.HttpContext.Request.Headers["Upgrade"]);
            }
            
            if (context.HttpContext.Request.Headers.ContainsKey("Connection"))
            {
                context.ProxyRequest.Headers.Add("Connection", context.HttpContext.Request.Headers["Connection"]);
            }
            
            if (context.HttpContext.Request.Headers.ContainsKey("Sec-WebSocket-Key"))
            {
                context.ProxyRequest.Headers.Add("Sec-WebSocket-Key", context.HttpContext.Request.Headers["Sec-WebSocket-Key"]);
            }
            
            if (context.HttpContext.Request.Headers.ContainsKey("Sec-WebSocket-Version"))
            {
                context.ProxyRequest.Headers.Add("Sec-WebSocket-Version", context.HttpContext.Request.Headers["Sec-WebSocket-Version"]);
            }
            
            if (context.HttpContext.Request.Headers.ContainsKey("Sec-WebSocket-Protocol"))
            {
                context.ProxyRequest.Headers.Add("Sec-WebSocket-Protocol", context.HttpContext.Request.Headers["Sec-WebSocket-Protocol"]);
            }
        });
    }
} 