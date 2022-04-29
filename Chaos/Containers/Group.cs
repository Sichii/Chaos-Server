using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chaos.Core.Definitions;
using Chaos.WorldObjects;

namespace Chaos.Containers;

public class Group : IEnumerable<User>
{
    private readonly List<User> Members;
    private readonly HashSet<GroupInvite> PendingInvites = new();
    private readonly object Sync = new();

    public User Leader
    {
        get
        {
            lock (Sync)
                return Members[0];
        }
    }

    private Group(User sender) => Members = new List<User> { sender };

    public void AcceptInvite(User sender, User receiver)
    {
        lock (Sync)
        {
            if (receiver.Group != null)
            {
                receiver.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You are already in a group");

                return;
            }

            var key = new GroupInvite(sender.Name, receiver.Name);

            if (!PendingInvites.TryGetValue(key, out var invite))
                return;

            PendingInvites.Remove(invite);

            //if the invite timed out
            if (invite.Expired)
                return;

            Add(receiver);
        }
    }

    private void Add(User user)
    {
        lock (Sync)
        {
            Members.Add(user);

            foreach (var member in Members)
            {
                if (member.Equals(user))
                    user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You have joined {Leader.Name}'s group");
                else
                    member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{user.Name} has joined the group");

                member.Client.SendSelfProfile();
            }
        }
    }

    private void AddOrRefreshInvite(GroupInvite invite)
    {
        lock (Sync)
            if (!PendingInvites.Add(invite))
                if (PendingInvites.TryGetValue(invite, out var existingInvite))
                    existingInvite.Refresh();
    }

    public static Group Create(User sender, User receiver)
    {
        var group = new Group(sender);
        group.Add(receiver);

        return group;
    }

    public IEnumerator<User> GetEnumerator()
    {
        List<User> snapshot;

        lock (Sync)
            snapshot = Members.ToList();

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Invite(User sender, User receiver)
    {
        lock (Sync)
        {
            var fromLeader = Leader.Equals(sender);

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

            var invite = new GroupInvite(sender.Name, receiver.Name);
            AddOrRefreshInvite(invite);
            receiver.Client.SendGroupRequest(GroupRequestType.FormalInvite, sender.Name);
        }
    }

    private void Kick(User user)
    {
        lock (Sync)
        {
            if (user.Group != this)
                return;

            user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You have been kicked from the group by {Leader.Name}");
            Remove(user);

            foreach (var member in Members)
            {
                member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{user.Name} has been kicked from the group");
                member.Client.SendSelfProfile();
            }
        }
    }

    public void Leave(User user)
    {
        lock (Sync)
        {
            if (user.Group != this)
                return;

            user.Client.SendServerMessage(ServerMessageType.ActiveMessage,
                Members.Count > 1 ? "You have left the group" : "The group has disbanded");

            Remove(user);

            foreach (var member in Members)
            {
                member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{user.Name} has left the group");
                member.Client.SendSelfProfile();
            }
        }
    }

    private void Remove(User user)
    {
        lock (Sync)
        {
            var leader = Leader;

            Members.Remove(user);
            user.Group = null;
            user.Client.SendSelfProfile();

            if (Members.Count == 0)
                return;

            //if there's only 1 person left in the group, disband it
            if (Members.Count == 1)
            {
                Leave(Leader);

                return;
            }

            //if this user was the leader
            if (user.Equals(leader))
            {
                var newLeader = Leader;

                foreach (var member in Members)
                {
                    member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{newLeader.Name} is now the group leader");
                    member.Client.SendSelfProfile();
                }
            }
        }
    }

    public override string ToString()
    {
        lock (Sync)
        {
            var groupString = "Group members";

            foreach (var user in Members)
                if (user.Equals(Leader))
                    groupString += '\n' + $"* {user.Name}";
                else
                    groupString += '\n' + $"  {user.Name}";

            groupString += '\n' + $"Total {Members.Count}";

            return groupString;
        }
    }

    private record GroupInvite(string Sender, string Receiver)
    {
        private DateTime Creation = DateTime.UtcNow;

        public bool Expired => DateTime.UtcNow.Subtract(Creation).TotalMinutes > 1;

        public virtual bool Equals(GroupInvite? other)
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