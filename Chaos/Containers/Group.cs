using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Objects.World;
using Chaos.Servers.Options;

namespace Chaos.Containers;

public sealed class Group : IEnumerable<Aisling>
{
    private readonly List<Aisling> Members;
    private readonly HashSet<GroupInvite> PendingInvites;
    private readonly AutoReleasingMonitor Sync;

    public Aisling Leader
    {
        get
        {
            using var @lock = Sync.Enter();

            return Members[0];
        }
    }

    private Group(Aisling sender)
    {
        Members = new List<Aisling> { sender };
        PendingInvites = new HashSet<GroupInvite>();
        Sync = new AutoReleasingMonitor();
    }

    public void AcceptInvite(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();

        if (receiver.Group != null)
        {
            receiver.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You are already in a group");

            return;
        }

        var key = new GroupInvite(sender.Name, receiver.Name);

        if (!PendingInvites.TryGetValue(key, out var invite))
            return;

        if(Members.Count == WorldOptions.Instance.MaxGroupSize)
        {
            receiver.Client.SendServerMessage(ServerMessageType.ActiveMessage, "The group is full");

            return;
        }
        
        PendingInvites.Remove(invite);

        //if the invite timed out
        if (invite.Expired)
            return;

        Add(receiver);
    }

    private void Add(Aisling aisling)
    {
        using var @lock = Sync.Enter();
        Members.Add(aisling);

        foreach (var member in Members)
        {
            if (member.Equals(aisling))
                aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You have joined {Leader.Name}'s group");
            else
                member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{aisling.Name} has joined the group");

            member.Client.SendSelfProfile();
        }
    }

    private void AddOrRefreshInvite(GroupInvite invite)
    {
        using var @lock = Sync.Enter();

        if (!PendingInvites.Add(invite))
            if (PendingInvites.TryGetValue(invite, out var existingInvite))
                existingInvite.Refresh();
    }

    public static Group Create(Aisling sender, Aisling receiver)
    {
        var group = new Group(sender)
        {
            receiver
        };

        return group;
    }

    public IEnumerator<Aisling> GetEnumerator()
    {
        List<Aisling> snapshot;

        using (Sync.Enter())
            snapshot = Members.ToList();

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Invite(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();
        var fromLeader = Leader.Equals(sender);

        if (!sender.Options.Group)
        {
            sender.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You have elected not to join groups");

            return;
        }

        if (!receiver.Options.Group)
        {
            sender.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{receiver.Name} refuses to join your group");

            return;
        }

        //if you invite yourself
        if (receiver.Equals(sender))
        {
            //leave the group
            Leave(sender);

            return;
        }

        //if the target is ignoring the sender, do nothing
        if (receiver.IgnoreList.Contains(sender.Name))
            return;

        //if the receiver is in a group
        if (receiver.Group != null)
        {
            //if theyre in the sender's group, and the sender is the leader
            if ((receiver.Group == this) && fromLeader)
            {
                //kick the receiver
                Kick(receiver);

                return;
            }

            //receiver is already in a grp
            sender.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{receiver.Name} is already in a group");
        }

        if (Members.Count == WorldOptions.Instance.MaxGroupSize)
        {
            sender.Client.SendServerMessage(ServerMessageType.ActiveMessage, "Your group is full");

            return;
        }

        var invite = new GroupInvite(sender.Name, receiver.Name);
        AddOrRefreshInvite(invite);
        receiver.Client.SendGroupRequest(GroupRequestType.FormalInvite, sender.Name);
    }

    private void Kick(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (aisling.Group != this)
            return;

        aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You have been kicked from the group by {Leader.Name}");
        Remove(aisling);

        foreach (var member in Members)
        {
            member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{aisling.Name} has been kicked from the group");
            member.Client.SendSelfProfile();
        }
    }

    public void Leave(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (aisling.Group != this)
            return;

        aisling.Client.SendServerMessage(
            ServerMessageType.ActiveMessage,
            Members.Count > 1 ? "You have left the group" : "The group has disbanded");

        Remove(aisling);

        foreach (var member in Members)
        {
            member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{aisling.Name} has left the group");
            member.Client.SendSelfProfile();
        }
    }

    private void Remove(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        var leader = Leader;

        Members.Remove(aisling);
        aisling.Group = null;
        aisling.Client.SendSelfProfile();

        if (Members.Count == 0)
            return;

        //if there's only 1 person left in the group, disband it
        if (Members.Count == 1)
        {
            Leave(Leader);

            return;
        }

        //if this user was the leader
        if (aisling.Equals(leader))
        {
            var newLeader = Leader;

            foreach (var member in Members)
            {
                member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{newLeader.Name} is now the group leader");
                member.Client.SendSelfProfile();
            }
        }
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

        groupString += '\n' + $"Total {Members.Count}";

        return groupString;
    }

    private sealed record GroupInvite(string Sender, string Receiver)
    {
        private DateTime Creation = DateTime.UtcNow;

        public bool Expired => DateTime.UtcNow.Subtract(Creation).TotalMinutes > 1;

        public bool Equals(GroupInvite? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Sender, other.Sender, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(Receiver, other.Receiver, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Sender, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(Receiver, StringComparer.OrdinalIgnoreCase);

            return hashCode.ToHashCode();
        }

        public void Refresh() => Creation = DateTime.UtcNow;
    }
}