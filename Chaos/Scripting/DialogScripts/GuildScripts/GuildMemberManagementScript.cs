using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildMemberManagementScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildMemberManagementScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildMemberManagementScript> logger)
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
            //occurs when you click "Members"
            case "generic_guild_members_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        //ensure the player is still in a guild
        if (!IsInGuild(source, out _, out var sourceRank))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        //all members can see the roster
        Subject.AddOption("Roster", "generic_guild_members_roster_initial");

        //only leaders and officers can promote/demote/admit/kick
        if (sourceRank.Tier <= 1)
            Subject.AddOptions(
                ("Promote", "generic_guild_members_promote_initial"),
                ("Demote", "generic_guild_members_demote_initial"),
                ("Admit", "generic_guild_members_admit_initial"),
                ("Kick", "generic_guild_members_kick_initial"));
    }
}