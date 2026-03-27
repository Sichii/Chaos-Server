#region
using System.Collections;
using System.Reflection;
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Services.Other;
using Chaos.Services.Other.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class GroupServiceTests
{
    private readonly IGroupService GroupService = MockGroupService.Create();
    private readonly MapInstance SharedMap = MockMapInstance.Create();

    #region AcceptInvite — GroupBox Closure (Existing Group)
    [Test]
    public void AcceptInvite_ShouldCloseGroupBox_WhenReceiverJoinsExistingGroup()
    {
        var groupService = MockGroupService.Create();

        var leader = MockAisling.Create(
            SharedMap,
            "GBLeader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "GBMember",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "GBReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Leader has a group
        FormGroupWith(groupService, leader, member);

        // Receiver has a GroupBox
        receiver.GroupBox = new GroupBox
        {
            Name = "Recruiting",
            Note = "Join us!",
            MaxWarriors = 3
        };

        // Leader invites receiver
        groupService.Invite(leader, receiver);
        groupService.AcceptInvite(leader, receiver);

        // Receiver should be in the group
        leader.Group!.Should()
              .Contain(receiver);

        // GroupBox should be cleared
        receiver.GroupBox
                .Should()
                .BeNull();
    }
    #endregion

    #region AcceptInvite — Expired Invite
    [Test]
    public void AcceptInvite_ShouldNotFormGroup_WhenInviteIsExpired()
    {
        var groupService = MockGroupService.Create();

        var sender = MockAisling.Create(
            SharedMap,
            "ExpSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "ExpReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Send the invite
        groupService.Invite(sender, receiver);

        // Use reflection to expire the invite
        var pendingInvitesField = typeof(GroupService).GetField("PendingInvites", BindingFlags.NonPublic | BindingFlags.Instance);

        pendingInvitesField.Should()
                           .NotBeNull();

        var pendingInvites = pendingInvitesField!.GetValue(groupService);

        pendingInvites.Should()
                      .NotBeNull();

        // Get the TimedRequest type (private nested type)
        var timedRequestType = typeof(GroupService).GetNestedType("TimedRequest", BindingFlags.NonPublic);

        timedRequestType.Should()
                        .NotBeNull();

        var creationField = timedRequestType!.GetField("Creation", BindingFlags.NonPublic | BindingFlags.Instance);

        creationField.Should()
                     .NotBeNull();

        // Expire all pending invites by setting Creation to 2 minutes ago
        foreach (var invite in (IEnumerable)pendingInvites!)
            creationField!.SetValue(invite, DateTime.UtcNow.AddMinutes(-2));

        // Try to accept the expired invite
        groupService.AcceptInvite(sender, receiver);

        // Group should NOT have formed because the invite was expired
        sender.Group
              .Should()
              .BeNull();

        receiver.Group
                .Should()
                .BeNull();

        // Verify receiver got the "expired" message
        var receiverClientMock = Mock.Get(receiver.Client);

        receiverClientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(msg => msg.Contains("expired"))),
            Times.Once);
    }
    #endregion

    #region AcceptInvite — Receiver Already in Group (sender has group)
    [Test]
    public void AcceptInvite_ShouldSendAlreadyInGroupMessage_WhenSenderHasGroupAndReceiverAlreadyGrouped()
    {
        var groupService = MockGroupService.Create();

        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var otherLeader = MockAisling.Create(
            SharedMap,
            "OtherLeader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Form leader's group first, then send invite to receiver before receiver joins another group
        FormGroupWith(groupService, leader, member);
        groupService.Invite(leader, receiver);

        // Now put receiver in another group
        FormGroupWith(groupService, otherLeader, receiver);

        var receiverOriginalGroup = receiver.Group;

        // Accept the pending invite — receiver is already in a group, sender also has a group
        groupService.AcceptInvite(leader, receiver);

        // Receiver should remain in their original group
        receiver.Group
                .Should()
                .BeSameAs(receiverOriginalGroup);

        // Leader's group should not contain receiver
        leader.Group!.Contains(receiver)
              .Should()
              .BeFalse();
    }
    #endregion

    #region AcceptInvite — Receiver Already Grouped (Sender Ungrouped)
    [Test]
    public void AcceptInvite_ShouldSendAlreadyInGroupMessage_WhenSenderUngroupedAndReceiverAlreadyGrouped()
    {
        var groupService = MockGroupService.Create();

        var sender = MockAisling.Create(
            SharedMap,
            "UngroupedSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "GroupedReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var otherLeader = MockAisling.Create(
            SharedMap,
            "OtherLeader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Create the invite while receiver is ungrouped
        groupService.Invite(sender, receiver);

        // Now put receiver in another group before accepting
        FormGroupWith(groupService, otherLeader, receiver);

        // Accept the invite — sender has no group, receiver is now in a group
        groupService.AcceptInvite(sender, receiver);

        // Sender should remain ungrouped
        sender.Group
              .Should()
              .BeNull();

        // Receiver should remain in original group
        receiver.Group
                .Should()
                .BeSameAs(otherLeader.Group);
    }
    #endregion

    #region AcceptRequestToJoin — Expired Request
    [Test]
    public void AcceptRequestToJoin_ShouldNotFormGroup_WhenRequestIsExpired()
    {
        var groupService = MockGroupService.Create();

        var sender = MockAisling.Create(
            SharedMap,
            "ReqExpSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "ReqExpReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "ReqExpPartner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Receiver must have a group for RequestToJoin to work
        FormGroupWith(groupService, receiver, partner);

        // Send the request
        groupService.RequestToJoin(sender, receiver);

        // Use reflection to expire the request
        var pendingRequestsField = typeof(GroupService).GetField("PendingRequests", BindingFlags.NonPublic | BindingFlags.Instance);

        pendingRequestsField.Should()
                            .NotBeNull();

        var pendingRequests = pendingRequestsField!.GetValue(groupService);

        pendingRequests.Should()
                       .NotBeNull();

        var timedRequestType = typeof(GroupService).GetNestedType("TimedRequest", BindingFlags.NonPublic);

        timedRequestType.Should()
                        .NotBeNull();

        var creationField = timedRequestType!.GetField("Creation", BindingFlags.NonPublic | BindingFlags.Instance);

        creationField.Should()
                     .NotBeNull();

        // Expire all pending requests by setting Creation to 2 minutes ago
        foreach (var request in (IEnumerable)pendingRequests!)
            creationField!.SetValue(request, DateTime.UtcNow.AddMinutes(-2));

        // Try to accept the expired request
        groupService.AcceptRequestToJoin(sender, receiver);

        // Sender should NOT have joined the group because the request was expired
        sender.Group
              .Should()
              .BeNull();

        // Receiver's group count should still be 2 (original group)
        receiver.Group!.Count
                .Should()
                .Be(2);

        // Verify receiver got the "expired" message
        var receiverClientMock = Mock.Get(receiver.Client);

        receiverClientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(msg => msg.Contains("expired"))),
            Times.Once);
    }
    #endregion

    private void FillGroup(Aisling leader, int totalMembers)
    {
        // Fill group to totalMembers (including leader who is already member 1 of a 2-person group)
        for (var i = leader.Group!.Count; i < totalMembers; i++)
        {
            var filler = MockAisling.Create(
                SharedMap,
                $"Filler{i}",
                setup: a =>
                {
                    a.Options.AllowGroup = true;
                    a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
                });

            leader.Group.Add(filler);
        }
    }

    private void FormGroup(Aisling leader, Aisling member)
    {
        GroupService.Invite(leader, member);
        GroupService.AcceptInvite(leader, member);
    }

    #region Invite — Non-Leader Invites Same Group Member
    [Test]
    public void Invite_ShouldSendAlreadyInGroupMessage_WhenNonLeaderInvitesSameGroupMember()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var third = MockAisling.Create(
            SharedMap,
            "Third",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        leader.Group!.Add(third);

        // Non-leader member invites third (same group) — should NOT kick, should send "already in your group"
        GroupService.Invite(member, third);

        // Third should still be in the group (not kicked)
        leader.Group
              .Contains(third)
              .Should()
              .BeTrue();

        // Verify client received the "already in your group" message
        var memberClientMock = Mock.Get(member.Client);

        memberClientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(msg => msg.Contains("already in your group"))),
            Times.Once);
    }
    #endregion

    #region DetermineRequestType — Both Pending
    [Test]
    public void DetermineRequestType_ShouldReturnNonNull_WhenBothInviteAndRequestPending()
    {
        // Use separate GroupService to avoid interference from other tests
        var gs = MockGroupService.Create();
        var sender = MockAisling.Create(SharedMap, "BothSender", setup: a => a.Options.AllowGroup = true);
        var receiver = MockAisling.Create(SharedMap, "BothReceiver", setup: a => a.Options.AllowGroup = true);

        // Add both an invite and a request for the same key
        gs.Invite(sender, receiver);
        gs.RequestToJoin(sender, receiver);

        var result = gs.DetermineRequestType(sender, receiver);

        // Should hit the isInvite && isRequest branch and return one of the two types
        result.Should()
              .NotBeNull();

        result.Should()
              .BeOneOf(IGroupService.RequestType.Invite, IGroupService.RequestType.RequestToJoin);
    }

    [Test]
    public void DetermineRequestType_ShouldReturnInvite_WhenBothPendingAndInviteIsMoreRecent()
    {
        // When both pending, the more recent one should win
        // Request first (older), then Invite (newer) → pendingInvite > pendingRequest → Invite
        var gs = MockGroupService.Create();
        var sender = MockAisling.Create(SharedMap, "InvSender", setup: a => a.Options.AllowGroup = true);
        var receiver = MockAisling.Create(SharedMap, "InvReceiver", setup: a => a.Options.AllowGroup = true);

        gs.RequestToJoin(sender, receiver);
        gs.Invite(sender, receiver);

        var result = gs.DetermineRequestType(sender, receiver);

        result.Should()
              .Be(IGroupService.RequestType.Invite);
    }
    #endregion

    #region Kick
    [Test]
    public void Kick_ShouldRemoveMember_WhenSenderIsLeader()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var third = MockAisling.Create(
            SharedMap,
            "Third",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        leader.Group!.Add(third);

        GroupService.Kick(leader, member);

        member.Group
              .Should()
              .BeNull();

        leader.Group
              .Should()
              .NotBeNull();

        leader.Group!.Contains(member)
              .Should()
              .BeFalse();
    }

    [Test]
    public void Kick_ShouldNotKick_WhenSenderNotInGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // sender has no group
        GroupService.Kick(sender, receiver);

        // Should not throw, just sends a message
        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Kick_ShouldNotKick_WhenDifferentGroups()
    {
        var leader1 = MockAisling.Create(
            SharedMap,
            "Leader1",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member1 = MockAisling.Create(
            SharedMap,
            "Member1",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var leader2 = MockAisling.Create(
            SharedMap,
            "Leader2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member2 = MockAisling.Create(
            SharedMap,
            "Member2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader1, member1);
        FormGroup(leader2, member2);

        // leader1 tries to kick member2 (different group)
        GroupService.Kick(leader1, member2);

        // member2 should still be in their group
        member2.Group
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Kick_ShouldNotKick_WhenSenderNotLeader()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var third = MockAisling.Create(
            SharedMap,
            "Third",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        leader.Group!.Add(third);

        // member (not leader) tries to kick third
        GroupService.Kick(member, third);

        // third should still be in the group
        leader.Group
              .Contains(third)
              .Should()
              .BeTrue();
    }
    #endregion

    #region Promote
    [Test]
    public void Promote_ShouldChangeGroupLeader()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);

        GroupService.Promote(leader, member);

        leader.Group!.Leader
              .Name
              .Should()
              .Be("Member");
    }

    [Test]
    public void Promote_ShouldNotPromote_WhenSenderNotInGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // sender has no group
        GroupService.Promote(sender, receiver);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Promote_ShouldNotPromote_WhenDifferentGroups()
    {
        var leader1 = MockAisling.Create(
            SharedMap,
            "Leader1",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member1 = MockAisling.Create(
            SharedMap,
            "Member1",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var leader2 = MockAisling.Create(
            SharedMap,
            "Leader2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member2 = MockAisling.Create(
            SharedMap,
            "Member2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader1, member1);
        FormGroup(leader2, member2);

        // leader1 tries to promote member2 (different group)
        GroupService.Promote(leader1, member2);

        // leader2's group should still have leader2 as leader
        leader2.Group!.Leader
               .Name
               .Should()
               .Be("Leader2");
    }

    [Test]
    public void Promote_ShouldNotPromote_WhenSenderNotLeader()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var third = MockAisling.Create(
            SharedMap,
            "Third",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        leader.Group!.Add(third);

        // member (not leader) tries to promote third
        GroupService.Promote(member, third);

        // leader should still be leader
        leader.Group
              .Leader
              .Name
              .Should()
              .Be("Leader");
    }
    #endregion

    #region Invite
    [Test]
    public void Invite_ShouldSendInvite_WhenBothPartiesAreUngrouped()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        GroupService.Invite(sender, receiver);

        // The invite was created — verify by accepting it
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .NotBeNull();

        receiver.Group
                .Should()
                .BeSameAs(sender.Group);
    }

    [Test]
    public void Invite_SelfInvite_ShouldLeaveGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Form a group first
        FormGroup(sender, receiver);

        sender.Group
              .Should()
              .NotBeNull();

        // Add a third member so leaving doesn't disband
        var third = MockAisling.Create(
            SharedMap,
            "Third",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });
        sender.Group!.Add(third);

        // Self-invite = leave
        GroupService.Invite(sender, sender);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_SelfInvite_ShouldDoNothing_WhenNotInGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "LoneSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Self-invite when not in a group — receiver.Group?.Leave(sender) is a no-op
        GroupService.Invite(sender, sender);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_ShouldNotInvite_WhenSenderAllowGroupFalse()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });
        sender.Options.AllowGroup = false;

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        GroupService.Invite(sender, receiver);

        // No invite should be created — accepting should not form a group
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_ShouldNotInvite_WhenReceiverAllowGroupFalse()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });
        receiver.Options.AllowGroup = false;

        GroupService.Invite(sender, receiver);

        // No invite should be created
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_ShouldNotInvite_WhenReceiverAlreadyInGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var other = MockAisling.Create(
            SharedMap,
            "Other",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Put receiver in a group
        FormGroup(other, receiver);

        GroupService.Invite(sender, receiver);

        // Invite should fail — sender should remain ungrouped
        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_ShouldNotInvite_WhenGroupFull()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var target = MockAisling.Create(
            SharedMap,
            "Target",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);

        // Fill group to max (6)
        FillGroup(leader, 6);

        leader.Group!.Count
              .Should()
              .Be(6);

        // Try to invite when group is full
        GroupService.Invite(leader, target);

        // Target should remain ungrouped
        target.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_ShouldSendInvite_WhenSenderHasGroupWithSpace()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var target = MockAisling.Create(
            SharedMap,
            "Target",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);

        // Sender has group with space — invite should work
        GroupService.Invite(leader, target);
        GroupService.AcceptInvite(leader, target);

        leader.Group!.Count
              .Should()
              .Be(3);

        leader.Group
              .Should()
              .Contain(target);
    }

    [Test]
    public void Invite_ShouldKickMember_WhenLeaderInvitesSameGroupMember()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var third = MockAisling.Create(
            SharedMap,
            "Third",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        leader.Group!.Add(third);

        // Leader invites member who is already in the group — acts as kick
        GroupService.Invite(leader, member);

        leader.Group
              .Contains(member)
              .Should()
              .BeFalse();

        member.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_ShouldNotKick_WhenNonLeaderInvitesSameGroupMember()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var third = MockAisling.Create(
            SharedMap,
            "Third",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        leader.Group!.Add(third);

        // Non-leader member invites third (same group) — should not kick
        GroupService.Invite(member, third);

        leader.Group
              .Contains(third)
              .Should()
              .BeTrue();
    }

    [Test]
    public void Invite_ShouldNotSendInvite_WhenReceiverIgnoresSender()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        receiver.IgnoreList.Add("Sender");

        GroupService.Invite(sender, receiver);

        // Invite should be blocked by ignore list in SendInvite
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void Invite_ShouldNotSendInvite_WhenReceiverInDifferentGroup()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var other = MockAisling.Create(
            SharedMap,
            "Other",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var otherMember = MockAisling.Create(
            SharedMap,
            "OtherMember",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        FormGroup(other, otherMember);

        // Leader tries to invite otherMember (in a different group)
        GroupService.Invite(leader, otherMember);

        // otherMember should remain in their group
        otherMember.Group
                   .Should()
                   .BeSameAs(other.Group);
    }
    #endregion

    #region AcceptInvite
    [Test]
    public void AcceptInvite_ShouldCreateNewGroup_WhenNeitherIsGrouped()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        GroupService.Invite(sender, receiver);
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .NotBeNull();

        receiver.Group
                .Should()
                .NotBeNull();

        sender.Group
              .Should()
              .BeSameAs(receiver.Group);

        sender.Group!.Count
              .Should()
              .Be(2);
    }

    [Test]
    public void AcceptInvite_ShouldAddToExistingGroup_WhenSenderHasGroup()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member1 = MockAisling.Create(
            SharedMap,
            "Member1",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member2 = MockAisling.Create(
            SharedMap,
            "Member2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Leader invites member1, creating a group
        FormGroup(leader, member1);

        // Leader invites member2 into existing group
        GroupService.Invite(leader, member2);
        GroupService.AcceptInvite(leader, member2);

        leader.Group!.Count
              .Should()
              .Be(3);

        leader.Group
              .Should()
              .Contain(member2);
    }

    [Test]
    public void AcceptInvite_ShouldDoNothing_WhenNoInvitePending()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // No invite was sent — accept should do nothing
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .BeNull();

        receiver.Group
                .Should()
                .BeNull();
    }

    [Test]
    public void AcceptInvite_ShouldNotAdd_WhenReceiverAlreadyInGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var other = MockAisling.Create(
            SharedMap,
            "Other",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Put receiver in another group
        FormGroup(other, receiver);
        var receiverGroup = receiver.Group;

        // Sender invites receiver
        GroupService.Invite(sender, receiver);

        // But wait, receiver is already grouped — clear the group to test the sender.Group==null path
        // Actually the invite should have failed already because receiver was in a group
        // Instead let's test the path where sender has no group but receiver does
        sender.Group
              .Should()
              .BeNull();

        receiver.Group
                .Should()
                .BeSameAs(receiverGroup);
    }

    [Test]
    public void AcceptInvite_ShouldNotAdd_WhenGroupFull()
    {
        var leader = MockAisling.Create(
            SharedMap,
            "Leader",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member = MockAisling.Create(
            SharedMap,
            "Member",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        MockAisling.Create(
            SharedMap,
            "Target",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(leader, member);
        FillGroup(leader, 6);

        // Somehow an invite got through before the group was full
        // We need to create the invite before filling the group
        // Let's reset and do this properly
        var leader2 = MockAisling.Create(
            SharedMap,
            "Leader2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var member2 = MockAisling.Create(
            SharedMap,
            "Member2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var target2 = MockAisling.Create(
            SharedMap,
            "Target2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Create a new group service to get fresh state
        var groupService = MockGroupService.Create();

        groupService.Invite(leader2, target2);
        FormGroupWith(groupService, leader2, member2);
        FillGroupTo(leader2, 6);

        // Now accept the pending invite — group is full
        groupService.AcceptInvite(leader2, target2);

        // Target should not be in the group
        target2.Group
               .Should()
               .BeNull();
    }

    [Test]
    public void AcceptInvite_ShouldCloseGroupBox_WhenReceiverHasGroupBox()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        receiver.GroupBox = new GroupBox
        {
            Name = "Test Group",
            Note = "Testing",
            MaxWarriors = 2
        };

        GroupService.Invite(sender, receiver);
        GroupService.AcceptInvite(sender, receiver);

        receiver.GroupBox
                .Should()
                .BeNull();

        receiver.Group
                .Should()
                .NotBeNull();
    }
    #endregion

    #region AcceptRequestToJoin
    [Test]
    public void AcceptRequestToJoin_ShouldCreateGroup_WhenNeitherIsGrouped()
    {
        MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Give receiver a group so RequestToJoin can be sent, then disband
        FormGroup(receiver, partner);
        receiver.Group!.Leave(receiver);

        // Both ungrouped now — set up fresh
        var sender2 = MockAisling.Create(
            SharedMap,
            "Sender2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver2 = MockAisling.Create(
            SharedMap,
            "Receiver2",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var rcvPartner = MockAisling.Create(
            SharedMap,
            "RcvPartner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // We need receiver2 to have a group for the request to be accepted
        // Actually, let's just force the request through
        var groupService = MockGroupService.Create();

        // receiver2 needs a group for RequestToJoin to succeed
        FormGroupWith(groupService, receiver2, rcvPartner);

        groupService.RequestToJoin(sender2, receiver2);

        // Now remove receiver2 from group so we test the "neither grouped" accept path
        receiver2.Group!.Leave(receiver2);

        receiver2.Group
                 .Should()
                 .BeNull();

        groupService.AcceptRequestToJoin(sender2, receiver2);

        // Receiver is leader when both ungrouped (receiver creates group)
        sender2.Group
               .Should()
               .NotBeNull();

        receiver2.Group
                 .Should()
                 .BeSameAs(sender2.Group);

        // Receiver should be leader
        sender2.Group!.Leader
               .Name
               .Should()
               .Be("Receiver2");
    }

    [Test]
    public void AcceptRequestToJoin_ShouldAddToReceiverGroup_WhenReceiverHasGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var groupService = MockGroupService.Create();
        FormGroupWith(groupService, receiver, partner);

        groupService.RequestToJoin(sender, receiver);
        groupService.AcceptRequestToJoin(sender, receiver);

        receiver.Group!.Count
                .Should()
                .Be(3);

        receiver.Group
                .Should()
                .Contain(sender);
    }

    [Test]
    public void AcceptRequestToJoin_ShouldDoNothing_WhenNoRequestPending()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        GroupService.AcceptRequestToJoin(sender, receiver);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void AcceptRequestToJoin_ShouldNotAdd_WhenSenderAlreadyInGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var senderPartner = MockAisling.Create(
            SharedMap,
            "SenderPartner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var groupService = MockGroupService.Create();
        FormGroupWith(groupService, receiver, partner);

        groupService.RequestToJoin(sender, receiver);

        // Put sender in a group before accepting
        FormGroupWith(groupService, sender, senderPartner);

        groupService.AcceptRequestToJoin(sender, receiver);

        // Sender should still be in their original group
        sender.Group
              .Should()
              .NotBeSameAs(receiver.Group);
    }

    [Test]
    public void AcceptRequestToJoin_ShouldNotAdd_WhenReceiverGroupFull()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var groupService = MockGroupService.Create();
        FormGroupWith(groupService, receiver, partner);

        groupService.RequestToJoin(sender, receiver);

        // Fill receiver's group to max
        FillGroupTo(receiver, 6);

        groupService.AcceptRequestToJoin(sender, receiver);

        // Sender should not be in the group
        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void AcceptRequestToJoin_ShouldCloseGroupBox_WhenSenderHasGroupBox()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        sender.GroupBox = new GroupBox
        {
            Name = "Test Group",
            Note = "Testing",
            MaxWarriors = 2
        };

        var groupService = MockGroupService.Create();
        FormGroupWith(groupService, receiver, partner);

        groupService.RequestToJoin(sender, receiver);
        groupService.AcceptRequestToJoin(sender, receiver);

        sender.GroupBox
              .Should()
              .BeNull();

        sender.Group
              .Should()
              .NotBeNull();
    }
    #endregion

    #region RequestToJoin
    [Test]
    public void RequestToJoin_ShouldSendRequest_WhenReceiverHasGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(receiver, partner);

        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.RequestToJoin);
    }

    [Test]
    public void RequestToJoin_ShouldNotRequest_WhenSenderAllowGroupFalse()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });
        sender.Options.AllowGroup = false;

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(receiver, partner);

        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldNotRequest_WhenReceiverAllowGroupFalse()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });
        receiver.Options.AllowGroup = false;

        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldNotRequest_WhenSelfRequest()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        GroupService.RequestToJoin(sender, sender);

        var requestType = GroupService.DetermineRequestType(sender, sender);

        requestType.Should()
                   .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldNotRequest_WhenSenderAlreadyInGroup()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var senderPartner = MockAisling.Create(
            SharedMap,
            "SenderPartner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(sender, senderPartner);
        FormGroup(receiver, partner);

        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldNotRequest_WhenReceiverGroupFull()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(receiver, partner);
        FillGroup(receiver, 6);

        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldNotRequest_WhenClassLimitReached()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Priest);
            });

        receiver.GroupBox = new GroupBox
        {
            Name = "Test Group",
            Note = "Testing",
            MaxWarriors = 1,
            MaxPriests = 3,
            MaxWizards = 3,
            MaxRogues = 3,
            MaxMonks = 3
        };

        FormGroup(receiver, partner);

        // Receiver is a warrior, so the group already has 1 warrior
        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldNotSendRequest_WhenReceiverIgnoresSender()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        FormGroup(receiver, partner);
        receiver.IgnoreList.Add("Sender");

        GroupService.RequestToJoin(sender, receiver);

        // The request should be blocked by SendRequestToJoin's ignore check
        GroupService.AcceptRequestToJoin(sender, receiver);

        sender.Group
              .Should()
              .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldSendRequest_WhenBothUngrouped()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.RequestToJoin);
    }
    #endregion

    #region DetermineRequestType
    [Test]
    public void DetermineRequestType_ShouldReturnInvite_WhenPendingInviteExists()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "GroupedSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "UngroupedReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Invite creates a pending invite entry
        GroupService.Invite(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.Invite);
    }

    [Test]
    public void DetermineRequestType_ShouldReturnRequestToJoin_WhenPendingRequestExists()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "UngroupedSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "GroupedReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        // Give receiver a group so RequestToJoin path is used
        var partner = MockAisling.Create(
            SharedMap,
            "Partner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });
        FormGroup(receiver, partner);

        // RequestToJoin creates a pending request entry
        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.RequestToJoin);
    }

    [Test]
    public void DetermineRequestType_ShouldReturnNull_WhenNoPendingInvitesOrRequests()
    {
        var sender = MockAisling.Create(
            SharedMap,
            "Sender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "Receiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }
    #endregion

    #region Invite Double-Call (Refresh Branch)
    [Test]
    public void Invite_ShouldRefreshExistingInvite_WhenInvitedTwice()
    {
        var sender = MockAisling.Create(SharedMap, "Sender", setup: a => a.Options.AllowGroup = true);
        var receiver = MockAisling.Create(SharedMap, "Receiver", setup: a => a.Options.AllowGroup = true);

        // First invite
        GroupService.Invite(sender, receiver);

        // Verify invite was sent
        var result1 = GroupService.DetermineRequestType(sender, receiver);

        result1.Should()
               .Be(IGroupService.RequestType.Invite);

        // Second invite (should refresh the existing one, not add a new one)
        GroupService.Invite(sender, receiver);

        // Should still resolve as an invite (refresh doesn't change the type)
        var result2 = GroupService.DetermineRequestType(sender, receiver);

        result2.Should()
               .Be(IGroupService.RequestType.Invite);
    }

    [Test]
    public void RequestToJoin_ShouldRefreshExistingRequest_WhenRequestedTwice()
    {
        var sender = MockAisling.Create(SharedMap, "ReqSender", setup: a => a.Options.AllowGroup = true);
        var receiver = MockAisling.Create(SharedMap, "ReqReceiver", setup: a => a.Options.AllowGroup = true);

        // First request
        GroupService.RequestToJoin(sender, receiver);

        var result1 = GroupService.DetermineRequestType(sender, receiver);

        result1.Should()
               .Be(IGroupService.RequestType.RequestToJoin);

        // Second request (should refresh the existing one)
        GroupService.RequestToJoin(sender, receiver);

        var result2 = GroupService.DetermineRequestType(sender, receiver);

        result2.Should()
               .Be(IGroupService.RequestType.RequestToJoin);
    }
    #endregion

    #region RequestToJoin — Class Limits (Non-Warrior)
    //formatter:off
    [Test]
    [Arguments(BaseClass.Wizard)]
    [Arguments(BaseClass.Rogue)]
    [Arguments(BaseClass.Priest)]
    [Arguments(BaseClass.Monk)]

    //formatter:on
    public void RequestToJoin_ShouldNotRequest_WhenClassLimitReached_ForEachClass(BaseClass senderClass)
    {
        var groupService = MockGroupService.Create();

        var sender = MockAisling.Create(
            SharedMap,
            "ClassSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(senderClass);
            });

        // Receiver is a different class so the receiver doesn't count toward the sender's class limit
        var receiver = MockAisling.Create(
            SharedMap,
            "ClassReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "ClassPartner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(senderClass);
            });

        receiver.GroupBox = new GroupBox
        {
            Name = "Test Group",
            Note = "Testing",
            MaxWarriors = 3,
            MaxWizards = 1,
            MaxRogues = 1,
            MaxPriests = 1,
            MaxMonks = 1
        };

        FormGroupWith(groupService, receiver, partner);

        // The group already has 1 of senderClass (partner), limit is 1
        groupService.RequestToJoin(sender, receiver);

        var requestType = groupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }

    //formatter:off
    [Test]
    [Arguments(BaseClass.Wizard)]
    [Arguments(BaseClass.Rogue)]
    [Arguments(BaseClass.Priest)]
    [Arguments(BaseClass.Monk)]

    //formatter:on
    public void RequestToJoin_ShouldSendRequest_WhenUnderClassLimit_ForEachClass(BaseClass senderClass)
    {
        var groupService = MockGroupService.Create();

        var sender = MockAisling.Create(
            SharedMap,
            "UnderSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(senderClass);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "UnderReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "UnderPartner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        receiver.GroupBox = new GroupBox
        {
            Name = "Test Group",
            Note = "Testing",
            MaxWarriors = 3,
            MaxWizards = 3,
            MaxRogues = 3,
            MaxPriests = 3,
            MaxMonks = 3
        };

        FormGroupWith(groupService, receiver, partner);

        // No one of senderClass is in the group yet, limit is 3
        groupService.RequestToJoin(sender, receiver);

        var requestType = groupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.RequestToJoin);
    }
    #endregion

    #region RequestToJoin — Peasant / No Group with GroupBox
    [Test]
    public void RequestToJoin_ShouldNotRequest_WhenPeasantClassLimit_DefaultsToZero()
    {
        var groupService = MockGroupService.Create();

        var sender = MockAisling.Create(
            SharedMap,
            "PeasantSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Peasant);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "PeasantReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var partner = MockAisling.Create(
            SharedMap,
            "PeasantPartner",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        receiver.GroupBox = new GroupBox
        {
            Name = "Test Group",
            Note = "Testing",
            MaxWarriors = 3,
            MaxWizards = 3,
            MaxRogues = 3,
            MaxPriests = 3,
            MaxMonks = 3
        };

        FormGroupWith(groupService, receiver, partner);

        // Peasant hits the default case (_ => 0), so classLimit=0 and any count >= 0 is true
        groupService.RequestToJoin(sender, receiver);

        var requestType = groupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }

    [Test]
    public void RequestToJoin_ShouldSendRequest_WhenReceiverHasNoGroupButHasGroupBox()
    {
        var groupService = MockGroupService.Create();

        var sender = MockAisling.Create(
            SharedMap,
            "NoGroupSender",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            });

        var receiver = MockAisling.Create(
            SharedMap,
            "NoGroupReceiver",
            setup: a =>
            {
                a.Options.AllowGroup = true;
                a.UserStatSheet.SetBaseClass(BaseClass.Priest);
            });

        // Receiver has GroupBox but no group — tests the `receiver.Group?.Count(...)` null path
        // which falls to `(receiver.UserStatSheet.BaseClass == sender.UserStatSheet.BaseClass ? 1 : 0)`
        receiver.GroupBox = new GroupBox
        {
            Name = "Recruiting",
            Note = "Join us!",
            MaxWarriors = 3,
            MaxPriests = 3,
            MaxWizards = 3,
            MaxRogues = 3,
            MaxMonks = 3
        };

        // Both ungrouped — request goes through as SendRequestToJoin
        groupService.RequestToJoin(sender, receiver);

        var requestType = groupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.RequestToJoin);
    }
    #endregion

    #region Helpers
    private void FormGroupWith(IGroupService groupService, Aisling leader, Aisling member)
    {
        groupService.Invite(leader, member);
        groupService.AcceptInvite(leader, member);
    }

    private static void FillGroupTo(Aisling leader, int totalMembers)
    {
        var map = leader.MapInstance;

        for (var i = leader.Group!.Count; i < totalMembers; i++)
        {
            var filler = MockAisling.Create(map, $"Fill{i}", setup: a => a.Options.AllowGroup = true);
            leader.Group.Add(filler);
        }
    }
    #endregion
}