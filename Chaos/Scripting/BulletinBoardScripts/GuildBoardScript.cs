using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Models.Board;
using Chaos.Models.World;
using Chaos.Scripting.BulletinBoardScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.BulletinBoardScripts;

/// <summary>
///     A board script for a guild
/// </summary>
/// <remarks>
///     Permissions:
///     <br />
///     Read: InGuild
///     <br />
///     Write: Member+
///     <br />
///     Delete: Post Owner / Council+
///     <br />
///     Highlight: Leader
///     <br />
///     Retention: Configurable
/// </remarks>
public sealed class GuildBoardScript : ConfigurableBulletinBoardScriptBase
{
    private readonly TimeSpan PostRetentionTime;
    private readonly IIntervalTimer PostRetentionTimer;
    public string GuildName { get; init; } = null!;

    /// <summary>
    ///     The number of hours a post will last before being automatically deleted. Use -1 to never delete posts
    /// </summary>
    public int PostRetentionHours { get; init; }

    /// <inheritdoc />
    public GuildBoardScript(BulletinBoard subject)
        : base(subject)
    {
        PostRetentionTime = TimeSpan.FromHours(PostRetentionHours);
        PostRetentionTimer = new IntervalTimer(TimeSpan.FromMinutes(5));
    }

    /// <inheritdoc />
    public override bool AllowedToDelete(Aisling aisling, Post post)
        => aisling.Name.EqualsI(post.Author) || IsCouncil(aisling) || IsLeader(aisling);

    /// <inheritdoc />
    public override bool AllowedToHighlight(Aisling aisling) => IsLeader(aisling);

    /// <inheritdoc />
    public override bool AllowedToPost(Aisling aisling) => IsInGuild(aisling) && !IsApplicant(aisling);

    /// <inheritdoc />
    public override bool AllowedToView(Aisling aisling) => IsMember(aisling);

    private bool IsApplicant(Aisling aisling)
        => aisling.Guild is not null
           && aisling.Guild.Name.EqualsI(GuildName)
           && aisling.Guild.TryGetRank(aisling.GuildRank!, out var rank)
           && (rank.Tier == 3);

    private bool IsCouncil(Aisling aisling)
        => aisling.Guild is not null
           && aisling.Guild.Name.EqualsI(GuildName)
           && aisling.Guild.TryGetRank(aisling.GuildRank!, out var rank)
           && (rank.Tier == 1);

    private bool IsInGuild(Aisling aisling) => aisling.Guild is not null && aisling.Guild.Name.EqualsI(GuildName);

    private bool IsLeader(Aisling aisling)
        => aisling.Guild is not null
           && aisling.Guild.Name.EqualsI(GuildName)
           && aisling.Guild.TryGetRank(aisling.GuildRank!, out var rank)
           && (rank.Tier == 0);

    private bool IsMember(Aisling aisling)
        => aisling.Guild is not null
           && aisling.Guild.Name.EqualsI(GuildName)
           && aisling.Guild.TryGetRank(aisling.GuildRank!, out var rank)
           && (rank.Tier == 2);

    /// <inheritdoc />
    public override bool ShouldRejectPost(Aisling aisling, Post post, [MaybeNullWhen(false)] out string reason)
    {
        reason = null;

        return false;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (PostRetentionHours == -1)
            return;

        PostRetentionTimer.Update(delta);

        if (!PostRetentionTimer.IntervalElapsed)
            return;

        //prune old posts based on retention time
        foreach (var post in Subject.ToList())
            if (DateTime.UtcNow.Subtract(post.CreationDate) > PostRetentionTime)
                Subject.Posts.Remove(post.PostId, out _);
    }
}