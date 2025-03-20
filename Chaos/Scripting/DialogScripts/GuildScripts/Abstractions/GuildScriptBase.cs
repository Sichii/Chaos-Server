#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;

public abstract class GuildScriptBase : DialogScriptBase
{
    protected IClientRegistry<IChaosWorldClient> ClientRegistry { get; }
    protected IFactory<Guild> GuildFactory { get; }
    protected IStore<Guild> GuildStore { get; }
    protected ILogger Logger { get; }

    /// <inheritdoc />
    protected GuildScriptBase(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
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

    protected bool GuildExists(string name) => GuildStore.Exists(name);

    protected static bool IsInGuild(Aisling source, [MaybeNullWhen(false)] out Guild guild, [MaybeNullWhen(false)] out GuildRank sourceRank)
    {
        guild = source.Guild;
        sourceRank = guild?.RankOf(source.Name);

        return guild is not null;
    }
}