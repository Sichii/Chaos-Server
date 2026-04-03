#region
using System.Net;
using System.Text.RegularExpressions;
using Chaos.Security.Abstractions;
using Chaos.Security.Definitions;
using Chaos.Security.Options;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
#endregion

namespace Chaos.Security.Tests;

public class AccessManagerTests : IDisposable
{
    private readonly AccessManager AccessManager;
    private readonly Mock<ILogger<AccessManager>> LoggerMock;
    private readonly IOptionsSnapshot<AccessManagerOptions> Options;
    private readonly string TestDirectory;
    private readonly IPAddress TestIpAddress = IPAddress.Parse("192.168.1.100");

    public AccessManagerTests()
    {
        LoggerMock = new Mock<ILogger<AccessManager>>();
        TestDirectory = Path.Combine(Path.GetTempPath(), $"AccessManagerTests_{Guid.NewGuid()}");

        var options = new AccessManagerOptions
        {
            Directory = TestDirectory,
            Mode = IpAccessMode.Blacklist,
            HashAlgorithmName = "SHA256",
            MaxCredentialAttempts = 3,
            LockoutMins = 5,
            MinPasswordLength = 4,
            MaxPasswordLength = 20,
            MinUsernameLength = 3,
            MaxUsernameLength = 15,
            ValidCharactersRegex = new Regex(@"^[a-zA-Z0-9]+$"),
            ValidFormatRegex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$"),
            ReservedUsernames = new[]
            {
                "admin",
                "root",
                "test"
            },
            PhraseFilter = new[]
            {
                "badword",
                "offensive"
            }
        };

        var optionsMock = new Mock<IOptionsSnapshot<AccessManagerOptions>>();

        optionsMock.Setup(x => x.Value)
                   .Returns(options);
        Options = optionsMock.Object;

        AccessManager = new AccessManager(Options, LoggerMock.Object);
    }

    public void Dispose()
    {
        AccessManager?.Dispose();

        if (Directory.Exists(TestDirectory))
            Directory.Delete(TestDirectory, true);
    }

    [Test]
    public async Task ChangePasswordAsync_Should_Fail_With_Wrong_Old_Password()
    {
        // Arrange
        var username = "testuser";
        var correctPassword = "correctpass123";
        var wrongPassword = "wrongpass";
        var newPassword = "newpass456";

        await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, correctPassword);

