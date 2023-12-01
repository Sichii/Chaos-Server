using Chaos.Common.Abstractions;
using Chaos.Common.Configuration;
using FluentAssertions;
using Moq;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class DirectoryBoundOptionsConfigurerTests
{
    [Fact]
    public void PostConfigure_ShouldHandleNullName()
    {
        // This test is mainly to ensure that the method can handle a null name without crashing
        // Given the provided code, there's no direct interaction with the name, so it's more of a sanity check

        // Arrange
        const string EXPECTED_DIRECTORY = "TestDirectory";

        var mockStagingDirectory = new Mock<IStagingDirectory>();

        mockStagingDirectory.SetupGet(m => m.StagingDirectory)
                            .Returns(EXPECTED_DIRECTORY);

        var mockOptions = new Mock<IDirectoryBound>();

        var configurer = new DirectoryBoundOptionsConfigurer<IDirectoryBound>(mockStagingDirectory.Object);

        // Act
        var act = () => configurer.PostConfigure(null, mockOptions.Object);

        // Assert
        act.Should()
           .NotThrow();
    }

    [Fact]
    public void PostConfigure_ShouldSetBaseDirectoryFromStagingDirectory()
    {
        // Arrange
        const string EXPECTED_DIRECTORY = "TestDirectory";

        var mockStagingDirectory = new Mock<IStagingDirectory>();

        mockStagingDirectory.SetupGet(m => m.StagingDirectory)
                            .Returns(EXPECTED_DIRECTORY);

        var mockOptions = new Mock<IDirectoryBound>();

        var configurer = new DirectoryBoundOptionsConfigurer<IDirectoryBound>(mockStagingDirectory.Object);

        // Act
        configurer.PostConfigure(null, mockOptions.Object);

        // Assert
        mockOptions.Verify(m => m.UseBaseDirectory(EXPECTED_DIRECTORY), Times.Once);
    }
}