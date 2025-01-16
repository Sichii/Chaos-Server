#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.ReactorTileScripts;

public class LeaderboardScript : ReactorTileScriptBase
{
    private IStorage<LeaderboardObj> Leaderboard { get; }

    /// <inheritdoc />
    public LeaderboardScript(ReactorTile subject, IStorage<LeaderboardObj> storage)
        : base(subject)
        => Leaderboard = storage;

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        //when the leaderboard is opened, add 1 to the score
        var leaderboard = Leaderboard.Value.Entries;

        if (leaderboard.TryGetValue(source.Name, out var sourceEntry))
            leaderboard[source.Name] = sourceEntry + 1;
        else
            leaderboard[source.Name] = 1;

        //save the leaderboard
        Leaderboard.Save();

        //print the leaderboard in highest > lowest order
        var builder = new StringBuilder();

        foreach (var kvp in leaderboard.OrderByDescending(kvp => kvp.Value))
            builder.AppendLine($"{kvp.Key:13} {kvp.Value}");

        source.SendServerMessage(ServerMessageType.WoodenBoard, builder.ToString());
    }

    public sealed class LeaderboardObj
    {
        public Dictionary<string, int> Entries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}