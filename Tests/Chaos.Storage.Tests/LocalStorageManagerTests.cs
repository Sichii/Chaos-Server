#region
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Storage.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Storage.Tests;

public sealed class LocalStorageManagerTests : IDisposable
{
    private readonly IOptions<JsonSerializerOptions> JsonOptions;
    private readonly IOptions<LocalStorageOptions> LocalOptions;
    private readonly LocalStorageManager Manager;
    private readonly IMemoryCache MemoryCache;
    private readonly string TempDir;

    public LocalStorageManagerTests()
    {
        TempDir = Path.Combine(
            Path.GetTempPath(),
            "LocalStorageManagerTests_"
            + Guid.NewGuid()
                  .ToString("N"));
        Directory.CreateDirectory(TempDir);

        MemoryCache = new MemoryCache(new MemoryCacheOptions());

        JsonOptions = Options.Create(
            new JsonSerializerOptions
            {
                WriteIndented = false
            });

        LocalOptions = Options.Create(
            new LocalStorageOptions
            {
                Directory = TempDir
            });

        Manager = new LocalStorageManager(MemoryCache, JsonOptions, LocalOptions);
    }

    public void Dispose()
    {
        MemoryCache.Dispose();

        if (Directory.Exists(TempDir))
            Directory.Delete(TempDir, true);
    }

    [Test]
    public void Constructor_Should_Create_Directory_When_Missing()
    {
        var dir = Path.Combine(
            Path.GetTempPath(),
            "LocalStorageCtor_"
            + Guid.NewGuid()
                  .ToString("N"));

        Directory.Exists(dir)
                 .Should()
                 .BeFalse();

        using var mem = new MemoryCache(new MemoryCacheOptions());
        var jso = Options.Create(new JsonSerializerOptions());

        var opts = Options.Create(
            new LocalStorageOptions
            {
                Directory = dir
            });

        _ = new LocalStorageManager(mem, jso, opts);

        Directory.Exists(dir)
                 .Should()
                 .BeTrue();
        Directory.Delete(dir, true);
    }

    [Test]
    public void GetOrAddEntry_Should_Create_In_Cache_When_Missing()
    {
        // Act
        var method = typeof(LocalStorageManager).GetMethod("GetOrAddEntry", BindingFlags.Instance | BindingFlags.NonPublic)!
                                                .MakeGenericMethod(typeof(Sample));
        var dict = (ConcurrentDictionary<string, Sample>)method.Invoke(Manager, [])!;

        // Assert
        dict.Should()
            .NotBeNull();

        dict.Should()
            .BeOfType<ConcurrentDictionary<string, Sample>>();
    }

    [Test]
    public async Task GetOrAddEntryAsync_Should_Create_In_Cache_When_Missing()
    {
        // Act
        var method = typeof(LocalStorageManager).GetMethod("GetOrAddEntryAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
                                                .MakeGenericMethod(typeof(Sample));
        var dict = await (Task<ConcurrentDictionary<string, Sample>>)method.Invoke(Manager, [])!;

        // Assert
        dict.Should()
            .NotBeNull();

        dict.Should()
            .BeOfType<ConcurrentDictionary<string, Sample>>();
    }

    [Test]
    public void Load_Should_Create_StorageObject_And_Read_Existing_File()
    {
        // Arrange
        var filePath = Path.Combine(TempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["default"] = new()
            {
                Id = 1,
                Name = "one"
            }
        };
        JsonSerializerEx.Serialize(filePath, initial, JsonOptions.Value);

        // Act
        var storage = Manager.Load<Sample>();

        // Assert
        storage.Should()
               .NotBeNull();

        storage.Value
               .Id
               .Should()
               .Be(1);

        storage.Value
               .Name
               .Should()
               .Be("one");
    }

    [Test]
    public async Task LoadAsync_Should_Create_StorageObject_And_Read_Existing_File()
    {
        // Arrange
        var filePath = Path.Combine(TempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["default"] = new()
            {
                Id = 2,
                Name = "two"
            }
        };
        await JsonSerializerEx.SerializeAsync(filePath, initial, JsonOptions.Value);

        // Act
        var storage = await Manager.LoadAsync<Sample>();

        // Assert
        storage.Should()
               .NotBeNull();

        storage.Value
               .Id
               .Should()
               .Be(2);

        storage.Value
               .Name
               .Should()
               .Be("two");
    }

    [Test]
    public void LoadOrCreateEntry_Should_Load_From_File_When_Exists_Otherwise_Empty()
    {
        // Arrange
        var filePath = Path.Combine(TempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["name"] = new()
            {
                Id = 5
            }
        };
        JsonSerializerEx.Serialize(filePath, initial, JsonOptions.Value);

        // Act
        var method = typeof(LocalStorageManager).GetMethod("LoadOrCreateEntry", BindingFlags.Instance | BindingFlags.NonPublic)!
                                                .MakeGenericMethod(typeof(Sample));
        var dict = (ConcurrentDictionary<string, Sample>)method.Invoke(Manager, [])!;

        // Assert
        dict.Should()
            .ContainKey("name");
    }

    [Test]
    public async Task LoadOrCreateEntryAsync_Should_Load_From_File_When_Exists_Otherwise_Empty()
    {
        // Arrange
        var filePath = Path.Combine(TempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["name2"] = new()
            {
                Id = 6
            }
        };
        await JsonSerializerEx.SerializeAsync(filePath, initial, JsonOptions.Value);

        // Act
        var method = typeof(LocalStorageManager).GetMethod("LoadOrCreateEntryAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
                                                .MakeGenericMethod(typeof(Sample));
        var dict = await (Task<ConcurrentDictionary<string, Sample>>)method.Invoke(Manager, [])!;

        // Assert
        dict.Should()
            .ContainKey("name2");
    }

    [Test]
    public void Save_Should_Persist_To_File()
    {
        // Arrange
        var storage = Manager.Load<Sample>();
        storage.GetInstance("default");

        storage.Value
               .Id
               .Should()
               .Be(0); // default
        storage.GetInstance("default");
        storage.Value.Id = 3;
        storage.Value.Name = "three";

        // Act
        Manager.Save(storage);

        // Assert
        var path = Path.Combine(TempDir, "Sample.json");

        File.Exists(path)
            .Should()
            .BeTrue();
        var data = JsonSerializerEx.Deserialize<Dictionary<string, Sample>>(path, JsonOptions.Value)!;

        data["default"]
            .Id
            .Should()
            .Be(3);

        data["default"]
            .Name
            .Should()
            .Be("three");
    }

    [Test]
    public async Task SaveAsync_Should_Persist_To_File()
    {
        // Arrange
        var storage = await Manager.LoadAsync<Sample>();
        storage.Value.Id = 4;
        storage.Value.Name = "four";

        // Act
        await Manager.SaveAsync(storage);

        // Assert
        var path = Path.Combine(TempDir, "Sample.json");

        File.Exists(path)
            .Should()
            .BeTrue();
        var data = await JsonSerializerEx.DeserializeAsync<Dictionary<string, Sample>>(path, JsonOptions.Value);

        data!["default"]
            .Id
            .Should()
            .Be(4);

        data!["default"]
            .Name
            .Should()
            .Be("four");
    }

    private sealed class Sample
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}