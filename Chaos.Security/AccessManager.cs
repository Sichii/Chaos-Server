using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Security.Abstractions;
using Chaos.Security.Definitions;
using Chaos.Security.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Security;

/// <summary>
///     Manages access to the server
/// </summary>
public sealed class AccessManager : BackgroundService, IAccessManager
{
    private readonly AutoReleasingSemaphoreSlim AccessSync;
    private readonly string BlacklistPath;
    private readonly PeriodicTimer CleanupTimer;
    private readonly AutoReleasingSemaphoreSlim CredentialSync;
    private readonly ConcurrentDictionary<string, CredentialFailureDetails> FailureDetails;
    private readonly ILogger<AccessManager> Logger;
    private readonly AccessManagerOptions Options;
    private readonly string WhitelistPath;

    /// <summary>
    ///     Creates a new instance of <see cref="AccessManager" />
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public AccessManager(IOptionsSnapshot<AccessManagerOptions> options, ILogger<AccessManager> logger)
    {
        Logger = logger;
        CredentialSync = new AutoReleasingSemaphoreSlim(1, 1);
        AccessSync = new AutoReleasingSemaphoreSlim(1, 1);
        Options = options.Value;
        BlacklistPath = Path.Combine(Options.Directory, "blacklist.txt");
        WhitelistPath = Path.Combine(Options.Directory, "whitelist.txt");
        FailureDetails = new ConcurrentDictionary<string, CredentialFailureDetails>(StringComparer.OrdinalIgnoreCase);
        CleanupTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        if (!File.Exists(BlacklistPath))
            File.Create(BlacklistPath).Dispose();

        if (!File.Exists(WhitelistPath))
            File.Create(WhitelistPath).Dispose();
    }

    /// <inheritdoc />
    public async Task BanishAsync(IPAddress ipAddress)
    {
        await using var @lock = await AccessSync.WaitAsync();

        var ipStr = ipAddress.ToString();

        await File.AppendAllLinesAsync(BlacklistPath, new[] { ipStr });

        var whiteList = (await File.ReadAllLinesAsync(WhitelistPath)).ToList();
        whiteList.RemoveAll(str => str.EqualsI(ipStr));

        await File.WriteAllLinesAsync(WhitelistPath, whiteList);
    }

    /// <inheritdoc />
    public async Task<CredentialValidationResult> ChangePasswordAsync(
        IPAddress ipAddress,
        string name,
        string oldPassword,
        string newPassword
    )
    {
        await using var sync = await CredentialSync.WaitAsync();

        if (FailureDetails.TryGetValue(ipAddress.ToString(), out var failureDetails)
            && (failureDetails.FailureCount > Options.MaxCredentialAttempts))
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.TooManyAttempts,
                FailureMessage = "Too many attempts"
            };

        var result = await InnerValidateCredentialsAsync(ipAddress, name, oldPassword);

        if (!result.Success)
        {
            if (result.Code == CredentialValidationResult.FailureCode.TooManyAttempts)
                Logger.LogWarning(
                    "{@ClientIp} has exceeded the maximum number of credential attempts while attempting to change password",
                    ipAddress.ToString());

            return result;
        }

        result = ValidatePasswordRules(newPassword);

        if (!result.Success)
            return result;

        var newHash = ComputeHash(newPassword);
        var passwordPath = Path.Combine(Options.Directory, name, "password.txt");

        await File.WriteAllTextAsync(passwordPath, newHash);