        // Act
        var result = await AccessManager.ChangePasswordAsync(
            TestIpAddress,
            username,
            wrongPassword,
            newPassword);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.InvalidPassword);
    }

    [Test]
    public async Task ChangePasswordAsync_Should_Update_Password_With_Valid_Credentials()
    {
        // Arrange
        var username = "testuser";
        var oldPassword = "oldpass123";
        var newPassword = "newpass456";

        await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, oldPassword);

        // Act
        var result = await AccessManager.ChangePasswordAsync(
            TestIpAddress,
            username,
            oldPassword,
            newPassword);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeTrue();

        // Verify new password works
        var validateResult = await AccessManager.ValidateCredentialsAsync(TestIpAddress, username, newPassword);

        validateResult.Success
                      .Should()
                      .BeTrue();
    }

    [Test]
    public async Task ChangePasswordAsync_Should_Validate_New_Password()
    {
        // Arrange
        var username = "testuser";
        var oldPassword = "oldpass123";
        var invalidNewPassword = "abc"; // too short

        await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, oldPassword);

        // Act
        var result = await AccessManager.ChangePasswordAsync(
            TestIpAddress,
            username,
            oldPassword,
            invalidNewPassword);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.PasswordTooShort);
    }

    [Test]
    public void Constructor_Should_Create_Required_Directories_And_Files()
    {
        // Assert
        Directory.Exists(TestDirectory)
                 .Should()
                 .BeTrue();

        File.Exists(Path.Combine(TestDirectory, "blacklist.txt"))
            .Should()
            .BeTrue();

        File.Exists(Path.Combine(TestDirectory, "whitelist.txt"))
            .Should()
            .BeTrue();

        File.Exists(Path.Combine(TestDirectory, "clientIdBan.txt"))
            .Should()
            .BeTrue();
    }

    [Test]
    public async Task IdBanishAsync_Should_Add_Client_ID_To_Ban_List()
    {
        // Arrange
        uint clientId1 = 12345;
        ushort clientId2 = 6789;

        // Act
        await AccessManager.IdBanishAsync(clientId1, clientId2);

        // Assert
        var banPath = Path.Combine(TestDirectory, "clientIdBan.txt");
        var content = await File.ReadAllTextAsync(banPath);

        content.Should()
               .Contain($"{clientId1},{clientId2}");
    }

    [Test]
    public async Task IdBanishAsync_Should_Not_Ban_Default_Values()
    {
        // Arrange
        var defaultClientId1 = 4278255360U;
          uint defaultClientId2 = 7695;

        // Act
        await AccessManager.IdBanishAsync(defaultClientId1, defaultClientId2);

        // Assert
        var banPath = Path.Combine(TestDirectory, "clientIdBan.txt");
        var content = await File.ReadAllTextAsync(banPath);

        content.Should()
               .BeEmpty();
    }

    [Test]
    public async Task IpBanishAsync_Should_Add_IP_To_Blacklist()
    {
        // Act
        await AccessManager.IpBanishAsync(TestIpAddress);

        // Assert
        var blacklistPath = Path.Combine(TestDirectory, "blacklist.txt");
        var content = await File.ReadAllTextAsync(blacklistPath);

        content.Should()
               .Contain(TestIpAddress.ToString());
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Create_Valid_Credentials()
    {
        // Arrange
        var username = "validuser";
        var password = "validpass123";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeTrue();

        var userDirectory = Path.Combine(TestDirectory, username);

        Directory.Exists(userDirectory)
                 .Should()
                 .BeTrue();

        File.Exists(Path.Combine(userDirectory, "password.txt"))
            .Should()
            .BeTrue();
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Duplicate_Username()
    {
        // Arrange
        var username = "duplicateuser";
        var password = "validpass123";

        await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.InvalidUsername);

        result.FailureMessage
              .Should()
              .Be("Username is taken");
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Filtered_Username()
    {
        // Arrange
        var username = "badworduser";
        var password = "validpass123";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.UsernameNotAllowed);

        result.FailureMessage
              .Should()
              .Be("Username is not allowed");
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Invalid_Username_Characters()
    {
        // Arrange
        var username = "invalid@user";
        var password = "validpass123";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.InvalidUsername);

        result.FailureMessage
              .Should()
              .Be("Invalid characters detected in username");
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Invalid_Username_Format()
    {
        // Arrange
        var username = "123invalidstart";
        var password = "validpass123";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.InvalidUsername);

        result.FailureMessage
              .Should()
              .Be("Invalid format detected in username");
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Password_Too_Long()
    {
        // Arrange
        var username = "validuser";
        var password = "verylongpasswordthatexceedslimittoolongtobeallowed";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.PasswordTooLong);
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Password_Too_Short()
    {
        // Arrange
        var username = "validuser";
        var password = "abc";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.PasswordTooShort);
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Reserved_Username()
    {
        // Arrange
        var username = "admin";
        var password = "validpass123";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.UsernameNotAllowed);

        result.FailureMessage
              .Should()
              .Be("Username is not allowed");
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Username_Too_Long()
    {
        // Arrange
        var username = "verylongusernamethatexceedslimit";
        var password = "validpass123";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.UsernameTooLong);
    }

    [Test]
    public async Task SaveNewCredentialsAsync_Should_Reject_Username_Too_Short()
    {
        // Arrange
        var username = "ab";
        var password = "validpass123";

        // Act
        var result = await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.UsernameTooShort);
    }

    [Test]
    public async Task ShouldAllowAsync_With_ClientId_Should_Return_False_For_Banned()
    {
        // Arrange
          uint clientId1 = 12345;
          uint clientId2 = 6789;
          await AccessManager.IdBanishAsync(clientId1, clientId2);

        // The implementation stores "clientId1,clientId2" but checks only clientId1
        // So this test needs to match the actual behavior
        var banPath = Path.Combine(TestDirectory, "clientIdBan.txt");
        var content = await File.ReadAllTextAsync(banPath);

        content.Should()
               .Contain("12345,6789");

        // Act
          var result = await AccessManager.ShouldAllowAsync(clientId1, clientId2);

          // The IsClientIdBanned method looks for exact clientId match
        result.Should()
              .BeFalse();
    }

    [Test]
    public async Task ShouldAllowAsync_With_ClientId_Should_Return_True_For_Non_Banned()
    {
        // Arrange
          uint clientId1 = 12345;
          uint clientId2 = 6789;

        // Act
          var result = await AccessManager.ShouldAllowAsync(clientId1, clientId2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public async Task ShouldAllowAsync_With_IP_Should_Return_False_For_Blacklisted()
    {
        // Arrange
        await AccessManager.IpBanishAsync(TestIpAddress);

        // Act
        var result = await AccessManager.ShouldAllowAsync(TestIpAddress);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public async Task ShouldAllowAsync_With_IP_Should_Return_True_For_Non_Blacklisted()
    {
        // Act
        var result = await AccessManager.ShouldAllowAsync(TestIpAddress);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public async Task ValidateCredentialsAsync_Should_Block_After_Max_Attempts()
    {
        // Arrange
        var username = "testuser";
        var correctPassword = "testpass123";
        var wrongPassword = "wrongpass";

        await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, correctPassword);

        // Act - exceed max attempts
        for (var i = 0; i < 4; i++)
            await AccessManager.ValidateCredentialsAsync(TestIpAddress, username, wrongPassword);

        var result = await AccessManager.ValidateCredentialsAsync(TestIpAddress, username, correctPassword);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.TooManyAttempts);
    }

    [Test]
    public async Task ValidateCredentialsAsync_Should_Fail_With_Invalid_Username()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "testpass123";

        // Act
        var result = await AccessManager.ValidateCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.InvalidUsername);

        result.FailureMessage
              .Should()
              .Be("Username does not exist");
    }

    [Test]
    public async Task ValidateCredentialsAsync_Should_Fail_With_Wrong_Password()
    {
        // Arrange
        var username = "testuser";
        var correctPassword = "testpass123";
        var wrongPassword = "wrongpass";

        await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, correctPassword);

        // Act
        var result = await AccessManager.ValidateCredentialsAsync(TestIpAddress, username, wrongPassword);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeFalse();

        result.Code
              .Should()
              .Be(CredentialValidationResult.FailureCode.InvalidPassword);

        result.FailureMessage
              .Should()
              .Be("Wrong password");
    }

    [Test]
    public async Task ValidateCredentialsAsync_Should_Succeed_With_Valid_Credentials()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass123";

        await AccessManager.SaveNewCredentialsAsync(TestIpAddress, username, password);

        // Act
        var result = await AccessManager.ValidateCredentialsAsync(TestIpAddress, username, password);

        // Assert
        result.Should()
              .NotBeNull();

        result.Success
              .Should()
              .BeTrue();
    }
}

