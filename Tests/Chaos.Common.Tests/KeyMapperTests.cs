#region
using System.Collections.Concurrent;
using Chaos.Common.Identity;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class KeyMapperTests
{
    [Test]
    public void GetId_ConcurrentAccess_ShouldBeThreadSafe()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const int THREAD_COUNT = 10;
        const int OPERATIONS_PER_THREAD = 100;

        var tasks = new List<Task>();
        var results = new ConcurrentBag<(string Key, int Id)>();

        // Act
        for (var i = 0; i < THREAD_COUNT; i++)
        {
            var threadIndex = i;

            tasks.Add(
                Task.Run(() =>
                {
                    for (var j = 0; j < OPERATIONS_PER_THREAD; j++)
                    {
                        var key = $"thread-{threadIndex}-key-{j}";
                        var id = keyMapper.GetId(key);
                        results.Add((key, id));
                    }
                }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        results.Should()
               .HaveCount(THREAD_COUNT * OPERATIONS_PER_THREAD);

        var ids = results.Select(r => r.Id)
                         .ToList();

        ids.Should()
           .OnlyHaveUniqueItems();

        // Verify mappings work by checking reverse lookups
        foreach ((var key, var expectedId) in results)
            keyMapper.GetKey(expectedId)
                     .Should()
                     .Be(key);
    }

    [Test]
    public void GetId_SequentialKeys_ShouldGenerateSequentialIds()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);

        // Act
        var id1 = keyMapper.GetId("key1");
        var id2 = keyMapper.GetId("key2");
        var id3 = keyMapper.GetId("key3");

        // Assert
        id1.Should()
           .Be(1);

        id2.Should()
           .Be(2);

        id3.Should()
           .Be(3);
    }

    [Test]
    public void GetId_WithEmptyKey_ShouldHandleEmptyKey()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const string EMPTY_KEY = "";

        // Act
        var result = keyMapper.GetId(EMPTY_KEY);

        // Assert
        result.Should()
              .Be(1);
    }

    [Test]
    public void GetId_WithExistingKey_ShouldReturnExistingId()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const string KEY = "existing-key";

        // Act
        var firstResult = keyMapper.GetId(KEY);
        var secondResult = keyMapper.GetId(KEY);

        // Assert
        firstResult.Should()
                   .Be(1);

        secondResult.Should()
                    .Be(1);
    }

    [Test]
    public void GetId_WithMultipleDifferentKeys_ShouldGenerateUniqueIds()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const string KEY1 = "key1";
        const string KEY2 = "key2";

        // Act
        var result1 = keyMapper.GetId(KEY1);
        var result2 = keyMapper.GetId(KEY2);

        // Assert
        result1.Should()
               .Be(1);

        result2.Should()
               .Be(2);

        result1.Should()
               .NotBe(result2);
    }

    [Test]
    public void GetId_WithNewKey_ShouldGenerateNewId()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const string KEY = "test-key";

        // Act
        var result = keyMapper.GetId(KEY);

        // Assert
        result.Should()
              .Be(1);
    }

    [Test]
    public void GetId_WithNullKey_ShouldThrowArgumentNullException()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);

        // Act & Assert
        var act = () => keyMapper.GetId(null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void GetId_WithSameKeyMultipleTimes_ShouldReturnSameId()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const string KEY = "repeated-key";

        // Act
        var id1 = keyMapper.GetId(KEY);
        var id2 = keyMapper.GetId(KEY);
        var id3 = keyMapper.GetId(KEY);

        // Assert
        id1.Should()
           .Be(1);

        id2.Should()
           .Be(1);

        id3.Should()
           .Be(1);
    }

    [Test]
    public void GetKey_RoundTrip_ShouldWorkCorrectly()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);

        var keys = new[]
        {
            "alpha",
            "beta",
            "gamma",
            "delta"
        };

        // Act & Assert
        foreach (var key in keys)
        {
            var id = keyMapper.GetId(key);
            var retrievedKey = keyMapper.GetKey(id);

            retrievedKey.Should()
                        .Be(key);
        }
    }

    [Test]
    public void GetKey_WithExistingId_ShouldReturnCorrectKey()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const string EXPECTED_KEY = "test-key";
        var id = keyMapper.GetId(EXPECTED_KEY);

        // Act
        var result = keyMapper.GetKey(id);

        // Assert
        result.Should()
              .Be(EXPECTED_KEY);
    }

    [Test]
    public void GetKey_WithMultipleMappings_ShouldReturnCorrectKey()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const string KEY1 = "first-key";
        const string KEY2 = "second-key";

        var id1 = keyMapper.GetId(KEY1);
        var id2 = keyMapper.GetId(KEY2);

        // Act
        var result1 = keyMapper.GetKey(id1);
        var result2 = keyMapper.GetKey(id2);

        // Assert
        result1.Should()
               .Be(KEY1);

        result2.Should()
               .Be(KEY2);
    }

    [Test]
    public void GetKey_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);
        const int NON_EXISTENT_ID = 999;

        // Act
        var result = keyMapper.GetKey(NON_EXISTENT_ID);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void GetKey_WithZeroId_ShouldReturnNullIfNotMapped()
    {
        // Arrange
        var idGenerator = new SequentialIdGenerator<int>();
        var keyMapper = new MockKeyMapper<int>(idGenerator);

        // Act
        var result = keyMapper.GetKey(0);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void KeyMapper_WithLongType_ShouldWork()
    {
        // Arrange
        var longIdGenerator = new SequentialIdGenerator<long>();
        var longKeyMapper = new MockKeyMapper<long>(longIdGenerator);

        // Act
        var result = longKeyMapper.GetId("long-key");

        // Assert
        result.Should()
              .Be(1L);
    }

    [Test]
    public void KeyMapper_WithUshortType_ShouldWork()
    {
        // Arrange
        var ushortIdGenerator = new SequentialIdGenerator<ushort>();
        var ushortKeyMapper = new MockKeyMapper<ushort>(ushortIdGenerator);

        // Act
        var result = ushortKeyMapper.GetId("ushort-key");

        // Assert
        result.Should()
              .Be(1);
    }
}