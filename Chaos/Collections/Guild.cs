#region
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Services.Servers.Options;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a guild in game. A guild is a hierarchical group of aislings
/// </summary>
public sealed class Guild : IDedicatedChannel, IEquatable<Guild>
{
    private readonly IChannelService ChannelService;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly List<GuildRank> GuildHierarchy;
    private readonly Lock Sync;

    /// <summary>
    ///     The name of the channel associated with this guild
    /// </summary>
    public string ChannelName { get; }

    /// <summary>
    ///     The unique identifier of this guild
    /// </summary>
    public string Guid { get; }

    /// <summary>
    ///     The name of this guild
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Guild" /> class.
    /// </summary>
    /// <param name="name">
    ///     The name of the guild
    /// </param>
    /// <param name="guid">
    ///     A unique identifier for this guild
    /// </param>
    /// <param name="channelService">
    ///     A services used to communicate via channels
    /// </param>
    /// <param name="clientRegistry">
    ///     A registry of clients in the world
    /// </param>
    public Guild(
        string name,
        string guid,
        IChannelService channelService,
        IClientRegistry<IChaosWorldClient> clientRegistry)
    {
        Name = name;
        Guid = guid;
        ChannelName = $"!guild-{Name}-{Guid}";
        ChannelService = channelService;
        ClientRegistry = clientRegistry;
        Sync = new Lock();

        GuildHierarchy =
        [
            new GuildRank("Leader", 0),
            new GuildRank("Council", 1),
            new GuildRank("Member", 2),
            new GuildRank("Applicant", 3)
        ];

        ChannelService.RegisterChannel(
            null,
            ChannelName,
            WorldOptions.Instance.GuildMessageColor,
            (sub, msg) =>
            {
                var aisling = (Aisling)sub;
                aisling.SendServerMessage(ServerMessageType.GuildChat, msg);
            },
            true,
            "!guild");
    }

    /// <inheritdoc />
    public bool Equals(Guild? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Guid == other.Guid;
    }

