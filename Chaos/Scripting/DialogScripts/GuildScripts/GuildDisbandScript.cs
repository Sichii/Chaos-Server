using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildDisbandScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildDisbandScript(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildDisbandScript> logger)
        : base(
            subject,
            clientRegistry,
            guildStore,
            guildFactory,
            logger) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            //occurs when you click "Disband"
            case "generic_guild_disband_initial":
            {
                OnDisplayingInitial(source);

                break;
            }

            //occurs after you confirm you want to disband the guild
            case "generic_guild_disband_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        //ensure the player is still in a guild
        if (!IsInGuild(source, out var guild, out var sourceRank))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        //ensure the player is the guild leader
        if (!IsLeader(sourceRank))
        {
            Subject.Reply(source, "You do not have the authority to disband the guild.", "generic_guild_members_initial");

            return;
        }

        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Disband)
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .LogInformation("Guild {@GuildName} has been disbanded by aisling {@AislingName}", guild.Name, source.Name);

        //disband the guild and remove it from the guild store
        guild.Disband();
        GuildStore.Remove(guild.Name);
    }

    private void OnDisplayingInitial(Aisling source)
    {
        //ensure the player is still in a guild
        if (!IsInGuild(source, out var guild, out _))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        //inject text paraeters for dialog text
        Subject.InjectTextParameters(guild.Name);
    }
}