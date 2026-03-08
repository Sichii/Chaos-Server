#region
using System.Runtime.Serialization;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
#endregion

namespace Chaos.Storage.Tests;

public class EntityRepositoryTests : IDisposable
{
    private readonly EntityRepository EntityRepository;
    private readonly IOptions<JsonSerializerOptions> JsonOptions;
    private readonly Mock<ILogger<EntityRepository>> LoggerMock;
    private readonly Mock<ITypeMapper> MapperMock;
    private readonly IOptionsSnapshot<EntityRepositoryOptions> RepositoryOptions;
    private readonly string TestDirectory;
    private readonly string TestFilePath;

    public EntityRepositoryTests()
    {
        MapperMock = new Mock<ITypeMapper>();
        LoggerMock = new Mock<ILogger<EntityRepository>>();

        JsonOptions = Options.Create(
            new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            });

        var optionsMock = new Mock<IOptionsSnapshot<EntityRepositoryOptions>>();

        optionsMock.Setup(x => x.Value)
                   .Returns(
                       new EntityRepositoryOptions
                       {
                           SafeSaves = true
                       });
        RepositoryOptions = optionsMock.Object;

        TestDirectory = Path.Combine(Path.GetTempPath(), $"EntityRepositoryTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(TestDirectory);
        TestFilePath = Path.Combine(TestDirectory, "test.json");

        EntityRepository = new EntityRepository(
            MapperMock.Object,
            JsonOptions,
            RepositoryOptions,
            LoggerMock.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(TestDirectory))
            Directory.Delete(TestDirectory, true);
    }

