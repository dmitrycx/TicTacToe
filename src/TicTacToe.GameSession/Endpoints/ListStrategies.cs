using FastEndpoints;
using TicTacToe.Shared.Enums;
using TicTacToe.GameSession.Domain.Services;

namespace TicTacToe.GameSession.Endpoints;

// 1. Define the DTO for the response - only business data, no UI concerns
public record StrategyInfo(
    string Name, 
    string DisplayName, 
    string Description
);

public record ListStrategiesResponse(List<StrategyInfo> Strategies);

// 2. Create the endpoint
public class ListStrategiesEndpoint(IMoveGeneratorFactory factory) 
    : EndpointWithoutRequest<ListStrategiesResponse>
{
    public override void Configure()
    {
        Get("/sessions/strategies");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Retrieves the list of available game simulation strategies.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var supportedTypes = factory.GetSupportedMoveTypes();

        var strategyInfos = supportedTypes.Select(type => GetStrategyInfo(type)).ToList();

        await SendAsync(new ListStrategiesResponse(strategyInfos), cancellation: ct);
    }

    // 3. Helper method that provides business metadata only
    private static StrategyInfo GetStrategyInfo(GameStrategy strategy) => strategy switch
    {
        GameStrategy.Random => new StrategyInfo(
            Name: "Random",
            DisplayName: "Random",
            Description: "Makes random valid moves."
        ),
        GameStrategy.RuleBased => new StrategyInfo(
            Name: "RuleBased",
            DisplayName: "Rule-Based",
            Description: "Uses predefined rules and heuristics."
        ),
        GameStrategy.AI => new StrategyInfo(
            Name: "AI",
            DisplayName: "AI",
            Description: "Uses artificial intelligence algorithms."
        ),
        GameStrategy.MinMax => new StrategyInfo(
            Name: "MinMax",
            DisplayName: "MinMax",
            Description: "Uses minimax algorithm for optimal play."
        ),
        GameStrategy.AlphaBeta => new StrategyInfo(
            Name: "AlphaBeta",
            DisplayName: "Alpha-Beta",
            Description: "Enhanced minimax with alpha-beta pruning."
        ),
        _ => new StrategyInfo(
            Name: strategy.ToString(),
            DisplayName: strategy.ToString(),
            Description: "An available game strategy."
        )
    };
} 