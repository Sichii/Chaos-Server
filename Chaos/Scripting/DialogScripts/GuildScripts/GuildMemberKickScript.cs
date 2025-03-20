#region
using System.Diagnostics;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildMemberKickScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildMemberKickScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildMemberKickScript> logger)
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
            case "generic_guild_members_kick_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_guild_members_kick_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!IsInGuild(source, out var guild, out var sourceRank))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        if (!TryFetchArgs<string>(out var name) || string.IsNullOrWhiteSpace(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!sourceRank.IsOfficerRank)
        {
            Subject.Reply(source, "You do not have permission to kick members", "generic_guild_members_initial");

            return;
        }

        if (!guild.HasMember(name))
        {
            Subject.Reply(source, $"{name} is not in your guild", "generic_guild_members_initial");

            return;
        }

        var targetCurrentRank = guild.RankOf(name);

        if (!sourceRank.IsSuperiorTo(targetCurrentRank))
        {
            Subject.Reply(source, $"You do not have permission to kick {name}", "generic_guild_members_initial");

            return;
        }

        if (!guild.TryKickMember(name, source))
            throw new UnreachableException(
                "The only failure reason is if the person being kicked is a leader. That should be checked for.");

        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Kick)
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .LogInformation(
                  "Aisling {@AislingName} kicked {@TargetAislingName} from {@GuildName}",
                  source.Name,
                  name,
                  guild.Name);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!IsInGuild(source, out var guild, out _))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        if (!TryFetchArgs<string>(out var name) || string.IsNullOrWhiteSpace(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(name, guild.Name);
    }
}