    [Test]
    public void Load_Should_Deserialize_Schema_From_File()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 1,
            Name = "Test"
        };
        File.WriteAllText(TestFilePath, JsonSerializer.Serialize(testSchema));

        // Act
        var result = EntityRepository.Load<TestSchema>(TestFilePath);

        // Assert
        result.Should()
              .NotBeNull();

        result.Id
              .Should()
              .Be(1);

        result.Name
              .Should()
              .Be("Test");
    }

    [Test]
    public void Load_Should_Throw_SerializationException_When_Json_Is_Null()
    {
        // Deserializing the JSON literal "null" for a reference type returns null,
        // which triggers the null-check branch in Load<TSchema>
        File.WriteAllText(TestFilePath, "null");

        var act = () => EntityRepository.Load<TestSchema>(TestFilePath);

        act.Should()
           .Throw<SerializationException>();
    }

    [Test]
    public void Load_Should_Throw_When_Deserialization_Fails()
    {
        // Arrange
        File.WriteAllText(TestFilePath, "invalid json");

        // Act & Assert
        var act = () => EntityRepository.Load<TestSchema>(TestFilePath);

        act.Should()
           .Throw<Exception>(); // Could be JsonException, AggregateException, or RetryableException
    }

    [Test]
    public void Load_Should_Throw_When_Path_Is_Empty()
    {
        // Act & Assert
        var act = () => EntityRepository.Load<TestSchema>(string.Empty);

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void LoadAndMap_Should_Deserialize_And_Map_Object()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 3,
            Name = "MapTest"
        };

        var testEntity = new TestEntity
        {
            Id = 3,
            Name = "MapTest"
        };
        File.WriteAllText(TestFilePath, JsonSerializer.Serialize(testSchema));

        MapperMock.Setup(x => x.Map<TestEntity>(It.IsAny<TestSchema>()))
                  .Returns(testEntity);

        // Act
        var result = EntityRepository.LoadAndMap<TestEntity, TestSchema>(TestFilePath);

        // Assert
        result.Should()
              .NotBeNull();

        result.Id
              .Should()
              .Be(3);

        result.Name
              .Should()
              .Be("MapTest");
        MapperMock.Verify(x => x.Map<TestEntity>(It.IsAny<TestSchema>()), Times.Once);
    }

    [Test]
    public void LoadAndMap_Should_Execute_PreMapAction()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 4,
            Name = "PreMapTest"
        };

        var testEntity = new TestEntity
        {
            Id = 4,
            Name = "Modified"
        };
        File.WriteAllText(TestFilePath, JsonSerializer.Serialize(testSchema));

        var preMapExecuted = false;

        MapperMock.Setup(x => x.Map<TestEntity>(It.IsAny<TestSchema>()))
                  .Returns(testEntity);

        // Act
        var result = EntityRepository.LoadAndMap<TestEntity, TestSchema>(
            TestFilePath,
            schema =>
            {
                preMapExecuted = true;
                schema.Name = "Modified";
            });

        // Assert
        preMapExecuted.Should()
                      .BeTrue();

        result.Should()
              .NotBeNull();
    }

    [Test]
    public void LoadAndMap_Should_Throw_SerializationException_When_Json_Is_Null()
    {
        // Deserializing "null" triggers the null schema branch in LoadAndMap<T, TSchema>
        File.WriteAllText(TestFilePath, "null");

        var act = () => EntityRepository.LoadAndMap<TestEntity, TestSchema>(TestFilePath);

        act.Should()
           .Throw<SerializationException>();
    }

    [Test]
    public async Task LoadAndMapAsync_Should_Deserialize_And_Map_Object()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 5,
            Name = "AsyncMapTest"
        };

        var testEntity = new TestEntity
        {
            Id = 5,
            Name = "AsyncMapTest"
        };
        await File.WriteAllTextAsync(TestFilePath, JsonSerializer.Serialize(testSchema));

        MapperMock.Setup(x => x.Map<TestEntity>(It.IsAny<TestSchema>()))
                  .Returns(testEntity);

        // Act
        var result = await EntityRepository.LoadAndMapAsync<TestEntity, TestSchema>(TestFilePath);

        // Assert
        result.Should()
              .NotBeNull();

        result.Id
              .Should()
              .Be(5);

        result.Name
              .Should()
              .Be("AsyncMapTest");
        MapperMock.Verify(x => x.Map<TestEntity>(It.IsAny<TestSchema>()), Times.Once);
    }

    [Test]
    public void LoadAndMapMany_Should_Deserialize_And_Map_Collection()
    {
        // Arrange
        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 1,
                Name = "MapTest1"
            },
            new()
            {
                Id = 2,
                Name = "MapTest2"
            }
        };

        var testEntities = new List<TestEntity>
        {
            new()
            {
                Id = 1,
                Name = "MapTest1"
            },
            new()
            {
                Id = 2,
                Name = "MapTest2"
            }
        };
        File.WriteAllText(TestFilePath, JsonSerializer.Serialize(testSchemas));

        MapperMock.Setup(x => x.MapMany<TestSchema, TestEntity>(It.IsAny<ICollection<TestSchema>>()))
                  .Returns(testEntities);

        // Act
        var result = EntityRepository.LoadAndMapMany<TestEntity, TestSchema>(TestFilePath)
                                     .ToList();

        // Assert
        result.Should()
              .HaveCount(2);

        result[0]
            .Id
            .Should()
            .Be(1);

        result[1]
            .Id
            .Should()
            .Be(2);
        MapperMock.Verify(x => x.MapMany<TestSchema, TestEntity>(It.IsAny<ICollection<TestSchema>>()), Times.Once);
    }

    [Test]
    public async Task LoadAndMapManyAsync_With_PreMapAction_Should_Execute_Action()
    {
        // Arrange
        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 40,
                Name = "PreMap1"
            },
            new()
            {
                Id = 41,
                Name = "PreMap2"
            }
        };
        await File.WriteAllTextAsync(TestFilePath, JsonSerializer.Serialize(testSchemas));

        var preMapCount = 0;

        MapperMock.Setup(x => x.Map<TestEntity>(It.IsAny<TestSchema>()))
                  .Returns<TestSchema>(s => new TestEntity
                  {
                      Id = s.Id,
                      Name = s.Name
                  });

        // Act
        var result = new List<TestEntity>();

        await foreach (var item in EntityRepository.LoadAndMapManyAsync<TestEntity, TestSchema>(
                           TestFilePath,
                           async schema =>
                           {
                               preMapCount++;
                               schema.Name = $"Modified{preMapCount}";
                               await Task.CompletedTask;
                           }))
            result.Add(item);

        // Assert
        result.Should()
              .HaveCount(2);

        preMapCount.Should()
                   .Be(2);
        MapperMock.Verify(x => x.Map<TestEntity>(It.IsAny<TestSchema>()), Times.Exactly(2));
    }

    [Test]
    public async Task LoadAsync_Should_Deserialize_Schema_From_File()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 2,
            Name = "AsyncTest"
        };
        await File.WriteAllTextAsync(TestFilePath, JsonSerializer.Serialize(testSchema));

        // Act
        var result = await EntityRepository.LoadAsync<TestSchema>(TestFilePath);

        // Assert
        result.Should()
              .NotBeNull();

        result.Id
              .Should()
              .Be(2);

        result.Name
              .Should()
              .Be("AsyncTest");
    }

    [Test]
    public void LoadMany_Should_Deserialize_Collection()
    {
        // Arrange
        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 1,
                Name = "Test1"
            },
            new()
            {
                Id = 2,
                Name = "Test2"
            },
            new()
            {
                Id = 3,
                Name = "Test3"
            }
        };
        File.WriteAllText(TestFilePath, JsonSerializer.Serialize(testSchemas));

        // Act
        var result = EntityRepository.LoadMany<TestSchema>(TestFilePath)
                                     .ToList();

        // Assert
        result.Should()
              .HaveCount(3);

        result[0]
            .Id
            .Should()
            .Be(1);

        result[1]
            .Id
            .Should()
            .Be(2);

        result[2]
            .Id
            .Should()
            .Be(3);
    }

    [Test]
    public void LoadMany_Should_Throw_SerializationException_When_Json_Is_Null()
    {
        // Deserializing "null" for a collection triggers the null schemas branch in LoadMany
        File.WriteAllText(TestFilePath, "null");

        var act = () => EntityRepository.LoadMany<TestSchema>(TestFilePath)
                                        .ToList();

        act.Should()
           .Throw<SerializationException>();
    }

    [Test]
    public async Task LoadManyAsync_Should_Deserialize_Collection()
    {
        // Arrange
        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 1,
                Name = "AsyncTest1"
            },
            new()
            {
                Id = 2,
                Name = "AsyncTest2"
            }
        };
        await File.WriteAllTextAsync(TestFilePath, JsonSerializer.Serialize(testSchemas));

        // Act
        var result = new List<TestSchema>();

        await foreach (var item in EntityRepository.LoadManyAsync<TestSchema>(TestFilePath))
            result.Add(item);

        // Assert
        result.Should()
              .HaveCount(2);

        result[0]
            .Name
            .Should()
            .Be("AsyncTest1");

        result[1]
            .Name
            .Should()
            .Be("AsyncTest2");
    }

    [Test]
    public void Save_Should_Serialize_Schema_To_File()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 10,
            Name = "SaveTest"
        };

        // Act
        EntityRepository.Save(testSchema, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        var content = File.ReadAllText(TestFilePath);

        content.Should()
               .Contain("\"Id\": 10");

        content.Should()
               .Contain("\"Name\": \"SaveTest\"");
    }

    [Test]
    public void Save_Should_Throw_When_Object_Is_Null()
    {
        // Act & Assert
        var act = () => EntityRepository.Save<TestSchema>(null!, TestFilePath);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Save_Should_Throw_When_Path_Is_Empty()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 1,
            Name = "Test"
        };

        // Act & Assert
        var act = () => EntityRepository.Save(testSchema, string.Empty);

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void SaveAndMap_Should_Map_And_Serialize_Object()
    {
        // Arrange
        var testEntity = new TestEntity
        {
            Id = 12,
            Name = "MapSaveTest"
        };

        var testSchema = new TestSchema
        {
            Id = 12,
            Name = "MapSaveTest"
        };

        MapperMock.Setup(x => x.Map<TestSchema>(testEntity))
                  .Returns(testSchema);

        // Act
        EntityRepository.SaveAndMap<TestEntity, TestSchema>(testEntity, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        MapperMock.Verify(x => x.Map<TestSchema>(testEntity), Times.Once);
    }

    [Test]
    public async Task SaveAndMapAsync_Should_Map_And_Serialize_Object()
    {
        // Arrange
        var testEntity = new TestEntity
        {
            Id = 13,
            Name = "AsyncMapSaveTest"
        };

        var testSchema = new TestSchema
        {
            Id = 13,
            Name = "AsyncMapSaveTest"
        };

        MapperMock.Setup(x => x.Map<TestSchema>(testEntity))
                  .Returns(testSchema);

        // Act
        await EntityRepository.SaveAndMapAsync<TestEntity, TestSchema>(testEntity, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        MapperMock.Verify(x => x.Map<TestSchema>(testEntity), Times.Once);
    }

    [Test]
    public void SaveAndMapMany_Should_Map_And_Serialize_Collection()
    {
        // Arrange
        var testEntities = new List<TestEntity>
        {
            new()
            {
                Id = 30,
                Name = "MapMany1"
            },
            new()
            {
                Id = 31,
                Name = "MapMany2"
            }
        };

        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 30,
                Name = "MapMany1"
            },
            new()
            {
                Id = 31,
                Name = "MapMany2"
            }
        };

        MapperMock.Setup(x => x.MapMany<TestEntity, TestSchema>(testEntities))
                  .Returns(testSchemas);

        // Act
        EntityRepository.SaveAndMapMany<TestEntity, TestSchema>(testEntities, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        MapperMock.Verify(x => x.MapMany<TestEntity, TestSchema>(testEntities), Times.Once);
    }

    [Test]
    public async Task SaveAndMapManyAsync_Should_Map_And_Serialize_Collection()
    {
        // Arrange
        var testEntities = new List<TestEntity>
        {
            new()
            {
                Id = 32,
                Name = "AsyncMapMany1"
            },
            new()
            {
                Id = 33,
                Name = "AsyncMapMany2"
            }
        };

        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 32,
                Name = "AsyncMapMany1"
            },
            new()
            {
                Id = 33,
                Name = "AsyncMapMany2"
            }
        };

        MapperMock.Setup(x => x.MapMany<TestEntity, TestSchema>(testEntities))
                  .Returns(testSchemas);

        // Act
        await EntityRepository.SaveAndMapManyAsync<TestEntity, TestSchema>(testEntities, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        MapperMock.Verify(x => x.MapMany<TestEntity, TestSchema>(testEntities), Times.Once);
    }

    [Test]
    public async Task SaveAsync_Should_Serialize_Schema_To_File()
    {
        // Arrange
        var testSchema = new TestSchema
        {
            Id = 11,
            Name = "AsyncSaveTest"
        };

        // Act
        await EntityRepository.SaveAsync(testSchema, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        var content = await File.ReadAllTextAsync(TestFilePath);

        content.Should()
               .Contain("\"Id\": 11");

        content.Should()
               .Contain("\"Name\": \"AsyncSaveTest\"");
    }

    [Test]
    public void SaveMany_Should_Serialize_Collection()
    {
        // Arrange
        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 20,
                Name = "SaveMany1"
            },
            new()
            {
                Id = 21,
                Name = "SaveMany2"
            }
        };

        // Act
        EntityRepository.SaveMany(testSchemas, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        var content = File.ReadAllText(TestFilePath);

        content.Should()
               .Contain("\"Id\": 20");

        content.Should()
               .Contain("\"Id\": 21");
    }

    [Test]
    public async Task SaveManyAsync_Should_Serialize_Collection()
    {
        // Arrange
        var testSchemas = new List<TestSchema>
        {
            new()
            {
                Id = 22,
                Name = "AsyncSaveMany1"
            },
            new()
            {
                Id = 23,
                Name = "AsyncSaveMany2"
            }
        };

        // Act
        await EntityRepository.SaveManyAsync(testSchemas, TestFilePath);

        // Assert
        File.Exists(TestFilePath)
            .Should()
            .BeTrue();
        var content = await File.ReadAllTextAsync(TestFilePath);

        content.Should()
               .Contain("\"Id\": 22");

        content.Should()
               .Contain("\"Id\": 23");
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // Test classes
    private class TestSchema
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}