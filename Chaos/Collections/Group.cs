using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Collections;

public sealed class Group : IEnumerable<Aisling>, IDedicatedChannel
{
    private readonly IChannelService ChannelService;
    private readonly List<Aisling> Members;
    private readonly AutoReleasingMonitor Sync;
    public string ChannelName { get; }

    public Aisling Leader
    {
        get
        {
            using var @lock = Sync.Enter();

            return Members[0];
        }
    }

    public int Count => Members.Count;

    public Group(Aisling sender, Aisling receiver, IChannelService channelService)
    {
        ChannelName = $"!group-{Guid.NewGuid()}";
        ChannelService = channelService;

        Members = new List<Aisling>
        {
            sender, receiver
        };

        //create a group chat channel and add both members to it
        ChannelService.RegisterChannel(
            null,
            ChannelName,
            WorldOptions.Instance.GroupMessageColor,
            (sub, msg) =>
            {
                var aisling = (Aisling)sub;
                aisling.SendServerMessage(ServerMessageType.GroupChat, msg);
            },
            true,
            "!group");

        JoinChannel(sender);
        JoinChannel(receiver);

        sender.SendActiveMessage($"You form a group with {receiver.Name}");
        receiver.SendActiveMessage($"You form a group with {sender.Name}");

        Sync = new AutoReleasingMonitor();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<Aisling> GetEnumerator()
    {
        List<Aisling> snapshot;

        using (Sync.Enter())
            snapshot = Members.ToList();

        return snapshot.GetEnumerator();
    }

    /// <inheritdoc />
    public void JoinChannel(IChannelSubscriber subscriber) => ChannelService.JoinChannel(subscriber, ChannelName, true);

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber subscriber) => ChannelService.LeaveChannel(subscriber, ChannelName);

    public void SendMessage(IChannelSubscriber from, string message) => ChannelService.SendMessage(from, ChannelName, message);

    public void Add(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        foreach (var member in this)
        {
            member.SendActiveMessage($"{aisling.Name} has joined the group");
            member.Client.SendSelfProfile();
        }

        Members.Add(aisling);

        JoinChannel(aisling);
        aisling.SendActiveMessage($"You have joined {Leader.Name}'s group");
        aisling.Group = this;
        aisling.Client.SendSelfProfile();
    }

    public bool Contains(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        return Members.Contains(aisling, WorldEntity.IdComparer);
    }

    private void Disband()
    {
        using var @lock = Sync.Enter();

        foreach (var member in this)
        {
            member.Group = null;
            LeaveChannel(member);
            member.SendActiveMessage("The group has been disbanded");
            member.Client.SendSelfProfile();
        }

        ChannelService.UnregisterChannel(ChannelName);
        Members.Clear();
    }

    public void Kick(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (Members.Count <= 2)
        {
            Disband();

            return;
        }

        if (!Remove(aisling))
            return;

        aisling.SendActiveMessage($"You have been kicked from the group by {Leader.Name}");

        foreach (var member in Members)
            member.SendActiveMessage($"{aisling.Name} has been kicked from the group");
    }

    public void Leave(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (Members.Count <= 2)
        {
            Disband();

            return;
        }

        var oldLeader = Leader;

        if (!Remove(aisling))
            return;

        aisling.SendActiveMessage("You have left the group");

        foreach (var member in Members)
        {
            member.SendActiveMessage($"{aisling.Name} has left the group");
            member.Client.SendSelfProfile();
        }

        if (oldLeader.Equals(aisling))
        {
            var newLeader = Leader;

            foreach (var member in Members)
                member.SendActiveMessage($"{newLeader.Name} is now the group leader");
        }
    }

    private bool Remove(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (!Members.Remove(aisling))
            return false;

        aisling.Group = null;
        aisling.Client.SendSelfProfile();
        LeaveChannel(aisling);

        return true;
    }

    public override string ToString()
    {
        using var @lock = Sync.Enter();

        var groupString = "Group members";

        foreach (var user in Members)
            if (user.Equals(Leader))
                groupString += '\n' + $"* {user.Name}";
            else
                groupString += '\n' + $"  {user.Name}";

        groupString += '\n' + $"Total {Count}";

        return groupString;
    }
}