using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildRankManagementScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildRankManagementScript(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildRankManagementScript> logger)
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
            case "generic_guild_ranks_initial":
                OnDisplayingInitial(source);

                break;
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        if (!IsInGuild(source, out var guild, out _))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        foreach (var rank in guild.GetRanks())
            Subject.AddOption(rank.Name, "generic_guild_ranks_manage");
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_guild_ranks_initial":
                OnNextInitial(source, optionIndex);

                break;
        }
    }

    private void OnNextInitial(Aisling source, byte? optionIndex = null)
    {
        if (!optionIndex.HasValue)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var optionText = Subject.GetOptionText(optionIndex.Value);
        Subject.Context = optionText;
    }
}