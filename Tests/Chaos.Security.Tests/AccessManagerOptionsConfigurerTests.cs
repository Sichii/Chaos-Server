#region
using System.Text.RegularExpressions;
using Chaos.Security.Configuration;
using Chaos.Security.Options;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Security.Tests;

public sealed class AccessManagerOptionsConfigurerTests
{
    [Test]
    public void PostConfigure_Regexes_ShouldMatchExpectedPatterns()
    {
        var stagingDir = MockStagingDirectory.Create();
        var configurer = new AccessManagerOptionsConfigurer(stagingDir.Object);

        var options = new AccessManagerOptions
        {
            Directory = "Security",
            ValidCharactersPattern = @"^[a-zA-Z0-9]+$",
            ValidFormatPattern = @"^[a-zA-Z][a-zA-Z0-9]*$",
            HashAlgorithmName = "SHA256"
        };

        configurer.PostConfigure(null, options);

        options.ValidCharactersRegex
               .IsMatch("abc123")
               .Should()
               .BeTrue();

        options.ValidCharactersRegex
               .IsMatch("abc!@#")
               .Should()
               .BeFalse();

        options.ValidFormatRegex
               .IsMatch("abc123")
               .Should()
               .BeTrue();

        options.ValidFormatRegex
               .IsMatch("123abc")
               .Should()
               .BeFalse();
    }

    [Test]
    public void PostConfigure_ShouldCallBasePostConfigure_AndCombineDirectory()
    {
        var stagingBase = Path.Combine(Path.GetTempPath(), $"Stage_{Guid.NewGuid():N}");
        Directory.CreateDirectory(stagingBase);

        try
        {
            var stagingDir = MockStagingDirectory.Create(stagingBase);
            var configurer = new AccessManagerOptionsConfigurer(stagingDir.Object);

            var options = new AccessManagerOptions
            {
                Directory = "Security",
                ValidCharactersPattern = @"^[a-zA-Z0-9]+$",
                ValidFormatPattern = @"^[a-zA-Z][a-zA-Z0-9]*$",
                HashAlgorithmName = "SHA256"
            };

            configurer.PostConfigure(null, options);

            // base.PostConfigure calls UseBaseDirectory which combines paths
            options.Directory
                   .Should()
                   .Be(Path.Combine(stagingBase, "Security"));
        } finally
        {
            if (Directory.Exists(stagingBase))
                Directory.Delete(stagingBase, true);
        }
    }

    [Test]
    public void PostConfigure_ShouldCompileValidCharactersRegex()
    {
        var stagingDir = MockStagingDirectory.Create();
        var configurer = new AccessManagerOptionsConfigurer(stagingDir.Object);

        var options = new AccessManagerOptions
        {
            Directory = "Security",
            ValidCharactersPattern = @"^[a-zA-Z0-9]+$",
            ValidFormatPattern = @"^[a-zA-Z][a-zA-Z0-9]*$",
            HashAlgorithmName = "SHA256"
        };

        configurer.PostConfigure(null, options);

        options.ValidCharactersRegex
               .Should()
               .NotBeNull();

        options.ValidCharactersRegex
               .Options
               .Should()
               .HaveFlag(RegexOptions.Compiled);
    }

    [Test]
    public void PostConfigure_ShouldCompileValidFormatRegex()
    {
        var stagingDir = MockStagingDirectory.Create();
        var configurer = new AccessManagerOptionsConfigurer(stagingDir.Object);

        var options = new AccessManagerOptions
        {
            Directory = "Security",
            ValidCharactersPattern = @"^[a-zA-Z0-9]+$",
            ValidFormatPattern = @"^[a-zA-Z][a-zA-Z0-9]*$",
            HashAlgorithmName = "SHA256"
        };

        configurer.PostConfigure("name", options);

        options.ValidFormatRegex
               .Should()
               .NotBeNull();

        options.ValidFormatRegex
               .Options
               .Should()
               .HaveFlag(RegexOptions.Compiled);
    }
}