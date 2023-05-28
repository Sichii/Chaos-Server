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

    public void AddMember(Aisling aisling)
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

    public void ChangeRank(Aisling aisling, int newTier)
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
    }

    public void Disband()
    {
        using var @lock = Sync.Enter();

        foreach (var aisling in GetOnlineMembers())
        {
            aisling.Guild = null;
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

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber subscriber) => ChannelService.LeaveChannel(subscriber, ChannelName);

    public GuildRank? RankOf(string name)
    {
        using var @lock = Sync.Enter();

        var ret = GuildHierarchy.FirstOrDefault(rank => rank.HasMember(name));

        if (ret is null)
            return null;

        return new GuildRank(ret.Name, ret.Tier, ret.GetMemberNames().ToList());
    }

    public void RemoveMember(string memberName)
    {
        ArgumentException.ThrowIfNullOrEmpty(memberName);

        using var @lock = Sync.Enter();

        foreach (var rank in GuildHierarchy)
            if (rank.RemoveMember(memberName))
            {
                var aisling = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(memberName))?.Aisling;

                if (aisling is not null)
                {
                    LeaveChannel(aisling);
                    aisling.Guild = null;
                    aisling.Client.SendSelfProfile();
                }

                break;
            }
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