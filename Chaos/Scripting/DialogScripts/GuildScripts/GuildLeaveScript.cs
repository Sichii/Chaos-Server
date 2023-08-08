using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildLeaveScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildLeaveScript(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildLeaveScript> logger
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
        switch (Subject.Template.TemplateKey.ToLower())
        {
            //occurs when you click "Leave"
            case "generic_guild_leave_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            //occurs after you confirm you want to leave the guild
            case "generic_guild_leave_accepted":
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

        //ensure that if the player is a guild leader, that there are other guild leaders
        if (IsLeader(sourceRank) && (sourceRank.Count <= 1))
        {
            Subject.Reply(
                source,
                "You are the only leader of the guild. You must promote another member to leader before leaving.",
                "generic_guild_members_initial");

            return;
        }

        //leave the guild
        if (!source.Guild!.Leave(source))
        {
            //something went wrong, but I don't know what
            Subject.Reply(
                source,
                "You can not leave the guild at this time. ((a GM has been notified of this issue))",
                "generic_guild_members_initial");

            Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Leave)
                  .WithProperty(Subject)
                  .WithProperty(Subject.DialogSource)
                  .WithProperty(source)
                  .WithProperty(guild)
                  .LogWarning("Aisling {@AislingName} failed to leave guild {@GuildName} for an unknown reason", source.Name, guild.Name);

            return;
        }

        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Leave)
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .LogInformation("Aisling {@AislingName} has left guild {@GuildName}", source.Name, guild.Name);
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