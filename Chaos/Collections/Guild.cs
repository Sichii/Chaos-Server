using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Collections;

public sealed class Guild : IDedicatedChannel
{
    private readonly IChannelService ChannelService;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly List<GuildRank> GuildHierarchy;
    private readonly AutoReleasingMonitor Sync;
    public string ChannelName { get; }
    public string Name { get; }

    public Guild(
        string name,
        IChannelService channelService,
        IClientRegistry<IWorldClient> clientRegistry
    )
    {
        Name = name;
        ChannelName = $"!guild-{Name}";
        ChannelService = channelService;
        ClientRegistry = clientRegistry;
        Sync = new AutoReleasingMonitor();

        GuildHierarchy = new List<GuildRank>
        {
            new("Leader", 0),
            new("Council", 1),
            new("Member", 2),
            new("Applicant", 3)
        };

        ChannelService.RegisterChannel(
            null,
            ChannelName,
            WorldOptions.Instance.GuildMessageColor,
            true,
            "!guild",
            ServerMessageType.GuildChat);
    }

    public void AddMember(Aisling aisling, Aisling by)
    {
        ArgumentNullException.ThrowIfNull(aisling);

        using var @lock = Sync.Enter();

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

        foreach (var member in GetOnlineMembers().Where(member => !member.Equals(by)))
            member.SendActiveMessage($"{aisling.Name} has been admitted to the guild by {by.Name}");
    }

    public void Associate(Aisling aisling)
    {
        ArgumentNullException.ThrowIfNull(aisling);

        using var @lock = Sync.Enter();

        var rank = GuildHierarchy.FirstOrDefault(x => x.HasMember(aisling.Name));

        if (rank is null)
            return;

        aisling.Guild = this;
        aisling.GuildRank = rank.Name;

        JoinChannel(aisling);
    }

    public void ChangeRank(Aisling aisling, int newTier, Aisling by)
    {
        using var @lock = Sync.Enter();

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
            foreach (var member in GetOnlineMembers().Where(member => !member.Equals(by)))
                member.SendActiveMessage($"{aisling.Name} has been promoted to {newRank.Name} by {by.Name}");
        else
            foreach (var member in GetOnlineMembers().Where(member => !member.Equals(by)))
                member.SendActiveMessage($"{aisling.Name} has been demoted to {newRank.Name} by {by.Name}");
    }

    public void Disband()
    {
        using var @lock = Sync.Enter();

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

    public IEnumerable<string> GetMemberNames()
    {
        List<string> names;

        using (Sync.Enter())
            names = GuildHierarchy.SelectMany(x => x.GetMemberNames()).ToList();

        return names;
    }

    public IEnumerable<Aisling> GetOnlineMembers()
    {
        List<Aisling> onlineMembers;

        using (Sync.Enter())
            onlineMembers = GuildHierarchy.SelectMany(x => x.GetOnlineMembers(ClientRegistry)).ToList();

        return onlineMembers;
    }

    public ICollection<GuildRank> GetRanks()
    {
        using var @lock = Sync.Enter();

        return GuildHierarchy.Select(x => new GuildRank(x.Name, x.Tier, x.GetMemberNames().ToList())).ToList();
    }

    public bool HasMember(string memberName)
    {
        using var @lock = Sync.Enter();

        return GuildHierarchy.Any(x => x.HasMember(memberName));
    }

    public void Initialize(IEnumerable<GuildRank> guildHierarchy)
    {
        using var @lock = Sync.Enter();

        GuildHierarchy.Clear();
        GuildHierarchy.AddRange(guildHierarchy);
    }

    /// <inheritdoc />
    public void JoinChannel(IChannelSubscriber subscriber) => ChannelService.JoinChannel(subscriber, ChannelName, true);

    public bool KickMember(string memberName, Aisling by)
    {
        ArgumentException.ThrowIfNullOrEmpty(memberName);

        using var @lock = Sync.Enter();

        var rank = GuildHierarchy.FirstOrDefault(x => x.HasMember(memberName));

        //leaders can not be removed
        if (rank is null or { Tier: 0 })
            return false;

        rank.RemoveMember(memberName);
        var aisling = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(memberName))?.Aisling;

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

    public bool Leave(Aisling aisling)
    {
        ArgumentNullException.ThrowIfNull(aisling);

        using var @lock = Sync.Enter();

        var memberName = aisling.Name;
        var rank = GuildHierarchy.FirstOrDefault(x => x.HasMember(memberName));

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

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber subscriber) => ChannelService.LeaveChannel(subscriber, ChannelName);

    public GuildRank RankOf(string name)
    {
        using var @lock = Sync.Enter();

        var ret = GuildHierarchy.FirstOrDefault(rank => rank.HasMember(name));

        if (ret is null)
            throw new InvalidOperationException("Attempted to get rank of a member that is not in the guild.");

        return new GuildRank(ret.Name, ret.Tier, ret.GetMemberNames().ToList());
    }

    public void SendMessage(IChannelSubscriber from, string message) => ChannelService.SendMessage(from, ChannelName, message);

    public bool TryGetRank(string rankName, [MaybeNullWhen(false)] out GuildRank guildRank)
    {
        guildRank = null;
        ArgumentNullException.ThrowIfNull(rankName);

        using var @lock = Sync.Enter();

        var rank = GuildHierarchy.FirstOrDefault(rank => rank.Name.EqualsI(rankName));

        if (rank is null)
            return false;

        guildRank = new GuildRank(rank.Name, rank.Tier, rank.GetMemberNames().ToList());

        return true;
    }

    public bool TryGetRank(int tier, [MaybeNullWhen(false)] out GuildRank guildRank)
    {
        guildRank = null;

        using var @lock = Sync.Enter();

        var rank = GuildHierarchy.FirstOrDefault(rank => rank.Tier == tier);

        if (rank is null)
            return false;

        guildRank = new GuildRank(rank.Name, rank.Tier, rank.GetMemberNames().ToList());

        return true;
    }
}