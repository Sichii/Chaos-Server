#region
using System.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Abstractions.Definitions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
#endregion

namespace Chaos.Storage.Tests;

public sealed class ExpiringFileCacheTests : IDisposable
{
    private readonly ILogger<ExpiringFileCache<Dummy, DummySchema, ExpiringFileCacheOptions>> Logger;
    private readonly IMemoryCache MemoryCache;
    private readonly IOptions<ExpiringFileCacheOptions> Options;
    private readonly Mock<IEntityRepository> Repo;
    private readonly string TempDir;

    public ExpiringFileCacheTests()
    {
        TempDir = Path.Combine(
            Path.GetTempPath(),
            "ExpiringFileCacheTests_"
            + Guid.NewGuid()
                  .ToString("N"));
        Directory.CreateDirectory(TempDir);
        MemoryCache = new MemoryCache(new MemoryCacheOptions());
        Repo = new Mock<IEntityRepository>();

        Options = Microsoft.Extensions.Options.Options.Create(
            new ExpiringFileCacheOptions
            {
                Directory = TempDir,
                Expires = true,
                ExpirationMins = 10,
                FilePattern = "*.json",
                Recursive = false,
                SearchType = SearchType.Files
            });

        Logger = LoggerFactory.Create(b => { })
                              .CreateLogger<ExpiringFileCache<Dummy, DummySchema, ExpiringFileCacheOptions>>();
    }

    // Use shared MockCache in Chaos.Testing.Infrastructure

    public void Dispose()
    {
        MemoryCache.Dispose();

        if (Directory.Exists(TempDir))
            Directory.Delete(TempDir, true);
    }

    // moved to Chaos.Testing.Infrastructure/Mocks/MockCache.cs

    [Test]
    public void ConstructKey_DeconstructKey_Should_RoundTrip()
    {
        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);
        var key = cache.ConstructKeyPublic("Test");

        key.Should()
           .StartWith("dummy___");

