#region
using Chaos.Storage.Abstractions.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Storage.Tests;

public sealed class ExpiringFileCacheOptionsTests
{
    [Test]
    public void Options_Should_Expose_Properties()
    {
        // Arrange
        var options = new ExpiringFileCacheOptions
        {
            Directory = "Dir",
            Expires = true,
            ExpirationMins = 5,
            FilePattern = "*.json",
            Recursive = true,
            SearchType = SearchType.Files
        };

        // Assert
        options.Directory
               .Should()
               .Be("Dir");

        options.Expires
               .Should()
               .BeTrue();

        options.ExpirationMins
               .Should()
               .Be(5);

        options.FilePattern
               .Should()
               .Be("*.json");

        options.Recursive
               .Should()
               .BeTrue();

        options.SearchType
               .Should()
               .Be(SearchType.Files);
    }

    [Test]
    public void UseBaseDirectory_Should_Combine_Base_And_Cache_Directory()
    {
        // Arrange
        var baseDir = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid()
                .ToString("N"));

        var options = new ExpiringFileCacheOptions
        {
            Directory = "Cache"
        };

        // Act
        options.UseBaseDirectory(baseDir);

        // Assert
        options.Directory
               .Should()
               .Be(Path.Combine(baseDir, "Cache"));
    }
}