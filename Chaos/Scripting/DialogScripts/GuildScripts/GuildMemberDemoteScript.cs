#region
using System.Diagnostics;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildMemberDemoteScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildMemberDemoteScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildMemberDemoteScript> logger)
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
            //occurs after you choose a guild member to demote
            case "generic_guild_members_demote_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }

            //occurs after you confirm the guild member to demote
            case "generic_guild_members_demote_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        //ensure the player is in a guild
        if (!IsInGuild(source, out var guild, out var sourceRank))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        //ensure the player name is present and not empty
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the player has permission to demote members (Tier 1+)
        if (!IsOfficer(sourceRank))
            Subject.Reply(source, "You do not have permission to demote members.", "generic_guild_members_initial");

        //ensure the player to demote is in the guild
        if (!guild.HasMember(name))
        {
            Subject.Reply(source, $"{name} is not in your guild", "generic_guild_members_initial");

            return;
        }

        var targetCurrentRank = guild.RankOf(name);

        //ensure the player to demote is not the same or higher rank (same or lower tier)
        if (!IsSuperiorRank(sourceRank, targetCurrentRank))
        {
            Subject.Reply(source, $"You do not have permission to demote {name}", "generic_guild_members_initial");

            return;
        }

        //ensure the player to demote is not already the lowest rank
        if (!CanBeDemoted(targetCurrentRank))
        {
            Subject.Reply(source, $"{name} is already the lowest rank and can not be further demoted.", "generic_guild_members_initial");

            return;
        }

        //grab the aisling to demote
        var aislingToDemote = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name))
                                            ?.Aisling;

        //ensure the aisling is online
        if (aislingToDemote is null)
        {
            Subject.Reply(source, $"{name} is not online", "generic_guild_members_initial");

            return;
        }

        //change the rank of the aisling
        guild.ChangeRank(aislingToDemote.Name, targetCurrentRank.Tier + 1, source);

        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Demote)
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .WithProperty(aislingToDemote)
              .LogInformation(
                  "Aisling {@AislingName} demoted {@TargetAislingName} to {@RankName} in {@GuildName}",
                  source.Name,
                  aislingToDemote.Name,
                  sourceRank.Name,
                  guild.Name);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        //ensure the player is in a guild
        if (!IsInGuild(source, out var guild, out _))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        //ensure the player name is present and not empty
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the player to demote is in the guild
        if (!guild.HasMember(name))
        {
            Subject.Reply(source, $"{name} is not in your guild", "generic_guild_members_initial");

            return;
        }

        var targetCurrentRank = guild.RankOf(name);

        //grab the rank the aisling will be demoted to
        if (!guild.TryGetRank(targetCurrentRank.Tier + 1, out var toRank))
            throw new UnreachableException(
                "This should only occur if the guild is missing the target rank tier + 1, but that should be checked already.");

        Subject.InjectTextParameters(name, toRank.Name);
    }
}