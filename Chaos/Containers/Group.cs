using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Objects.World;

namespace Chaos.Containers;

public sealed class Group : IEnumerable<Aisling>
{
    private readonly List<Aisling> Members;
    private readonly AutoReleasingMonitor Sync;

    public int Count => Members.Count;

    public Aisling Leader
    {
        get
        {
            using var @lock = Sync.Enter();

            return Members[0];
        }
    }

    public Group(Aisling sender, Aisling receiver)
    {
        Members = new List<Aisling>
        {
            sender, receiver
        };

        sender.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You form a group with {receiver.Name}");
        receiver.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You form a group with {sender.Name}");

        Sync = new AutoReleasingMonitor();
    }

    public void Add(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        foreach (var member in this)
        {
            member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{aisling.Name} has joined the group");
            member.Client.SendSelfProfile();
        }

        Members.Add(aisling);

        aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You have joined {Leader.Name}'s group");
        aisling.Group = this;
        aisling.Client.SendSelfProfile();
    }

    private void Disband()
    {
        using var @lock = Sync.Enter();

        foreach (var member in this)
        {
            member.Group = null;
            member.Client.SendServerMessage(ServerMessageType.ActiveMessage, "The group has been disbanded");
            member.Client.SendSelfProfile();
        }

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

        aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You have been kicked from the group by {Leader.Name}");

        foreach (var member in Members)
            member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{aisling.Name} has been kicked from the group");
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

        aisling.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You have left the group");

        foreach (var member in Members)
        {
            member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{aisling.Name} has left the group");
            member.Client.SendSelfProfile();
        }

        if (oldLeader.Equals(aisling))
        {
            var newLeader = Leader;

            foreach (var member in Members)
                member.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{newLeader.Name} is now the group leader");
        }
    }

    private bool Remove(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (!Members.Remove(aisling))
            return false;

        aisling.Group = null;
        aisling.Client.SendSelfProfile();

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