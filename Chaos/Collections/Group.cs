using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Servers.Options;

namespace Chaos.Collections;

public sealed class Group : IEnumerable<Aisling>
{
    private readonly IChannelService ChannelService;
    private readonly List<Aisling> Members;
    private readonly AutoReleasingMonitor Sync;
    public string ChannelName { get; }

    public int Count => Members.Count;

    public Aisling Leader
    {
        get
        {
            using var @lock = Sync.Enter();

            return Members[0];
        }
    }

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
            true,
            "!group",
            ServerMessageType.GroupChat);

        ChannelService.JoinChannel(sender, ChannelName);
        ChannelService.JoinChannel(receiver, ChannelName);

        sender.SendActiveMessage($"You form a group with {receiver.Name}");
        receiver.SendActiveMessage($"You form a group with {sender.Name}");

        Sync = new AutoReleasingMonitor();
    }

    public void Add(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        foreach (var member in this)
        {
            member.SendActiveMessage($"{aisling.Name} has joined the group");
            member.Client.SendSelfProfile();
        }

        Members.Add(aisling);

        ChannelService.JoinChannel(aisling, ChannelName);
        aisling.SendActiveMessage($"You have joined {Leader.Name}'s group");
        aisling.Group = this;
        aisling.Client.SendSelfProfile();
    }

    private void Disband()
    {
        using var @lock = Sync.Enter();

        foreach (var member in this)
        {
            member.Group = null;
            ChannelService.LeaveChannel(member, ChannelName);
            member.SendActiveMessage("The group has been disbanded");
            member.Client.SendSelfProfile();
        }

        ChannelService.UnregisterChannel(ChannelName);
        Members.Clear();
    }

    /// <inheritdoc />
    public IEnumerator<Aisling> GetEnumerator()
    {
        List<Aisling> snapshot;

        using (Sync.Enter())
            snapshot = Members.ToList();

        return snapshot.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
        ChannelService.LeaveChannel(aisling, ChannelName);

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