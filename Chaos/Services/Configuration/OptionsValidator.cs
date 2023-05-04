using Chaos.Networking.Options;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Configuration;

public sealed class OptionsValidator : IValidateOptions<LobbyOptions>,
                                       IValidateOptions<ChaosOptions>
{
    private readonly ILogger<OptionsValidator> Logger;

    public OptionsValidator(ILogger<OptionsValidator> logger) => Logger = logger;

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, LobbyOptions options)
    {
        foreach (var server in options.Servers)
        {
            if (server.Description.Length > 18)
            {
                Logger.LogError(
                    "Value for \"{OptionName}\" is too long ({OptionValue}), trimming it to 18 characters",
                    $"{nameof(LobbyOptions)}:{nameof(LobbyOptions.Servers)}:{nameof(LoginServerInfo.Description)}",
                    server.Description);

                server.Description = server.Description[..18];
            }

            if (server.Name.Length > 9)
            {
                Logger.LogError(
                    "Value for \"{OptionName}\" is too long ({OptionValue}), trimming it to 9 characters",
                    $"{nameof(LobbyOptions)}:{nameof(LobbyOptions.Servers)}:{nameof(LoginServerInfo.Name)}",
                    server.Name);

                server.Name = server.Name[..9];
            }
        }

        return ValidateOptionsResult.Success;
    }

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, ChaosOptions options)
    {
        if (string.IsNullOrEmpty(options.StagingDirectory))
            return ValidateOptionsResult.Fail("StagingDirectory is required");

        return ValidateOptionsResult.Success;
    }
}