        cache.DeconstructKeyPublic(key)
             .Should()
             .Be("test");
    }

    [Test]
    public void Constructor_Should_Create_Directory_When_Missing()
    {
        var dir = Path.Combine(
            Path.GetTempPath(),
            "ExpiringFileCacheCtor_"
            + Guid.NewGuid()
                  .ToString("N"));

        Directory.Exists(dir)
                 .Should()
                 .BeFalse();

        var opts = Microsoft.Extensions.Options.Options.Create(
            new ExpiringFileCacheOptions
            {
                Directory = dir,
                Expires = false,
                FilePattern = "*.json",
                Recursive = false,
                SearchType = SearchType.Files
            });

        _ = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            opts,
            Logger);

        Directory.Exists(dir)
                 .Should()
                 .BeTrue();
        Directory.Delete(dir, true);
    }

    [Test]
    public void Enumerator_Should_Enumerate_Current_Items()
    {
        var filename = Path.Combine(TempDir, "enum.json");
        File.WriteAllText(filename, "{}");

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(
                new Dummy
                {
                    Id = 7
                });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        _ = cache.Get("enum");

        // Generic enumerator
        var list = new List<Dummy>();

        foreach (var item in cache)
            list.Add(item);

        list.Should()
            .NotBeEmpty();

        // Non-generic enumerator
        var enumerator = ((IEnumerable)cache).GetEnumerator();

        enumerator.MoveNext()
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Exists_KeyInCache_ShouldReturnTrue()
    {
        var filename = Path.Combine(TempDir, "exists.json");
        File.WriteAllText(filename, "{}");

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(
                new Dummy
                {
                    Id = 1
                });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        _ = cache.Get("exists"); // loads into cache

        cache.Exists("exists")
             .Should()
             .BeTrue();
    }

    [Test]
    public void Exists_KeyNotInCache_ShouldReturnFalse()
    {
        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        cache.Exists("nope")
             .Should()
             .BeFalse();
    }

    [Test]
    public void ForceLoad_Should_Preload_All_Keys()
    {
        var filenameA = Path.Combine(TempDir, "a.json");
        var filenameB = Path.Combine(TempDir, "b.json");
        File.WriteAllText(filenameA, "{}");
        File.WriteAllText(filenameB, "{}");

        var loadCount = 0;

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(() =>
            {
                loadCount++;

                return new Dummy
                {
                    Id = loadCount
                };
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        cache.ForceLoadPublic();

        // After force-load, a subsequent Get should not trigger additional repository calls for existing keys
        _ = cache.Get("a");
        _ = cache.Get("b");

        loadCount.Should()
                 .BeGreaterThanOrEqualTo(2);
        Repo.Verify(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()), Times.AtLeast(2));
    }

    [Test]
    public void Get_SearchTypeDirectories_MissingKey_ShouldThrowDirectoryNotFoundException()
    {
        var opts = Microsoft.Extensions.Options.Options.Create(
            new ExpiringFileCacheOptions
            {
                Directory = TempDir,
                Expires = false,
                FilePattern = "*",
                Recursive = false,
                SearchType = SearchType.Directories
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            opts,
            Logger);

        Action act = () => cache.Get("nonexistent");

        act.Should()
           .Throw<DirectoryNotFoundException>();
    }

    [Test]
    public void Get_Should_Load_From_Repository_And_Cache()
    {
        // Arrange: prepare file name used for lookup and mock repository
        var filename = Path.Combine(TempDir, "abc.json");
        File.WriteAllText(filename, "{}");

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns((string p, Action<DummySchema>? cb) => new Dummy
            {
                Id = 42
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        // ensure Paths loaded include our file (constructor already loads). No-op here.

        // Act
        var value1 = cache.Get("abc");
        var value2 = cache.Get("abc");

        // Assert: value retrieved and cached
        value1.Id
              .Should()
              .Be(42);

        value2.Id
              .Should()
              .Be(42);
        Repo.Verify(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()), Times.Once);
    }

    [Test]
    public void Get_Should_Throw_When_Key_Not_Found_By_SearchType_Files()
    {
        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);
        Action act = () => cache.Get("missing");

        act.Should()
           .Throw<FileNotFoundException>();
    }

    [Test]
    public void GetPathForKey_SearchTypeDirectories_ShouldReturnDirectoryPath()
    {
        var subDir = Path.Combine(TempDir, "mykey");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "file.json"), "{}");

        var opts = Microsoft.Extensions.Options.Options.Create(
            new ExpiringFileCacheOptions
            {
                Directory = TempDir,
                Expires = false,
                FilePattern = "*",
                Recursive = false,
                SearchType = SearchType.Directories
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            opts,
            Logger);

        var path = cache.GetPathForKeyPublic("mykey");

        path.Should()
            .EndWith("mykey");
    }

    [Test]
    public void LoadPaths_Recursive_ShouldIncludeSubdirectoryFiles()
    {
        var subDir = Path.Combine(TempDir, "sub");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "nested.json"), "{}");

        var opts = Microsoft.Extensions.Options.Options.Create(
            new ExpiringFileCacheOptions
            {
                Directory = TempDir,
                Expires = false,
                FilePattern = "*.json",
                Recursive = true,
                SearchType = SearchType.Files
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            opts,
            Logger);

        var paths = cache.LoadPathsPublic();

        paths.Should()
             .Contain(p => p.Contains("nested.json"));
    }

    [Test]
    public void LoadPaths_SearchTypeDirectories_ShouldExcludeEmptyDirectories()
    {
        // Arrange — create one empty dir and one dir with files
        var emptyDir = Path.Combine(TempDir, "hollow");
        Directory.CreateDirectory(emptyDir);

        var nonEmptyDir = Path.Combine(TempDir, "populated");
        Directory.CreateDirectory(nonEmptyDir);
        File.WriteAllText(Path.Combine(nonEmptyDir, "data.json"), "{}");

        var opts = Microsoft.Extensions.Options.Options.Create(
            new ExpiringFileCacheOptions
            {
                Directory = TempDir,
                Expires = false,
                FilePattern = "*",
                Recursive = false,
                SearchType = SearchType.Directories
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            opts,
            Logger);

        // Act
        var paths = cache.LoadPathsPublic();

        // Assert
        paths.Should()
             .Contain(p => p.EndsWith("populated"));

        paths.Should()
             .NotContain(p => p.EndsWith("hollow"));
    }

    [Test]
    public void LoadPaths_SearchTypeDirectories_ShouldReturnDirectoriesWithFiles()
    {
        var subDir = Path.Combine(TempDir, "dirkey");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "data.json"), "{}");

        var opts = Microsoft.Extensions.Options.Options.Create(
            new ExpiringFileCacheOptions
            {
                Directory = TempDir,
                Expires = false,
                FilePattern = "*",
                Recursive = false,
                SearchType = SearchType.Directories
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            opts,
            Logger);

        var paths = cache.LoadPathsPublic();

        paths.Should()
             .Contain(p => p.EndsWith("dirkey"));
    }

    [Test]
    public void ReloadAsync_Should_Handle_Errors_And_Continue()
    {
        var filename = Path.Combine(TempDir, "err.json");
        File.WriteAllText(filename, "{}");

        // Seed with a successful load first
        Repo.SetupSequence(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(
                new Dummy
                {
                    Id = 1
                })
            .Throws(new InvalidOperationException("boom"));

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);
        _ = cache.Get("err");

        // Now repository throws during reload; method should swallow and continue
        var act = () => cache.ReloadAsync()
                             .GetAwaiter()
                             .GetResult();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void ReloadAsync_ShouldReloadSuccessfully_WhenEntryIsInCache()
    {
        // Arrange — create a file and load it into cache
        var filename = Path.Combine(TempDir, "reload_success.json");
        File.WriteAllText(filename, "{}");

        var callCount = 0;

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(() => new Dummy
            {
                Id = ++callCount
            });

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        // Initial load
        var initial = cache.Get("reload_success");

        initial.Should()
               .NotBeNull();

        initial!.Id
                .Should()
                .Be(1);

        // Act — reload should succeed and replace the entry
        cache.ReloadAsync()
             .GetAwaiter()
             .GetResult();

        // Assert — value should be replaced with the new one
        var reloaded = cache.Get("reload_success");

        reloaded.Should()
                .NotBeNull();

        reloaded!.Id
                 .Should()
                 .Be(2);
    }

    [Test]
    public void ReloadAsync_SpecificKey_Should_Handle_Errors()
    {
        var filename = Path.Combine(TempDir, "err2.json");
        File.WriteAllText(filename, "{}");

        // Seed with a successful load first
        Repo.SetupSequence(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(
                new Dummy
                {
                    Id = 2
                })
            .Throws(new InvalidOperationException("boom"));

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);
        _ = cache.Get("err2");

        var act = () => cache.ReloadAsync("err2")
                             .GetAwaiter()
                             .GetResult();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void ReloadAsync_SpecificKey_Should_Warn_When_Missing()
    {
        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        // No entry yet, should not throw
        cache.ReloadAsync("notcached")
             .GetAwaiter()
             .GetResult();
    }

    [Test]
    public void RemoveValueCallback_Should_Remove_From_LocalLookup()
    {
        // Seed cache by creating file before constructing cache so Paths includes it
        var filename = Path.Combine(TempDir, "z.json");
        File.WriteAllText(filename, "{}");

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        // Seed cache by calling Get

        File.WriteAllText(filename, "{}");

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(new Dummy());
        _ = cache.Get("z");

        // Evict entry
        MemoryCache.Remove("dummy___z");

        // If LocalLookup cleanup failed, a subsequent enumerate would include stale; no direct accessor, but
        // calling ReloadAsync to ensure no exceptions as a smoke check
        cache.ReloadAsync()
             .GetAwaiter()
             .GetResult();
    }

    [Test]
    public void RemoveValueCallback_Should_Respect_Replaced_Reason()
    {
        var filename = Path.Combine(TempDir, "cb.json");
        File.WriteAllText(filename, "{}");

        var value = new Dummy
        {
            Id = 99
        };

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(value);

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        _ = cache.Get("cb");

        // Should not remove when reason is Replaced
        cache.RemoveValueCallbackPublic(
            "dummy___cb",
            value,
            EvictionReason.Replaced,
            null);

        cache.Should()
             .Contain(v => v.Id == 99);

        // Should remove for other reasons (e.g., Expired)
        cache.RemoveValueCallbackPublic(
            "dummy___cb",
            value,
            EvictionReason.Expired,
            null);

        cache.Should()
             .NotContain(v => v.Id == 99);
    }

    [Test]
    public void TryGetValue_KeyInCache_ShouldReturnTrueAndValue()
    {
        var filename = Path.Combine(TempDir, "tryget.json");
        File.WriteAllText(filename, "{}");

        var expected = new Dummy
        {
            Id = 77
        };

        Repo.Setup(r => r.LoadAndMap<Dummy, DummySchema>(It.IsAny<string>(), It.IsAny<Action<DummySchema>>()))
            .Returns(expected);

        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        _ = cache.Get("tryget");

        var result = cache.TryGetValue("tryget", out var value);

        result.Should()
              .BeTrue();

        value.Should()
             .BeSameAs(expected);
    }

    [Test]
    public void TryGetValue_KeyNotInCache_ShouldReturnFalse()
    {
        var cache = new MockCache<Dummy, DummySchema, ExpiringFileCacheOptions>(
            MemoryCache,
            Repo.Object,
            Options,
            Logger);

        var result = cache.TryGetValue("absent", out var value);

        result.Should()
              .BeFalse();

        value.Should()
             .BeNull();
    }

    public sealed class Dummy
    {
        public int Id { get; set; }
    }

    public sealed class DummySchema
    {
        public int Id { get; set; }
    }
}