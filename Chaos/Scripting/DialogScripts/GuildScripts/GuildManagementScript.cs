using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildManagementScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildManagementScript(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildManagementScript> logger
    )
        : base(
            subject,
            clientRegistry,
            guildStore,
            guildFactory,
            logger) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        //ensure the player is still in a guild
        if (!IsInGuild(source, out _, out var sourceRank))
            Subject.AddOption("Create", "generic_guild_create_initial");
        else if (IsLeader(sourceRank))
            Subject.AddOptions(
                ("Ranks", "generic_guild_ranks_initial"),
                ("Members", "generic_guild_members_initial"),
                ("Disband", "generic_guild_disband_initial"),
                ("Leave", "generic_guild_leave_initial"));
        else
            Subject.AddOptions(
                ("Members", "generic_guild_members_initial"),
                ("Leave", "generic_guild_leave_initial"));
    }
}