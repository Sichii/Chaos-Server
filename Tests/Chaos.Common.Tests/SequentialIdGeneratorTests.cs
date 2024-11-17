#region
using Chaos.Common.Abstractions;
using Chaos.Common.Identity;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class SequentialIdGeneratorTests
{
    [Test]
    public void NextId_ShouldGenerateSequentialIds()
    {
        // Arrange
        var idGenerator = SequentialIdGenerator<int>.Shared;

        // Act
        var id1 = idGenerator.NextId;
        var id2 = idGenerator.NextId;

        // Assert
        id2.Should()
           .BeGreaterThan(id1);
    }

    [Test]
    public void NextId_ShouldStartFromSpecifiedId()
    {
        // Arrange
        const int START_ID = 100;
        IIdGenerator<int> idGenerator = new SequentialIdGenerator<int>(START_ID);

        // Act
        var id1 = idGenerator.NextId;
        var id2 = idGenerator.NextId;

        // Assert
        id1.Should()
           .Be(START_ID + 1);

        id2.Should()
           .BeGreaterThan(id1);
    }

    [Test]
    public void NextId_ShouldUseDefaultStartIdWhenNotSpecified()
    {
        // Arrange
        IIdGenerator<int> idGenerator = new SequentialIdGenerator<int>();

        // Act
        var id1 = idGenerator.NextId;
        var id2 = idGenerator.NextId;

        // Assert
        id1.Should()
           .Be(1);

        id2.Should()
           .BeGreaterThan(id1);
    }
}