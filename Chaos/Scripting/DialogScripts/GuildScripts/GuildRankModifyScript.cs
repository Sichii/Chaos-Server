using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildRankModifyScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildRankModifyScript(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IGuildFactory guildFactory,
        ILogger<GuildRankModifyScript> logger
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
            case "generic_guild_ranks_modify":
                OnDisplayingModify(source);

                break;

            case "generic_guild_ranks_modify_confirmation":
                OnDisplayingConfirmation(source);

                break;

            case "generic_guild_ranks_modify_accepted":
                OnDisplayingAccepted(source);

                break;
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (Subject.Context is not string currentRankName)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!TryFetchArgs<string>(out var newRankName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (newRankName.Length is < 3 or > 17)
        {
            Subject.Reply(
                source,
                "That rank name is invalid. A valid rank name must be between 3 and 17 characters long.",
                "generic_guild_ranks_initial");

            return;
        }

        if (!IsInGuild(source, out var guild, out var rank))
        {
            Subject.Reply(source, "You are not in a guild.", "top");

            return;
        }

        if (!IsLeader(rank))
        {
            Subject.Reply(source, "You do not have permission to modify guild ranks", "top");

            return;
        }

        if (guild.TryGetRank(newRankName, out _))
        {
            Subject.Reply(source, "That rank name is already in use.", "top");

            return;
        }

        guild.ChangeRankName(currentRankName, newRankName);

        Logger.WithProperty(source)
              .WithProperty(guild)
              .WithProperty(Subject.DialogSource)
              .LogInformation(
                  "Aisling {AislingName} change rank {RankName} to {NewRankName}",
                  source.Name,
                  currentRankName,
                  newRankName);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (Subject.Context is not string currentRankName)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!TryFetchArgs<string>(out var newRankName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(currentRankName, newRankName);
    }

    private void OnDisplayingModify(Aisling source)
    {
        if (Subject.Context is not string currentRankName)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(currentRankName);
    }
}