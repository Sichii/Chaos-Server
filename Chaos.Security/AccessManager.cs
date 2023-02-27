using System.Net;
using Chaos.Security.Abstractions;
using Chaos.Security.Definitions;
using Chaos.Security.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Security;

public sealed class AccessManager : IAccessManager
{
    private readonly string BlacklistPath;
    private readonly AccessManagerOptions Options;
    private readonly string WhitelistPath;

    public AccessManager(IOptionsSnapshot<AccessManagerOptions> options)
    {
        Options = options.Value;
        BlacklistPath = Path.Combine(Options.Directory, "blacklist.txt");
        WhitelistPath = Path.Combine(Options.Directory, "whitelist.txt");

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        if (!File.Exists(BlacklistPath))
            File.Create(BlacklistPath);

        if (!File.Exists(WhitelistPath))
            File.Create(WhitelistPath);
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
    public async Task<bool> ShouldAllowAsync(IPAddress ipAddress) => Options.Mode switch
    {
        IpAccessMode.Blacklist => !await IsBlacklisted(ipAddress),
        IpAccessMode.Whitelist => await IsWhitelisted(ipAddress),
        _                      => throw new ArgumentOutOfRangeException()
    };
}