        return result;
    }

    /// <summary>
    ///     Computes a hash for the given password
    /// </summary>
    /// <param name="password">The password to hash</param>
    private string ComputeHash(string password)
    {
        #pragma warning disable SYSLIB0045
        using var algorithm = HashAlgorithm.Create(Options.HashAlgorithmName);
        #pragma warning restore SYSLIB0045

        var buffer = Encoding.UTF8.GetBytes(password);
        var hash = algorithm!.ComputeHash(buffer);

        return Convert.ToHexString(hash);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupTimer.WaitForNextTickAsync(stoppingToken);
            } catch (OperationCanceledException)
            {
                return;
            }

            var now = DateTime.UtcNow;

            foreach ((var ip, var details) in FailureDetails.ToList())
                if (now.Subtract(details.MostRecentFailureTime).TotalMinutes > Options.LockoutMins)
                    FailureDetails.TryRemove(ip, out _);
        }
    }

    /// <summary>
    ///     Validates a username and password combination
    /// </summary>
    /// <param name="ipAddress">The ip address of the client</param>
    /// <param name="name">The name to validate</param>
    /// <param name="password">The password to validate</param>
    /// <returns><c>true</c> if the username exists, and the password specified matches the record, otherwise <c>false</c></returns>
    private async Task<CredentialValidationResult> InnerValidateCredentialsAsync(IPAddress ipAddress, string name, string password)
    {
        var ipStr = ipAddress.ToString();

        if (FailureDetails.TryGetValue(ipStr, out var failureDetails) && (failureDetails.FailureCount > Options.MaxCredentialAttempts))
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.TooManyAttempts,
                FailureMessage = "Too many attempts, try again in a few minutes"
            };

        var characterDir = Path.Combine(Options.Directory, name);
        var passwordPath = Path.Combine(characterDir, "password.txt");

        if (!Directory.Exists(characterDir))
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.InvalidUsername,
                FailureMessage = "Username does not exist"
            };

        var givenHash = ComputeHash(password);
        var actualHash = await File.ReadAllTextAsync(passwordPath);

        var valid = givenHash.EqualsI(actualHash);

        if (valid)
            return CredentialValidationResult.SuccessResult;

        FailureDetails.AddOrUpdate(
            ipStr,
            str => new CredentialFailureDetails
            {
                IpAddress = str,
                MostRecentFailureTime = DateTime.UtcNow,
                FailureCount = 1
            },
            (_, details) =>
            {
                details.FailureCount++;
                details.MostRecentFailureTime = DateTime.UtcNow;

                return details;
            });

        return new CredentialValidationResult
        {
            Code = CredentialValidationResult.FailureCode.InvalidPassword,
            FailureMessage = "Wrong password"
        };
    }

    private async Task<bool> IsBlacklisted(IPAddress ipAddress) =>
        await File.ReadLinesAsync(BlacklistPath)
                  .Select(line => IPAddress.TryParse(line, out var ip) ? ip : null)
                  .Where(obj => obj is not null)
                  .ContainsAsync(ipAddress);

    private async Task<bool> IsWhitelisted(IPAddress ipAddress) =>
        await File.ReadLinesAsync(WhitelistPath)
                  .Select(line => IPAddress.TryParse(line, out var ip) ? ip : null)
                  .Where(obj => obj is not null)
                  .ContainsAsync(ipAddress);

    /// <inheritdoc />
    public async Task<CredentialValidationResult> SaveNewCredentialsAsync(IPAddress ipAddress, string name, string password)
    {
        await using var sync = await CredentialSync.WaitAsync();

        var result = ValidateUserNameRules(name);

        if (!result.Success)
            return result;

        result = ValidatePasswordRules(password);

        if (!result.Success)
            return result;

        var characterDir = Path.Combine(Options.Directory, name);

        if (Directory.Exists(characterDir))
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.InvalidUsername,
                FailureMessage = "Username is taken"
            };

        Directory.CreateDirectory(characterDir);
        var hash = ComputeHash(password);

        var passwordPath = Path.Combine(characterDir, "password.txt");
        await File.WriteAllTextAsync(passwordPath, hash);

        return CredentialValidationResult.SuccessResult;
    }

    /// <inheritdoc />
    public async Task<bool> ShouldAllowAsync(IPAddress ipAddress)
    {
        await using var @lock = await AccessSync.WaitAsync();

        return Options.Mode switch
        {
            IpAccessMode.Blacklist => !await IsBlacklisted(ipAddress),
            IpAccessMode.Whitelist => await IsWhitelisted(ipAddress),
            _                      => throw new ArgumentOutOfRangeException()
        };
    }

    /// <inheritdoc />
    public async Task<CredentialValidationResult> ValidateCredentialsAsync(IPAddress ipAddress, string name, string password)
    {
        await using var sync = await CredentialSync.WaitAsync();

        var result = await InnerValidateCredentialsAsync(ipAddress, name, password);

        if (result is { Success: false, Code: CredentialValidationResult.FailureCode.TooManyAttempts })
            Logger.LogWarning(
                "{@ClientIp} has exceeded the maximum number of credential attempts while attempting to log in",
                ipAddress.ToString());

        return result;
    }

    /// <summary>
    ///     Validates a password against the rules specified in the configuration
    /// </summary>
    /// <param name="password">The password to validate</param>
    private CredentialValidationResult ValidatePasswordRules(string password)
    {
        if (password.Length < Options.MinPasswordLength)
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.PasswordTooShort,
                FailureMessage = $"Password too short. Min length is {Options.MinPasswordLength}"
            };

        if (password.Length > Options.MaxPasswordLength)
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.PasswordTooLong,
                FailureMessage = $"Password too long. Max length is {Options.MaxPasswordLength}"
            };

        return CredentialValidationResult.SuccessResult;
    }

    /// <summary>
    ///     Validates a username against the rules specified in the configuration
    /// </summary>
    /// <param name="name">The name to validate</param>
    private CredentialValidationResult ValidateUserNameRules(string name)
    {
        if (!Options.ValidCharactersRegex.IsMatch(name))
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.UsernameNotAllowed,
                FailureMessage = "Invalid characters detected in username"
            };

        if (name.Length > Options.MaxUsernameLength)
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.UsernameTooLong,
                FailureMessage = $"Username is too long. Max length is {Options.MaxUsernameLength}"
            };

        if (name.Length < Options.MinUsernameLength)
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.UsernameTooShort,
                FailureMessage = $"Username is too short. Min length is {Options.MinUsernameLength}"
            };

        if (Options.ReservedUsernames.ContainsI(name))
            return new CredentialValidationResult
            {
                Code = CredentialValidationResult.FailureCode.UsernameNotAllowed,
                FailureMessage = "Username is not allowed"
            };

        foreach (var filtered in Options.PhraseFilter)
            if (name.ContainsI(filtered))
                return new CredentialValidationResult
                {
                    Code = CredentialValidationResult.FailureCode.UsernameNotAllowed,
                    FailureMessage = "Username is not allowed"
                };

        return CredentialValidationResult.SuccessResult;
    }

    private sealed class CredentialFailureDetails
    {
        internal required int FailureCount { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        internal required string IpAddress { get; init; }
        internal required DateTime MostRecentFailureTime { get; set; }
    }
}