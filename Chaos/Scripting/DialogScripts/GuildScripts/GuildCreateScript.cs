#region
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

public class GuildCreateScript : GuildScriptBase
{
    private const int GUILD_CREATION_COST = 1_000_000;

    /// <inheritdoc />
    public GuildCreateScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildCreateScript> logger)
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
            //occurs when you click "Create"
            case "generic_guild_create_initial":
            {
                OnDisplayingInitial(source);

                break;
            }

            //occurs after you choose a guild name
            case "generic_guild_create_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }

            //occurs after you confirm the guild name and cost
            case "generic_guild_create_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        //ensure the guild name is present
        if (!TryFetchArgs<string>(out var guildName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the guild doesn't already exist
        if (GuildExists(guildName))
        {
            Subject.Reply(source, "Apologies, but that guild name is already taken.", "generic_guild_create_name_input");

            return;
        }

        //ensure the player isn't already in a guild
        if (IsInGuild(source, out _, out _))
        {
            Subject.Reply(source, "You are already in a guild.", "top");

            return;
        }

        //ensure the player has enough gold and take it
        if (!source.TryTakeGold(GUILD_CREATION_COST))
        {
            Subject.Reply(source, "You do not have enough gold to create a guild.", "top");

            return;
        }

        //inject text paraeters for dialog text
        Subject.InjectTextParameters(guildName);

        //create the guild, set the player as the leader, and save it
        var newGuild = GuildFactory.Create(
            guildName,
            Guid.NewGuid()
                .ToString());

        newGuild.AddMember(source, source);
        newGuild.ChangeRank(source, 0, source);
        GuildStore.Save(newGuild);

        Logger.WithTopics(
                  [
                      Topics.Entities.Guild,
                      Topics.Actions.Create
                  ])
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(newGuild)
              .LogInformation("New guild {@GuildName} created by {@AislingName}", newGuild.Name, source.Name);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        //ensure the guild name is present
        if (!TryFetchArgs<string>(out var guildName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //inject text paraeters for dialog text
        Subject.InjectTextParameters(guildName, GUILD_CREATION_COST.ToString("N0"));
    }

    //inject text paraeters for dialog text
    // ReSharper disable once UnusedParameter.Local
    private void OnDisplayingInitial(Aisling source) => Subject.InjectTextParameters(GUILD_CREATION_COST.ToString("N0"));

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        //occurs when the player enters a guild name and clicks ok
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_guild_create_name_input":
            {
                OnNextNameInput(source);

                break;
            }
        }
    }

    private void OnNextNameInput(Aisling source)
    {
        //ensure the guild name is present
        if (!TryFetchArgs<string>(out var guildName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the guild doesn't already exist
        if (GuildExists(guildName))
        {
            Subject.Reply(source, "Apologies, but that guild name is already taken.", "generic_guild_create_name_input");

            return;
        }

        //enforce guild name rules
        //guild name rules here
        if (guildName.Length is < 3 or > 17)
            Subject.Reply(
                source,
                "That guild name is invalid. A valid guild name must be between 3 and 17 characters long.",
                "generic_guild_create_name_input");
    }
}