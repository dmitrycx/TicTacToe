using TicTacToe.GameEngine.Endpoints;
using TicTacToe.GameEngine.Endpoints.DTOs;
using TicTacToe.GameEngine.Persistence;

var builder = WebApplication.CreateBuilder(args);

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

// Map endpoints
app.MapPost("/games", CreateGame.HandleAsync);
app.MapGet("/games/{gameId:guid}", GetGameState.HandleAsync);
app.MapPost("/games/{gameId:guid}/move", MakeMove.HandleAsync);

app.Run();

// Make Program accessible for testing
public partial class Program { }
