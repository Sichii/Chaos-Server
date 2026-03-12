#region
using FluentAssertions;
#endregion

namespace Chaos.Storage.Tests;

public sealed class LocalStorageOptionsTests
{
    [Test]
    public void UseBaseDirectory_Should_Combine_Base_And_Local_Directory()
    {
        // Arrange
        var baseDir = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid()
                .ToString("N"));

        var options = new LocalStorageOptions
        {
            Directory = "Data"
        };

        // Act
        options.UseBaseDirectory(baseDir);

        // Assert
        options.Directory
               .Should()
               .Be(Path.Combine(baseDir, "Data"));
    }
}