public sealed class AccessManagerWhitelistTests : IDisposable
{
      private readonly AccessManager AccessManager;
      private readonly Mock<ILogger<AccessManager>> LoggerMock;
      private readonly string TestDirectory;
      private readonly IPAddress TestIpAddress = IPAddress.Parse("10.0.0.1");

      public AccessManagerWhitelistTests()
      {
            LoggerMock = new Mock<ILogger<AccessManager>>();
            TestDirectory = Path.Combine(Path.GetTempPath(), $"AccessManagerWhitelistTests_{Guid.NewGuid()}");

            var options = new AccessManagerOptions
            {
                  Directory = TestDirectory,
                  Mode = IpAccessMode.Whitelist,
                  HashAlgorithmName = "SHA256",
                  MaxCredentialAttempts = 3,
                  LockoutMins = 5,
                  MinPasswordLength = 4,
                  MaxPasswordLength = 20,
                  MinUsernameLength = 3,
                  MaxUsernameLength = 15,
                  ValidCharactersRegex = new Regex(@"^[a-zA-Z0-9]+$"),
                  ValidFormatRegex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$"),
                  ReservedUsernames = [],
                  PhraseFilter = []
            };

            var optionsMock = new Mock<IOptionsSnapshot<AccessManagerOptions>>();

            optionsMock.Setup(x => x.Value)
                       .Returns(options);

            AccessManager = new AccessManager(optionsMock.Object, LoggerMock.Object);
      }

      public void Dispose()
      {
            AccessManager?.Dispose();

            if (Directory.Exists(TestDirectory))
                  Directory.Delete(TestDirectory, true);
      }

      [Test]
      public async Task IpBanishAsync_RemovesIpFromWhitelist()
      {
            // Arrange: put IP on the whitelist
            var whitelistPath = Path.Combine(TestDirectory, "whitelist.txt");
            await File.AppendAllLinesAsync(whitelistPath, [TestIpAddress.ToString()]);

            // Verify it is whitelisted before banning
            var before = await AccessManager.ShouldAllowAsync(TestIpAddress);

            before.Should()
                  .BeTrue();

            // Act: ban the IP (should remove it from whitelist)
            await AccessManager.IpBanishAsync(TestIpAddress);

            // Assert: IP no longer appears in whitelist file
            var content = await File.ReadAllTextAsync(whitelistPath);

            content.Should()
                   .NotContain(TestIpAddress.ToString());
      }

      [Test]
      public async Task ShouldAllowAsync_WithWhitelistMode_AllowsWhitelistedIp()
      {
            // Add IP to whitelist file directly
            var whitelistPath = Path.Combine(TestDirectory, "whitelist.txt");
            await File.AppendAllLinesAsync(whitelistPath, [TestIpAddress.ToString()]);

            var result = await AccessManager.ShouldAllowAsync(TestIpAddress);

            result.Should()
                  .BeTrue();
      }

      [Test]
      public async Task ShouldAllowAsync_WithWhitelistMode_DeniesNonWhitelistedIp()
      {
            // Whitelist is empty — IP should be denied
            var result = await AccessManager.ShouldAllowAsync(TestIpAddress);

            result.Should()
                  .BeFalse();
      }
}