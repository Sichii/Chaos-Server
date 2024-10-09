using Chaos.Collections;
using Chaos.Common.Synchronization;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Services.Other;

public sealed class GroupService(ILogger<GroupService> logger, IChannelService channelService) : IGroupService
{
    private readonly IChannelService ChannelService = channelService;
    private readonly ILogger<GroupService> Logger = logger;
    private readonly HashSet<TimedRequest> PendingInvites = [];
    private readonly HashSet<TimedRequest> PendingRequests = [];
    private readonly AutoReleasingMonitor Sync = new();

    /// <inheritdoc />
    public void AcceptInvite(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();
        var key = new TimedRequest(sender.Name, receiver.Name);

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

        if (sender.Group == null)
        {
            if (receiver.Group == null)
            {
                //sender is leader (receiver is joining sender)
                var group = new Group(sender, receiver, ChannelService);
                sender.Group = group;
                receiver.Group = group;

                //if receiver had a groupBox open, close it
                if (receiver.GroupBox is not null)
                {
                    receiver.GroupBox = null;
                    receiver.Display();
                }
            } else
                receiver.SendActiveMessage("You are already in a group");
        } else
        {
            if (receiver.Group == null)
            {
                if (sender.Group.Count >= WorldOptions.Instance.MaxGroupSize)
                {
                    receiver.SendActiveMessage("The group is full");

                    return;
                }

                sender.Group.Add(receiver);

                if (receiver.GroupBox is not null)
                {
                    receiver.GroupBox = null;
                    receiver.Display();
                }
            } else
                receiver.SendActiveMessage("You are already in a group");
        }
    }

    /// <inheritdoc />
    public void AcceptRequestToJoin(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();
        var key = new TimedRequest(sender.Name, receiver.Name);

        if (!PendingRequests.TryGetValue(key, out var request))
            return;

        if (request.Expired)
        {
            sender.Client.SendServerMessage(
                ServerMessageType.ActiveMessage,
                $"{receiver.Name} tried to accept your request, but it was expired");

            receiver.SendActiveMessage("Failed to accept request because it was expired");

            return;
        }

        PendingRequests.Remove(request);

        if (sender.Group == null)
        {
            if (receiver.Group == null)
            {
                //receiver is leader (sender is joining receiver)
                var group = new Group(receiver, sender, ChannelService);
                sender.Group = group;
                receiver.Group = group;
            } else
            {
                if (receiver.Group.Count >= WorldOptions.Instance.MaxGroupSize)
                {
                    sender.SendActiveMessage("The group is full");

                    return;
                }

                receiver.Group.Add(sender);
            }

            //if sender had a groupBox open, close it
            if (sender.GroupBox is not null)
            {
                sender.GroupBox = null;
                sender.Display();
            }
        } else
            sender.SendActiveMessage("You are already in a group");
    }

    /// <inheritdoc />
    public IGroupService.RequestType? DetermineRequestType(Aisling sender, Aisling receiver)
    {
        var key = new TimedRequest(sender.Name, receiver.Name);

        using var @lock = Sync.Enter();

        var isInvite = PendingInvites.TryGetValue(key, out var pendingInvite);
        var isRequest = PendingRequests.TryGetValue(key, out var pendingRequest);

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (isInvite && isRequest)
            return pendingInvite > pendingRequest ? IGroupService.RequestType.Invite : IGroupService.RequestType.RequestToJoin;

        if (isInvite)
            return IGroupService.RequestType.Invite;

        if (isRequest)
            return IGroupService.RequestType.RequestToJoin;

        return null;
    }

