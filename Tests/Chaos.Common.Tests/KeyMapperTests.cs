using System.Numerics;
using Chaos.Common.Abstractions;
using Chaos.Common.Utilities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class KeyMapperTests
{
    [Fact]
    public void GetId_ShouldInvokeIdGeneratorOnlyOncePerUniqueKey()
    {
        // Arrange
        var idGeneratorMock = new Mock<IIdGenerator<BigInteger>>();
        var sequence = 0;

        idGeneratorMock.Setup(g => g.NextId)
                       .Returns(() => new BigInteger(sequence++));

        var mapper = new KeyMapper<BigInteger>(idGeneratorMock.Object);

        // Act
        var _ = mapper.GetId("key1");
        var __ = mapper.GetId("key1");

        // Assert
        idGeneratorMock.Verify(g => g.NextId, Times.Once);
    }

    [Fact]
    public void GetId_ShouldReturnSameIdForSameKey()
    {
        // Arrange
        var idGeneratorMock = new Mock<IIdGenerator<BigInteger>>();
        var sequence = 0;

        idGeneratorMock.Setup(g => g.NextId)
                       .Returns(() => new BigInteger(sequence++));

        var mapper = new KeyMapper<BigInteger>(idGeneratorMock.Object);

        // Act
        var id1 = mapper.GetId("key1");
        var id2 = mapper.GetId("key1"); // Retrieve the ID for the same key

        // Assert
        id1.Should()
           .Be(id2);
    }

    [Fact]
    public void GetId_ShouldReturnUniqueIdsForDifferentKeys()
    {
        // Arrange
        var idGeneratorMock = new Mock<IIdGenerator<BigInteger>>();
        var sequence = 0;

        idGeneratorMock.Setup(g => g.NextId)
                       .Returns(() => new BigInteger(sequence++));

        var mapper = new KeyMapper<BigInteger>(idGeneratorMock.Object);

        // Act
        var id1 = mapper.GetId("key1");
        var id2 = mapper.GetId("key2");

        // Assert
        id1.Should()
           .NotBe(id2);
    }
}