    /// <inheritdoc />
    public void JoinChannel(IChannelSubscriber subscriber) => ChannelService.JoinChannel(subscriber, ChannelName, true);

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber subscriber) => ChannelService.LeaveChannel(subscriber, ChannelName);

    public void SendMessage(IChannelSubscriber from, string message) => ChannelService.SendMessage(from, ChannelName, message);

    /// <summary>
    ///     Adds a new member to the guild
    /// </summary>
    /// <param name="aisling">
    ///     The aisling to add to the guild
    /// </param>
    /// <param name="by">
    ///     The aisling doing the adding
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the aisling is already in a guild
    /// </exception>
    public void AddMember(Aisling aisling, Aisling by)
    {
        ArgumentNullException.ThrowIfNull(aisling);

        using var @lock = Sync.EnterScope();

        if (aisling.Guild is not null)
        {
            if (aisling.Guild == this)
                return;

            throw new InvalidOperationException(
                $"Attempted to add \"{aisling.Name}\" to guild \"{Name}\", but that player is already in guild \"{aisling.Guild.Name}\"");
        }

        var lowestRank = GuildHierarchy.MaxBy(x => x.Tier)!;

        lowestRank.AddMember(aisling.Name);
        aisling.Guild = this;
        aisling.GuildRank = lowestRank.Name;
        JoinChannel(aisling);
        aisling.Client.SendSelfProfile();

        foreach (var member in GetOnlineMembers()
                     .Where(member => !member.Equals(by)))
            member.SendActiveMessage($"{aisling.Name} has been admitted to the guild by {by.Name}");
    }

    /// <summary>
    ///     Associates an aisling with this guild
    /// </summary>
    /// <param name="aisling">
    ///     The aisling to associate
    /// </param>
    /// <remarks>
    ///     When an aisling logs in, they will be associated with the guild specified in their save file. A check is made to
    ///     verify that the aisling actually holds a rank in the guild
    /// </remarks>
    public void Associate(Aisling aisling)
    {
        ArgumentNullException.ThrowIfNull(aisling);

        using var @lock = Sync.EnterScope();

        var rank = UnsafeRankof(aisling.Name);

        if (rank is null)
            return;

        aisling.Guild = this;
        aisling.GuildRank = rank.Name;

        JoinChannel(aisling);
    }

    /// <summary>
    ///     Changes the rank of a member in the guild
    /// </summary>
    /// <param name="aisling">
    ///     The aisling whose rank to change
    /// </param>
    /// <param name="newTier">
    ///     The new rank tier the aisling is being set to
    /// </param>
    /// <param name="by">
    ///     The aisling doing the change
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the aisling is not in a guild
    /// </exception>
    public void ChangeRank(Aisling aisling, int newTier, Aisling by)
    {
        using var @lock = Sync.EnterScope();

        if (aisling.Guild is null)
            throw new InvalidOperationException(
                $"Attempted to change rank of \"{aisling.Name}\" in guild \"{Name}\", but that player is not in a guild");

        if (string.IsNullOrEmpty(aisling.GuildRank))
            throw new InvalidOperationException(
                $"Attempted to change rank of \"{aisling.Name}\" in guild \"{Name}\", but that player does not have a rank");

        var newRank = GuildHierarchy.First(r => r.Tier == newTier);
        var currentRank = GuildHierarchy.First(r => r.Name.EqualsI(aisling.GuildRank!));

        if (currentRank.Tier == 0)
            return;

        currentRank.RemoveMember(aisling.Name);
        newRank.AddMember(aisling.Name);
        aisling.GuildRank = newRank.Name;
        aisling.Client.SendSelfProfile();

        if (newRank.Tier < currentRank.Tier)
            foreach (var member in GetOnlineMembers()
                         .Where(member => !member.Equals(by)))
                member.SendActiveMessage($"{aisling.Name} has been promoted to {newRank.Name} by {by.Name}");
        else
            foreach (var member in GetOnlineMembers()
                         .Where(member => !member.Equals(by)))
                member.SendActiveMessage($"{aisling.Name} has been demoted to {newRank.Name} by {by.Name}");
    }

    /// <summary>
    ///     Changes the name of a rank in the guild
    /// </summary>
    /// <param name="currentRankName">
    ///     The current rank name
    /// </param>
    /// <param name="newRankName">
    ///     The name the rank is being changed to
    /// </param>
    public void ChangeRankName(string currentRankName, string newRankName)
    {
        ArgumentNullException.ThrowIfNull(newRankName);
        ArgumentNullException.ThrowIfNull(currentRankName);

        using var @lock = Sync.EnterScope();

        if (!UnsafeTryGetRank(currentRankName, out var rank))
            return;

        rank.ChangeRankName(newRankName, ClientRegistry);
    }

    /// <summary>
    ///     Disbands the guild
    /// </summary>
    /// <remarks>
    ///     Un associated the guild from all logged in members. When other members try to log in, if the guild with this name
    ///     no longer exists, they will fail to associate with a guild, and automatically become unassociated. If a new guild
    ///     is created with the same name, they will not hold a rank in the new guild, and will automatically become
    ///     unassociated
    /// </remarks>
    public void Disband()
    {
        using var @lock = Sync.EnterScope();

        foreach (var aisling in GetOnlineMembers())
        {
            LeaveChannel(aisling);
            aisling.Guild = null;
            aisling.GuildRank = null;
            aisling.Client.SendSelfProfile();
            aisling.SendActiveMessage($"{Name} has been disbanded");
        }

        GuildHierarchy.Clear();
        ChannelService.UnregisterChannel(ChannelName);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is Guild other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => Guid.GetHashCode();

    /// <summary>
    ///     Gets the names of all members in the guild
    /// </summary>
    public IEnumerable<string> GetMemberNames()
    {
        List<string> names;

        using (Sync.EnterScope())
            names = GuildHierarchy.SelectMany(x => x.GetMemberNames())
                                  .ToList();

        return names;
    }

    /// <summary>
    ///     Gets all aislings in the guild that are currently logged in
    /// </summary>
    public IEnumerable<Aisling> GetOnlineMembers()
    {
        List<Aisling> onlineMembers;

        using (Sync.EnterScope())
            onlineMembers = GuildHierarchy.SelectMany(x => x.GetOnlineMembers(ClientRegistry))
                                          .ToList();

        return onlineMembers;
    }

    /// <summary>
    ///     Gets all ranks in the guild
    /// </summary>
    /// <remarks>
    ///     These ranks are deep cloned and will not affect actual ranks if modified
    /// </remarks>
    public ICollection<GuildRank> GetRanks()
    {
        using var @lock = Sync.EnterScope();

        return GuildHierarchy.Select(DeepClone.CreateRequired)
                             .ToList();
    }

    /// <summary>
    ///     Determines if a member is in the guild
    /// </summary>
    /// <param name="memberName">
    ///     The name to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the member is in the guild, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool HasMember(string memberName)
    {
        using var @lock = Sync.EnterScope();

        return GuildHierarchy.Any(x => x.HasMember(memberName));
    }

    /// <summary>
    ///     Initializes the guild with the specified rank information
    /// </summary>
    /// <param name="guildHierarchy">
    ///     The rank information for the guild
    /// </param>
    public void Initialize(IEnumerable<GuildRank> guildHierarchy)
    {
        using var @lock = Sync.EnterScope();

        GuildHierarchy.Clear();
        GuildHierarchy.AddRange(guildHierarchy);
    }

    /// <summary>
    ///     Kicks a member out of the guild
    /// </summary>
    /// <param name="memberName">
    ///     The name of the member to kick
    /// </param>
    /// <param name="by">
    ///     The aisling doing the kicking
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a member with the specified name was in the guild and was kicked, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <remarks>
    ///     This will work on members who are not online. When the member logs in, the association check will fail due to the
    ///     aisling not holding a rank in the guild, and so they will become unassociated
    /// </remarks>
    public bool KickMember(string memberName, Aisling by)
    {
        ArgumentException.ThrowIfNullOrEmpty(memberName);

        using var @lock = Sync.EnterScope();

        var rank = UnsafeRankof(memberName);

        //leaders can not be removed
        if (rank is null or { Tier: 0 })
            return false;

        rank.RemoveMember(memberName);

        var aisling = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(memberName))
                                    ?.Aisling;

        if (aisling is not null)
        {
            LeaveChannel(aisling);
            aisling.Guild = null;
            aisling.GuildRank = null;
            aisling.Client.SendSelfProfile();
        }

        foreach (var member in GetOnlineMembers())
            member.SendActiveMessage($"{memberName} has been kicked from the guild by {by}");

        return true;
    }

    /// <summary>
    ///     Leaves the guild
    /// </summary>
    /// <param name="aisling">
    ///     The aisling leaving the guild
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the aisling was in the guild and left, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool Leave(Aisling aisling)
    {
        ArgumentNullException.ThrowIfNull(aisling);

        using var @lock = Sync.EnterScope();

        var memberName = aisling.Name;
        var rank = UnsafeRankof(memberName);

        //leaders can only leave if there's another leader
        if (rank is null or { Tier: 0, Count: <= 1 })
            return false;

        rank.RemoveMember(memberName);

        LeaveChannel(aisling);
        aisling.Guild = null;
        aisling.GuildRank = null;
        aisling.Client.SendSelfProfile();

        aisling.SendActiveMessage($"You have left {Name}");

        foreach (var member in GetOnlineMembers())
            member.SendActiveMessage($"{memberName} has left the guild");

        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="left">
    /// </param>
    /// <param name="right">
    /// </param>
    /// <returns>
    /// </returns>
    public static bool operator ==(Guild? left, Guild? right) => Equals(left, right);

    /// <summary>
    /// </summary>
    /// <param name="left">
    /// </param>
    /// <param name="right">
    /// </param>
    /// <returns>
    /// </returns>
    public static bool operator !=(Guild? left, Guild? right) => !Equals(left, right);

    /// <summary>
    ///     Gets the rank of a member in the guild
    /// </summary>
    /// <param name="name">
    ///     The name of the member to get the rank for
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the member is not in the guild
    /// </exception>
    /// <remarks>
    ///     The returned rank is deep cloned and will not affect the actual rank if modified
    /// </remarks>
    public GuildRank RankOf(string name)
    {
        using var @lock = Sync.EnterScope();

        var ret = UnsafeRankof(name);

        if (ret is null)
            throw new InvalidOperationException("Attempted to get rank of a member that is not in the guild.");

        return DeepClone.CreateRequired(ret);
    }

    /// <summary>
    ///     Attempts to get a rank by name
    /// </summary>
    /// <param name="rankName">
    ///     The name of the rank to retrieve
    /// </param>
    /// <param name="guildRank">
    ///     If one was found, the rank
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a rank was found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <remarks>
    ///     The returned rank is deep cloned and will not affect the actual rank if modified
    /// </remarks>
    public bool TryGetRank(string rankName, [MaybeNullWhen(false)] out GuildRank guildRank)
    {
        guildRank = null;
        ArgumentNullException.ThrowIfNull(rankName);

        using var @lock = Sync.EnterScope();

        if (!UnsafeTryGetRank(rankName, out var rank))
            return false;

        guildRank = DeepClone.CreateRequired(rank);

        return true;
    }

    /// <summary>
    ///     Attempts to get a rank by tier
    /// </summary>
    /// <param name="tier">
    ///     The tier of the rank to retrieve
    /// </param>
    /// <param name="guildRank">
    ///     If one was found, the rank
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a rank was found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <remarks>
    ///     The returned rank is deep cloned and will not affect the actual rank if modified
    /// </remarks>
    public bool TryGetRank(int tier, [MaybeNullWhen(false)] out GuildRank guildRank)
    {
        guildRank = null;

        using var @lock = Sync.EnterScope();

        if (!UnsafeTryGetRank(tier, out var rank))
            return false;

        guildRank = DeepClone.CreateRequired(rank);

        return true;
    }

    private GuildRank? UnsafeRankof(string memberName)
    {
        using var @lock = Sync.EnterScope();

        return GuildHierarchy.FirstOrDefault(rank => rank.HasMember(memberName));
    }

    private bool UnsafeTryGetRank(string rankName, [MaybeNullWhen(false)] out GuildRank guildRank)
    {
        ArgumentNullException.ThrowIfNull(rankName);

        using var @lock = Sync.EnterScope();

        guildRank = GuildHierarchy.FirstOrDefault(rank => rank.Name.EqualsI(rankName));

        return guildRank is not null;
    }

    private bool UnsafeTryGetRank(int tier, [MaybeNullWhen(false)] out GuildRank guildRank)
    {
        using var @lock = Sync.EnterScope();

        guildRank = GuildHierarchy.FirstOrDefault(rank => rank.Tier == tier);

        return guildRank is not null;
    }
}