using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Guilds;

public class GuildManagementScript : DialogScriptBase
{
    private readonly ILogger<GuildManagementScript> Logger;

    /// <inheritdoc />
    public GuildManagementScript(Dialog subject, ILogger<GuildManagementScript> logger)
        : base(subject) =>
        Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (source.Guild is null)
            Subject.AddOption("Create", "generic_guild_create_initial");
        else
        {
            if (!source.Guild.TryGetRank(source.GuildRank!, out var rank))
            {
                Logger.WithProperties(source, source.Guild)
                      .LogError(
                          "{@AislingName} has rank name {@RankName} in guild {@GuildName} but that rank does not exist",
                          source.Name,
                          source.GuildRank,
                          source.Guild.Name);

                Subject.ReplyToUnknownInput(source);

                return;
            }

            switch (rank.Tier)
            {
                case 0:
                {
                    Subject.AddOptions(
                        ("Ranks", "generic_guild_ranks_initial"),
                        ("Members", "generic_guild_members_initial"),
                        ("Disband", "generic_guild_disband_initial"),
                        ("Leave", "generic_guild_leave_initial"));

                    break;
                }
                default:
                {
                    Subject.AddOptions(
                        ("Members", "generic_guild_members_initial"),
                        ("Leave", "generic_guild_leave_initial"));

                    break;
                }
            }
        }
    }
}