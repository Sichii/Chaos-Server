#region
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

public class GuildMemberAdmitScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildMemberAdmitScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildMemberAdmitScript> logger)
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
            case "generic_guild_members_admit_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_guild_members_admit_accepted":
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

        //only leaders and officers can promote/demote/admit/kick
        if (!sourceRank.IsOfficerRank)
        {
            Subject.Reply(source, "You do not have permission to admit members", "generic_guild_members_initial");

            return;
        }

        //ensure the name is present
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var aislingToAdmit = source.MapInstance
                                   .GetEntities<Aisling>()
                                   .FirstOrDefault(aisling => aisling.Name.EqualsI(name));

        //ensure the player is online
        if (aislingToAdmit is null)
        {
            Subject.Reply(source, $"{name} is not nearby", "generic_guild_members_initial");

            return;
        }

        //ensure the player is not in a guild
        if (IsInGuild(aislingToAdmit, out _, out _))
        {
            Subject.Reply(source, $"{name} is already in a guild", "generic_guild_members_initial");

            return;
        }

        guild.AddMember(aislingToAdmit, source);

        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Join)
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .WithProperty(aislingToAdmit)
              .LogInformation(
                  "Aisling {@AislingName} admitted {@TargetAislingName} to {@GuildName}",
                  source.Name,
                  aislingToAdmit.Name,
                  guild.Name);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        //ensure the name is present
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(name);
    }
}