#region
using Chaos.Common.Abstractions;
using Chaos.Extensions.DependencyInjection;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Abstractions.Definitions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Moq;
#endregion

namespace Chaos.Storage.Tests;

public sealed class StorageExtensionsTests
{
    [Test]
    public void AddLocalStorage_Should_Register_Services()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddSingleton<IStagingDirectory>(sp => MockStagingDirectory.Create()
                                                                           .Object);
        services.AddSingleton<IConfiguration>(sp => new ConfigurationBuilder().Build());

        // Act
        services.AddMemoryCache();
        services.Configure<LocalStorageOptions>(o => o.Directory = "Test");
        services.AddLocalStorage();
        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetRequiredService<IStorageManager>()
                .Should()
                .BeOfType<LocalStorageManager>();

        provider.GetRequiredService<IReadOnlyStorage<Sample>>()
                .Should()
                .NotBeNull();

        provider.GetRequiredService<IStorage<Sample>>()
                .Should()
                .NotBeNull();
    }
}

public sealed class StorageExtensionsAdditionalTests
{
    [Test]
    public void AddExpiringCache_Should_Register_Default_Implementation()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IStagingDirectory>(sp => MockStagingDirectory.Create()
                                                                           .Object);
        services.AddSingleton<IConfiguration>(sp => new ConfigurationBuilder().Build());
        services.AddLogging();
        services.AddMemoryCache();
        services.AddSingleton<IEntityRepository>(new Mock<IEntityRepository>().Object);

        services.Configure<ExpiringFileCacheOptions>(o =>
        {
            o.Directory = Path.Combine(
                Path.GetTempPath(),
                "StorageExtensionsTests_"
                + Guid.NewGuid()
                      .ToString("N"));

            // set init-only via object initializer and copy mutable
            var configured = new ExpiringFileCacheOptions
            {
                Directory = o.Directory,
                FilePattern = "*.json",
                SearchType = SearchType.Files
            };
            o.Directory = configured.Directory;
        });

        services.AddExpiringCache<Sample, SampleSchema, ExpiringFileCacheOptions>();

        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<ISimpleCache<Sample>>();

        cache.Should()
             .NotBeNull();
    }

    [Test]
    public void AddExpiringCacheImpl_Should_Register_Custom_Implementation()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IStagingDirectory>(sp => MockStagingDirectory.Create()
                                                                           .Object);
        services.AddSingleton<IConfiguration>(sp => new ConfigurationBuilder().Build());
        services.AddLogging();
        services.AddMemoryCache();
        services.AddSingleton<IEntityRepository>(new Mock<IEntityRepository>().Object);

        services.Configure<ExpiringFileCacheOptions>(o =>
        {
            o.Directory = Path.Combine(
                Path.GetTempPath(),
                "StorageExtensionsTests_"
                + Guid.NewGuid()
                      .ToString("N"));

            var configured = new ExpiringFileCacheOptions
            {
                Directory = o.Directory,
                FilePattern = "*.json",
                SearchType = SearchType.Files
            };
            o.Directory = configured.Directory;
        });

        services.AddExpiringCacheImpl<Sample, MockCache<Sample, SampleSchema, ExpiringFileCacheOptions>, ExpiringFileCacheOptions>();

        var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ISimpleCache<Sample>>();

        cache.Should()
             .NotBeNull();

        cache.Should()
             .BeOfType<MockCache<Sample, SampleSchema, ExpiringFileCacheOptions>>();
    }

    private sealed class Sample
    {
        public int Id { get; set; }
    }

    private sealed class SampleSchema
    {
        public int Id { get; set; }
    }
}

internal sealed class Sample
{
    public int Id { get; set; }
}