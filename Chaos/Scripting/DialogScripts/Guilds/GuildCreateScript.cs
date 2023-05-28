using Chaos.Collections;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Guilds;

public class GuildCreateScript : DialogScriptBase
{
    private const int GUILD_CREATION_COST = 1_000_000;
    private readonly IGuildFactory GuildFactory;
    private readonly IStore<Guild> GuildStore;
    private readonly ILogger<GuildCreateScript> Logger;

    /// <inheritdoc />
    public GuildCreateScript(
        Dialog subject,
        IGuildFactory guildFactory,
        IStore<Guild> guildStore,
        ILogger<GuildCreateScript> logger
    )
        : base(subject)
    {
        GuildFactory = guildFactory;
        GuildStore = guildStore;
        Logger = logger;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_guild_create_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_guild_create_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_guild_create_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<string>(out var guildName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (GuildStore.Exists(guildName))
        {
            Subject.Reply(source, "Apologies, but that guild name is already taken.", "generic_guild_create_name_input");

            return;
        }

        if (source.Guild is not null)
        {
            Subject.Reply(source, "You are already in a guild.", "top");

            return;
        }

        if (!source.TryTakeGold(GUILD_CREATION_COST))
        {
            Subject.Reply(source, "You do not have enough gold to create a guild.", "top");

            return;
        }

        Subject.InjectTextParameters(guildName);

        var guild = GuildFactory.Create(guildName);
        guild.AddMember(source);
        guild.ChangeRank(source, 0);
        GuildStore.Save(guild);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<string>(out var guildName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(guildName, GUILD_CREATION_COST.ToString("N0"));
    }

    private void OnDisplayingInitial(Aisling source) => Subject.InjectTextParameters(GUILD_CREATION_COST.ToString("N0"));

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
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
        if (!TryFetchArgs<string>(out var guildName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (GuildStore.Exists(guildName))
        {
            Subject.Reply(source, "Apologies, but that guild name is already taken.", "generic_guild_create_name_input");

            return;
        }

        //guild name rules here
        if (guildName.Length is < 3 or > 17)
            Subject.Reply(
                source,
                "That guild name is invalid. A valid guild name must be between 3 and 17 characters long.",
                "generic_guild_create_name_input");
    }
}