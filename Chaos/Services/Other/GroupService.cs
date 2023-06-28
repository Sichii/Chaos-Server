using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Extensions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Other;

public sealed class GroupService : IGroupService
{
    private readonly IChannelService ChannelService;
    private readonly ILogger<GroupService> Logger;
    private readonly HashSet<GroupInvite> PendingInvites;
    private readonly AutoReleasingMonitor Sync;

    public GroupService(ILogger<GroupService> logger, IChannelService channelService)
    {
        Logger = logger;
        ChannelService = channelService;
        PendingInvites = new HashSet<GroupInvite>();
        Sync = new AutoReleasingMonitor();
    }

    /// <inheritdoc />
    public void AcceptInvite(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();

        if (sender.Group == null)
        {
            if (receiver.Group == null)
            {
                var key = new GroupInvite(sender.Name, receiver.Name);

                if (!PendingInvites.TryGetValue(key, out var invite))
                    return;

                if (invite.Expired)
                {
                    sender.Client.SendServerMessage(
                        ServerMessageType.ActiveMessage,
                        $"{receiver.Name} tried to accept your invite, but it was expired");

                    receiver.SendActiveMessage("Failed to join group, invite was expired");

                    return;
                }

                PendingInvites.Remove(invite);

                var group = new Group(sender, receiver, ChannelService);
                sender.Group = group;
                receiver.Group = group;
            } else
                receiver.SendActiveMessage("You are already in a group");
        } else
        {
            if (receiver.Group == null)
            {
                var key = new GroupInvite(sender.Name, receiver.Name);

                if (!PendingInvites.TryGetValue(key, out var invite))
                    return;

                if (invite.Expired)
                {
                    sender.Client.SendServerMessage(
                        ServerMessageType.ActiveMessage,
                        $"{receiver.Name} tried to accept your invite, but it was expired");

                    receiver.SendActiveMessage("Failed to join group, invite was expired");

                    return;
                }

                if (sender.Group.Count >= WorldOptions.Instance.MaxGroupSize)
                {
                    receiver.SendActiveMessage("The group is full");

                    return;
                }

                PendingInvites.Remove(invite);
                sender.Group.Add(receiver);
            } else
                receiver.SendActiveMessage("You are already in a group");
        }
    }

    /// <inheritdoc />
    public void Invite(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();

        if (!sender.Options.Group)
        {
            sender.SendOrangeBarMessage("You have elected not to join groups");

            return;
        }

        if (!receiver.Options.Group)
        {
            sender.SendOrangeBarMessage($"{receiver.Name} refuses to join your group");

            return;
        }

        //if you invite yourself
        if (receiver.Equals(sender))
        {
            receiver.Group?.Leave(sender);

            return;
        }

        //if the target is ignoring the sender, log a warning
        //dont return here, let things play out, there should be another check to prevent sending an invite
        if (receiver.IgnoreList.Contains(sender.Name))
            Logger.WithProperty(sender)
                  .WithProperty(receiver)
                  .LogWarning(
                      "Aisling {@FromAisling} attempted to send a group invite to aisling {@TargetAisling}, but that player is ignoring them. (potential harassment)",
                      sender.Name,
                      receiver.Name);

        if (sender.Group == null)
        {
            if (receiver.Group == null)
                SendInvite(sender, receiver);
            else
                sender.SendActiveMessage($"{receiver.Name} is already in a group");
        } else
        {
            if (receiver.Group == null)
            {
                if (sender.Group.Count >= WorldOptions.Instance.MaxGroupSize)
                    sender.SendActiveMessage("Your group is full");
                else
                    SendInvite(sender, receiver);
            } else if (sender.Group == receiver.Group)
            {
                if (sender.Group.Leader.Equals(sender))
                    sender.Group.Kick(receiver);
                else
                    sender.SendActiveMessage($"{receiver.Name} is already in your group");
            } else
                sender.SendActiveMessage($"{receiver.Name} is already in a group");
        }
    }

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Only aislings can invite or be invited")]
    private void SendInvite(Aisling sender, Aisling receiver)
    {
        if (receiver.IgnoreList.Contains(sender.Name))
            return;

        var key = new GroupInvite(sender.Name, receiver.Name);

        if (!PendingInvites.Add(key))
            if (PendingInvites.TryGetValue(key, out var existingInvite))
                existingInvite.Refresh();

        receiver.Client.SendGroupRequest(GroupRequestType.FormalInvite, sender.Name);
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