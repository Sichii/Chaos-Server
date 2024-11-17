#region
using Chaos.Common.Identity;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class EphemeralRandomIdGeneratorTests
{
    [Test]
    public void NextId_ShouldGenerateIdsOfCorrectType()
    {
        // Arrange
        var idGenerator = EphemeralRandomIdGenerator<int>.Shared;

        // Act
        var id = idGenerator.NextId;

        // Assert
        id.Should()
          .BeOfType(typeof(int));
    }

    [Test]
    public void NextId_ShouldGenerateNonSequentialIds()
    {
        // Arrange
        var idGenerator = EphemeralRandomIdGenerator<int>.Shared;

        // Act
        var ids = Enumerable.Range(1, 1000)
                            .Select(_ => idGenerator.NextId)
                            .ToList();

        // Assert
        ids.Should()
           .NotBeInAscendingOrder();
    }

    [Test]
    public void NextId_ShouldGenerateUniqueIds()
    {
        // Arrange
        var idGenerator = EphemeralRandomIdGenerator<int>.Shared;

        // Act
        var ids = Enumerable.Range(1, byte.MaxValue)
                            .Select(_ => idGenerator.NextId)
                            .ToList();

        // Assert
        ids.Should()
           .OnlyHaveUniqueItems();
    }
}