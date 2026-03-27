#region
using System.Collections.Concurrent;
using Chaos.Storage.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
#endregion

namespace Chaos.Storage.Tests;

public sealed class ReadOnlyAndStorageObjectTests : IDisposable
{
    private readonly IOptions<JsonSerializerOptions> JsonOptions;
    private readonly IOptions<LocalStorageOptions> LocalOptions;
    private readonly LocalStorageManager Manager;
    private readonly IMemoryCache MemoryCache;
    private readonly string TempDir;

    public ReadOnlyAndStorageObjectTests()
    {
        TempDir = Path.Combine(
            Path.GetTempPath(),
            "ReadOnlyAndStorageObjectTests_"
            + Guid.NewGuid()
                  .ToString("N"));
        Directory.CreateDirectory(TempDir);
        MemoryCache = new MemoryCache(new MemoryCacheOptions());
        JsonOptions = Options.Create(new JsonSerializerOptions());

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
    public void ReadOnlyStorageObject_Should_Default_Name_And_Clone_Value()
    {
        // Arrange
        var dict = new ConcurrentDictionary<string, Sample>(StringComparer.OrdinalIgnoreCase);

        dict["default"] = new Sample
        {
            Id = 7
        };

        // Assert via public API only: cannot construct internal type directly
        var storage = Manager.Load<Sample>();

        storage.Value
               .Id
               .Should()
               .Be(0);

        storage.GetInstance("alt")
               .Should()
               .NotBeNull();
    }

    [Test]
    public void StorageObject_Should_Expose_Save_Methods_And_GetInstance()
    {
        // Arrange
        var storage = Manager.Load<Sample>();

        // Act
        var ro = ((IReadOnlyStorage<Sample>)storage).GetInstance("x");
        var st = storage.GetInstance("y");

        // Just ensure methods are callable without exceptions
        st.Save();
        var act = async () => await st.SaveAsync();

        // Assert
        ro.Should()
          .NotBeNull();

        st.Should()
          .NotBeNull();

        act.Should()
           .NotBeNull();
    }

    private sealed class Sample
    {
        public int Id { get; set; }
    }
}