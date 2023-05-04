using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Chaos.Common.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Mutators;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Configuration;

public sealed class OptionsConfigurer : IPostConfigureOptions<IConnectionInfo>,
                                        IPostConfigureOptions<LobbyOptions>,
                                        IPostConfigureOptions<LoginOptions>,
                                        IPostConfigureOptions<WorldOptions>,
                                        IPostConfigureOptions<MetaDataCacheOptions>,
                                        IPostConfigureOptions<UserSaveManagerOptions>

{
    private readonly IStagingDirectory StagingDirectory;
    public OptionsConfigurer(IStagingDirectory stagingDirectory) => StagingDirectory = stagingDirectory;

    /// <inheritdoc />
    public void PostConfigure(string? name, IConnectionInfo options) =>
        options.Address = Dns.GetHostAddresses(options.HostName).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;

    /// <inheritdoc />
    public void PostConfigure(string? name, LobbyOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);

        foreach (var server in options.Servers)
            PostConfigure(name, server);
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, LoginOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);
        PostConfigure(name, options.WorldRedirect);

        if (Point.TryParse(options.StartingPointStr, out var point))
            options.StartingPoint = point;
        else
            throw new ConfigurationErrorsException($"Unable to parse starting point from config ({options.StartingPointStr})");
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, WorldOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);
        PostConfigure(name, options.LoginRedirect);

        WorldOptions.Instance = options;
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, MetaDataCacheOptions options)
    {
        options.UseBaseDirectory(StagingDirectory.StagingDirectory);
        // ReSharper disable once ArrangeMethodOrOperatorBody
        options.PrefixMutators.Add(new EnchantmentMetaNodeMutator());
        //add more mutators here
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, UserSaveManagerOptions options) => options.UseBaseDirectory(StagingDirectory.StagingDirectory);
}