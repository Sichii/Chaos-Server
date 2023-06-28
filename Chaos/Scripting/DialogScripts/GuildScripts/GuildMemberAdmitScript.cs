using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildMemberAdmitScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildMemberAdmitScript(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IGuildFactory guildFactory,
        ILogger<GuildMemberAdmitScript> logger
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
        if (!IsOfficer(sourceRank))
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

        var aislingToAdmit = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name))?.Aisling;

        //ensure the player is online
        if (aislingToAdmit is null)
        {
            Subject.Reply(source, $"{name} is not online", "generic_guild_members_initial");

            return;
        }

        //ensure the player is not in a guild
        if (IsInGuild(aislingToAdmit, out _, out _))
        {
            Subject.Reply(source, $"{name} is already in a guild", "generic_guild_members_initial");

            return;
        }

        guild.AddMember(aislingToAdmit, source);

        Logger.WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .WithProperty(aislingToAdmit)
              .LogDebug(
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