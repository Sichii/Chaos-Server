using System.Text;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildMemberRosterScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildMemberRosterScript(
        Dialog subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildMemberRosterScript> logger
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
            //occurs when you click "Roster"
            case "generic_guild_members_roster_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        //ensure the player is still in a guild
        if (!IsInGuild(source, out var guild, out _))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        //build a message containing the guild roster
        /*
        Guild: ChaosBros
        Members: 3
        
        Rank: Leader
        ---------------
        Sichi
        
        Rank: Officer
        ---------------
        Sichii
        Sichiii
        
        Rank: Member
        ---------------
        
        Rank: Applicant
        ---------------
        
         */
        var builder = new StringBuilder();
        var separator = new string('-', 15);

        builder.AppendLine($"Guild: {guild.Name}");
        builder.AppendLine($"Members: {guild.GetMemberNames().Count()}");
        builder.AppendLine();

        foreach (var rank in guild.GetRanks())
        {
            builder.AppendLine($"Rank: {rank.Name}");
            builder.AppendLine(separator);

            foreach (var member in rank.GetMemberNames())
                builder.AppendLine(member);

            builder.AppendLine();
        }

        //send the message to the player
        source.Client.SendServerMessage(ServerMessageType.ScrollWindow, builder.ToString());
    }
}