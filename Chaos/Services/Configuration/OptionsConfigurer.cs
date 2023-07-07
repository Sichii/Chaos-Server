using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Chaos.Common.Abstractions;
using Chaos.MetaData;
using Chaos.MetaData.ItemMetadata;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.ItemScripts.Enchantments;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Configuration;

public sealed class OptionsConfigurer : IPostConfigureOptions<IConnectionInfo>,
                                        IPostConfigureOptions<LobbyOptions>,
                                        IPostConfigureOptions<LoginOptions>,
                                        IPostConfigureOptions<WorldOptions>,
                                        IPostConfigureOptions<MetaDataStoreOptions>,
                                        IPostConfigureOptions<GuildStoreOptions>,
                                        IPostConfigureOptions<MailStoreOptions>,
                                        IPostConfigureOptions<AislingStoreOptions>

{
    private readonly IStagingDirectory StagingDirectory;
    public OptionsConfigurer(IStagingDirectory stagingDirectory) => StagingDirectory = stagingDirectory;

    /// <inheritdoc />
    public void PostConfigure(string? name, AislingStoreOptions options) => options.UseBaseDirectory(StagingDirectory.StagingDirectory);

    /// <inheritdoc />
    public void PostConfigure(string? name, GuildStoreOptions options) => options.UseBaseDirectory(StagingDirectory.StagingDirectory);

    /// <inheritdoc />
    public void PostConfigure(string? name, IConnectionInfo options)
    {
        if (!string.IsNullOrEmpty(options.HostName))
            options.Address = Dns.GetHostAddresses(options.HostName).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;
    }

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
    public void PostConfigure(string? name, MetaDataStoreOptions options)
    {
        options.UseBaseDirectory(StagingDirectory.StagingDirectory);
        // ReSharper disable once ArrangeMethodOrOperatorBody
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(MagicPrefixScript.Mutate));
        //add more mutators here
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, WorldOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);
        PostConfigure(name, options.LoginRedirect);

        WorldOptions.Instance = options;
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, MailStoreOptions options) => options.UseBaseDirectory(StagingDirectory.StagingDirectory);
}