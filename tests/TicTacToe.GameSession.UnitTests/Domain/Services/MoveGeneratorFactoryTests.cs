using Moq;
using TicTacToe.GameSession.Domain.Services;

namespace TicTacToe.GameSession.UnitTests.Domain.Services;

public class MoveGeneratorFactoryTests
{
    private readonly Mock<IMoveGenerator> _mockRandomGenerator;
    private readonly Mock<IMoveGenerator> _mockRuleBasedGenerator;
    private readonly MoveGeneratorFactory _factory;

    public MoveGeneratorFactoryTests()
    {
        _mockRandomGenerator = new Mock<IMoveGenerator>();
        _mockRandomGenerator.Setup(x => x.Type).Returns(MoveType.Random);

        _mockRuleBasedGenerator = new Mock<IMoveGenerator>();
        _mockRuleBasedGenerator.Setup(x => x.Type).Returns(MoveType.RuleBased);

        var generators = new List<IMoveGenerator>
        {
            _mockRandomGenerator.Object,
            _mockRuleBasedGenerator.Object
        };

        _factory = new MoveGeneratorFactory(generators);
    }

    [Fact]
    public void CreateGenerator_RandomType_ShouldReturnRandomMoveGenerator()
    {
        // Act
        var generator = _factory.CreateGenerator(MoveType.Random);

        // Assert
        Assert.NotNull(generator);
        Assert.Same(_mockRandomGenerator.Object, generator);
        Assert.Equal(MoveType.Random, generator.Type);
    }

    [Fact]
    public void CreateGenerator_RuleBasedType_ShouldReturnRuleBasedMoveGenerator()
    {
        // Act
        var generator = _factory.CreateGenerator(MoveType.RuleBased);

        // Assert
        Assert.NotNull(generator);
        Assert.Same(_mockRuleBasedGenerator.Object, generator);
        Assert.Equal(MoveType.RuleBased, generator.Type);
    }

    [Fact]
    public void CreateGenerator_UnsupportedType_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            _factory.CreateGenerator(MoveType.AI));
        
        Assert.Contains("is not supported", exception.Message);
        Assert.Equal("moveType", exception.ParamName);
    }

    [Fact]
    public void GetSupportedMoveTypes_ShouldReturnExpectedTypes()
    {
        // Act
        var supportedTypes = _factory.GetSupportedMoveTypes().ToList();

        // Assert
        Assert.NotNull(supportedTypes);
        Assert.Equal(2, supportedTypes.Count);
        Assert.Contains(MoveType.Random, supportedTypes);
        Assert.Contains(MoveType.RuleBased, supportedTypes);
        Assert.DoesNotContain(MoveType.AI, supportedTypes);
    }

    [Fact]
    public void CreateGenerator_MultipleCalls_ShouldReturnSameInstances()
    {
        // Act
        var generator1 = _factory.CreateGenerator(MoveType.Random);
        var generator2 = _factory.CreateGenerator(MoveType.Random);

        // Assert
        Assert.NotNull(generator1);
        Assert.NotNull(generator2);
        Assert.Same(generator1, generator2); // Should be the same instance (singleton behavior)
        Assert.Same(_mockRandomGenerator.Object, generator1);
        Assert.Same(_mockRandomGenerator.Object, generator2);
    }
} 