    /// <inheritdoc />
    public void Invite(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();

        if (!sender.Options.AllowGroup)
        {
            sender.SendOrangeBarMessage("You have elected not to join groups");

            return;
        }

        if (!receiver.Options.AllowGroup)
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
            Logger.WithTopics(Topics.Entities.Group, Topics.Actions.Invite, Topics.Qualifiers.Harassment)
                  .WithProperty(sender)
                  .WithProperty(receiver)
                  .LogWarning(
                      "Aisling {@FromAislingName} attempted to send a group invite to aisling {@TargetAislingName}, but that player is ignoring them. (potential harassment)",
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

    public void RequestToJoin(Aisling sender, Aisling receiver)
    {
        using var @lock = Sync.Enter();

        if (!sender.Options.AllowGroup)
        {
            sender.SendOrangeBarMessage("You have elected not to join groups");

            return;
        }

        if (!receiver.Options.AllowGroup)
        {
            sender.SendOrangeBarMessage($"{receiver.Name} has elected not to group with others");

            return;
        }

        //if you invite yourself
        if (receiver.Equals(sender))
        {
            sender.SendOrangeBarMessage("You look around dumbly, what are you doing?");

            return;
        }

        //check class limits if the target has a groupBox open
        if (receiver.GroupBox is not null)
        {
            var classLimit = sender.UserStatSheet.BaseClass switch
            {
                BaseClass.Warrior => receiver.GroupBox.MaxWarriors,
                BaseClass.Wizard  => receiver.GroupBox.MaxWizards,
                BaseClass.Rogue   => receiver.GroupBox.MaxRogues,
                BaseClass.Priest  => receiver.GroupBox.MaxPriests,
                BaseClass.Monk    => receiver.GroupBox.MaxMonks,
                _                 => 0
            };

            var currentClassCount = receiver.Group?.Count(m => m.UserStatSheet.BaseClass == sender.UserStatSheet.BaseClass)
                                    ?? (receiver.UserStatSheet.BaseClass == sender.UserStatSheet.BaseClass ? 1 : 0);

            if (currentClassCount >= classLimit)
            {
                sender.SendActiveMessage($"The group has reached it's limit of {sender.UserStatSheet.BaseClass}s");

                return;
            }
        }

        //if the target is ignoring the sender, log a warning
        //dont return here, let things play out, there should be another check to prevent sending an invite
        if (receiver.IgnoreList.Contains(sender.Name))
            Logger.WithTopics(Topics.Entities.Group, Topics.Actions.Invite, Topics.Qualifiers.Harassment)
                  .WithProperty(sender)
                  .WithProperty(receiver)
                  .LogWarning(
                      "Aisling {@FromAislingName} attempted to request a group invite from aisling {@TargetAislingName}, but that player is ignoring them. (potential harassment)",
                      sender.Name,
                      receiver.Name);

        if (sender.Group == null)
        {
            if (receiver.Group == null)
                SendRequestToJoin(sender, receiver);
            else if (receiver.Group.Count >= WorldOptions.Instance.MaxGroupSize)
                sender.SendActiveMessage($"{receiver.Name}'s group is full");
            else
                SendRequestToJoin(sender, receiver);
        } else
            sender.SendActiveMessage("You are already in a group");
    }

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Only aislings can invite or be invited")]
    private void SendInvite(Aisling sender, Aisling receiver)
    {
        if (receiver.IgnoreList.Contains(sender.Name))
            return;

        var key = new TimedRequest(sender.Name, receiver.Name);

        if (!PendingInvites.Add(key))
            if (PendingInvites.TryGetValue(key, out var existingInvite))
                existingInvite.Refresh();

        receiver.Client.SendDisplayGroupInvite(ServerGroupSwitch.Invite, sender.Name);
    }

    private void SendRequestToJoin(Aisling sender, Aisling receiver)
    {
        if (receiver.IgnoreList.Contains(sender.Name))
            return;

        var key = new TimedRequest(sender.Name, receiver.Name);

        if (!PendingRequests.Add(key))
            if (PendingRequests.TryGetValue(key, out var existingInvite))
                existingInvite.Refresh();

        receiver.Client.SendDisplayGroupInvite(ServerGroupSwitch.Invite, sender.Name);
    }

    private sealed record TimedRequest(string Sender, string Receiver) : IComparable<TimedRequest>
    {
        private DateTime Creation = DateTime.UtcNow;

        public bool Expired
            => DateTime.UtcNow.Subtract(Creation)
                       .TotalMinutes
               > 1;

        /// <inheritdoc />
        public int CompareTo(TimedRequest? other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (other is null)
                return 1;

            return Creation.CompareTo(other.Creation);
        }

        public bool Equals(TimedRequest? other)
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

        public static bool operator >(TimedRequest? left, TimedRequest? right) => Comparer<TimedRequest>.Default.Compare(left, right) > 0;

        public static bool operator >=(TimedRequest? left, TimedRequest? right) => Comparer<TimedRequest>.Default.Compare(left, right) >= 0;

        public static bool operator <(TimedRequest? left, TimedRequest? right) => Comparer<TimedRequest>.Default.Compare(left, right) < 0;

        public static bool operator <=(TimedRequest? left, TimedRequest? right) => Comparer<TimedRequest>.Default.Compare(left, right) <= 0;

        public void Refresh() => Creation = DateTime.UtcNow;
    }
}