using FastEndpoints;
using FastEndpoints.Swagger;
using TicTacToe.GameEngine.Persistence;

var builder = WebApplication.CreateBuilder(args);

// FastEndpoints registration
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

// Add services to the container
builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();

// Add CORS for cross-origin requests
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

// FastEndpoints
app.UseFastEndpoints();
app.UseSwaggerGen();

app.UseHttpsRedirection();

app.Run();

// Make Program accessible for testing
public partial class Program { }
