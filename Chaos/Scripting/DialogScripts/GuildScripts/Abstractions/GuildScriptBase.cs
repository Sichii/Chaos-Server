using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;

public abstract class GuildScriptBase : DialogScriptBase
{
    protected IClientRegistry<IWorldClient> ClientRegistry { get; }
    protected IFactory<Guild> GuildFactory { get; }
    protected IStore<Guild> GuildStore { get; }
    protected ILogger Logger { get; }

    /// <inheritdoc />
    protected GuildScriptBase(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger logger)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        GuildStore = guildStore;
        Logger = logger;
        GuildFactory = guildFactory;
    }

    protected static bool CanBeDemoted(GuildRank rank) => rank.Tier != 0;

    protected static bool CanBePromoted(GuildRank rank) => rank.Tier != 1;

    protected bool GuildExists(string name) => GuildStore.Exists(name);

    protected static bool IsInferiorRank(GuildRank rank, GuildRank other) => rank.Tier > other.Tier;

    protected static bool IsInGuild(Aisling source, [MaybeNullWhen(false)] out Guild guild, [MaybeNullWhen(false)] out GuildRank sourceRank)
    {
        guild = source.Guild;
        sourceRank = guild?.RankOf(source.Name);

        return guild is not null;
    }

    protected static bool IsLeader(GuildRank rank) => rank.Tier == 0;

    protected static bool IsOfficer(GuildRank rank) => rank.Tier <= 1;

    protected static bool IsSuperiorRank(GuildRank rank, GuildRank other) => rank.Tier < other.Tier;
}